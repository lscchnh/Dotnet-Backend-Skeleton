using BackendSkeleton.Extensions;
using BackendSkeleton.Metrics;
using BackendSkeleton.Middlewares;
using BackendSkeleton.Options;
using BackendSkeleton.Services;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
	options.OutputFormatters.RemoveType<StringOutputFormatter>();
	options.Filters.Add(new ProducesAttribute("application/json"));
	options.Filters.Add(new ConsumesAttribute("application/json"));
}).AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddProblemDetails(opts =>
{
	opts.MapExceptionsToProblemDetails();
	opts.EnrichWithInstance();
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
	{
		opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend Skeleton", Version = "v1", Description = "Backend Skeleton OpenAPI" });
		opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
	}
);
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddSingleton<MetricCollector>();

builder.Host.UseSerilog((ctx, lc) => lc
		.WriteTo.Console()
		.ReadFrom.Configuration(ctx.Configuration));

// Options configuration
builder.Services.ConfigureOptions<ApplicationOptions>(ApplicationOptions.Application);

builder.Services.AddHealthChecks();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();

app.UseProblemDetails();

app.UseSwagger(c =>
{
	c.PreSerializeFilters.Add((swagger, httpReq) =>
	{
		swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}", Description = httpReq.Host.Value.Contains("localhost", StringComparison.OrdinalIgnoreCase) ? "Local development environment" : "Production environment" } };
	});
});

app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Skeleton"));

app.UseMetricServer();
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics();

app.MapHealthChecks();

app.UseMiddleware<ResponseMetricMiddleware>();

Log.Information("Starting service...");

app.Run();
