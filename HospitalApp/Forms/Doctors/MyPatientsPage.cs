using System;
using System.Drawing;
using System.Windows.Forms;
using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Doctors
{
    public class MyPatientsPage: Panel
    {
        private Doctor currentDoctor;
        private DataGridView grid = null!;
        private TextBox txtSearch = null!;
        private ComboBox cmbStatus = null!;
        private List<Admission> admissions = new();
        public MyPatientsPage(Doctor doctor)
        {
            this.currentDoctor = doctor;
            this.BackColor = Theme.Background;
            this.Padding = new Padding(30);

            cmbStatus = new ComboBox();
            cmbStatus.Items.AddRange(new Object[] {"All", "Admitted", "Critical", "Discharged"});
            cmbStatus.SelectedIndex = 0;

            setupLayout();
            loadPatients("All", "");
        }

        private void setupLayout()
        {
            
        }

        private void loadPatients(string status, string search)
        {
            admissions.Clear();
            grid.Rows.Clear();

            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string where = status == "All" ? string.Empty : "AND a.Status = @status";
                string like = search == string.Empty ? string.Empty : "AND p.Fullname LIKE @search";

                string query = $@"SELECT a.AdmissionID, p.PatientID, p.Fullname, a.RoomNumber, a.AdmittedAt, a.ExpectedLeave, a.Status
                                  FROM Admissions a
                                  JOIN Patients p ON a.PatientID = p.PatientID
                                  WHERE a.DoctorID = @did {where} {like}
                                  ORDER BY
                                      CASE a.Status
                                          WHEN 'Critical' THEN 0
                                          WHEN 'Admitted' THEN 1
                                          ELSE 2
                                      END,
                                      a.AdmittedAt DESC";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@did",currentDoctor.doctorID);

                if (status != "All") cmd.Parameters.AddWithValue("@status", status);
                if (search != string.Empty) cmd.Parameters.AddWithValue("@search", "%" + search + "%");

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    admissions.Add(new Admission
                    {
                        admissionID = (int)reader["AdmissionID"],
                        patientID = (int)reader["PatientID"],
                        fullname = (string)reader["Fullname"],
                        status = Enum.TryParse<Admission.AdmissionStatus>((string)reader["Status"], out var newStatus) ? newStatus : Admission.AdmissionStatus.Admitted
                    });

                    int rowIdx = grid.Rows.Add(
                        (int)reader["AdmissionID"],
                        (int)reader["PatientID"],
                        (string)reader["Fullname"],
                        reader["RoomNumber"] as string ?? string.Empty,
                        ((DateTime)reader["AdmittedAt"]).ToString("dd/MM/yyyy"),
                        reader["ExpectedLeave"] == DBNull.Value ? "Not Set" : ((DateTime)reader["ExpectedLeave"]).ToString("dd/MM/yyyy"),
                        (string)reader["Status"]
                    );

                    if ((string)reader["Status"] == "Critical")
                    {
                        grid.Rows[rowIdx].DefaultCellStyle.BackColor = Color.FromArgb(40, 248, 113, 113);
                        grid.Rows[rowIdx].DefaultCellStyle.ForeColor = Theme.Danger;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error loading patients: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void detailsClick(Object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) {
                MessageBox.Show("Please select a patient.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int admID = (int)grid.SelectedRows[0].Cells["AdmissionID"].Value!;
            Admission? admission = admissions.Find((admission) => admission.admissionID == admID);

            if (admission == null) return;

            if(new PatientDetailsForm(admission, currentDoctor).ShowDialog() == DialogResult.OK)
            {
                loadPatients(cmbStatus.SelectedItem!.ToString()!, txtSearch.Text);
            }
        }

        private void prescribeClick(Object? sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) {
                MessageBox.Show("Please select a patient.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int patID = (int)grid.SelectedRows[0].Cells["PatientID"].Value!;
            int admID = (int)grid.SelectedRows[0].Cells["AdmissionID"].Value!;

            new PrescriptionForm(patID, admID, currentDoctor.doctorID).ShowDialog();
        }
    }

    public class PatientDetailsForm: Form
    {
        private int admissionID;
        private string patientName;
        private Doctor currentDoctor;
        private string roomNumber = string.Empty;
        private DateTime admittedAt = DateTime.Now;
        private DateTime? expectedLeave = null;
        private string status = "Admitted";
        private DateTimePicker dtp = null!;
        private ComboBox cmbSt = null!;
        public PatientDetailsForm(Admission admission, Doctor currentDoctor)
        {
            this.admissionID = admission.admissionID;
            this.patientName = admission.fullname ?? "Unknown";
            this.currentDoctor = currentDoctor;

            cmbSt = new ComboBox();
            cmbSt.Items.AddRange(new object[] {"Admitted", "Critical", "Discharged"});
            
            setupForm();
            loadData();
            setupLayout();
        }

        private void setupForm()
        {
            this.Text            = "Patient Details — " + patientName;
            this.ClientSize      = new Size(480, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = Theme.Card;
        }

        private void loadData()
        {
            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT RoomNumber, AdmittedAt, ExpectedLeave, Status 
                                 FROM Admissions 
                                 WHERE AdmissionID = @aid";
                
                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@aid", admissionID);

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    roomNumber = reader["RoomNumber"] as string ?? string.Empty;
                    admittedAt = (DateTime)reader["AdmittedAt"];
                    expectedLeave = reader["ExpectedLeave"] == DBNull.Value ? null : (DateTime)reader["ExpectedLeave"];
                    status = (string)reader["Status"];  
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to load patient data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveClick()
        {
            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"UPDATE Admissions SET ExpectedLeave = @el, Status = @s 
                                 WHERE AdmissionID = @aid";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@el", dtp.Value);
                cmd.Parameters.AddWithValue("@s", cmbSt.SelectedItem!.ToString());
                cmd.Parameters.AddWithValue("@aid", admissionID);
                cmd.ExecuteNonQuery();

                if (cmbSt.SelectedItem!.ToString() == "Critical")
                {
                    string queryViewers = @"UPDATE ViewersList SET IsAllowed = 0 
                                            WHERE AdmissionID = @aid";

                    using SqlCommand cmdViewers = new SqlCommand(queryViewers, conn);

                    cmdViewers.Parameters.AddWithValue("@aid", admissionID);
                    cmdViewers.ExecuteNonQuery();

                    MessageBox.Show(
                        "Status set to Critical.\nAll visitors have been automatically suspended.",
                        "Visitors Suspended",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Failed to save changes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void setupLayout()
        {
            
        }
    }

    public class PrescriptionForm: Form
    {
        private int patientID;
        private int admissionID;
        private int doctorID;
        private TextBox txtMedicine = null!;
        private TextBox txtDiagnosis = null!;
        private TextBox txtDosage = null!;
        private TextBox txtDuration = null!;
        private Label lblResult = null!;
        public PrescriptionForm(int patientID, int admissionID, int doctorID)
        {
            this.patientID = patientID;
            this.admissionID = admissionID;
            this.doctorID = doctorID;

            setupForm();
            setupLayout();
        }

        private void setupForm()
        {
            this.Text            = "Write Prescription";
            this.ClientSize      = new Size(480, 540);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = Theme.Card;
        }

        private void setupLayout()
        {
            
        }

        private void saveClick(Object? sender, EventArgs e)
        {
            if (txtMedicine.Text.Trim() == string.Empty || txtDiagnosis.Text.Trim() == string.Empty)
            {
                lblResult.ForeColor = Theme.Danger;
                lblResult.Text = "⚠  Diagnosis and medicine name are required.";
                return;
            }

            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string queryRecord = @"INSERT INTO MedicalHistory (PatientID, DoctorID, AdmissionID, Diagnosis, Notes)
                                       OUTPUT INSERTED.RecordID
                                       VALUES (@pid, @did, @aid, @diag, '')";

                using SqlCommand cmdRecord = new SqlCommand(queryRecord, conn);

                cmdRecord.Parameters.AddWithValue("@pid", patientID);
                cmdRecord.Parameters.AddWithValue("@did", doctorID);
                cmdRecord.Parameters.AddWithValue("@aid", admissionID);
                cmdRecord.Parameters.AddWithValue("@diag", txtDiagnosis.Text.Trim());

                int recordID = (int)cmdRecord.ExecuteScalar()!;

                string queryPrescription = @"INSERT INTO Prescriptions (RecordID, PatientID, DoctorID, Medicine, Dosage, Duration)
                                             VALUES (@rid, @pid, @did, @med, @dos, @dur)";

                using SqlCommand cmdPrescription = new SqlCommand(queryPrescription, conn);

                cmdPrescription.Parameters.AddWithValue("@rid", recordID);
                cmdPrescription.Parameters.AddWithValue("@pid", patientID);
                cmdPrescription.Parameters.AddWithValue("@did", doctorID);
                cmdPrescription.Parameters.AddWithValue("@med", txtMedicine.Text.Trim());
                cmdPrescription.Parameters.AddWithValue("@dos", txtDosage.Text.Trim());
                cmdPrescription.Parameters.AddWithValue("@dur", txtDuration.Text.Trim());

                cmdPrescription.ExecuteNonQuery();

                lblResult.ForeColor    = Theme.Success;
                lblResult.Text         = "✓  Prescription saved successfully.";
                txtDiagnosis.Text      = "";
                txtMedicine.Text       = "";
                txtDosage.Text         = "";
                txtDuration.Text       = "";
            }
            catch(Exception ex)
            {
                lblResult.ForeColor = Theme.Danger;
                lblResult.Text = "⚠  Error: " + ex.Message;
            }
        }
    }
}