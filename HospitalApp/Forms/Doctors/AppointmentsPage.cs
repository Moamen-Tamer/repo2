using System;
using System.Drawing;
using System.Windows.Forms;
using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Doctors
{
    public class AppointmentPage: Panel
    {
        private Doctor currentDoctor;
        private DataGridView grid = null!;
        private ComboBox cmbFilter = null!;
        public AppointmentPage(Doctor doctor)
        {
            currentDoctor = doctor;
            this.BackColor = Theme.Background;
            this.Padding = new Padding(20);

            cmbFilter = new ComboBox();
            cmbFilter.Items.AddRange(new object[] {"All", "Today", "Pending", "Confirmed", "Done", "Cancelled"});
            cmbFilter.SelectedIndex = 0;

            setupLayout();
            loadAppointments("All");
        }

        private void loadAppointments (string filter)
        {
            grid.Rows.Clear();

            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string where = filter switch
                {
                    "All" => "",
                    "Today" => "AND CAST(a.AppDateTime AS DATE) = CAST(GETDATE() AS DATE)",
                    _ => "AND a.Status = @status"
                };

                string query = $@"SELECT a.AppointmentID, p.Fullname, a.AppDateTime, a.Status, a.Note
                                  FROM Appointments a
                                  JOIN Patients p ON a.PatientID = p.PatientID
                                  WHERE a.DoctorID = @did {where}
                                  ORDER BY a.AppDateTime DESC";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@did", currentDoctor.doctorID);
                
                if (filter != "All" && filter != "Today")
                {
                    cmd.Parameters.AddWithValue("@status", filter);
                }

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    grid.Rows.Add(
                        (int)reader["AppointmentID"],
                        (string)reader["Fullname"],
                        ((DateTime)reader["AppDateTime"]).ToString("dd/MM/yyyy  hh:mm tt"),
                        (string)reader["Status"],
                        reader["Note"] == DBNull.Value ? string.Empty : (string)reader["Note"]
                    );
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("cmbFilter.SelectedIndex = 0;" + ex.Message);
            }
        }

        private void updateStatus(string newStatus)
        {
            if (grid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an appointment.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = (int)grid.SelectedRows[0].Cells["AppointmentID"].Value!;

            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"UPDATE Appointments SET Status = @ns
                                 WHERE AppointmentID = @aid";
                
                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ns", newStatus);
                cmd.Parameters.AddWithValue("@aid", id);

                cmd.ExecuteNonQuery();

                loadAppointments(cmbFilter.SelectedItem!.ToString()!);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error updating appointment: " + ex.Message);
            }
        }

        private void setupLayout()
        {
            
        }
    }
}