
namespace Demo1 {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            builder.Services.AddProblemDetails(o => {
                o.CustomizeProblemDetails = (ctx) => {
                    ctx.ProblemDetails.Extensions.Add("ApplicationName", "My Demo API");
                    if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToUpper() == "DEVELOPMENT") {
                        ctx.ProblemDetails.Extensions.Add("MachineName", Environment.MachineName);
                    }
                };
            });

            var app = builder.Build();


            //app.Run(async (context) => {
            //    await context.Response.WriteAsync("<html><body><b>Hello World!</b></body></html>");
            //});

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.MapOpenApi();
            
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //app.UseAuthorization();

            app.MapControllers();            

            //app.Run(async (context) => {
            //    await context.Response.WriteAsync("<html><body><b>Login:...</b></body></html>");
            //});

            app.Run();
        }
    }
}
