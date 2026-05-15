using Library_Management.Helpers;

using Library_Management.Models;

using Microsoft.Data.SqlClient;

namespace Library_Management.Repositories

{

	public class UserRepository

	{

		private readonly DbHelper _dbHelper;

		public UserRepository(DbHelper dbHelper)

		{

			_dbHelper = dbHelper;

		}

		// Add User

		public string AddUser(User user)

		{

			using SqlConnection conn = _dbHelper.GetConnection();

			string query = @"INSERT INTO Users

                            (FullName, Email, PhoneNumber, Address)

                            VALUES

                            (@FullName, @Email, @PhoneNumber, @Address);

                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

			using SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@FullName", user.FullName);

			cmd.Parameters.AddWithValue("@Email", user.Email);

			cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);

			cmd.Parameters.AddWithValue("@Address", user.Address);

			//conn.Open();

			var newId = cmd.ExecuteScalar();

			if (newId != null && newId != DBNull.Value)

			{

				user.UserId = Convert.ToInt32(newId);

				return "User Added Successfully";

			}

			return "Failed to Add User";

		}

		// Get User By Id

		public User? GetUserById(int userId)

		{

			using SqlConnection conn = _dbHelper.GetConnection();

			string query = "SELECT * FROM Users WHERE UserId = @UserId";

			using SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@UserId", userId);

			//conn.Open();

			using SqlDataReader reader = cmd.ExecuteReader();

			if (reader.Read())

			{

				return new User

				{

					UserId = Convert.ToInt32(reader["UserId"]),

					FullName = reader["FullName"]?.ToString() ?? string.Empty,

					Email = reader["Email"]?.ToString() ?? string.Empty,

					PhoneNumber = reader["PhoneNumber"]?.ToString() ?? string.Empty,

					Address = reader["Address"]?.ToString() ?? string.Empty,

					RegistrationDate = Convert.ToDateTime(reader["RegistrationDate"])

				};

			}

			return null;

		}

		// Get All Users

		public List<User> GetAllUsers()

		{

			List<User> users = new List<User>();

			using SqlConnection conn = _dbHelper.GetConnection();

			string query = "SELECT * FROM Users";

			using SqlCommand cmd = new SqlCommand(query, conn);

			//conn.Open();

			using SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())

			{

				users.Add(new User

				{

					UserId = Convert.ToInt32(reader["UserId"]),

					FullName = reader["FullName"]?.ToString() ?? string.Empty,

					Email = reader["Email"]?.ToString() ?? string.Empty,

					PhoneNumber = reader["PhoneNumber"]?.ToString() ?? string.Empty,

					Address = reader["Address"]?.ToString() ?? string.Empty,

					RegistrationDate = Convert.ToDateTime(reader["RegistrationDate"])

				});

			}

			return users;

		}

	}

}
