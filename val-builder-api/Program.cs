using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Services;
using val_builder_api.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IValSectionService, ValSectionService>();
builder.Services.AddScoped<IValHeaderService, ValHeaderService>();
builder.Services.AddScoped<ICompanyPlanService, CompanyPlanService>();
builder.Services.AddScoped<IValTemplateItemService, ValTemplateItemService>();
builder.Services.AddScoped<IValDetailService, ValDetailService>();
builder.Services.AddScoped<IValPdfService, ValPdfService>();
builder.Services.AddScoped<IValPdfAttachmentService, ValPdfAttachmentService>();
builder.Services.AddScoped<IValAnnotationService, ValAnnotationService>();
builder.Services.AddScoped<IBracketMappingService, BracketMappingService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "https://localhost:5173",
                "http://localhost:3000",
                "https://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();