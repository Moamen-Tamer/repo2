using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class Record
    {
        public int recordID {get; set;}
        public int patientID {get; set;}
        public int doctorID {get; set;}
        public int admissionID {get; set;}
        public DateTime recordDate {get; set;}
        public string diagnosis {get; set;} = string.Empty;
        public string? notes {get; set;}
        public Doctor? doctor {get; set;}

        public static Record FromReader(SqlDataReader reader)
        {
            return new Record
            {
                recordID = (int)reader["RecordID"],
                patientID = (int)reader["PatientID"],
                admissionID = (int)reader["AdmissionID"],
                recordDate = (DateTime)reader["RecordDate"],
                diagnosis = (string)reader["Diagnosis"],
                notes = reader["Notes"] as string,
                doctor = new Doctor
                {
                    doctorID = (int)reader["DoctorID"],
                    fullname = (string)reader["Fullname"]    
                }
            };
        }
    }
}