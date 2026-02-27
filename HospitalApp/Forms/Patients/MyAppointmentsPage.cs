using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Patients
{
    public class MyAppointments: Panel
    {
        private Patient currentPatient;
        private DataGridView grid = null!;
        private ComboBox cmbFilter = null!;
        public MyAppointments(Patient patient)
        {
            this.currentPatient = patient;
            this.BackColor = Theme.Background;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(30);

            setupLayout();
            loadAppointments("All");
        }

        private void setupLayout()
        {
            
        }

        private void loadAppointments(string filter) 
        {
            grid.Rows.Clear();

            try {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string where = filter == "All" ? "" : "AND a.Status = @status";

                string query = $@"SELECT a.AppointmentID, d.Fullname AS Fullname, a.AppDateTime, a.Status, a.Notes
                                  FROM Appointments a
                                  JOIN Doctors d ON a.DoctorID = d.DoctorID
                                  WHERE a.PatientID = @pid {where}
                                  ORDER BY a.AppDateTime DESC";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@pid", currentPatient.patientID);

                if (filter != "All") cmd.Parameters.AddWithValue("@status", filter);

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) {
                    int row = grid.Rows.Add(
                        (int)reader["AppointmentID"],
                        (string)reader["Fullname"],
                        ((DateTime)reader["AppDateTime"]).ToString("dd/MM/yyyy  hh:mm tt"),
                        (string)reader["Status"],
                        reader["Notes"] == DBNull.Value ? string.Empty : (string)reader["Notes"]
                    );
                    
                    string status = (string)reader["Status"];

                    grid.Rows[row].DefaultCellStyle.ForeColor = status switch {
                        "Confirmed" => Theme.Success,
                        "Cancelled" => Theme.Danger,
                        "Pending"   => Theme.Warning,
                        _           => Theme.TextPrimary
                    };
                }
            } catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelClick(object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;

            string? status = grid.SelectedRows[0].Cells["Status"].Value?.ToString();

            if (status != "Pending") {
                MessageBox.Show("Only Pending appointments can be cancelled.", "Cannot Cancel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)grid.SelectedRows[0].Cells["AppointmentID"].Value!;

            var confirm = MessageBox.Show("Cancel this appointment?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"UPDATE Appointments SET Status = 'Cancelled' 
                                 WHERE AppointmentID = @id";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                loadAppointments(cmbFilter.SelectedItem!.ToString()!);
            } catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}