using HospitalApp.Database;
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
                    currentPatient = new Patient
                    {
                        patientID = (int)reader["PatientID"],
                        userID = currentUser.userID,
                        fullname = (string)reader["Fullname"],
                        dateOfBirth = (DateTime)reader["DateOfBirth"],
                        gender = Enum.Parse<Patient.GenderType>((string)reader["Gender"]),
                        phone = reader["Phone"] == DBNull.Value ? string.Empty : (string)reader["Phone"],
                        bloodType = (string)reader["BloodType"],
                        address = reader["Address"] == DBNull.Value ? string.Empty : (string)reader["Address"]
                    };
                }
            } catch { }
        }

        private void showPage(string page)
        {
            if (activePage == page) return;

            contentPanel.Controls.Clear();

            UIHelper.SetNavActive(
                page == "Doctors" ? btnDoctors
                : page == "Appointments" ? btnAppointments
                : page == "History" ? btnHistory
                : btnViewers,
                btnDoctors, btnAppointments, btnHistory, btnViewers
            );

            Control newPage = page switch
            {
                "Doctors" => new DoctorsPage(),
                "Appointments" => new MyAppointments(currentPatient),
                "History" => new MedicalHistory(currentPatient),
                "Viewers" => new ViewersList(currentPatient),
                _ => new Panel()
            };

            newPage.Dock = DockStyle.Fill;
            this.Controls.Add(newPage);
        }

        private void ShowNoPatient()
        {
            var lbl = new Label
            {
                Text      = "No patient profile found for this account.\nPlease contact administration.",
                Font      = Theme.FontHeading,
                ForeColor = Theme.TextSecondary,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Theme.Background
            };

            this.Controls.Add(lbl);
        }
    }
}