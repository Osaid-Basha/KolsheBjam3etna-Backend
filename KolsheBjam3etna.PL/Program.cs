using KolsheBjam3etna.BLL.Service.Class;
using KolsheBjam3etna.BLL.Service.Implementation;
using KolsheBjam3etna.BLL.Service.Implementations;
using KolsheBjam3etna.BLL.Service.Interface;
using KolsheBjam3etna.DAL.Data;
using KolsheBjam3etna.DAL.Models;
using KolsheBjam3etna.DAL.Repository.Class;
using KolsheBjam3etna.DAL.Repository.Implementations;
using KolsheBjam3etna.DAL.Repository.Interface;
using KolsheBjam3etna.DAL.Utils;
using KolsheBjam3etna.PL.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace KolsheBjam3etna.PL
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:3000",
                            "http://localhost:5173",
                             "http://localhost:8081",
                              "http://localhost:8082"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var jwtSettings = builder.Configuration.GetSection("Jwt");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings["Key"])
                    )
                };
            });

            builder.Services.AddScoped<ISeedData, RoleSeedData>();
            builder.Services.AddScoped<ISeedData, UserSeedData>();
            builder.Services.AddScoped<EmailService>();
            builder.Services.AddScoped<ISeedData, UniversitySeedData>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ILocalFileStorageService, LocalFileStorageService>();
            builder.Services.AddScoped<IUpdateProfileService, UpdateProfileService>();
            builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddSignalR();
            builder.Services.AddScoped<IChatService, ChatService>();
            builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
            builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
            builder.Services.AddScoped<IProductAdRepository, ProductAdRepository>();
            builder.Services.AddScoped<IProductAdService, ProductAdService>();
            builder.Services.AddScoped<ISwapAdRepository, SwapAdRepository>();
            builder.Services.AddScoped<ISwapAdService, SwapAdService>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IEventPublicRepository, EventPublicRepository>();
            builder.Services.AddScoped<IEventPublicService, EventPublicService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IAdminRolesService, AdminRolesService>();
            builder.Services.AddScoped<IOfferRepository, OfferRepository>();
            builder.Services.AddScoped<IOfferService, OfferService>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<INewsRepository, NewsRepository>();
            builder.Services.AddScoped<INewsService, NewsService>();
            builder.Services.AddScoped<IPartnerOfferRepository, PartnerOfferRepository>();
            builder.Services.AddScoped<IPartnerOfferService, PartnerOfferService>();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<IAIService, AIService>();
            builder.Services.AddScoped<IAdminClubsService, AdminClubsService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<ChatHub>("/hubs/chat");
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seedData = services.GetService<IEnumerable<ISeedData>>();
                foreach (var seed in seedData)
                {
                    await seed.Seed();
                }
            }

            app.Run();
        }
    }
}