

using MissingContentLengthHeaderBugRepro;
using System.Text;
using System.Text.Json;

var fakeHost = new FakeApi();

await fakeHost.RunAsync();

var httpClient = new HttpClient();

var response = await httpClient.SendAsync(new HttpRequestMessage
{
    RequestUri = new Uri($"https://localhost:{fakeHost.HttpsPort}/api/v1/counter"),
    Method = HttpMethod.Post,
    Content = new StringContent(JsonSerializer.Serialize(new SampleResponse { Counter = 3}), Encoding.UTF8, "application/json")
});

var body = await response.Content.ReadAsByteArrayAsync();

Console.WriteLine("=== HEADERS === \n");

foreach (var header in response.Headers)
{
    Console.WriteLine($"Key: {header.Key}");
}

Console.WriteLine("=== END === \n");

Console.WriteLine("=== CONTENT HEADERS === \n");

foreach (var header in response.Content.Headers)
{
    Console.WriteLine($"Key: {header.Key}");
}
Console.WriteLine("=== END === \n");

Console.WriteLine("Content-Length property:" + response.Content.Headers.ContentLength);
