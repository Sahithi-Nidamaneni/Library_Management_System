using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Data.SqlClient;

namespace Library_Management.Helpers
{
	public class DbHelper
	{
		private readonly string _connectionString;

		public DbHelper(IConfiguration configuration)
		{
			// ============================
			// ✅ OPTION 1: USING KEY VAULT (RECOMMENDED)
			// ============================

			/*
            string keyVaultUrl = "https://studentkey01.vault.azure.net/";
            string secretName = "SqlConnectionString";

            // ✅ Uses Managed Identity (best practice)
            var credential = new DefaultAzureCredential();

            var secretClient = new SecretClient(new Uri(keyVaultUrl), credential);

            var secret = secretClient.GetSecret(secretName);

            // ✅ Add increased timeout for serverless DB
            _connectionString = secret.Value + ";Connect Timeout=60;";
            */


			// ============================
			// ✅ OPTION 2: NORMAL CONFIG (CURRENTLY USED)
			// ============================

			_connectionString = configuration.GetConnectionString("DefaultConnection");
		}

		// ============================
		// ✅ CONNECTION METHOD WITH RETRY (IMPORTANT)
		// ============================
		public SqlConnection GetConnection()
		{
			var connection = new SqlConnection(_connectionString);

			int retries = 5;

			while (true)
			{
				try
				{
					connection.Open();  // ✅ Always open here
					break;
				}
				catch (SqlException)
				{
					if (--retries == 0)
						throw;

					Thread.Sleep(5000); // wait 5 seconds before retry
				}
			}

			return connection;
		}
	}
}
