using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using miminal.Data;
using miminal.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Configuração simples do JWT (para exemplo). Em produção, guarde o segredo em variáveis de ambiente ---
builder.Configuration["Jwt:Key"] = builder.Configuration["Jwt:Key"] ?? "minha-chave-secreta-super-segura-123!";
builder.Configuration["Jwt:Issuer"] = builder.Configuration["Jwt:Issuer"] ?? "VeiculosApi";
builder.Configuration["Jwt:Audience"] = builder.Configuration["Jwt:Audience"] ?? "VeiculosApiClients";

// --- EF InMemory (substitua por SQLite/SQLServer em produção) ---
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("VeiculosDb"));

// --- Token service ---
builder.Services.AddSingleton<TokenService>();

// --- Authentication / JWT ---
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true
    };
});

// --- Authorization policy para "Admin" ---
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AdminPolicy", policy => policy.RequireClaim("role", "admin"));
});

// --- Swagger (com suporte a Bearer token) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Veiculos API", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe 'Bearer {token}'"
    };
    c.AddSecurityDefinition("bearerAuth", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { securityScheme, new[] { "bearerAuth" } }
    });
});

var app = builder.Build();

// --- Seed inicial (um admin de exemplo) ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Admins.Add(new Models.Admin { Id = 1, Email = "adm@teste.com", Password = "123456", Name = "Administrador" });
    db.SaveChanges();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// --- Minimal endpoints ---

// Endpoint público
app.MapGet("/", () => Results.Ok("Veiculos API - Minimal API is running"));

// Login (gera JWT)
app.MapPost("/auth/login", async (DTOs.LoginDTO dto, AppDbContext db, TokenService tokenService) =>
{
    var admin = await db.Admins.FirstOrDefaultAsync(a => a.Email == dto.Email && a.Password == dto.Senha);
    if (admin == null) return Results.Unauthorized();
    
    var token = tokenService.GenerateToken(admin);
    return Results.Ok(new { token });
});

// Rotas de veículos (GET públicas)
app.MapGet("/vehicles", async (AppDbContext db) =>
{
    var list = await db.Vehicles.ToListAsync();
    return Results.Ok(list);
});

app.MapGet("/vehicles/{id:int}", async (int id, AppDbContext db) =>
{
    var v = await db.Vehicles.FindAsync(id);
    return v is null ? Results.NotFound() : Results.Ok(v);
});

// Rotas protegidas: criar, atualizar e deletar -> exigem AdminPolicy
app.MapPost("/vehicles", async (DTOs.VehicleDTO dto, AppDbContext db) =>
{
    var vehicle = new Models.Vehicle
    {
        Make = dto.Make,
        Model = dto.Model,
        Year = dto.Year,
        Plate = dto.Plate
    };
    db.Vehicles.Add(vehicle);
    await db.SaveChangesAsync();
    return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
}).RequireAuthorization("AdminPolicy");

app.MapPut("/vehicles/{id:int}", async (int id, DTOs.VehicleDTO dto, AppDbContext db) =>
{
    var vehicle = await db.Vehicles.FindAsync(id);
    if (vehicle == null) return Results.NotFound();
    vehicle.Make = dto.Make;
    vehicle.Model = dto.Model;
    vehicle.Year = dto.Year;
    vehicle.Plate = dto.Plate;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization("AdminPolicy");

app.MapDelete("/vehicles/{id:int}", async (int id, AppDbContext db) =>
{
    var vehicle = await db.Vehicles.FindAsync(id);
    if (vehicle == null) return Results.NotFound();
    db.Vehicles.Remove(vehicle);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization("AdminPolicy");

app.Run();

