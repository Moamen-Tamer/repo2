using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class Viewer
    {
        public int viewerID {get; set;}
        public int admissionID {get; set;}
        public string viewerName {get; set;} = string.Empty;
        public string? relation {get; set;}
        public string? phone {get; set;}
        public bool isAllowed {get; set;} = true;

        public static Viewer FromReader(SqlDataReader reader)
        {
            return new Viewer
            {
                viewerID = (int)reader["ViewerID"],
                admissionID = (int)reader["AdmissionID"],
                viewerName = (string)reader["ViewerName"],
                relation = reader["Relation"] as string,
                phone = reader["Phone"] as string,
                isAllowed = (bool)reader["IsAllowed"]
            };
        }
    }
}