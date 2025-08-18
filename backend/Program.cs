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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "RedMujer API", Version = "v1" });
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


builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// builder.Services.AddScoped<IEmprendimientoRepository, EmprendimientoRepository>();
// builder.Services.AddScoped<IEmprendimientoService, EmprendimientoService>();

// builder.Services.AddScoped<IRegionRepository, RegionRepository>();
// builder.Services.AddScoped<IRegionService, RegionService>();

// builder.Services.AddScoped<IComunaRepository, ComunaRepository>();
// builder.Services.AddScoped<IComunaService, ComunaService>();

// builder.Services.AddScoped<IMultimediaRepository, MultimediaRepository>();
// builder.Services.AddScoped<IMultimediaService, MultimediaService>();

// builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
// builder.Services.AddScoped<IUbicacionService, UbicacionService>();

// builder.Services.AddScoped<IPersonaRepository, PersonaRepository>();
// builder.Services.AddScoped<IPersonaService, PersonaService>();

// builder.Services.AddScoped<IContactoRepository, ContactoRepository>();
// builder.Services.AddScoped<IContactoService, ContactoService>();

// builder.Services.AddScoped<IRegistroRepository, RegistroRepository>();
// builder.Services.AddScoped<IRegistroService, RegistroService>();

// builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
// builder.Services.AddScoped<ICategoriaService, CategoriaService>();

// builder.Services.AddScoped<IEmprendimientoCategoriaRepository, EmprendimientoCategoriaRepository>();
// builder.Services.AddScoped<IEmprendimientoCategoriaService, EmprendimientoCategoriaService>();

// builder.Services.AddScoped<IEmprendimientoUbicacionRepository, EmprendimientoUbicacionRepository>();
// builder.Services.AddScoped<IEmprendimientoUbicacionService, EmprendimientoUbicacionService>();

// builder.Services.AddScoped<IPersonaEmprendimientoRepository, PersonaEmprendimientoRepository>();
// builder.Services.AddScoped<IPersonaEmprendimientoService, PersonaEmprendimientoService>();

// builder.Services.AddScoped<IPlataformaRepository, PlataformaRepository>();
// builder.Services.AddScoped<IPlataformaService, PlataformaService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

// Orden correcto: CORS antes que Auth, después Swagger y archivos estáticos
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RedMujer API V1");
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