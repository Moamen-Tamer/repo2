using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class Prescription
    {
        public int prescriptionID {get; set;}
        public int recordID {get; set;}
        public int patientID {get; set;}
        public int doctorID {get; set;}
        public string medicine {get; set;} = string.Empty;
        public string dosage {get; set;} = string.Empty;
        public string duration {get; set;} = string.Empty;
        public DateTime issuedAt {get; set;}

        public static Prescription FromRead(SqlDataReader reader)
        {
            return new Prescription
            {
                prescriptionID = (int)reader["PrescriptionID"],
                recordID = (int)reader["RecordID"],
                patientID = (int)reader["PatientID"],
                doctorID = (int)reader["doctorID"],
                medicine = (string)reader["Medicine"],
                dosage = (string)reader["Dosage"],
                duration = (string)reader["Duration"],
                issuedAt = (DateTime)reader["IssuedAt"],
            };
        }
    }
}