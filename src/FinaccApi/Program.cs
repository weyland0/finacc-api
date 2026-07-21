using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using FinaccApi.DataAccess;
using FinaccApi.Shared.Infrastructure.Jwt;
using FinaccApi.Shared.Application.Auth;
using FinaccApi.Shared.Infrastructure.Auth;
using FinaccApi.Modules.Access;
using FinaccApi.Modules.Organizations;
using FinaccApi.Modules.Categories;

var builder = WebApplication.CreateBuilder(args);

// Регистрирует встроенный генератор OpenAPI-документа.
// В development-режиме ниже он будет доступен по адресу /openapi/v1.json.
builder.Services.AddOpenApi();

// Регистрирует метаданные об API endpoints.
// Swagger использует эти данные, чтобы понять, какие маршруты, параметры и DTO есть в приложении.
builder.Services.AddEndpointsApiExplorer();

// Регистрирует генератор Swagger-документа от Swashbuckle.
// Этот документ затем отображается в Swagger UI.
builder.Services.AddSwaggerGen(options =>
{
    // Описание версии API, которое будет показано в Swagger UI.
    // Имя "v1" также используется в URL: /swagger/v1/swagger.json.
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Finacc API",
        Version = "v1"
    });

    // Описывает JWT Bearer-авторизацию для Swagger.
    // Благодаря этому в Swagger UI появляется кнопка Authorize.
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Paste JWT token without the Bearer prefix.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    });

    // Говорит Swagger-документу использовать описанную выше Bearer-схему.
    // После авторизации Swagger UI будет добавлять JWT в заголовок Authorization.
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

// Регистрирует EF Core DbContext для работы с PostgreSQL.
// UseSnakeCaseNamingConvention приводит имена таблиц и колонок к snake_case.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        .UseSnakeCaseNamingConvention());

// Привязывает секцию Jwt из appsettings*.json к классу JwtOptions.
// После этого JwtOptions можно получать через IOptions<JwtOptions>.
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(JwtOptions.SectionName));

// Читает JWT-настройки при старте приложения.
// Если секция Jwt не настроена, приложение падает сразу, а не во время первого запроса.
var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT options are not configured.");

// Создает ключ, которым приложение будет проверять подпись входящих JWT-токенов.
var signingKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes(jwtOptions.Secret));

// Регистрирует JWT Bearer-аутентификацию.
// Аутентификация отвечает на вопрос: "Кто сделал запрос?"
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Правила проверки JWT-токена из заголовка Authorization: Bearer <token>.
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Проверяем, что токен выпущен ожидаемым issuer.
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            // Проверяем, что токен предназначен для ожидаемой audience.
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            // Проверяем цифровую подпись токена.
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            // Проверяем срок действия токена.
            ValidateLifetime = true,

            // Небольшой допуск на расхождение времени между сервером и клиентом.
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var userIdClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(userIdClaim, out var userId) || userId <= 0)
                {
                    context.Fail("Token does not contain a valid user id.");
                }

                return Task.CompletedTask;
            }
        };
    });

// Регистрирует сервисы авторизации.
// Авторизация отвечает на вопрос: "Имеет ли пользователь право выполнить это действие?"
builder.Services.AddAuthorization();

// Регистрирует MVC/Web API контроллеры, помеченные атрибутами вроде [ApiController].
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

// Позволяет сервисам получать доступ к текущему HTTP-запросу через IHttpContextAccessor.
builder.Services.AddHttpContextAccessor();

// Регистрирует ICurrentUser, который использует HttpContext
// для получения информации о текущем пользователе.
builder.Services.AddScoped<ICurrentUser, CurrentUserWithHttpContext>();

// Регистрирует зависимости модуля Access: handlers, сервисы и другую инфраструктуру модуля.
builder.Services.AddAccessModule(builder.Configuration);

builder.Services.AddOrganizationsModule(builder.Configuration);

builder.Services.AddCategoriesModule(builder.Configuration);


var app = builder.Build();

// Включает OpenAPI/Swagger только в development-режиме,
// чтобы служебная документация API не публиковалась наружу без явного решения.
if (app.Environment.IsDevelopment())
{   
    // Публикует встроенный OpenAPI-документ по адресу /openapi/v1.json.
    app.MapOpenApi();

    // Публикует Swagger JSON по адресу /swagger/v1/swagger.json.
    app.UseSwagger();

    // Публикует интерактивную страницу Swagger UI по адресу /swagger.
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Finacc API v1");
    });
}

// Перенаправляет HTTP-запросы на HTTPS.
app.UseHttpsRedirection();

// Сначала аутентификация: приложение определяет пользователя по JWT-токену.
app.UseAuthentication();

// Затем авторизация: приложение проверяет права пользователя на конкретный endpoint.
app.UseAuthorization();

// Подключает маршруты контроллеров к HTTP pipeline.
app.MapControllers();

// Запускает веб-приложение и начинает принимать HTTP-запросы.
app.Run();
