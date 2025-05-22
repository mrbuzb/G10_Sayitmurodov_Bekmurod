namespace UserContacts.Server.Middlewares;

public class NightBlockMiddleware
{
    private readonly RequestDelegate _next;

    public NightBlockMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var currentHour = DateTime.Now.Hour;

        if (currentHour >= 18 || currentHour < 9 || context.Request.Path.ToString().Contains("get"))
        {
            context.Response.StatusCode = 403;
            await context.Response.WriteAsJsonAsync(new
            {
                message = "Bizning Apilar 9:00 dan 18:00 gacha ishlaydi"
            });

            return; 
        }

        await _next(context); 
    }
}
