using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrderServices.Domain.Interface;
using OrderServices.Infrastructure.Data;
using OrderServices.Infrastructure.Repository;
using OrderServices.Services;
using OrderServices.Services.Abstract;
using OrderServices.Services.Concrete;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

;

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddHttpClient("ProductService")
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };
    });


builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Order Service",
        Description = "Stock service for product stocks  "
    });
});

// Add DbContext for PostgreSQL
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderDbConnection")));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


builder.Services.AddScoped<IRequestContextService, RequestContextService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();


}
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
});



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scoup = app.Services.CreateScope())
{

    using (var context = scoup.ServiceProvider.GetRequiredService<OrderDbContext>())
    {
        if (!context.Database.CanConnect())
        {
            context.Database.EnsureCreated();
        }
    }
}

app.Run();
