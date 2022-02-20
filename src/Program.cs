using BackendSkeleton.Extensions;
using BackendSkeleton.Options;
using BackendSkeleton.Services;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Models;
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

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateBootstrapLogger();

builder.Services.AddProblemDetails(opts =>
{
	opts.OnBeforeWriteDetails = ((ctx, pr) =>
	{
		pr.Instance = $"{ctx.Request.Path}{ctx.Request.QueryString}";
		Log.Error(pr.Detail);
	});
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
	{
		opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Backend Skeleton", Version = "v1", Description = "Backend Skeleton OpenAPI" });
		opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
	}
);
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

builder.Host.UseSerilog((ctx, lc) => lc
		.WriteTo.Console()
		.ReadFrom.Configuration(ctx.Configuration));

// Options configuration
builder.Services.ConfigureApplicationOptions<ApplicationOptions>(ApplicationOptions.Application);

builder.Services.AddHealthChecks();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();

app.UseProblemDetails();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend Skeleton"));

app.MapControllers();

app.MapHealthChecks("/health/ready", new HealthCheckOptions()
{
	Predicate = (check) => check.Tags.Contains("ready"),
});

app.MapHealthChecks("/health/live", new HealthCheckOptions()
{
	Predicate = (_) => false
});

Log.Information("Starting service...");

app.Run();
