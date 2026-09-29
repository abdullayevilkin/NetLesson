namespace CustomProject.Middleware;

// Versiya 1: Invoke (sinxron)
public class SyncMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SyncMiddleware> _logger;

    // Constructor: DI container burda dependency-lər inject edir
    public SyncMiddleware(RequestDelegate next, ILogger<SyncMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // Invoke: reflection ilə tapılır və çağırılır
    // İlk parametr həmişə HttpContext olmalıdır
    // Digər parametrlər DI container-dən resolve edilir
    public void Invoke(HttpContext context)
    {
        _logger.LogInformation("SyncMiddleware: sorğu başladı");

        // Cavab yaz
        context.Response.ContentType = "text/plain";
        context.Response.WriteAsync("Sync Middleware -> ").Wait();

        // Sonrakı middleware-ə keç
        _next(context).Wait();
    }
}

// Versiya 2: InvokeAsync (asinkron - TOPDUr!)
public class AsyncMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AsyncMiddleware> _logger;

    public AsyncMiddleware(RequestDelegate next, ILogger<AsyncMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // InvokeAsync: async çalışır, .Wait() lazım deyil
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("AsyncMiddleware: sorğu başladı");

        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Async Middleware -> ");

        // await ilə async keç
        await _next(context);
    }
}

// Versiya 3: DI-dən digər xidmət inject etmə
public class CustomServiceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomServiceMiddleware> _logger;
    private readonly IConfiguration _config;

    public CustomServiceMiddleware(
        RequestDelegate next,
        ILogger<CustomServiceMiddleware> logger,
        IConfiguration config)  // Başqa xidmət
    {
        _next = next;
        _logger = logger;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var appName = _config["AppName"] ?? "Unknown";
        _logger.LogInformation($"App: {appName}");

        await context.Response.WriteAsync($"[{appName}] ");
        await _next(context);
    }
}

// Versiya 4: Invoke-da başqa parametr - DI container resolve edir
public class FactoryMiddleware
{
    private readonly RequestDelegate _next;

    public FactoryMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // HttpContext-dən sonra başqa parametr varsa, DI-dən gelir
    public async Task InvokeAsync(HttpContext context, IConfiguration config)
    {
        var version = config["Version"] ?? "1.0";
        await context.Response.WriteAsync($"v{version} ");
        await _next(context);
    }
}
