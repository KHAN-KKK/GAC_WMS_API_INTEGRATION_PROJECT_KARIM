using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System.Buffers.Text;
using Warehouse_ERPIntegration.API.Data;
using Warehouse_ERPIntegration.API.Mapping;
using Warehouse_ERPIntegration.API.Services;
using Warehouse_ERPIntegration.API.Services.Interface;
using Quartz;
using Warehouse_ERPIntegration.API.Jobs;
using Warehouse_ERPIntegration.API.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<IntegrationDbContext>(options=>options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<ISalesOrderService, SalesOrderService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddHttpClient<IWmsIntegrationService, WmsIntegrationService>();

//6.Quartz.NET(File - Based Legacy Integration Job)
builder.Services.AddQuartz(q =>
{
    var pollInterval = builder.Configuration.GetValue<int>("LegacyIntegration:PollIntervalMinutes");
    var jobKey = new JobKey("LegacyFileIntegrationJob");

    q.AddJob<LegacyFileIntegrationJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("LegacyFileIntegrationJob-trigger")
        .StartNow()
        .WithSimpleSchedule(x => x.WithIntervalInMinutes(pollInterval).RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }

    await next();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
