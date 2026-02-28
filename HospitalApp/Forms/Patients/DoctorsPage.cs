using HospitalApp.Database;
using HospitalApp.Models;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Forms.Patients
{
    public class DoctorsPage: Panel
    {
        private List<Doctor> doctors = new();
        
        public DoctorsPage()
        {
            this.BackColor = Theme.Background;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(30);

            setupToolbar();
            setupCards();
            loadDoctors();
        }

        private void setupToolbar()
        {
            
        }

        private void setupCards()
        {
            
        }

        private void loadDoctors()
        {
            doctors.Clear();

            try {
                using SqlConnection conn = DBConnection.connectDB();
                conn.Open();

                string query = @"SELECT d.*, dep.DepartmentName 
                                 FROM Doctors d
                                 LEFT JOIN Departments dep ON d.DepartmentID = dep.DepartmentID
                                 ORDER BY d.FullName";

                using SqlCommand cmd = new SqlCommand(query, conn);

                using SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read()) {
                    doctors.Add(new Doctor {
                        doctorID = (int)reader["DoctorID"],
                        fullname = (string)reader["Fullname"],
                        specialization = (string)reader["Specialization"],
                        phone = (string)reader["Phone"],
                        email = (string)reader["Email"],
                        bio = reader["Bio"] == DBNull.Value ? string.Empty : (string)reader["Bio"],
                        isAvailable = (bool)reader["IsAvailable"],
                        department = new Department
                        {
                            departmentID = (int)reader["DepartmentID"],
                            departmentName = (string)reader["DepartmentName"]
                        },
                    });
                }
            } catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            renderCards(doctors);
        }

        private void renderCards(List<Doctor> doctors)
        {
            
        }

        private void filterDoctors(string search)
        {
            search = search.ToLower().Trim();
            
            if (search == string.Empty)
            {
                renderCards(doctors);
                return;    
            }

            var filtered = doctors.FindAll((doctor) =>
                doctor.fullname.ToLower().Contains(search) ||
                doctor.specialization.ToLower().Contains(search) ||
                (doctor.department?.departmentName.ToLower().Contains(search) ?? false));

            renderCards(filtered);
        }
    }
}