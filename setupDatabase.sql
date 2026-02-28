-- =============================================
-- DATABASE
-- =============================================
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HospitalDB')
    DROP DATABASE HospitalDB;
GO

CREATE DATABASE HospitalDB;
GO

USE HospitalDB;
GO

-- =============================================
-- TABLES
-- =============================================
CREATE TABLE Users (
    UserID INT IDENTITY(1, 1) PRIMARY KEY,
    Username NVARCHAR(80) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'Patient'
        CONSTRAINT ck_Users_Role
            CHECK (Role IN ('Doctor', 'Patient')),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE Departments (
    DepartmentID INT IDENTITY(1, 1) PRIMARY KEY,
    DepartmentName NVARCHAR(100)  NOT NULL UNIQUE,
    Description NVARCHAR(500) NOT NULL
);
GO

CREATE TABLE Doctors (
    DoctorID INT IDENTITY(1, 1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE REFERENCES Users(UserID),
    DepartmentID INT NOT NULL REFERENCES Departments(DepartmentID),
    Fullname NVARCHAR(150) NOT NULL,
    Specialization NVARCHAR(50) NOT NULL DEFAULT 'GeneralPractitioner'
        CONSTRAINT ck_Doctors_Specialization
            CHECK (Specialization IN (
                'GeneralPractitioner', 'Cardiologist','Neurologist', 'Nutritionist', 'Psychiatrist','Endocrinologist','Gastroenterologist', 'Nephrologist','Radiologist', 'Surgeon','Urologist'
            )),
    Phone NVARCHAR(30) NOT NULL,
    Email NVARCHAR(150) NOT NULL,
    Bio NVARCHAR(500),
    IsAvailable BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Patients (
    PatientID INT IDENTITY(1, 1) PRIMARY KEY,
    UserID INT NOT NULL UNIQUE REFERENCES Users(UserID),
    Fullname NVARCHAR(150) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(10)  NOT NULL
        CONSTRAINT ck_Patients_Gender
            CHECK (Gender IN ('Male', 'Female')),
    Phone NVARCHAR(30),
    Address NVARCHAR(300),
    BloodType NVARCHAR(5) DEFAULT 'O+'
        CONSTRAINT ck_Patients_BloodType
            CHECK (BloodType IN ('A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-')),
    WeightKg DECIMAL(5, 1),
    HeightCm Decimal(5, 1),
    CholesterolMgDl INT,
    BpSystolic INT,
    BpDiastolic INT,
    BloodSugarMgDl INT,
    MedicalNotes NVARCHAR(1000)
);
GO

CREATE TABLE Admissions (
    AdmissionID INT IDENTITY(1, 1) PRIMARY KEY,
    PatientID INT NOT NULL REFERENCES Patients(PatientID),
    DoctorID INT NOT NULL REFERENCES Doctors(DoctorID),
    RoomNumber NVARCHAR(20),
    AdmittedAt DATETIME NOT NULL DEFAULT GETDATE(),
    ExpectedLeave DATE,
    ActualLeave DATETIME,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Admitted'
        CONSTRAINT cl_Admissions_Status
            CHECK (Status IN ('Admitted', 'Critical', 'Discharged'))
);
GO

CREATE TABLE Appointments (
    AppointmentID INT IDENTITY(1, 1) PRIMARY KEY,
    PatientID INT NOT NULL REFERENCES Patients(PatientID),
    DoctorID INT NOT NULL REFERENCES Doctors(DoctorID),
    AppDateTime DATETIME NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
        CONSTRAINT ck_Appointments_Status
            CHECK (Status IN ('Pending', 'Confirmed', 'Done', 'Cancelled')),
    Note NVARCHAR(500)
);
GO

CREATE TABLE ViewersList (
    ViewerID INT IDENTITY(1, 1) PRIMARY KEY,
    AdmissionID INT NOT NULL REFERENCES Admissions(AdmissionID),
    ViewerName NVARCHAR(150) NOT NULL,
    Relation NVARCHAR(80),
    Phone NVARCHAR(30),
    IsAllowed BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE MedicalHistory (
    RecordID INT IDENTITY(1, 1) PRIMARY KEY,
    PatientID INT NOT NULL REFERENCES Patients(PatientID),
    DoctorID INT NOT NULL REFERENCES Doctors(DoctorID),
    AdmissionID INT NOT NULL REFERENCES Admissions(AdmissionID),
    RecordDate DATETIME NOT NULL DEFAULT GETDATE(),
    Diagnosis NVARCHAR(500) NOT NULL,
    Notes NVARCHAR(1000)
);
GO

CREATE TABLE Prescriptions (
    PrescriptionID INT IDENTITY(1, 1) PRIMARY KEY,
    RecordID INT NOT NULL REFERENCES MedicalHistory(RecordID) ON DELETE CASCADE,
    PatientID INT NOT NULL REFERENCES Patients(PatientID),
    DoctorID INT NOT NULL REFERENCES Doctors(DoctorID),
    Medicine NVARCHAR(200) NOT NULL,
    Dosage NVARCHAR(200) NOT NULL,
    Duration NVARCHAR(100) NOT NULL,
    IssuedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

CREATE TABLE PatientsMeals (
    MealID INT IDENTITY(1, 1) PRIMARY KEY,
    AdmissionID INT NOT NULL REFERENCES Admissions(AdmissionID),
    MealDate DATE NOT NULL,
    LunchVariant TINYINT NOT NULL DEFAULT 1  
        CONSTRAINT ck_PatientsMeals_Variant 
            CHECK (LunchVariant BETWEEN 1 AND 7),
    HasFruit BIT NOT NULL DEFAULT 1,         
    HasMahalabiya BIT NOT NULL DEFAULT 0,    
    IsBreakfastServed BIT NOT NULL DEFAULT 0,
    IsLunchServed BIT NOT NULL DEFAULT 0,
    IsDinnerServed BIT NOT NULL DEFAULT 0,
    Notes NVARCHAR(300),
    CONSTRAINT uq_PatientMeal UNIQUE (AdmissionID, MealDate)
);
GO

CREATE TABLE AppointmentDishes (
    AppointmentDishID INT IDENTITY(1,1) PRIMARY KEY,
    DishName NVARCHAR(200) NOT NULL,
    MealType NVARCHAR(20)  NOT NULL
        CONSTRAINT CK_AppointmentDishes_MealType
            CHECK (MealType IN ('Breakfast','Lunch','Dinner')),
    Calories INT,
    ProteinG DECIMAL(5,1),
    CarbsG DECIMAL(5,1),
    FatG DECIMAL(5,1),
    SodiumMg DECIMAL(7,1),
    Description NVARCHAR(500),
    Tags NVARCHAR(500) NOT NULL DEFAULT ''
);
GO

CREATE TABLE DietPlans (
    PlanID INT IDENTITY(1, 1) PRIMARY KEY,
    PatientID INT NOT NULL REFERENCES Patients(PatientID),
    DoctorID INT NOT NULL REFERENCES Doctors(DoctorID),
    AppointmentID INT REFERENCES Appointments(AppointmentID),
    PlanTitle NVARCHAR(200) NOT NULL,
    Goals NVARCHAR(500),
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active'
        CONSTRAINT ck_DietPlans_Status
            CHECK (Status IN ('Active', 'Completed', 'Cancelled')),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    ReviewDate DATE,
    Notes NVARCHAR(1000)
);
GO

CREATE TABLE DietPlanDishes (
    PlanDishID INT IDENTITY(1, 1) PRIMARY KEY,
    PlanID INT NOT NULL REFERENCES DietPlans(PlanID) ON DELETE CASCADE,
    AppointmentDishID INT NOT NULL REFERENCES AppointmentDishes(AppointmentDishID),
    DayNumber INT NOT NULL,     
    CONSTRAINT uq_PlanDayDish UNIQUE (PlanID, AppointmentDishID, DayNumber)
);
GO

INSERT INTO Users (Username, Password, Role) VALUES
('Ahmed Mostafa', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Doctor'),
('Sara El-Sayed', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Doctor'),
('Karim Nasser', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Doctor'),
('Nour Ibrahim', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Doctor'),
('Hana Farouk', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Doctor'),
('Tarek Samir', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Doctor'),
('Omar Salah', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Patient'),
('Layla Adel', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Patient'),
('Youssef Mahmoud', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Patient'),
('Mariam Nabil', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Patient'),
('Hassan Ali', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Patient'),
('Dina Kamal', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Patient'),
('Khaled Fouad', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Patient'),
('Nadia Wael', '$2a$12$KIXHw5Y1v9Z2wU3oQkX7V.eJzGpLmNqRsT4uVwXyZaAbBcCdDeEf', 'Patient');

INSERT INTO Doctors (UserID, Fullname, Gender, Specialization, Phone, IsAvailable) VALUES
(1, 'Dr. Ahmed Mostafa', 'Male', 'Cardiologist', '01012345678', 1),
(2, 'Dr. Sara El-Sayed', 'Female', 'Neurologist', '01123456789', 1),
(3, 'Dr. Karim Nasser', 'Male', 'GeneralPractitioner', '01234567890', 1),
(4, 'Dr. Nour Ibrahim', 'Female', 'Nutritionist', '01098765432', 1),
(5, 'Dr. Hana Farouk', 'Female', 'Endocrinologist', '01187654321', 0),
(6, 'Dr. Tarek Samir', 'Male', 'Surgeon', '01276543210', 1);

INSERT INTO Patients (UserID, Fullname, Gender, DateOfBirth, BloodType, Phone, WeightKg, HeightCm, BloodSugarMgDl, CholesterolMgDl, BpSystolic, BpDiastolic) VALUES
(7, 'Omar Salah', 'Male', '1985-03-14', 'B+', '01011112222', 88.0, 178, 95, 185, 118, 76),
(8, 'Layla Adel', 'Female', '1992-07-22', 'A-', '01122223333', 62.5, 163, 82, 172, 122, 80),
(9, 'Youssef Mahmoud', 'Male', '1978-11-05', 'O+', '01233334444', 102.0, 182, 140, 255, 145, 92),
(10, 'Mariam Nabil', 'Female', '1990-01-30', 'AB+', '01044445555', 55.0, 160, 78, 165, 110, 70),
(11, 'Hassan Ali', 'Male', '1970-09-18', 'A+', '01155556666', 95.0, 175, 115, 230, 138, 88),
(12, 'Dina Kamal', 'Female', '1998-05-12', 'B-', '01266667777', 57.0, 165, 68, 155, 108, 68),
(13, 'Khaled Fouad', 'Male', '1965-12-25', 'O-', '01377778888', 78.0, 170, 200, 290, 185, 115),
(14, 'Nadia Wael', 'Female', '1988-08-09', 'AB-', '01488889999', 70.5, 168, 90, 195, 125, 82);

INSERT INTO Admissions (PatientID, DoctorID, RoomNumber, AdmittedAt, ExpectedLeave, Status) VALUES
(1, 1, '201-A', '2025-12-10 09:00', '2025-12-17 09:00', 'Discharged'),  
(2, 2, '305-B', '2026-01-05 11:30', '2026-01-12 11:30', 'Discharged'),  
(3, 1, '201-C', '2026-01-20 08:00', '2026-02-03 08:00', 'Critical'),    
(5, 3, '110-A', '2026-02-01 14:00', '2026-02-10 14:00', 'Admitted'),   
(7, 6, '402-B', '2026-02-15 10:00', '2026-02-22 10:00', 'Admitted'),    
(4, 2, '308-A', '2026-02-20 09:30', NULL,               'Admitted');

INSERT INTO ViewersList (AdmissionID, ViewerName, Relation, Phone, IsAllowed) VALUES
(1, 'Fatma Salah', 'Wife', '01099991111', 1),
(1, 'Tarek Salah', 'Brother', '01088882222', 1),
(3, 'Amira Mahmoud', 'Wife', '01077773333', 0),
(3, 'Samer Mahmoud', 'Son', '01066664444', 0),
(3, 'Hoda Mahmoud', 'Daughter','01055555555', 0),
(4, 'Samira Ali', 'Wife', '01044446666', 1),
(4, 'Mona Ali', 'Sister', '01033337777', 1),
(5, 'Rana Fouad', 'Wife', '01022228888', 1),
(5, 'Amir Fouad', 'Son', '01011119999', 1),
(6, 'Sherif Nabil', 'Husband', '01099990000', 1);

INSERT INTO Appointments (PatientID, DoctorID, AppDateTime, Status, Note) VALUES
(1, 1, '2026-01-15 10:00', 'Done', 'Chest pain follow-up after discharge'),
(2, 2, '2026-01-20 11:00', 'Done', 'Recurring headaches, dizziness'),
(3, 1, '2026-01-25 09:00', 'Done', 'Pre-admission cardiac assessment'),
(4, 4, '2026-02-10 10:00', 'Done', 'Weight management and diet guidance'),
(6, 4, '2026-02-12 14:30', 'Done', 'Initial nutrition consultation'),
(5, 3, '2026-03-05 09:30', 'Confirmed', 'Annual checkup'),
(8, 4, '2026-03-08 11:00', 'Confirmed', 'High cholesterol — nutrition plan request'),
(1, 1, '2026-03-10 10:00', 'Confirmed', 'ECG follow-up'),
(7, 6, '2026-03-12 08:30', 'Pending', 'Post-surgery follow-up consultation'),
(2, 2, '2026-03-15 15:00', 'Pending', 'MRI results review'),
(4, 4, '2026-03-20 10:30', 'Pending', 'Diet plan review after 6 weeks'),
(6, 3, '2026-02-28 09:00', 'Cancelled', 'Cancelled — patient unavailable'),
(5, 1, '2026-02-20 11:00', 'Cancelled', 'Cancelled — doctor unavailable');

INSERT INTO MedicalHistory (PatientID, DoctorID, AdmissionID, Diagnosis, Notes) VALUES
(1, 1, 1, 'Hypertension Stage 2', 'Patient responded well to medication adjustment. BP normalized before discharge.'),
(2, 2, 2, 'Migraine with aura', 'Neurological exam normal. MRI scheduled for follow-up.'),
(3, 1, 3, 'Acute Myocardial Infarction', 'Emergency stenting performed. Patient in critical care. Strict visitor suspension in place.'),
(5, 3, 4, 'Type 2 Diabetes — uncontrolled', 'HbA1c elevated. Insulin therapy initiated. Dietary referral recommended.'),
(7, 6, 5, 'Appendicitis — acute', 'Laparoscopic appendectomy performed successfully. Recovery normal.'),
(4, 2, 6, 'Tension-type headache — chronic', 'No structural abnormality on CT. Stress management and medication prescribed.'),
(1, 1, NULL, 'Stable angina', 'Follow-up ECG. Medication continued. Next review in 3 months.'),
(4, 4, NULL, 'Borderline cholesterol', 'Diet modification plan provided. Recheck in 6 weeks.'),
(6, 4, NULL, 'Underweight / nutritional risk', 'BMI 18.1 — high-protein meal plan started. Weekly weigh-in recommended.');

INSERT INTO Prescriptions (RecordID, PatientID, DoctorID, Medicine, Dosage, Duration) VALUES
(1, 1, 1, 'Amlodipine', '5mg once daily', '3 months'),
(1, 1, 1, 'Perindopril', '4mg once daily', '3 months'),
(2, 2, 2, 'Sumatriptan', '50mg as needed (max 2/day)','On demand'),
(3, 3, 1, 'Aspirin', '100mg once daily', 'Indefinite'),
(3, 3, 1, 'Atorvastatin', '40mg at night', 'Indefinite'),
(3, 3, 1, 'Clopidogrel', '75mg once daily', '12 months'),
(4, 5, 3, 'Metformin', '500mg twice daily', '3 months'),
(4, 5, 3, 'Glipizide', '5mg before breakfast', '3 months'),
(5, 7, 6, 'Amoxicillin', '500mg three times daily', '7 days'),
(5, 7, 6, 'Ibuprofen', '400mg every 8 hours', '5 days'),
(6, 4, 2, 'Amitriptyline', '10mg at night', '1 month'),
(7, 1, 1, 'Nitroglycerin spray','1–2 sprays as needed', 'Indefinite'),
(9, 6, 4, 'Multivitamin', '1 tablet daily', '2 months');

INSERT INTO DietPlans (PatientID, DoctorID, AppointmentID, PlanTitle, Goals, Status, ReviewDate, Notes) VALUES
(4, 4, 4, 
    'Low-Cholesterol Balanced Plan',
    'Reduce LDL by 15%, increase HDL, maintain healthy BMI',
    'Active',
    '2026-03-20',
    'Avoid saturated fats and trans fats. Increase oily fish (salmon, sardines) twice weekly. Daily 30-min walk. Recheck lipid panel before review.'),

(6, 4, 5, 
    'High-Calorie Muscle-Building Plan',
    'Reach target weight of 62kg within 10 weeks, improve energy levels',
    'Active',
    '2026-03-25',
    'Aim for 2500 kcal/day. 6 small meals. High-protein snacks between meals (yogurt, eggs, nuts). Avoid skipping breakfast. Weekly weigh-in on Sundays.');

INSERT INTO PatientsMeals (AdmissionID, MealDate, MealType,
                           Description,
                           IsBreakfastServed, IsLunchServed, IsDinnerServed) VALUES
(3, '2026-02-28', 'Breakfast',
    'Fava beans (foul)  |  Triangle cheese  |  Cream cheese block  |  Baladi bread loaf',
    1, 0, 0),
(3, '2026-02-28', 'Lunch',
    'Grilled chicken  |  Cooked vegetables  |  Rice  |  Orzo',
    0, 0, 0),
(3, '2026-02-28', 'Dinner',
    'Fava beans (foul)  |  Triangle cheese  |  Cream cheese block  |  Baladi bread loaf',
    0, 0, 0),
(4, '2026-02-28', 'Breakfast',
    'Fava beans (foul)  |  Triangle cheese  |  Cream cheese block  |  Baladi bread loaf' ||
    CHAR(10) || '(Optional: small jam jar + sesame halawa bar)',
    1, 0, 0),
(4, '2026-02-28', 'Lunch',
    'Kofta  |  Cooked vegetables  |  Rice  |  Orzo' ||
    CHAR(10) || '+ One orange' ||
    CHAR(10) || '+ Low-sugar mahalabiya',
    0, 0, 0),
(4, '2026-02-28', 'Dinner',
    'Fava beans (foul)  |  Triangle cheese  |  Cream cheese block  |  Baladi bread loaf' ||
    CHAR(10) || '(Optional: small jam jar + sesame halawa bar)',
    0, 0, 0),
(5, '2026-02-28', 'Breakfast',
    'Fava beans (foul)  |  Triangle cheese  |  Cream cheese block  |  Baladi bread loaf' ||
    CHAR(10) || '(Optional: small jam jar + sesame halawa bar)',
    1, 0, 0),
(5, '2026-02-28', 'Lunch',
    'kabab halla  |  Cooked vegetables  |  Pasta  |  Orzo' ||
    CHAR(10) || '+ One orange',
    0, 1, 0),
(5, '2026-02-28', 'Dinner',
    'Fava beans (foul)  |  Triangle cheese  |  Cream cheese block  |  Baladi bread loaf' ||
    CHAR(10) || '(Optional: small jam jar + sesame halawa bar)',
    0, 0, 0),
(6, '2026-02-27', 'Breakfast',
    'Fava beans (foul)  |  Triangle cheese  |  Cream cheese block  |  Baladi bread loaf' ||
    CHAR(10) || '(Optional: small jam jar + sesame halawa bar)',
    1, 0, 0),
(6, '2026-02-27', 'Lunch',
    'Grilled chicken  |  Cooked vegetables  |  Rice  |  Orzo' ||
    CHAR(10) || '+ One orange',
    0, 1, 0),
(6, '2026-02-27', 'Dinner',
    'Fava beans (foul)  |  Triangle cheese  |  Cream cheese block  |  Baladi bread loaf' ||
    CHAR(10) || '(Optional: small jam jar + sesame halawa bar)',
    0, 0, 1);