using Microsoft.Data.SqlClient;

namespace HospitalApp.Database
{
    public static class DBConnection
    {
        private static string conn = "Server=MOAMEN\\SQLEXPRESS;Database=HospitalDB;Trusted_Connection=True;TrustServerCertificate=True;";
        public static SqlConnection connectDB()
        {
            return new SqlConnection(conn);
        }
    }
}