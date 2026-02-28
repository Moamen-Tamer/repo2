using System.Data.Common;
using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Patients
{
    public class NutritionAdvice: Panel
    {
        private Patient currentPatient;
        private DataGridView gridAppointments = null!;
        private DataGridView gridDietPlan = null!;
        public NutritionAdvice(Patient patient)
        {
            this.currentPatient = patient;
            this.BackColor = Theme.Background;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(30);

            setupLayout();
            loadNutritionAppointments();
        }

        private void setupLayout()
        {
            
        }

        private void loadNutritionAppointments()
        {
            gridAppointments.Rows.Clear();

            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT a.AppointmentID, d.Fullname AS DoctorName, a.AppDateTime, a.Status, a.Note
                                 FROM Appointments a
                                 JOIN Doctors d ON a.DoctorID = d.DoctorID
                                 WHERE a.PatientID = @pid AND d.Specialization = 'Nutritionist'
                                 ORDER BY a.AppDateTime DESC";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@pid", currentPatient.patientID);

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int row = gridAppointments.Rows.Add(
                        (int)reader["AppointmentID"],
                        (string)reader["DoctorName"],
                        ((DateTime)reader["AppDateTime"]).ToString("dd/MM/yyyy  hh:mm tt"),
                        (string)reader["Status"],
                        reader["Note"] == DBNull.Value ? string.Empty : (string)reader["Note"]
                    );

                    string status = (string)reader["Status"];

                    gridAppointments.Rows[row].DefaultCellStyle.ForeColor = status switch
                    {
                        "Confirmed" => Theme.Success,
                        "Cancelled" => Theme.Danger,
                        "Pending"   => Theme.Warning,
                        _           => Theme.TextPrimary
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadDietPlan()
        {
            gridDietPlan.Rows.Clear();

            if (gridAppointments.SelectedRows.Count == 0) return;

            int appID = (int)gridAppointments.SelectedRows[0].Cells["AppointmentID"].Value!;

            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT PlanTitle, Goals, Status, ReviewDate, Notes
                                 FROM DietPlans
                                 WHERE AppointmentID = @aid AND PatientsID = @pid
                                 ORDER BY CreatedAt DESC";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@aid", appID);
                cmd.Parameters.AddWithValue("@pid", currentPatient.patientID);

                using SqlDataReader reader = cmd.ExecuteReader();

                bool found = false;

                while (reader.Read())
                {
                    found = true;
                    gridDietPlan.Rows.Add(
                        (string)reader["PlanTitle"],
                        reader["Goals"] == DBNull.Value ? string.Empty : (string)reader["Goals"],
                        (string)reader["Status"],
                        reader["ReviewDate"] == DBNull.Value ? "Not Set" : ((DateTime)reader["ReviewDate"]).ToString("dd/MM/yyyy"),
                        reader["Notes"] == DBNull.Value ? string.Empty : (string)reader["Notes"]
                    );
                }

                if (!found)
                {
                    gridDietPlan.Rows.Add("No diet plan yet", "", "", "", "");
                    gridDietPlan.Rows[0].DefaultCellStyle.ForeColor = Theme.TextMuted;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading diet plan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}