using Microsoft.Extensions.Configuration;

namespace eWAY.Rapid.Tests.IntegrationTests
{
    public abstract class SdkTestBase
    {
        static SdkTestBase()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
            var props = builder.GetSection("settings");
            PASSWORD = props["PASSWORD"];
            APIKEY = props["APIKEY"];
            ENDPOINT = props["ENDPOINT"];
            APIVERSION = int.Parse(props["APIVERSION"]);
        }

        public static string PASSWORD { get; private set; }
        public static string APIKEY  { get; private set; }
        public static string ENDPOINT  { get; private set; }
        public static int APIVERSION  { get; private set; }

        protected IRapidClient CreateRapidApiClient()
        {
            var client = RapidClientFactory.NewRapidClient(APIKEY, PASSWORD, ENDPOINT);
            client.SetVersion(GetVersion());
            return client;
        }

        protected int GetVersion()
        {
            string version = System.Environment.GetEnvironmentVariable("APIVERSION");
            int v;
            if (version != null && int.TryParse(version, out v))
            {
                return v;
            }
            return APIVERSION;
        }
    }
}
