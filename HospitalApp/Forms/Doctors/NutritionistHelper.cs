using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Doctors
{
    public static class NutritionistHelper
    {
        public static void OpenDietPlan(Doctor doctor, DataGridView grid)
        {
            if (doctor.specialization != "Nutritionist")
            {
                MessageBox.Show("Only Nutritionist doctors can write diet plans.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an appointment.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string status = grid.SelectedRows[0].Cells["Status"].Value?.ToString() ?? "";

            if (status != "Confirmed" && status != "Done")
            {
                MessageBox.Show("You can only write a diet plan for Confirmed or Done appointments.", "Invalid Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int appointmentID = (int)grid.SelectedRows[0].Cells["AppointmentID"].Value!;

            int patientID = 0;

            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();
                string query = @"SELECT PatientID FROM Appointments
                                 WHERE AppointmentID = @aid";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@aid", appointmentID);

                var result = cmd.ExecuteScalar();

                if (result != null) patientID = (int)result;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (patientID == 0) return;

            new DietPlanForm(doctor, patientID, appointmentID).ShowDialog();
        }
    }
}