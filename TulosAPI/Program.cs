
using DAL;
using Entities.Context;
using Entities.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
            builder.Services.AddDbContext<TulosDbContext>(
    options => options.UseSqlServer(@"Server=(LocalDb)\MSSQLLocalDB;Database=ChatRoomDb;Trusted_Connection=True;TrustServerCertificate=True;"));
            builder.Services.AddAuthorization();
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
                options.AddPolicy("AllowLocalhost3000", builder =>
                {
                    builder.WithOrigins("http://localhost:3000")
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            // Add email sender
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddHttpClient("MailTrapApiClient", (services, client) =>
            {
                var mailSettings = services.GetRequiredService<IOptions<MailSettings>>().Value;
                client.BaseAddress = new Uri(mailSettings.ApiBaseUrl);
                client.DefaultRequestHeaders.Add("Api-Token", mailSettings.ApiToken);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Apply CORS policy
            app.UseCors("AllowLocalhost3000");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            app.MapIdentityApi<ApplicationUser>();
            app.Run();
        }
    }
}
