using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class Appointment
    {
        public enum appointmentStatus
        {
            Pending,
            Confirmed,
            Cancelled,
            Completed,
            NoShow
        }

        public int appointmentID {get; set;}
        public int patientID {get; set;}
        public int doctorID {get; set;}
        public DateTime appDateTime {get; set;}
        public appointmentStatus status {get; set;} = appointmentStatus.Pending;
        public string? note {get; set;}

        public static Appointment FromReader(SqlDataReader reader)
        {
            return new Appointment
            {
                appointmentID = (int)reader["AppointmentID"],
                patientID = (int)reader["PatientID"],
                doctorID = (int)reader["DoctorID"],
                appDateTime = (DateTime)reader["AppDateTime"],
                note = reader["Note"] as string,
                status = Enum.TryParse<appointmentStatus>((string)reader["Status"], out var Status) ? Status : appointmentStatus.Pending
            };
        }
    }
}