using System.Net;
using Serilog.Formatting.Elasticsearch;

namespace Nerosoft.Euonia.Hosting;

internal class SerilogElasticsearchFormatter : ElasticsearchJsonFormatter
{
    private readonly string _sender;

    public SerilogElasticsearchFormatter(string sender)
    {
        if (string.IsNullOrEmpty(sender))
        {
            sender = Dns.GetHostName();
        }

        _sender = sender;
    }

    protected override void WriteTimestamp(DateTimeOffset timestamp, ref string delim, TextWriter output)
    {
        base.WriteTimestamp(timestamp, ref delim, output);
        WriteJsonProperty("sender", _sender, ref delim, output);
    }
}
