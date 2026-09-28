// MIDDLEWARE — HTTP sorğusu ilə cavab arasında işləyən kod parçasıdır.
// Sorğu middleware-lərdən ardıcıl keçir (buna "pipeline" deyilir),
// cavab isə eyni yolla TƏRS sıra ilə geri qayıdır:
//
//   Sorğu  →  [1] → [2] → [3] → [4] app.Run
//   Cavab  ←  [1] ← [2] ← [3] ←
//
// Middleware-lərin sırası vacibdir: burada necə yazılıbsa, o sıra ilə işləyir.

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// [1] INLINE MIDDLEWARE (app.Use)
// next() növbəti middleware-i çağırır.
// next()-dən ƏVVƏLKİ kod sorğu gedəndə, SONRAKI kod cavab qayıdanda işləyir.
app.Use(async (context, next) =>
{
    Console.WriteLine($"[1] Sorğu gəldi: {context.Request.Method} {context.Request.Path}");

    await next();

    Console.WriteLine($"[1] Cavab qayıtdı: status {context.Response.StatusCode}");
});

// [2] SHORT-CIRCUIT (pipeline-ı yarıda kəsmək)
// next() çağırılmasa, sonrakı middleware-lər ümumiyyətlə işləmir.
// Tipik istifadə: icazə yoxlaması, rate limiting, "saytda təmir işləri gedir" səhifəsi.
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/admin")
    {
        Console.WriteLine("[2] /admin bloklandı, next() çağırılmır");
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        await context.Response.WriteAsync("Giriş qadağandır");
        return;
    }

    await next();
});

// [3] CLASS ŞƏKLİNDƏ MIDDLEWARE
// Real layihələrdə çox vaxt bu forma işlədilir: kod ayrıca class-da olur,
// test etmək və başqa layihədə təkrar istifadə etmək asan olur.
// Class aşağıda, faylın sonundadır.
app.UseMiddleware<RequestTimingMiddleware>();

// [4] TERMINAL MIDDLEWARE (app.Run(handler))
// next parametri yoxdur, pipeline həmişə burada bitir.
app.Run(async context =>
{
    Console.WriteLine("[4] app.Run: cavab yazılır");
    await context.Response.WriteAsync("Salam, middleware!");
});

// Bu isə başqa şeydir: parametrsiz app.Run() server-i işə salır.
app.Run();

// Class middleware qaydaları:
// - Konstruktor RequestDelegate qəbul edir, bu növbəti middleware-dir
// - InvokeAsync(HttpContext) adlı metodu olmalıdır
class RequestTimingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine("[3] Vaxt ölçülməyə başladı");

        await next(context);

        Console.WriteLine($"[3] Sorğu {stopwatch.ElapsedMilliseconds} ms çəkdi");
    }
}
