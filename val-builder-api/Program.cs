using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using val_builder_api.Data;
using val_builder_api.Services;
using val_builder_api.Services.Impl;

[ExcludeFromCodeCoverage]
public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Register services
        builder.Services.AddScoped<ICompanyService, CompanyService>();
        builder.Services.AddScoped<IValSectionService, ValSectionService>();
        builder.Services.AddScoped<IValHeaderService, ValHeaderService>();
        builder.Services.AddScoped<ICompanyPlanService, CompanyPlanService>();
        builder.Services.AddScoped<IValTemplateItemService, ValTemplateItemService>();
        builder.Services.AddScoped<IValDetailService, ValDetailService>();
        builder.Services.AddScoped<IValPdfService, ValPdfService>();
        builder.Services.AddScoped<IValPdfAttachmentService, ValPdfAttachmentService>();
        builder.Services.AddScoped<IValAnnotationService, ValAnnotationService>();

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowApp", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:5173",  // Vite HTTP
                        "https://localhost:5173", // Vite HTTPS
                        "http://localhost:3000",  // Alternative port HTTP
                        "https://localhost:3000") // Alternative port HTTPS
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

        // Only redirect to HTTPS in production, not in development
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}