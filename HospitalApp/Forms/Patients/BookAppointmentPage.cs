using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Patients
{
    public class BookAppointment: Panel
    {
        private Patient currentPatient;
        private ComboBox cmbDoctor = null!;
        private DateTimePicker dtpDate = null!;
        private DateTimePicker dtpTime = null!;
        private TextBox txtNotes = null!;
        private Label lblResult = null!;
        private List<Doctor> doctors = new();
        public BookAppointment(Patient patient)
        {
            this.currentPatient = patient;
            this.BackColor = Theme.Background;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(30);

            setupLayout();
            loadDoctors("All");
        }

        private void setupLayout()
        {
            
        }

        private void loadDoctors(string filter)
        {
            doctors.Clear();
            cmbDoctor.Items.Clear();
            cmbDoctor.Items.Add("-- Select a doctor --");

            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string where = filter == "All" ? "" : "WHERE Specialization = @s";

                string query = $@"SELECT DoctorID, Fullname, Specialization, IsAvailable
                                  FROM Doctors {where}
                                  ORDER BY Fullname";

                using SqlCommand cmd = new SqlCommand(query, conn);

                if(filter != "All") cmd.Parameters.AddWithValue("@s", filter);

                using SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    Doctor newDoctor = new Doctor
                    {
                        doctorID = (int)reader["DoctorID"],
                        fullname = (string)reader["Fullname"],
                        specialization = (string)reader["Specialization"],
                        isAvailable = (bool)reader["IsAvailable"]
                    };

                    doctors.Add(newDoctor);

                    string label = newDoctor.fullname + " — " + newDoctor.specialization + (newDoctor.isAvailable ? string.Empty : " (Unavailable)");
                    
                    cmbDoctor.Items.Add(label);
                }
                
                cmbDoctor.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading doctors: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void bookClick(Object? sender, EventArgs e)
        {
            lblResult.Text = "";

            if (cmbDoctor.SelectedIndex == 0)
            {
                lblResult.ForeColor = Theme.Danger;
                lblResult.Text = "⚠  Please select a doctor.";
                return;
            }

            Doctor selectedDoctor = doctors[cmbDoctor.SelectedIndex - 1];
            DateTime appDT = dtpDate.Value.Date.Add(dtpTime.Value.TimeOfDay);

            if (appDT <= DateTime.Now)
            {
                lblResult.ForeColor = Theme.Danger;
                lblResult.Text      = "⚠  Please select a future date and time.";
                return;
            }

            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"INSERT INTO Appointments (PatientID, DoctorID, AppDateTime, Status, Note)
                                 VALUES (@pid, @did, @dt, 'Pending', @note)";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@pid", currentPatient.patientID);
                cmd.Parameters.AddWithValue("@did", selectedDoctor.doctorID);
                cmd.Parameters.AddWithValue("@dt", appDT);
                cmd.Parameters.AddWithValue("@note", txtNotes.Text.Trim());
                
                cmd.ExecuteNonQuery();

                lblResult.ForeColor = Theme.Success;
                lblResult.Text = "✓  Appointment booked successfully! Status: Pending";
                cmbDoctor.SelectedIndex = 0;
                txtNotes.Text = "";
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = Theme.Danger;
                lblResult.Text = "⚠  Error: " + ex.Message;
            }
        }
    }
}