using Library_Management.Helpers;

using Library_Management.Models;

using Microsoft.Data.SqlClient;

namespace Library_Management.Repositories

{

	public class MembershipRepository

	{

		private readonly DbHelper _dbHelper;

		public MembershipRepository(DbHelper dbHelper)

		{

			_dbHelper = dbHelper;

		}

		// Add Membership — generated ID bhi return karta hai

		public string AddMembership(Membership membership)

		{

			using SqlConnection conn = _dbHelper.GetConnection();

			string query = @"INSERT INTO Memberships
                (UserId, MembershipType, StartDate, EndDate, IsActive, DocumentUrl, Email)
                VALUES
                (@UserId, @MembershipType, @StartDate, @EndDate, @IsActive, @DocumentUrl, @Email);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";


			using SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@UserId", membership.UserId);

			cmd.Parameters.AddWithValue("@MembershipType", membership.MembershipType);

			cmd.Parameters.AddWithValue("@StartDate", membership.StartDate);

			cmd.Parameters.AddWithValue("@EndDate", membership.EndDate);

			cmd.Parameters.AddWithValue("@Email",
				string.IsNullOrEmpty(membership.Email)
				? (object)DBNull.Value
				: membership.Email);


			cmd.Parameters.AddWithValue("@IsActive", membership.IsActive);

			cmd.Parameters.AddWithValue("@DocumentUrl",

				string.IsNullOrEmpty(membership.DocumentUrl)

				? (object)DBNull.Value

				: membership.DocumentUrl);

			//conn.Open();

			var newId = cmd.ExecuteScalar();

			if (newId != null && newId != DBNull.Value)

			{

				membership.MembershipId = Convert.ToInt32(newId);

				return "Membership Added Successfully";

			}

			return "Failed to Add Membership";

		}

		// Get Membership By UserId

		public Membership? GetMembershipByUserId(int userId)

		{

			using SqlConnection conn = _dbHelper.GetConnection();

			string query = "SELECT * FROM Memberships WHERE UserId = @UserId";

			using SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@UserId", userId);

			//conn.Open();

			using SqlDataReader reader = cmd.ExecuteReader();

			if (reader.Read())

			{

				return new Membership

				{

					MembershipId = Convert.ToInt32(reader["MembershipId"]),

					UserId = Convert.ToInt32(reader["UserId"]),

					MembershipType = reader["MembershipType"]?.ToString() ?? string.Empty,

					StartDate = Convert.ToDateTime(reader["StartDate"]),

					EndDate = Convert.ToDateTime(reader["EndDate"]),

					Email = reader["Email"] == DBNull.Value
	? null
	: reader["Email"].ToString(),


					IsActive = Convert.ToBoolean(reader["IsActive"]),

					DocumentUrl = reader["DocumentUrl"] == DBNull.Value

						? null

						: reader["DocumentUrl"].ToString()

				};

			}

			return null;

		}

	}

}
