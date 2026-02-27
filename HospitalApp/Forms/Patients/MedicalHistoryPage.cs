using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Patients
{
    public class MedicalHistory: Panel
    {
        private Patient currentPatient;
        private DataGridView gridRecords = null!;
        private DataGridView gridPrescriptions = null!;
        private List<Record> records = new();
        public MedicalHistory(Patient patient)
        {
            this.currentPatient = patient;
            this.BackColor = Theme.Background;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(30);

            setupLayout();
            loadRecords();
        }

        private void setupLayout()
        {
            
        }

        private void loadRecords()
        {
            records.Clear();
            gridRecords.Rows.Clear();

            try 
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT r.RecordID, r.DoctorID, r.RecordDate, d.Fullname, r.Diagnosis, r.Notes
                                 FROM MedicalHistory r
                                 JOIN Doctors d ON r.DoctorID = d.DoctorID
                                 WHERE r.PatientID = @pid
                                 ORDER BY r.RecordDate DESC";

                using SqlCommand    cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@pid", currentPatient.patientID);

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) 
                {
                    records.Add(new Record 
                    {
                        recordID = (int)reader["RecordID"],
                        recordDate = (DateTime)reader["RecordDate"],
                        diagnosis = (string)reader["Diagnosis"],
                        notes = reader["Notes"] == DBNull.Value ? string.Empty : (string)reader["Notes"],
                        doctor = new Doctor
                        {
                            doctorID = (int)reader["DoctorID"],
                            fullname = (string)reader["Fullname"]
                        }
                    });

                    gridRecords.Rows.Add(
                        (int)reader["RecordID"],
                        ((DateTime)reader["RecordDate"]).ToString("dd/MM/yyyy"),
                        (string)reader["Fullname"],
                        (string)reader["Diagnosis"],
                        reader["Notes"] == DBNull.Value ? string.Empty : (string)reader["Notes"]
                    );
                }
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show("Error loading records: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPrescriptionsForRecord() 
        {
            gridPrescriptions.Rows.Clear();

            if (gridRecords.SelectedRows.Count == 0) return;

            int recordId = (int)gridRecords.SelectedRows[0].Cells["RecordID"].Value!;

            try 
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT * FROM Prescriptions 
                                 WHERE RecordID = @rid 
                                 ORDER BY IssuedAt";

                using SqlCommand cmd = new SqlCommand(query, conn);
                
                cmd.Parameters.AddWithValue("@rid", recordId);

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) 
                {
                    gridPrescriptions.Rows.Add(
                        (string)reader["Medicine"],
                        (string)reader["Dosage"],
                        (string)reader["Duration"],
                        ((DateTime)reader["IssuedAt"]).ToString("dd/MM/yyyy")
                    );
                }
            } 
            catch (Exception ex) 
            { 
                MessageBox.Show("Error loading prescriptions: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}