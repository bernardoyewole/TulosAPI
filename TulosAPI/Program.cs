
using DAL;
using Entities.Context;
using Entities.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TulosAPI.Services;
using IEmailSender = TulosAPI.Services.IEmailSender;

namespace TulosAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var conStrBuilder = new SqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
            // Replace the password using Secret Manager
            conStrBuilder.UserID = builder.Configuration["DbUserId"];
            conStrBuilder.Password = builder.Configuration["DbPassword"];

            var connectionString = conStrBuilder.ConnectionString; // Get the complete connection string

            // Add DbContext with dynamically built connection string
            builder.Services.AddDbContext<TulosDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddIdentityApiEndpoints<ApplicationUser>().AddEntityFrameworkStores<TulosDbContext>();

            builder.Services.AddScoped<IGenericRepository<ApplicationUser>, GenericRepository<ApplicationUser>>();
            builder.Services.AddScoped<IGenericRepository<Favorite>, GenericRepository<Favorite>>();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            });

            // Add and configure CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.WithOrigins("http://localhost:3000", "https://tulos.bernardoyewole.com")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            // Add email sender
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Configure MailSettings, pulling sensitive values from Secret Manager
            builder.Services.Configure<MailSettings>(mailSettings =>
            {
                builder.Configuration.GetSection("MailSettings").Bind(mailSettings);
                // Use Secret Manager to replace sensitive fields
                mailSettings.ApiToken = builder.Configuration["MailSettings:ApiToken"];
            });

            // Configure HttpClient for MailTrap API with injected secrets
            builder.Services.AddHttpClient("MailTrapApiClient", (services, client) =>
            {
                var mailSettings = services.GetRequiredService<IOptions<MailSettings>>().Value;
                client.BaseAddress = new Uri(mailSettings.ApiBaseUrl);
                client.DefaultRequestHeaders.Add("Api-Token", mailSettings.ApiToken);
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
            }

            // Apply CORS policy
            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.MapIdentityApi<ApplicationUser>();
            app.Run();
        }
    }
}
