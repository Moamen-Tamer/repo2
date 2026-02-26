using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class Patient
    {
        public enum GenderType
        {
            Male,
            Female
        }
        public int patientID {get; set;}
        public int userID {get; set;}
        public string fullname {get; set;} = string.Empty;
        public DateTime dateOfBirth {get; set;}
        public GenderType gender {get; set;}
        public string? phone {get; set;} = string.Empty;
        public string bloodType {get; set;} =  string.Empty;
        public string? address {get; set;} = string.Empty;

        public static Patient FromReader(SqlDataReader reader)
        {
            Patient patient = new Patient
            {
                patientID = (int)reader["PatientID"],
                userID = (int)reader["UserID"],
                fullname = (string)reader["Fullname"],
                dateOfBirth = (DateTime)reader["DateOfBirth"],
                phone = reader["Phone"] as string,
                bloodType = (string)reader["BloodType"],
                address = reader["Address"] as string
            };

            if (Enum.TryParse<GenderType>((string)reader["Gender"], out var Gender))
            {
                patient.gender = Gender;
            }

            return patient;
        }
    }
}