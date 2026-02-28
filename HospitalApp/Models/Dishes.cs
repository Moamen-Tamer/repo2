using HospitalApp.Helpers;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class Dish
    {
        public int appointmentDishID {get; set;}
        public string dishName {get; set;} = string.Empty;
        public MealType mealType {get; set;}
        public int calories {get; set;}
        public decimal protein {get; set;}
        public decimal carbs {get; set;}
        public decimal fat {get; set;}
        public decimal sodium {get; set;}
        public string description {get; set;} = string.Empty;
        public string tags {get; set;} = string.Empty;

        public static Dish FromReader(SqlDataReader reader)
        {
            return new Dish
            {
                appointmentDishID = (int)reader["AppointmentDishID"],
                dishName = (string)reader["DishName"],
                mealType = Enum.Parse<MealType>((string)reader["MealType"]),
                calories = reader["Calories"] == DBNull.Value ? 0 : (int)reader["Calories"],
                protein = reader["ProteinG"] == DBNull.Value ? 0 : (int)reader["ProteinG"],
                carbs = reader["CarbsG"] == DBNull.Value ? 0 : (int)reader["CarbsG"],
                fat = reader["FatG"] == DBNull.Value ? 0 : (int)reader["FatG"],
                sodium = reader["SodiumMg"] == DBNull.Value ? 0 : (int)reader["SodiumMg"],
                description = reader["Description"] == DBNull.Value ? string.Empty : (string)reader["Description"],
                tags = (string)reader["Tags"]
            };
        }
    }
}