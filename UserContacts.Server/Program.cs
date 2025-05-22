using UserContacts.Server.Configurations;
using UserContacts.Server.Endpoints;
using UserContacts.Server.Middlewares;

namespace UserContacts.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Configuration();
            builder.ConfigurationJwtAuth();
            builder.ConfigureJwtSettings();
            builder.ConfigureSerilog();
            builder.Services.ConfigureDependecies();
            builder.Services.AddResponseCaching();
            var app = builder.Build();


            app.MapAuthEndpoints();
            app.MapAdminEndpoints();
            app.MapContactEndpoints();
            app.MapRoleEndpoints();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<NightBlockMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
