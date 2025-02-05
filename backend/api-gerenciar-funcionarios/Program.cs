
using api_gerenciar_funcionarios.Config;
using api_gerenciar_funcionarios.Core.Application;
using api_gerenciar_funcionarios.Core.Domain;
using api_gerenciar_funcionarios.Dtos;
using api_gerenciar_funcionarios.Infrastructure.Persistence;
using api_gerenciar_funcionarios.Infrastructure.Repositories;
using api_gerenciar_funcionarios.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configuração da string de conexão a partir das variáveis de ambiente
var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnection");
Console.WriteLine($"Connection String: {connectionString}");

// Adicionar os serviços necessários, como o Entity Framework, usando a string de conexão
builder.Services.AddDbContext<FuncionarioDbContext>(options =>
    options.UseNpgsql(connectionString));

// Injeção de Dependência
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
builder.Services.AddScoped<CriarFuncionarioUseCase>();
builder.Services.AddScoped<AtualizarFuncionarioUseCase>();
builder.Services.AddScoped<DeletarFuncionarioUseCase>();
builder.Services.AddScoped<ObterFuncionariosUseCase>();
builder.Services.AddScoped<ObterFuncionarioPorIdUseCase>();
builder.Services.AddScoped<IValidator<FuncionarioEntradaDTO>, FuncionarioValidator>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configurações de senha
    options.Password.RequiredLength = 8; // Comprimento mínimo
    options.Password.RequireNonAlphanumeric = true; // Exige caractere especial
    options.Password.RequireDigit = true; // Exige número
    options.Password.RequireLowercase = true; // Exige letra minúscula
    options.Password.RequireUppercase = true; // Exige letra maiúscula
})
.AddEntityFrameworkStores<FuncionarioDbContext>()
.AddErrorDescriber<CustomIdentityErrorDescriber>()
.AddDefaultTokenProviders();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar autenticação JWT
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
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors(policy => policy
    .AllowAnyOrigin() // Permite qualquer origem (ou use WithOrigins para especificar)
    .AllowAnyMethod() // Permite qualquer método HTTP (GET, POST, etc.)
    .AllowAnyHeader()); // Permite qualquer cabeçalho

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
