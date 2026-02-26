using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class Admission
    {
        public enum AdmissionStatus
        {
            Admitted,
            Critical,
            Discharged,
            Cancelled
        }
        public int admissionID {get; set;}
        public int patientID {get; set;}
        public string? fullname {get; set;}
        public int doctorID {get; set;}
        public string? roomNumber {get; set;} = string.Empty;
        public DateTime admittedAt {get; set;} = DateTime.Now;
        public DateTime? expectedLeave {get; set;}
        public DateTime? actualLeave {get; set;} 
        public AdmissionStatus status {get; set;} = AdmissionStatus.Admitted;

        public static Admission FromRead(SqlDataReader reader)
        {
            return new Admission
            {
                admissionID = (int)reader["AdmissionID"],
                patientID = (int)reader["PatientID"],
                doctorID = (int)reader["DoctorID"],
                roomNumber = reader["RoomNumber"] as string,
                admittedAt = (DateTime)reader["AdmittedAt"],
                expectedLeave = reader["ExpectedLeave"] == DBNull.Value ? null : (DateTime)reader["ExpectedLeave"],
                actualLeave = reader["ActualLeave"] == DBNull.Value ? null : (DateTime)reader["ActualLeave"],
                status = Enum.TryParse<AdmissionStatus>((string)reader["Status"], out var status)? status : AdmissionStatus.Admitted
            };
        }
    }
}