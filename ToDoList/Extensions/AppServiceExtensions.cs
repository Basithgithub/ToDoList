using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TodoList.DTOs.CloudinarySettings;
using TodoList.Services;
using TodoList.Services.Contracts;
using ToDoList.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace TodoList.Extensions
{
    public static class AppServiceExtensions
    {
        public static IServiceCollection AddApplicationExtensions(this IServiceCollection services, IConfiguration config)
        {
            //JWT implementation
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
             {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                 ValidateIssuer = true,
                 ValidateAudience = true,
                 ValidateLifetime = true,
                 ValidateIssuerSigningKey = true,
                 ValidIssuer = "test-issuer",
                 ValidAudience = "test-audience",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.TOKEN_KEY))
                   };
             });

            services.AddAuthorization();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });
            //DbConnection
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<TodoListDBContext>(options => options.UseSqlServer(connectionString));

            services.AddHttpContextAccessor();
            //Dependency Injection of current iser
            services.AddSingleton(typeof(CurrentUser));
            services.AddScoped(typeof(TokenService));
            services.AddScoped<IPhotoService, PhotoService>();  //Cloudinary implementation
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITodoService, TodoService>();   //Cloudinary implementation
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));   //Cloudinary implementation

            return services;
        }
    }
}
