using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace BackendSkeleton.ServiceTests
{
	[TestClass]
	public class TestBase
	{
		public TestContext TestContext { get; set; }
		private static ICompositeService _dockerCompositeService;
		protected static HttpClient _httpClient;

		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext testContext)
		{
			var file = Path.Combine(Directory.GetCurrentDirectory(), "Assets/docker-compose.yml");
			var env = new string[]
			{
				$"APPLICATION_CONNECTION_STRING=connectionstring",
				$"APPLICATION_HTTP_CLIENT_ADDRESS={(string)testContext.Properties["webAppUrl"]}"
			};
			var serviceName = "backendskeleton";
			_dockerCompositeService = new Ductus.FluentDocker.Builders.Builder()
						.UseContainer()
						.UseCompose()
						.FromFile(file)
						.WithEnvironment(env)
						.RemoveOrphans()
						.WaitForHttp(serviceName, "http://localhost:80/health")
						.Build().Start();

			var ep = _dockerCompositeService.Containers.First(x => x.Name.Equals($"assets-{serviceName}-1")).ToHostExposedEndpoint("80/tcp");

			_httpClient = new HttpClient
			{
				BaseAddress = new System.Uri($"http://localhost:{ep.Port}")
			};
		}

		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
			_dockerCompositeService.Remove(true);
			_dockerCompositeService.Dispose();
		}
	}
}
