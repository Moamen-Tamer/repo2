using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Patients
{
    public class ViewersList: Panel
    {
        private Patient currentPatient;
        private DataGridView grid = null!;
        private ComboBox cmbAdmission= null!;
        private List<Admission> admissions = new();
        public ViewersList(Patient patient)
        {
            this.currentPatient = patient;
            this.BackColor = Theme.Background;
            this.Padding = new Padding(20);

            setupLayout();
            loadAdmissions();
        }

        private void setupLayout()
        {
            
        }

        private void loadAdmissions() {
            admissions.Clear();
            cmbAdmission.Items.Clear();
            try {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT AdmissionID, AdmittedAt, Status FROM Admissions 
                                 WHERE PatientID = @pid 
                                 ORDER BY AdmittedAt DESC";

                using SqlCommand    cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@pid", currentPatient.patientID);

                using SqlDataReader reader = cmd.ExecuteReader();

                cmbAdmission.Items.Add("-- Select admission --");

                while (reader.Read()) {
                    admissions.Add(new Admission {
                        admissionID = (int)reader["AdmissionID"],
                        admittedAt = (DateTime)reader["AdmittedAt"],
                        status = Enum.Parse<Admission.AdmissionStatus>((string)reader["Status"])
                    });

                    cmbAdmission.Items.Add($"Admitted: {((DateTime)reader["AdmittedAt"]):dd/MM/yyyy} - [{reader["Status"]}]");
                }
                cmbAdmission.SelectedIndex = 0;
            } catch (Exception ex) { 
                MessageBox.Show("Error: " + ex.Message); 
            }
        }

        private void LoadViewers() {
            grid.Rows.Clear();

            if (cmbAdmission.SelectedIndex == 0) return;

            int admId = admissions[cmbAdmission.SelectedIndex - 1].admissionID;

            try {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT * FROM ViewersList 
                                 WHERE AdmissionID = @aid 
                                 ORDER BY ViewerName";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@aid", admId);

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) {
                    bool allowed = (bool)reader["IsAllowed"];

                    int row = grid.Rows.Add(
                        (int)reader["ViewerID"],
                        (string)reader["ViewerName"],
                        reader["Relation"] == DBNull.Value ? string.Empty : (string)reader["Relation"],
                        reader["Phone"]    == DBNull.Value ? string.Empty : (string)reader["Phone"],
                        allowed ? "✓ Allowed" : "✗ Suspended by Doctor"
                    );

                    grid.Rows[row].DefaultCellStyle.ForeColor = allowed ? Theme.Success : Theme.Danger;
                }
            } catch (Exception ex) { 
                MessageBox.Show("Error: " + ex.Message); 
            }
        }

        private void gridCellClick(object? sender, DataGridViewCellEventArgs e) {
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == grid.Columns["Delete"]!.Index) {
                int id = (int)grid.Rows[e.RowIndex].Cells["ViewerID"].Value!;

                var confirm = MessageBox.Show("Remove this visitor?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes) return;

                try {
                    using SqlConnection conn = DBConnection.connectDB();
                    conn.Open();

                    string query = @"DELETE FROM ViewersList 
                                     WHERE ViewerID=@id";

                    using SqlCommand cmd = new SqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                    
                    LoadViewers();
                } catch (Exception ex) { 
                    MessageBox.Show("Error: " + ex.Message); 
                }
            }
        }

        private void btnAddClick(object? sender, EventArgs e) {
            if (cmbAdmission.SelectedIndex == 0) {
                MessageBox.Show("Please select a hospital stay first.", "Select Admission",MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int admId = admissions[cmbAdmission.SelectedIndex - 1].admissionID;
            var form  = new AddViewerDialog(admId);
            if (form.ShowDialog() == DialogResult.OK) LoadViewers();
        }
    }

    public class AddViewerDialog: Form
    {
        private int admissionID;
        private TextBox txtName     = null!;
        private TextBox txtRelation = null!;
        private TextBox txtPhone    = null!;
        private Label   lblError    = null!;
        public AddViewerDialog(int admID)
        {
            this.admissionID = admID;
            
            setupForm();
            setupLayout();
        }

        private void setupForm()
        {
            
        }

        private void setupLayout()
        {
            
        }

        private void btnSaveClick(object? sender, EventArgs e) {
            if (txtName.Text.Trim() == "") { 
                lblError.Text = "⚠  Name is required.";
                return; 
            }

            try {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"INSERT INTO ViewersList (AdmissionID, ViewerName, Relation, Phone, IsAllowed) 
                                 VALUES (@aid, @name, @rel, @phone, 1)";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@aid",   admissionID);
                cmd.Parameters.AddWithValue("@name",  txtName.Text.Trim());
                cmd.Parameters.AddWithValue("@rel",   txtRelation.Text.Trim());
                cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());

                cmd.ExecuteNonQuery();

                this.DialogResult = DialogResult.OK;
            } catch (Exception ex) { 
                lblError.Text = "⚠  " + ex.Message; 
            }
        }
    }
}