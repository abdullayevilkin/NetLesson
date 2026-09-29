using CustomProject.Middleware;

var builder = WebApplication.CreateBuilder(args);

// DI container-ə class-based middleware-ləri qeyd et
builder.Services.AddTransient<AsyncMiddleware>();
builder.Services.AddTransient<FactoryMiddleware>();

var app = builder.Build();

// UseMiddleware<T> - reflection ilə Invoke/InvokeAsync-ı tapıb çağırır
app.UseMiddleware<AsyncMiddleware>();
app.UseMiddleware<FactoryMiddleware>();

app.Run();