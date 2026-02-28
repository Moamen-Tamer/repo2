using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class Doctor
    {
        public int doctorID {get; set;}
        public int userID {get; set;}
        public string fullname {get; set;} = string.Empty;
        public int departmentID {get; set;}
        public Department? department {get; set;}
        public string specialization {get; set;} = string.Empty;
        public string phone {get; set;} = string.Empty;
        public string email {get; set;} = string.Empty;
        public string? bio {get; set;} = string.Empty;
        public bool isAvailable {get; set;} = true;

        public static Doctor FromReader(SqlDataReader reader)
        {
            return new Doctor
            {
                doctorID = (int)reader["DoctorID"],
                userID = (int)reader["UserID"],
                fullname = (string)reader["Fullname"],
                departmentID = (int)reader["DepartmentID"],
                department = new Department
                {
                    departmentID = (int)reader["DepartmentID"],
                    departmentName = (string)reader["DepartmentName"]
                },
                specialization = (string)reader["Specialization"],
                phone = (string)reader["Phone"],
                email = (string)reader["Email"],
                bio = reader["Bio"] as string,
                isAvailable = (bool)reader["IsAvailable"]
            };
        }
    }
}