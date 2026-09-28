using System.Text.Json;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var product = new { Id = 1, Name = "Laptop", Price = 1500 };
string[] supportedTypes = ["application/json", "application/xml", "text/plain"];
var extensions = new Dictionary<string, string>
{
    ["json"] = "application/json",
    ["xml"] = "application/xml",
    ["txt"] = "text/plain",
};

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    // Server-driven: client Accept header göndərir, formatı SERVER seçir
    if (path == "/server-driven")
    {
        context.Response.Headers.Vary = "Accept";
        var accepts = context.Request.GetTypedHeaders().Accept;

        var chosen = accepts.Count == 0
            ? supportedTypes[0]
            : accepts
                .Where(a => a.Quality != 0)
                .OrderByDescending(a => a.Quality ?? 1)
                .SelectMany(a => supportedTypes.Where(s => new MediaTypeHeaderValue(s).IsSubsetOf(a)))
                .FirstOrDefault();

        if (chosen is null)
        {
            context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
            await context.Response.WriteAsync("Dəstəklənən formatlar: " + string.Join(", ", supportedTypes));
            return;
        }

        await WriteProduct(context, chosen);
        return;
    }

    // Agent-driven: server variantların siyahısını qaytarır, CLIENT özü seçir
    if (path == "/agent-driven")
    {
        context.Response.StatusCode = StatusCodes.Status300MultipleChoices;
        context.Response.Headers.Link = string.Join(", ",
            extensions.Select(e => $"</agent-driven/product.{e.Key}>; rel=\"alternate\"; type=\"{e.Value}\""));
        await context.Response.WriteAsJsonAsync(
            extensions.Select(e => new { url = $"/agent-driven/product.{e.Key}", type = e.Value }));
        return;
    }

    if (path.StartsWith("/agent-driven/product."))
    {
        var ext = path["/agent-driven/product.".Length..];
        if (extensions.TryGetValue(ext, out var mediaType))
        {
            await WriteProduct(context, mediaType);
            return;
        }
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        return;
    }

    await next();
});

//app.MapGet("/", () => "Hello World!");
app.Run(async (HttpContext context) =>
{
    if(context.Request.Method == "GET")
    {
        if (context.Request.Query.ContainsKey("id"))
        {
            string id = context.Request.Query["id"];
            await context.Response.WriteAsync("this is id " + id);

        }
    }
});


app.Run();

async Task WriteProduct(HttpContext context, string mediaType)
{
    context.Response.ContentType = mediaType;
    var body = mediaType switch
    {
        "application/json" => JsonSerializer.Serialize(product),
        "application/xml" => $"<product><id>{product.Id}</id><name>{product.Name}</name><price>{product.Price}</price></product>",
        _ => $"Id: {product.Id}, Name: {product.Name}, Price: {product.Price}",
    };
    await context.Response.WriteAsync(body);
}
