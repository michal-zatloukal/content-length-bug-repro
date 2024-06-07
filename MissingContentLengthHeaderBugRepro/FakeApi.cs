using Microsoft.AspNetCore.Mvc;

namespace MissingContentLengthHeaderBugRepro;

public class FakeApi
{
    private readonly object _counterLock = new object();
    private int _counter = 0;

    private readonly IDictionary<string, string> _requestHeaders = new Dictionary<string, string>();

    private WebApplication? _app;

    public ushort HttpsPort { get; private set; }
    public async Task RunAsync()
    {
        var builder = WebApplication.CreateBuilder();

        builder.WebHost.ConfigureKestrel(_ => { });
        _app = builder.Build();

        _app.MapPost("api/v1/counter", HandleRequestWithBody);

        await _app.StartAsync();
        HttpsPort = (ushort)new Uri(_app.Urls.First(s => s.StartsWith("https://"))).Port;
    }

    private IResult HandleRequestWithBody([FromBody] SampleRequest command, HttpContext context)
    {
        lock (_counterLock)
        {
            _counter += command.IncrementBy;
            context.Response.Headers.Append("Custom-Header3", "value4, value5");
            context.Response.Headers.Append("Custom-Header4", "value6");
            return Results.Ok(new SampleResponse
            {
                Counter = _counter
            });
        }
    }
}
