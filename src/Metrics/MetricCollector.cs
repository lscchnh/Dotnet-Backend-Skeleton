using Prometheus;
using System.Globalization;

namespace BackendSkeleton.Metrics;
public class MetricCollector
{
	private readonly Counter _requestCounter;
	private readonly Histogram _responseTimeHistogram;

	public MetricCollector()
	{
		_requestCounter = Prometheus.Metrics.CreateCounter("total_requests", "The total number of requests serviced by this API.");

		_responseTimeHistogram = Prometheus.Metrics.CreateHistogram("request_duration_seconds", "The duration in seconds between the response to a request.", new HistogramConfiguration
		{
			Buckets = Histogram.ExponentialBuckets(0.01, 2, 10),
			LabelNames = new[]
			{
				"status_code", "method"
			}
		});
	}

	public void RegisterRequest()
	{
		_requestCounter.Inc();
	}

	public void RegisterResponseTime(int statusCode, string method, TimeSpan elapsed)
	{
		_responseTimeHistogram.Labels(statusCode.ToString(CultureInfo.InvariantCulture), method).Observe(elapsed.TotalSeconds);
	}
}
