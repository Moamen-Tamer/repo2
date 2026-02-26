using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Patients
{
    public class Patients: Form
    {
        private User currentUser;
        private Patient currentPatient = null!;
        public Patients(User user)
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
            this.ClientSize      = new Size(1150, 680);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
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
            
        }

        private void ShowNoPatient()
        {
            
        }
    }
}