using DotnetBackendSkeleton.Extensions;
using DotnetBackendSkeleton.Metrics;
using DotnetBackendSkeleton.Middlewares;
using DotnetBackendSkeleton.Options;
using DotnetBackendSkeleton.Repositories;
using DotnetBackendSkeleton.Services;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;

#pragma warning disable CA1852

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
		opt.SwaggerDoc("v1", new OpenApiInfo
		{
			Title = "Todo API",
			Version = "v1",
			Description = "Todo OpenAPI",
			TermsOfService = new Uri("https://example.com/terms"),
			Contact = new OpenApiContact
			{
				Name = "Contact",
				Url = new Uri("https://example.com/contact")
			},
			License = new OpenApiLicense
			{
				Name = "License",
				Url = new Uri("https://example.com/license")
			}
		});

		var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
		opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
	}
);

builder.Host.UseSerilog((ctx, lc) => lc
		.Enrich.FromLogContext()
		.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
		.ReadFrom.Configuration(ctx.Configuration));

// Options configuration
builder.Services.ConfigureOptions<ApplicationOptions>(ApplicationOptions.Application);

builder.Services.AddDbContext<TodoItemContext>();
builder.Services.AddHttpClient<ITodoService, TodoService>();

builder.Services.AddSingleton<MetricCollector>();

builder.Services.AddHealthChecks().AddApplicativeHealthChecks();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();
app.UseProblemDetails();

app.UseStatusCodePages();

app.UseSwagger(c =>
{
	c.PreSerializeFilters.Add((swagger, httpReq) =>
	{
		swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}", Description = httpReq.Host.Value.Contains("localhost", StringComparison.OrdinalIgnoreCase) ? "Local development environment" : "Production environment" } };
	});
});

app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dotnet Backend Skeleton");
	c.RoutePrefix = string.Empty;
});

app.UseMetricServer();
app.UseHttpMetrics();

app.MapControllers();
app.MapMetrics();

app.MapHealthChecks();

app.UseMiddleware<ResponseMetricMiddleware>();

Log.Information("Starting service...");

app.Run();
