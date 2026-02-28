namespace HospitalApp.Helpers
{
    public enum RoleType
    {
        Doctor,
        Patient
    }

    public enum Specialization
    {
        GeneralPractitioner,
        Cardiologist,
        Neurologist,
        Nutritionist,   
        Psychiatrist,
        Endocrinologist,
        Gastroenterologist,
        Nephrologist,
        Radiologist,
        Surgeon,
        Urologist
    }

    public enum BloodType
    {
        A_Positive,
        A_Negative,
        B_Positive,
        B_Negative,
        AB_Positive,
        AB_Negative,
        O_Positive,
        O_Negative
    }

    public enum GenderType
    {
        Male,
        Female
    }

    public enum AdmissionStatus
    {
        Admitted,
        Critical,
        Discharged
    }

    public enum AppointmentStatus
    {
        Pending,
        Confirmed,
        Done,
        Cancelled
    }

    public enum CholesterolLevel
    {
        Low,
        Normal,
        BorderlineHigh,
        High,
        Unknown
    }

    public enum BloodPressureStatus
    {
        Low,            
        Normal,         
        Elevated,       
        HighStage1,     
        HighStage2,     
        HypertensiveCrisis, 
        Unknown
    }

    public enum BloodSugarStatus
    {
        Low,        
        Normal,     
        PreDiabetic,
        Diabetic,   
        Unknown
    }

    public enum BmiCategory
    {
        Underweight,  
        Normal, 
        Overweight,     
        Obese,          
        SeverelyObese,  
        Unknown
    }

    public enum MealType
    {
        Breakfast,
        Lunch,
        Dinner
    }

    public enum DietTag
    {
        LowSodium,
        LowSugar,
        LowFat,
        LowCholesterol,
        HighProtein,
        HighFiber,
        Diabetic,
        HeartHealthy,
        Hypertension,
        WeightLoss,
        GeneralWellness
    }

    public static class BloodTypeHelper
    {
        public static string display(BloodType bloodType) => bloodType switch
        {
            BloodType.A_Positive => "A+",
            BloodType.A_Negative  => "A-",
            BloodType.B_Positive  => "B+",
            BloodType.B_Negative  => "B-",
            BloodType.AB_Positive => "AB+",
            BloodType.AB_Negative => "AB-",
            BloodType.O_Positive  => "O+",
            BloodType.O_Negative  => "O-",
            _ => bloodType.ToString()
        };

        public static BloodType classify(string bloodType) => bloodType switch
        {
            "A+"  => BloodType.A_Positive,
            "A-"  => BloodType.A_Negative,
            "B+"  => BloodType.B_Positive,
            "B-"  => BloodType.B_Negative,
            "AB+" => BloodType.AB_Positive,
            "AB-" => BloodType.AB_Negative,
            "O+"  => BloodType.O_Positive,
            "O-"  => BloodType.O_Negative,
            _     => throw new ArgumentException("Invalid blood type")
        };

        public static string[] displayAll() => Enum.GetValues<BloodType>().Select(display).ToArray();
    }

    public static class CholesterolHelper
    {
        private const int lowMax = 149;
        private const int normalMax = 199;
        private const int borderlineMax = 239;

        public static string display(CholesterolLevel cholesterol) => cholesterol switch
        {
            CholesterolLevel.Low => "Low  (< 150 mg/dL)",
            CholesterolLevel.Normal        => "Normal  (150–199 mg/dL)",
            CholesterolLevel.BorderlineHigh => "Borderline High  (200–239 mg/dL)",
            CholesterolLevel.High          => "High  (≥ 240 mg/dL)",
            _                              => "Unknown"
        };

        public static CholesterolLevel classify(int cholesterol)
        {
            if (cholesterol <= 0) return CholesterolLevel.Unknown;
            if (cholesterol <= 149) return CholesterolLevel.Low;
            if (cholesterol <= 199) return CholesterolLevel.Normal;
            if (cholesterol <= 239) return CholesterolLevel.BorderlineHigh;

            return CholesterolLevel.High;
        }
    }

    public static class BloodPressureHelper
    {
        public static string ToDisplay(BloodPressureStatus s) => s switch
        {
            BloodPressureStatus.Low => "Low  (< 90/60 mmHg)",
            BloodPressureStatus.Normal => "Normal  (90–119 / 60–79 mmHg)",
            BloodPressureStatus.Elevated => "Elevated  (120–129 / < 80 mmHg)",
            BloodPressureStatus.HighStage1 => "High — Stage 1  (130–139 / 80–89 mmHg)",
            BloodPressureStatus.HighStage2 => "High — Stage 2  (≥ 140 / ≥ 90 mmHg)",
            BloodPressureStatus.HypertensiveCrisis => "⚠ Hypertensive Crisis  (> 180 / > 120 mmHg)",
            _ => "Unknown"
        };

        public static BloodPressureStatus classify(int systolic, int diastolic)
        {
            if (systolic <= 0 || diastolic <= 0) return BloodPressureStatus.Unknown;
            if (systolic > 180 || diastolic > 120) return BloodPressureStatus.HypertensiveCrisis;
            if (systolic >= 140 || diastolic >= 90) return BloodPressureStatus.HighStage2;
            if (systolic >= 130 || diastolic >= 80) return BloodPressureStatus.HighStage1;
            if (systolic >= 120 && diastolic < 80) return BloodPressureStatus.Elevated;
            if (systolic < 90  || diastolic < 60) return BloodPressureStatus.Low;

            return BloodPressureStatus.Normal;
        }
    }

    public static class BloodSugarHelper
    {
        public static string ToDisplay(BloodSugarStatus s) => s switch
        {
            BloodSugarStatus.Low => "Low / Hypoglycemia  (< 70 mg/dL)",
            BloodSugarStatus.Normal => "Normal  (70–99 mg/dL)",
            BloodSugarStatus.PreDiabetic => "Pre-Diabetic  (100–125 mg/dL)",
            BloodSugarStatus.Diabetic => "Diabetic  (≥ 126 mg/dL)",
            _ => "Unknown"
        };

        public static BloodSugarStatus classify(int mgDl)
        {
            if (mgDl <= 0) return BloodSugarStatus.Unknown;
            if (mgDl < 70) return BloodSugarStatus.Low;
            if (mgDl <= 99) return BloodSugarStatus.Normal;
            if (mgDl <= 125) return BloodSugarStatus.PreDiabetic;

            return BloodSugarStatus.Diabetic;
        }
    }

    public static class BmiHelper
    {
        public static double Calculate(double weightKg, double heightCm)
        {
            if (weightKg <= 0 || heightCm <= 0) return 0;

            double heightM = heightCm / 100.0;
            
            return Math.Round(weightKg / (heightM * heightM), 1);
        }

        public static string ToDisplay(BmiCategory c) => c switch
        {
            BmiCategory.Underweight => "Underweight (BMI < 18.5)",
            BmiCategory.Normal => "Normal (BMI 18.5–24.9)",
            BmiCategory.Overweight => "Overweight (BMI 25–29.9)",
            BmiCategory.Obese => "Obese (BMI 30–34.9)",
            BmiCategory.SeverelyObese => "Severely Obese (BMI ≥ 35)",
            _ => "Unknown"
        };

        public static BmiCategory classify(double bmi)
        {
            if (bmi <= 0) return BmiCategory.Unknown;
            if (bmi < 18.5) return BmiCategory.Underweight;
            if (bmi < 25.0) return BmiCategory.Normal;
            if (bmi < 30.0) return BmiCategory.Overweight;
            if (bmi < 35.0) return BmiCategory.Obese;

            return BmiCategory.SeverelyObese;
        }
    }

    public static class AdmissionMealHelper
    {
        public static string GetBreakfastDescription(bool isDiabetic)
        {
            string basic = "Fava beans (foul)  |  Triangle cheese  |  Cream cheese block  |  Baladi bread loaf";
            string optional = isDiabetic ? "" : "\n(Optional: small jam jar + sesame halawa bar)";

            return basic + optional;
        }

        public static string GetDinnerDescription(bool isDiabetic) =>
            GetBreakfastDescription(isDiabetic);

        public static string GetLunchMainCourse(int variant) => variant switch
        {
            1 => "Grilled chicken  |  Cooked vegetables  |  Rice  |  Orzo",
            2 => "kabab halla  |  Cooked vegetables  |  Rice  |  Orzo",
            3 => "Kofta  |  Cooked vegetables  |  Rice  |  Orzo",
            4 => "Grilled chicken  |  Cooked vegetables  |  Pasta  |  Orzo",
            5 => "kabab halla  |  Cooked vegetables  |  Pasta  |  Orzo",
            6 => "Kofta  |  Cooked vegetables  |  Pasta  |  Orzo",
            7 => "Grilled chicken  |  Cooked vegetables  |  Rice  |  Orzo",
            _ => "Grilled chicken  |  Cooked vegetables  |  Rice  |  Orzo"
        };

        public static string GetLunchDescription(int variant, bool hasFruit, bool hasMahalabiya, bool isWinter)
        {
            string main = GetLunchMainCourse(variant);

            string fruit = hasFruit ? (isWinter ? "\n+ One orange" : "\n+ Sugar-free juice box") : string.Empty;

            string dessert = hasMahalabiya ? "\n+ Low-sugar mahalabiya" : string.Empty;

            return main + fruit + dessert;
        }

        public static int getDaySlot(DateTime date)
        {
            var oldDate = new DateTime(2000, 1, 1);

            return ((int)(date - oldDate).TotalDays % 7) + 1;
        }

        public static bool isWinter(DateTime date) => date.Month <= 3 || date.Month >= 10;
    }
}