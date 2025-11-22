using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// project packages
using NeuroPuentesAPI.repositories;
using NeuroPuentesAPI.services;

var builder = WebApplication.CreateBuilder(args);

// === AUTENTICACIÓN JWT ===
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("Jwt:Key no está configurado en la configuración.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NeuroPuentes API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando el esquema Bearer.   
                          Escribe 'Bearer' [espacio] y tu token en la caja de texto.   
                          Ejemplo: 'Bearer eyJhbGciOi...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// ==========================================================
// === ¡ESTE ES EL BLOQUE IA NO TOCAR ! ===
// ==========================================================
builder.Services.AddHttpClient("IaApiClient", client =>
{
    // Lee la URL de tu API de Python desde appsettings.json
    string? baseUrl = builder.Configuration["IaApiBaseUrl"]; 
    if (string.IsNullOrEmpty(baseUrl))
    {
        baseUrl = "http://127.0.0.1:5001"; 
    }
    client.BaseAddress = new Uri(baseUrl);
    
    // Aumentamos el tiempo de espera a 5 minutos (para que la IA piense)
    client.Timeout = TimeSpan.FromMinutes(10); 
});


builder.Services.AddScoped<IIaApiService, IaApiService>();

builder.Services.AddScoped<IEntrevistaRepository, EntrevistaRepository>();
builder.Services.AddScoped<IEntrevistaService, EntrevistaService>();

builder.Services.AddScoped<IDialogoRepository, DialogoRepository>();
builder.Services.AddScoped<IDialogoService, DialogoService>();

builder.Services.AddScoped<IEvalEntrevistaRepository, EvalEntrevistaRepository>();
builder.Services.AddScoped<IEvalEntrevistaService, EvalEntrevistaService>();

builder.Services.AddScoped<IEvalCategoriaRepository, EvalCategoriaRepository>();
builder.Services.AddScoped<IEvalCategoriaService, EvalCategoriaService>();

builder.Services.AddScoped<IFeedbackEntrevistaRepository, FeedbackEntrevistaRepository>();
builder.Services.AddScoped<IFeedbackEntrevistaService, FeedbackEntrevistaService>();

builder.Services.AddScoped<IFeedbackUsuarioRepository, FeedbackUsuarioRepository>();
builder.Services.AddScoped<IFeedbackUsuarioService, FeedbackUsuarioService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IContextoRepository, ContextoRepository>();
builder.Services.AddScoped<IContextoService, ContextoService>();

builder.Services.AddScoped<ICaracteristicaRepository, CaracteristicaRepository>();
builder.Services.AddScoped<ICaracteristicaService, CaracteristicaService>();

builder.Services.AddScoped<IStatsUsuarioRepository, StatsUsuarioRepository>();
builder.Services.AddScoped<IStatsUsuarioService, StatsUsuarioService>();

builder.Services.AddScoped<ITipRepository, TipRepository>();
builder.Services.AddScoped<ITipService, TipService>();

builder.Services.AddScoped<ICaractsRelRepository, CaractsRelRepository>();
builder.Services.AddScoped<ICaractsRelService, CaractsRelService>();

builder.Services.AddScoped<ICalificacionUsuarioRepository, CalificacionUsuarioRepository>();
builder.Services.AddScoped<ICalificacionUsuarioService, CalificacionUsuarioService>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NeuroPuentes API V1");
});

var mediaPath = Path.Combine(Directory.GetCurrentDirectory(), "media");
if (!Directory.Exists(mediaPath))
{
    Directory.CreateDirectory(mediaPath);
}
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(mediaPath),
    RequestPath = "/media"
});

app.MapControllers();
app.Run();