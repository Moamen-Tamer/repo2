using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class User
    {
        public enum RoleType
        {
            Patient,
            Doctor
        }
        public int userID {get; set;}
        public string username {get; set;} = string.Empty;
        public string password {get; set;} = string.Empty;
        public RoleType role {get; set;}
        public DateTime createdAt {get; set;} = DateTime.Now;

        public static User FromReader(SqlDataReader reader)
        {
            User user = new User
            {
                userID = (int)reader["UserID"],
                username = (string)reader["Username"],
                password = (string)reader["Password"],
                createdAt = (DateTime)reader["CreatedAt"]
            };

            if (Enum.TryParse<RoleType>((string)reader["Role"], out var Role))
            {
                user.role = Role;
            }
            else
            {
                throw new Exception("Invalid role value in database.");
            }

            return user;
        }
    }
}