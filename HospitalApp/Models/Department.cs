using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class Department
    {
        public int departmentID {get; set;}
        public string departmentName {get; set;} = string.Empty;
        public string description {get; set;} = string.Empty;

        public static Department FromReader(SqlDataReader reader)
        {
            return new Department
            {
                departmentID = (int)reader["DepartmentID"],
                departmentName = (string)reader["DepartmentName"],
                description = (string)reader["Desctiption"]
            };
        }
    }
}