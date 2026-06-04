
using Asp.Versioning;
using Demo1.DbContexts;
using Demo1.Services;
using Demo1.Services.Repositories;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace Demo1 {
    public class Program {
        public static void Main(string[] args) {
            //MyLogger logger = new MyLogger();

            //Class1 c1 = new Class1(logger);
            //Class2 c2 = new Class2(logger);

            string template = "{Timestamp:dd-MM-yyyy} [{MachineName}-{ThreadId}] ({RequestId}) {Message}{NewLine}{Properties}{NewLine}{NewLine}";

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: template)
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Minute, outputTemplate: template)
                //.Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            //builder.Logging.ClearProviders();
            //builder.Logging.AddConsole();

            builder.Host.UseSerilog();

            // Add services to the container.

            builder.Services.AddControllers(o => {
                // o.ReturnHttpNotAcceptable = true;
            })
                .AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters();


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(_ => {
                // license goes here
            }, typeof(Program));

            // add the FileExtensionContentTypeProvider as a singleton service to be injected into the FilesController
            builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
            //builder.Services.AddScoped<IEmailService, DevelopmentEmailService>();

#if DEBUG
            builder.Services.AddTransient<IEmailService, DevelopmentEmailService>();
#else
            builder.Services.AddTransient<IEmailService, ProductionEmailService>();
#endif
            //builder.Services.AddScoped<IEmailService, ProductionEmailService>();

            builder.Services.AddScoped<ICityRepository, CityRepository>();
            builder.Services.AddScoped<ILandMarkRepository, LandMarkRepository>();

            // Inject MyMainContext as a service to be used in the controllers
            builder.Services.AddDbContext<MyMainContext>(opt =>
                opt.UseSqlite("Data Source=MyCities.db"));


            builder.Services.AddProblemDetails();

            //builder.Services.AddProblemDetails(o => {
            //    o.CustomizeProblemDetails = (ctx) => {
            //        ctx.ProblemDetails.Extensions.Add("ApplicationName", "My Demo API");
            //        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToUpper() == "DEVELOPMENT") {
            //            ctx.ProblemDetails.Extensions.Add("MachineName", Environment.MachineName);
            //        }
            //    };
            //});



            builder.Services.AddAuthentication("Bearer").AddJwtBearer(options => {
                options.TokenValidationParameters = new() {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Authentication:Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Authentication:SecretKey"]
                            ?? throw new ArgumentNullException("Secret key not found in settings"))),

                    ValidateLifetime = true,
                    //ClockSkew = TimeSpan.Zero

                    RequireExpirationTime = true,
                    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
                };
            });

            builder.Services.AddAuthorization(options => {
                options.AddPolicy("AdminOnly", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("IsAdmin", "Admin");
                });
                options.AddPolicy("CanGetBulk", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("BulkEnabled", "True");
                });
            });


            builder.Services.AddApiVersioning(setupAction => {
                setupAction.ReportApiVersions = true;
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new ApiVersion(1);
            }).AddMvc();


            // ---------------------------------------------------------------------------------------------
            // Pipeline below
            // ---------------------------------------------------------------------------------------------


            var app = builder.Build();

            //app.Run(async (context) => {
            //    await context.Response.WriteAsync("<html><body><b>Hello World!</b></body></html>");
            //});

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                using (var scope = app.Services.CreateScope()) {
                    var ctx = scope.ServiceProvider.GetRequiredService<MyMainContext>();
                    ctx.Database.Migrate();
                }


                app.MapOpenApi();

                app.UseSwagger();
                app.UseSwaggerUI();
            } else {
                app.UseExceptionHandler();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            //app.Run(async (context) => {
            //    await context.Response.WriteAsync("<html><body><b>Login:...</b></body></html>");
            //});

            app.Run();
        }
    }

    //internal class Class1 {
    //    private readonly MyLogger _logger;

    //    public Class1(MyLogger logger) {
    //        _logger = logger;

    //        Child = new Class3(logger);
    //    }

    //    public int ID { get; set; }

    //    public string Name { get; set; }

    //    public void DoSomething() {
    //        Console.WriteLine("Something");
    //        _logger.Write("Did something in Class1");
    //    }

    //    public Class3 Child { get; set; };
    //}

    //internal class Class2 {
    //    private readonly MyLogger _logger;

    //    public Class2(MyLogger logger) {
    //        _logger = logger;
    //    }

    //    public int ID { get; set; }

    //    public string Name { get; set; }

    //    public void DoSomething() {
    //        Console.WriteLine("Something");
    //        _logger.Write("Did something in Class2");
    //    }
    //}


    //internal class Class3 {
    //    private readonly MyLogger _logger;

    //    public Class3(MyLogger logger) {
    //        _logger = logger;
    //    }

    //    public int ID { get; set; }

    //    public string Name { get; set; }

    //    public void DoSomething() {
    //        Console.WriteLine("Something");
    //        _logger.Write("Did something in Class2");
    //    }
    //}


    //internal class MyLogger {
    //    public void Write(string message) {
    //        Console.WriteLine(message);
    //    }
    //}
}
