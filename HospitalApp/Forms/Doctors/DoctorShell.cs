using System;
using System.Drawing;
using System.Windows.Forms;
using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Doctors
{
    public class DoctorShell: Form
    {
        private User currentUser;
        private Doctor currentDoctor = null!;

        private Panel  contentPanel  = null!;
        private Label  lblPageTitle  = null!;
        private Button btnPatients   = null!;
        private Button btnAppointments = null!;
        private Button btnViewers    = null!;
        public DoctorShell(User user)
        {
            currentUser = user;
            loadDoctor();
            setupForm();
            setupLayout();
            showPage("Patients");
        }

        private void setupForm()
        {
            this.Text            = "CareFlow — Doctor Dashboard";
            this.ClientSize      = new Size(1180, 700);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = true;
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.BackColor       = Theme.Background;
        }

        private void setupLayout()
        {
            
        }

        private void loadDoctor()
        {
            try
            {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT d.*, dep.DepartmentName FROM Doctor d
                                 LEFT JOIN Departments dep ON d.DepartmentID = dep.DepartmentID
                                 WHERE d.UserID = @u";

                using SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@u", currentUser.userID);

                using SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    currentDoctor = new Doctor
                    {
                        doctorID = (int)reader["DoctorID"],
                        fullname = (string)reader["Fullname"],
                        specialization = (string)reader["Specialization"],
                        isAvailable = (bool)reader["IsAvailable"],
                        department = reader["DepartmentName"] == DBNull.Value
                        ? null
                        : new Department
                        {
                            departmentID = (int)reader["DepartmentID"],
                            departmentName = (string)reader["DepartmentName"]
                        }
                    };
                }
            }
            catch { }
        }

        private void showPage(string page)
        {
            
        }
    }
}