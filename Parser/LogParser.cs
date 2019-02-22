using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using csv_prometheus_exporter.Prometheus;
using CsvParser;
using NLog;

namespace csv_prometheus_exporter.Parser
{
    public class LogParser
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly LabelDict _labels;
        private readonly IList<ColumnReader> _readers;
        private readonly Stream _stream;

        private LogParser(Stream stream, IList<ColumnReader> readers, string environment)
        {
            _stream = stream;
            _readers = readers;
            _labels = new LabelDict(environment);
        }

        private ParsedMetrics ConvertCsvLine(ICsvReaderRow line, LabelDict labels)
        {
            if (_readers.Count != line.Count) throw new ParserError();

            var result = new ParsedMetrics(labels);
            foreach (var (reader, column) in _readers.Zip(line, (a, b) => new KeyValuePair<ColumnReader, string>(a, b)))
                reader?.Invoke(result, column);

            return result;
        }

        private IEnumerable<ParsedMetrics> ReadAll()
        {
            using (var sshStream = new SSHStream(_stream))
            using (var parser = new CsvReader(sshStream, Encoding.UTF8,
                new CsvReader.Config
                    {Quotes = '"', ColumnSeparator = ' ', ReadinBufferSize = 1024, WithQuotes = false}))
            {
                while (_stream.CanRead)
                {
                    ParsedMetrics result = null;
                    try
                    {
                        if (parser.MoveNext()) result = ConvertCsvLine(parser.Current, _labels);
                    }
                    catch (ParserError)
                    {
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal(ex, $"Unexpected exception: {ex.Message}");
                    }

                    yield return result;
                }
            }

            Logger.Info("End of stream");
        }

        public static void ParseFile(Stream stdout, string environment, IList<ColumnReader> readers,
            IDictionary<string, MetricBase> metrics)
        {
            if (string.IsNullOrEmpty(environment))
                environment = "N/A";

            var envDict = new LabelDict(environment);

            foreach (var entry in new LogParser(stdout, readers, environment).ReadAll())
            {
                if (entry == null)
                {
                    metrics["parser_errors"].WithLabels(envDict).Add(1);
                    continue;
                }

                metrics["lines_parsed"].WithLabels(entry.Labels).Add(1);

                foreach (var (name, amount) in entry.Metrics)
                    if (metrics.TryGetValue(name, out var metric))
                        metric.WithLabels(entry.Labels).Add(amount);
            }
        }
    }
}