using HospitalApp.Helpers;
using Microsoft.Data.SqlClient;

namespace HospitalApp.Models
{
    public class DailyMeal
    {
        public int mealID {get; set;}
        public int admissionID {get; set;}
        public DateTime mealDate {get; set;}
        public int lunchVariant {get; set;}
        public bool hasFruit {get; set;}
        public bool hasMahalabiya {get; set;}
        public bool isBreakfastServed {get; set;}
        public bool isLunchServed {get; set;}
        public bool isDinnerServed {get; set;}
        public string notes {get; set;} = string.Empty;
        public string breakfastText(bool isDiabetic) => AdmissionMealHelper.GetBreakfastDescription(isDiabetic);
        public string lunchText() => AdmissionMealHelper.GetLunchDescription(lunchVariant, hasFruit, hasMahalabiya, AdmissionMealHelper.isWinter(mealDate));
        public string DinnerText(bool isDiabetic) => AdmissionMealHelper.GetDinnerDescription(isDiabetic);

        public static DailyMeal FromReader(SqlDataReader reader)
        {
            return new DailyMeal
            {
                mealID = (int)reader["MealID"],
                admissionID = (int)reader["AdmissionID"],
                mealDate = (DateTime)reader["MealDate"],
                lunchVariant = (byte)reader["LunchVariant"],
                hasFruit = reader["HasFruit"] != DBNull.Value && (bool)reader["HasFruit"],
                hasMahalabiya = reader["HasMahalabiya"] != DBNull.Value && (bool)reader["HasMahalabiya"],
                isBreakfastServed = reader["IsBreakfastServed"] != DBNull.Value && (bool)reader["IsBreakfastServed"],
                isLunchServed = reader["IsLunchServed"] != DBNull.Value && (bool)reader["IsLunchServed"],
                isDinnerServed = reader["IsDinnerServed"] != DBNull.Value && (bool)reader["IsDinnerServed"],
                notes = reader["Notes"] == DBNull.Value ? string.Empty : (string)reader["Notes"]
            };
        }
    }
}