using HospitalApp.Helpers;
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
        public string phone {get; set;} = string.Empty;
        public string address {get; set;} = string.Empty;
        public BloodType bloodType {get; set;} = BloodType.O_Positive;
        public string bloodTypeDisplay => BloodTypeHelper.display(bloodType);
        public double weightKg {get; set;}
        public double heightCm {get; set;}
        public double bmi {get; set;}
        public int cholesterolMgDl {get; set;}
        public int bpSystolic {get; set;}
        public int bpDiastolic {get; set;}
        public int bloodSugarMgDl {get; set;}
        public CholesterolLevel cholesterolStatus => CholesterolHelper.classify(cholesterolMgDl);
        public BloodPressureStatus bpStatus => BloodPressureHelper.classify(bpSystolic, bpDiastolic);
        public BloodSugarStatus sugarStatus => BloodSugarHelper.classify(bloodSugarMgDl);
        public BmiCategory bmiCategory => BmiHelper.classify(bmi);
        public string medicalNotes {get; set;} = string.Empty;
        public int age => DateTime.Now.Year - dateOfBirth.Year - (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear ? 1 : 0);
        public bool isDiabetic => sugarStatus is BloodSugarStatus.Diabetic or BloodSugarStatus.PreDiabetic;

        public static Patient FromReader(SqlDataReader reader)
        {
            double weight = reader["WeightKg"] == DBNull.Value ? 0 : Convert.ToDouble(reader["WeightKg"]);
            double height = reader["HeightCm"] == DBNull.Value ? 0 : Convert.ToDouble(reader["HeightCm"]);

            return new Patient
            {
                patientID = (int)reader["PatientID"],
                userID = (int)reader["UserID"],
                fullname = (string)reader["Fullname"],
                dateOfBirth = (DateTime)reader["DateOfBirth"],
                gender = Enum.Parse<GenderType>((string)reader["Gender"]),
                phone = reader["Phone"] == DBNull.Value ? string.Empty : (string)reader["Phone"],
                address = reader["Address"] == DBNull.Value ? string.Empty : (string)reader["Address"],
                bloodType = BloodTypeHelper.classify(reader["BloodType"]?.ToString() ?? "unknown"),
                weightKg = weight,
                heightCm = height,
                bmi = BmiHelper.Calculate(weight, height),
                cholesterolMgDl = reader["CholesterolMgDl"] == DBNull.Value ? 0 : (int)reader["CholesterolMgDl"],
                bpSystolic = reader["BpSystolic"] == DBNull.Value ? 0 : (int)reader["BpSystolic"],
                bpDiastolic = reader["BpDiastolic"] == DBNull.Value ? 0 : (int)reader["BpDiastolic"],
                bloodSugarMgDl = reader["BloodSugarMgDl"] == DBNull.Value ? 0 : (int)reader["BloodSugarMgDl"],
                medicalNotes = reader["MedicalNotes"] == DBNull.Value ? string.Empty : (string)reader["MedicalNotes"]
            };
        }
    }
}