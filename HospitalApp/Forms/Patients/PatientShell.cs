using HospitalApp.Database;
using HospitalApp.Helpers;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Patients
{
    public class PatientShell: Form
    {
        private User currentUser;
        private Patient currentPatient = null!;
        private Panel contentPanel = null!;
        private Button btnDoctors = null!;
        private Button btnAppointments = null!;
        private Button btnHistory = null!;
        private Button btnViewers = null!;
        private Button btnNutrition = null!;
        private string activePage = string.Empty;
        public PatientShell(User user)
        {
            this.currentUser = user;
            
            loadPatient();
            setupForm();
            setupLayout();
            showPage("Doctors");
        }

        private void setupForm()
        {
            this.Text            = "CareFlow — Patient Portal";
            this.ClientSize      = new Size(1200, 720);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = true;
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.BackColor       = Theme.Background;
        }

        private void setupLayout()
        {
            
        }

        private void loadPatient()
        {
            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT * FROM Patients
                                 WHERE UserID = @uid";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@uid", currentUser.userID);

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    double weight = reader["WeightKg"] == DBNull.Value ? 0 : Convert.ToDouble(reader["WeightKg"]);
                    double height = reader["HeightCm"] == DBNull.Value ? 0 : Convert.ToDouble(reader["HeightCm"]);

                    currentPatient = new Patient
                    {
                        patientID = (int)reader["PatientID"],
                        userID = (int)reader["UserID"],
                        fullname = (string)reader["Fullname"],
                        dateOfBirth = (DateTime)reader["DateOfBirth"],
                        gender = Enum.Parse<Patient.GenderType>((string)reader["Gender"]),
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
            } catch { }

            if (currentPatient == null) ShowNoPatient();
        }

        private void showPage(string page)
        {
            if (activePage == page) return;
            if (currentPatient == null) return;

            activePage = page;
            contentPanel.Controls.Clear();

            Button active = page switch
            {
                "Doctors" => btnDoctors,
                "Appointments" => btnAppointments,
                "History" => btnHistory,
                "Viewers" => btnViewers,
                "Nutrition" => btnNutrition,
                _ => btnDoctors
            };

            UIHelper.SetNavActive(active, btnDoctors, btnAppointments, btnHistory, btnViewers, btnNutrition);

            Control newPage = page switch
            {
                "Doctors" => new DoctorsPage(),
                "Appointments" => new MyAppointments(currentPatient),
                "History" => new MedicalHistory(currentPatient),
                "Viewers" => new ViewersList(currentPatient),
                "Nutrition" => new NutritionAdvice(currentPatient),
                _ => new Panel()
            };

            newPage.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(newPage);
        }

        private void ShowNoPatient()
        {
            if (contentPanel == null) return;

            var lbl = new Label
            {
                Text      = "No patient profile found for this account.\nPlease contact administration.",
                Font      = Theme.FontHeading,
                ForeColor = Theme.TextSecondary,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Theme.Background
            };

            contentPanel.Controls.Add(lbl);
        }
    }
}