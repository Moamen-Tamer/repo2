using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Doctors
{
    public class DietPlanForm: Form
    {
        private Doctor currentDoctor;
        private int patientID;
        private int appointmentID;
        private Label lblResult = null!;
        private TextBox txtTitle = null!;
        private TextBox txtGoals = null!;
        private TextBox txtNotes = null!;
        private DateTimePicker dtpReview = null!;

        public DietPlanForm(Doctor doctor, int patientID, int appointmentID)
        {
            currentDoctor       = doctor;
            this.patientID      = patientID;
            this.appointmentID  = appointmentID;

            setupForm();
            setupLayout();
        }

        private void setupForm()
        {
            this.Text            = "Write Diet Plan";
            this.ClientSize      = new Size(520, 480);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;
            this.MinimizeBox     = false;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.BackColor       = Theme.Card;
        }

        private void setupLayout()
        {
            
        }

        private string loadPatientHealthSummary()
        {
            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT p.Fullname, p.BloodSugarMgDL, p.CholesterolMgDl, p.BpSystolic, p.BpDiastolic, p.WeightKg, p.HeightCm
                                 FROM Patients P
                                 WHERE p.PatientID = @pid";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@pid", patientID);

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    double weight = reader["WeightKg"] == DBNull.Value ? 0 : Convert.ToDouble(reader["WeightKg"]);
                    double height = reader["HeightCm"] == DBNull.Value ? 0 : Convert.ToDouble(reader["HeightCm"]);
                    int sugar = reader["BloodSugarMgDl"] == DBNull.Value ? 0 : (int)reader["BloodSugarMgDl"];
                    int chol  = reader["CholesterolMgDl"] == DBNull.Value ? 0 : (int)reader["CholesterolMgDl"];
                    int bpS   = reader["BpSystolic"] == DBNull.Value ? 0 : (int)reader["BpSystolic"];
                    int bpD   = reader["BpDiastolic"] == DBNull.Value ? 0 : (int)reader["BpDiastolic"];
                    
                    return $"Sugar: {sugar} mg/dL  |  Chol: {chol} mg/dL  |  BP: {bpS}/{bpD}  |  Weight: {weight}kg  |  Height: {height}cm";
                }
            }
            catch { }

            return "Health data unavailable";
        }

        private void saveClick(Object? sender, EventArgs e)
        {
            lblResult.Text = "";

            if (txtTitle.Text.Trim() == string.Empty)
            {
                lblResult.ForeColor = Theme.Danger;
                lblResult.Text      = "⚠  Plan title is required.";
                return;
            }

            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"INSERT INTO DietPlans (PatientID, DoctorID, AppointmentID, PlanTitle, Goals, Status, ReviewDate, Notes)
                                 VALUES (@pid, @did, @aid, @title, @goals, 'Active', @review, @notes)";

                using SqlCommand cmd = new SqlCommand(query, conn);
                
                cmd.Parameters.AddWithValue("@pid",    patientID);
                cmd.Parameters.AddWithValue("@did",    currentDoctor.doctorID);
                cmd.Parameters.AddWithValue("@aid",    appointmentID);
                cmd.Parameters.AddWithValue("@title",  txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@goals",  txtGoals.Text.Trim());
                cmd.Parameters.AddWithValue("@review", dtpReview.Value.Date);
                cmd.Parameters.AddWithValue("@notes",  txtNotes.Text.Trim());

                cmd.ExecuteNonQuery();

                lblResult.ForeColor = Theme.Success;
                lblResult.Text      = "✓  Diet plan saved successfully.";
                txtTitle.Text       = "";
                txtGoals.Text       = "";
                txtNotes.Text       = "";
            }
            catch (Exception ex)
            {
                lblResult.ForeColor = Theme.Danger;
                lblResult.Text      = "⚠  Error: " + ex.Message;
            }
        }
    }
}