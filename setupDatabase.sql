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
            CHECK (Role IN ('Doctor', 'Patient'))
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
    IsServed BIT NOT NULL DEFAULT 0,
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
('doctor',          '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 1
('patient',         '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 2
('ahmed.hassan',    '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 3
('sara.mohamed',    '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 4
('khaled.nour',     '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 5
('mona.farouk',     '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 6
('youssef.kamal',   '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 7
('dalia.samy',      '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 8
('hany.gamal',      '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 9
('laila.fathy',     '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 10
('tamer.hassan',    '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 11
('rania.ibrahim',   '$2a$12$Q/8dULQrRXVi6bGTVK5szO7jSPTvmw5.nAguo3qF73RWpRFTEM5Ua',  'Doctor'),   -- UserID 12
('mohamed.ali',     '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 13
('fatma.ahmed',     '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 14
('omar.khaled',     '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 15
('nour.samir',      '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 16
('hassan.mahmoud',  '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 17
('amira.tarek',     '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 18
('karim.nasser',    '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 19
('dina.walid',      '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 20
('mahmoud.fawzy',   '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 21
('yasmin.hossam',   '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 22
('samy.adel',       '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 23
('rana.mostafa',    '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 24
('wael.shawky',     '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 25
('heba.gamal',      '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient'),  -- UserID 26
('amir.zaki',       '$2a$12$Pk/Zs4JsCVWPYalLnlVpQenYy2V/5aCqObasJFeo7leUalSH9SPwy', 'Patient');  -- UserID 27
GO

-- ── DEPARTMENTS ───────────────────────────────────────────────────────────
INSERT INTO Departments (DepartmentName, Description) VALUES
('Cardiology',      'Heart and cardiovascular system specialists'),
('Neurology',       'Brain, spine and nervous system specialists'),
('Orthopedics',     'Bones, joints and musculoskeletal system'),
('Dermatology',     'Skin, hair and nail conditions'),
('Pediatrics',      'Medical care for children and adolescents'),
('Oncology',        'Cancer diagnosis and treatment'),
('Psychiatry',      'Mental health and behavioral disorders'),
('General Surgery', 'Surgical procedures and post-operative care'),
('Radiology',       'Medical imaging and diagnostic scans'),
('Emergency',       'Acute and emergency medical care');
GO

INSERT INTO Doctors (UserID, Fullname, DepartmentID, Specialization, Phone, Email, Bio, IsAvailable) VALUES
(3,  'Dr. Ahmed Hassan',  1,  'Cardiologist',          '01001234501', 'ahmed.hassan@careflow.eg',
     'Leading cardiologist with 15 years of experience. Fellowship at Cairo University. Performed over 2000 successful cardiac catheterizations.', 1),

(4,  'Dr. Sara Mohamed',  2,  'Neurologist',            '01001234502', 'sara.mohamed@careflow.eg',
     'PhD from Ain Shams University. Specialist in stroke prevention, epilepsy management and neurological rehabilitation. Published 18 peer-reviewed papers.', 1),

(5,  'Dr. Khaled Nour',   3,  'Surgeon',                '01001234503', 'khaled.nour@careflow.eg',
     'Orthopedic surgeon trained in Germany at Charite Hospital Berlin. Expert in total knee and hip replacement. 12 years restoring mobility to complex cases.', 1),

(6,  'Dr. Mona Farouk',   8,  'Surgeon',                '01001234504', 'mona.farouk@careflow.eg',
     'Head of General Surgery department. Pioneer in minimally invasive laparoscopic procedures. Reduced average patient recovery time by 40%.', 0),

(7,  'Dr. Youssef Kamal', 5,  'Cardiologist',           '01001234505', 'youssef.kamal@careflow.eg',
     'Pediatric specialist with focus on congenital heart conditions in children. Trained at Great Ormond Street Hospital, London.', 1),

(8,  'Dr. Dalia Samy',    6,  'GeneralPractitioner',    '01001234506', 'dalia.samy@careflow.eg',
     'Oncologist specializing in breast and colorectal cancer. Uses latest targeted therapy and immunotherapy protocols. Member of Egyptian Cancer Society board.', 1),

(9,  'Dr. Hany Gamal',    7,  'Psychiatrist',           '01001234507', 'hany.gamal@careflow.eg',
     'Psychiatrist with 10 years in clinical and forensic settings. Expertise in mood disorders, PTSD and addiction recovery. Trauma-informed care advocate.', 1),

(10, 'Dr. Laila Fathy',   9,  'Radiologist',            '01001234508', 'laila.fathy@careflow.eg',
     'Radiologist specializing in CT, MRI and interventional procedures. Trained at Mayo Clinic. Expert in image-guided tumor ablation.', 1),

(11, 'Dr. Tamer Hassan',  10, 'Surgeon',                '01001234509', 'tamer.hassan@careflow.eg',
     'Emergency physician with ATLS certification. 8 years managing high-volume ER cases. Known for rapid triage and decisive critical care under pressure.', 1),

(12, 'Dr. Rania Ibrahim', 4,  'GeneralPractitioner',    '01001234510', 'rania.ibrahim@careflow.eg',
     'Dermatologist specializing in medical and cosmetic skin conditions. Expertise in acne, psoriasis and skin cancer screening.', 1);
GO

INSERT INTO Patients (UserID, Fullname, DateOfBirth, Gender, Phone, BloodType, Address) VALUES
(13, 'Mohamed Ali Saeed',    '1985-03-15', 'Male',   '01098765401', 'A+',  'Cairo, Nasr City'),
(14, 'Fatma Ahmed Hassan',   '1992-07-22', 'Female', '01098765402', 'B-',  'Giza, Haram'),
(15, 'Omar Khaled Rashad',   '1978-11-08', 'Male',   '01098765403', 'O+',  'Alexandria, Sidi Gaber'),
(16, 'Nour Samir Attia',     '2000-01-30', 'Female', '01098765404', 'AB+', 'Cairo, Maadi'),
(17, 'Hassan Mahmoud Fares', '1965-09-12', 'Male',   '01098765405', 'A-',  'Cairo, Dokki'),
(18, 'Amira Tarek Zaki',     '1995-04-05', 'Female', '01098765406', 'O-',  'Alexandria, Mandara'),
(19, 'Karim Nasser Yousef',  '1988-12-19', 'Male',   '01098765407', 'B+',  'Giza, 6th October'),
(20, 'Dina Walid Kamel',     '1972-06-28', 'Female', '01098765408', 'AB-', 'Cairo, Heliopolis'),
(21, 'Mahmoud Fawzy Said',   '1990-02-14', 'Male',   '01098765409', 'A+',  'Mansoura, Dakahlia'),
(22, 'Yasmin Hossam Badr',   '2003-08-07', 'Female', '01098765410', 'O+',  'Cairo, Zamalek'),
(23, 'Samy Adel Naguib',     '1958-05-23', 'Male',   '01098765411', 'B+',  'Suez, Port Tawfik'),
(24, 'Rana Mostafa Lotfy',   '1997-10-11', 'Female', '01098765412', 'A-',  'Cairo, New Cairo'),
(25, 'Wael Shawky Ramadan',  '1983-07-04', 'Male',   '01098765413', 'O+',  'Tanta, Gharbia'),
(26, 'Heba Gamal Mansour',   '1969-03-30', 'Female', '01098765414', 'AB+', 'Cairo, Shubra'),
(27, 'Amir Zaki Fouad',      '2005-11-16', 'Male',   '01098765415', 'B-',  'Alexandria, Smouha');
GO

INSERT INTO Admissions (PatientID, DoctorID, RoomNumber, AdmittedAt, ExpectedLeave, Status) VALUES
(1,  1, '101-A', '2026-02-10 09:00', '2026-02-28', 'Admitted'),
(2,  2, '205-B', '2026-02-15 14:00', '2026-03-01', 'Critical'),
(3,  3, '310-C', '2026-02-18 11:00', '2026-02-28', 'Admitted'),
(5,  1, '102-A', '2026-02-20 08:30', '2026-03-05', 'Admitted'),
(7,  6, '408-D', '2026-02-22 16:00', '2026-03-10', 'Admitted'),
(9,  9, '001-E', '2026-02-24 02:15', '2026-02-27', 'Critical'),
(4,  4, '220-B', '2026-01-05 10:00', '2026-01-15', 'Discharged'),
(6,  5, '315-C', '2026-01-10 09:00', '2026-01-20', 'Discharged'),
(8,  7, '412-D', '2026-01-14 15:00', '2026-01-25', 'Discharged'),
(10, 2, '206-B', '2026-01-20 11:00', '2026-01-30', 'Discharged'),
(11, 3, '311-C', '2026-01-22 09:30', '2026-02-02', 'Discharged'),
(13, 1, '103-A', '2025-12-01 08:00', '2025-12-10', 'Discharged'),
(14, 8, '501-F', '2025-11-15 13:00', '2025-11-22', 'Discharged'),
(15, 9, '002-E', '2025-10-30 22:00', '2025-11-03', 'Discharged');
GO

INSERT INTO Appointments (PatientID, DoctorID, AppDateTime, Status, Note) VALUES
(1,  1, '2026-02-25 09:00', 'Confirmed', 'Follow-up cardiac stress test results'),
(2,  2, '2026-02-25 10:30', 'Confirmed', 'Weekly neurological assessment — stroke recovery'),
(4,  1, '2026-02-25 11:00', 'Pending',   'Routine heart checkup requested by patient'),
(10, 2, '2026-02-25 14:00', 'Pending',   'Persistent headaches since last discharge'),
(6,  5, '2026-02-26 09:00', 'Confirmed', 'Post-discharge pediatric follow-up'),
(12, 3, '2026-02-26 10:00', 'Pending',   'Right knee pain worsening over 3 weeks'),
(3,  3, '2026-02-26 11:30', 'Confirmed', 'Post-surgery rehabilitation check'),
(8,  7, '2026-02-27 15:00', 'Pending',   'Anxiety and sleep disorder consultation'),
(14, 10, '2026-02-27 16:00','Pending',   'Skin rash not responding to OTC treatment'),
(5,  1, '2026-02-28 08:30', 'Confirmed', 'Chest pain monitoring — hypertension management'),
(9,  9, '2026-02-28 09:00', 'Pending',   'Discharged yesterday, mild fever returned'),
(11, 6, '2026-03-01 13:00', 'Pending',   'Chemotherapy session #4 follow-up'),
(7,  6, '2026-03-02 11:00', 'Confirmed', 'Oncology treatment review and blood work'),
(13, 8, '2026-03-03 14:00', 'Pending',   'CT scan review after liver procedure'),
(15, 9, '2026-03-05 10:00', 'Pending',   'Post-ER follow-up — abdominal injury'),
(1,  2, '2026-02-10 10:00', 'Done',      'Neurological evaluation before cardiac surgery'),
(3,  1, '2026-02-05 09:00', 'Done',      'Pre-admission cardiac clearance'),
(4,  10, '2026-01-20 11:00','Done',      'Post-discharge skin wound checkup'),
(6,  2, '2026-01-25 14:00', 'Done',      'Pediatric neurology consult — febrile seizure'),
(2,  3, '2026-02-01 10:00', 'Cancelled', 'Patient hospitalized — rescheduled'),
(8,  10, '2026-01-30 15:00','Done',      'Eczema flare-up treatment follow-up'),
(10, 5, '2026-02-08 09:30', 'Cancelled', 'Patient requested cancellation'),
(11, 7, '2026-01-28 16:00', 'Done',      'Initial psychiatric evaluation');
GO

INSERT INTO ViewersList (AdmissionID, ViewerName, Relation, Phone, IsAllowed) VALUES
(1, 'Layla Ali',       'Wife',    '01120001001', 1),
(1, 'Karim Ali',       'Son',     '01120001002', 1),
(1, 'Sahar Mohamed',   'Sister',  '01120001003', 1),
(2, 'Hassan Ahmed',    'Husband', '01120002001', 0),
(2, 'Dina Ahmed',      'Sister',  '01120002002', 0),
(2, 'Youssef Fathy',   'Father',  '01120002003', 0),
(3, 'Rania Omar',      'Wife',    '01120003001', 1),
(3, 'Khaled Omar',     'Brother', '01120003002', 1),
(4, 'Samia Hassan',    'Wife',    '01120004001', 1),
(4, 'Tarek Hassan',    'Son',     '01120004002', 1),
(4, 'Eman Mahmoud',    'Daughter','01120004003', 1),
(5, 'Nadia Karim',     'Mother',  '01120005001', 1),
(5, 'Omar Nasser',     'Brother', '01120005002', 1),
(6, 'Hana Fawzy',      'Wife',    '01120006001', 0),
(6, 'Said Fawzy',      'Father',  '01120006002', 0);
GO

INSERT INTO MedicalHistory (PatientID, DoctorID, AdmissionID, Diagnosis, Notes) VALUES
(1,  1, 1,  'Coronary Artery Disease',       'Patient admitted with acute chest pain. ECG shows mild ST-segment changes. Started on dual antiplatelet therapy and statins.'),
(2,  2, 2,  'Acute Ischemic Stroke',          'Sudden onset left-sided hemiplegia. CT confirms large MCA infarct. IV thrombolysis given within 3hr window. ICU monitoring.'),
(3,  3, 3,  'Severe Knee Osteoarthritis',     'Right knee degeneration confirmed on X-ray. Total knee replacement surgery completed. Physio begins Day 3.'),
(5,  1, 4,  'Hypertensive Heart Disease',     'Chronic hypertension with early left ventricular hypertrophy. BP 165/100 on admission. Titrating antihypertensive therapy.'),
(7,  6, 5,  'Non-Hodgkin Lymphoma Stage 3',   'Third cycle of R-CHOP chemotherapy. Tolerated well. Next cycle in 21 days. CBC to be monitored weekly.'),
(9,  9, 6,  'Blunt Abdominal Trauma',         'Brought in by ambulance after road accident. Free fluid on FAST exam. Emergency laparotomy performed. Recovering in ICU.'),
(4,  4, 7,  'Acute Urticaria',                'Widespread allergic reaction — cause identified as shellfish. Treated with IV antihistamines and steroids. Discharged after 48hr.'),
(6,  5, 8,  'Viral Pneumonia',                'Pediatric patient with bilateral lower lobe infiltrates. Supportive care with O2 and antivirals. Full recovery.'),
(8,  7, 9,  'Major Depressive Episode',       'Admitted after self-referral. Started on SSRIs and CBT program. Discharged stable with outpatient follow-up plan.'),
(10, 2, 10, 'Complex Partial Seizures',       'EEG confirmed temporal lobe focus. Initiated levetiracetam. Seizure-free for 72hr before discharge. Driving restrictions advised.'),
(11, 3, 11, 'Lumbar Disc Herniation L4-L5',   'Conservative management with NSAIDs, physio and muscle relaxants. Symptoms improved 70%. Surgery deferred.'),
(13, 1, 12, 'Atrial Fibrillation',            'Paroxysmal AF detected on Holter. Rate controlled with metoprolol. Anticoagulation started. Cardiology outpatient follow-up.'),
(14, 8, 13, 'Hepatic Cyst — Interventional',  'Ultrasound-guided percutaneous drainage of 8cm hepatic cyst. Procedure successful. Post-procedure monitoring normal.'),
(15, 9, 14, 'Appendicitis',                   'Classic presentation with rebound tenderness. Emergency appendectomy performed laparoscopically. Discharged day 2 post-op.');
GO

INSERT INTO Prescriptions (RecordID, PatientID, DoctorID, Medicine, Dosage, Duration) VALUES
(1,  1,  1, 'Aspirin',        '100mg once daily at night',         '12 months'),
(1,  1,  1, 'Atorvastatin',   '40mg once daily at bedtime',        '12 months'),
(1,  1,  1, 'Clopidogrel',    '75mg once daily with food',         '6 months'),
(1,  1,  1, 'Metoprolol',     '50mg twice daily',                  '6 months'),
(2,  2,  2, 'Alteplase',      'IV 0.9mg/kg — once (hospital)',     'One time'),
(2,  2,  2, 'Clopidogrel',    '75mg once daily',                   '12 months'),
(2,  2,  2, 'Atorvastatin',   '80mg once daily at night',          '12 months'),
(2,  2,  2, 'Lisinopril',     '5mg once daily — titrate up',       '6 months'),
(3,  3,  3, 'Celecoxib',      '200mg twice daily with food',       '3 weeks'),
(3,  3,  3, 'Enoxaparin',     '40mg SC once daily',                '2 weeks'),
(3,  3,  3, 'Paracetamol',    '1g every 6 hours as needed',        '2 weeks'),
(4,  5,  1, 'Amlodipine',     '5mg once daily — may titrate up',   '3 months'),
(4,  5,  1, 'Ramipril',       '2.5mg once daily in morning',       '3 months'),
(4,  5,  1, 'Indapamide',     '1.5mg once daily in morning',       '3 months'),
(7,  4,  4, 'Cetirizine',     '10mg once daily at night',          '2 weeks'),
(7,  4,  4, 'Prednisolone',   '30mg once daily — taper over 5d',   '5 days'),
(9,  8,  7, 'Sertraline',     '50mg once daily in morning',        '6 months'),
(9,  8,  7, 'Clonazepam',     '0.5mg at night if needed',          '1 month'),
(10, 10, 2, 'Levetiracetam',  '500mg twice daily',                 '12 months'),
(12, 13, 1, 'Metoprolol',     '50mg twice daily',                  '6 months'),
(12, 13, 1, 'Rivaroxaban',    '20mg once daily with evening meal', '12 months'),
(13, 14, 8, 'Ciprofloxacin',  '500mg twice daily',                 '7 days'),
(13, 14, 8, 'Paracetamol',    '500mg every 6hr as needed',         '3 days');
GO

INSERT INTO PatientsMeals (AdmissionID, MealDate, LunchVariant, HasFruit, HasMahalabiya, IsBreakfastServed, IsLunchServed, IsDinnerServed, IsServed, Notes) VALUES

-- AdmissionID 1 — Mohamed Ali (Admitted 3 days ago)
(1, '2026-02-24', 4, 1, 0, 1, 1, 1, 1, 'Stable condition'),
(1, '2026-02-25', 6, 1, 0, 1, 1, 1, 1, NULL),
(1, '2026-02-26', 2, 1, 0, 1, 0, 0, 0, 'Under observation'),

-- AdmissionID 2 — Fatma Ahmed (Critical)
(2, '2026-02-24', 3, 0, 1, 1, 1, 1, 1, 'Strict glucose monitoring'),
(2, '2026-02-25', 5, 1, 0, 1, 1, 1, 1, NULL),
(2, '2026-02-26', 1, 1, 0, 1, 0, 0, 0, 'Awaiting lab results'),

-- AdmissionID 3 — Omar Khaled (Short stay, minor surgery)
(3, '2026-02-25', 2, 1, 0, 1, 1, 1, 1, 'Recovering well'),
(3, '2026-02-26', 4, 1, 0, 1, 1, 0, 0, 'Discharge planned tomorrow'),

-- AdmissionID 4 — Hassan Mahmoud (Admitted 3 days)
(4, '2026-02-25', 6, 1, 0, 1, 1, 1, 1, NULL),
(4, '2026-02-26', 5, 1, 0, 1, 1, 1, 1, 'Requested dessert'),
(4, '2026-02-27', 3, 1, 0, 1, 0, 0, 0, 'Blood pressure improving'),

-- AdmissionID 5 — Karim Nasser (New admission today)
(5, '2026-02-27', 7, 1, 0, 0, 0, 0, 0, 'Admitted this morning'),

-- AdmissionID 6 — Mahmoud Fawzy (Critical, irregular meal schedule)
(6, '2026-02-24', 1, 0, 0, 1, 1, 1, 1, 'Fluid restriction applied'),
(6, '2026-02-25', 4, 0, 0, 1, 1, 1, 1, NULL),
(6, '2026-02-26', 5, 1, 0, 1, 0, 0, 0, 'Dialysis scheduled');
GO

-- ── Breakfasts ────────────────────────────────────────────────────────────
INSERT INTO AppointmentDishes (DishName, MealType, Calories, ProteinG, CarbsG, FatG, SodiumMg, Description, Tags) VALUES
('Oatmeal with Berries & Flaxseed',
 'Breakfast', 320, 9.0, 54.0, 7.0, 75.0,
 'Steel-cut oats with blueberries, strawberries and ground flaxseed. Slow-release carbs, soluble fibre, omega-3.',
 'LowSodium,LowFat,Diabetic,HighFiber,HeartHealthy,GeneralWellness'),

('Scrambled Egg Whites & Spinach on Whole-Wheat Toast',
 'Breakfast', 270, 24.0, 28.0, 6.0, 290.0,
 'Three egg whites scrambled with baby spinach on one slice of whole-wheat toast.',
 'HighProtein,LowFat,LowCholesterol,WeightLoss,HeartHealthy'),

('Low-Fat Greek Yogurt with Walnuts & Cinnamon',
 'Breakfast', 250, 18.0, 22.0, 8.0, 85.0,
 'Plain low-fat Greek yogurt topped with crushed walnuts and cinnamon. Probiotic, blood-sugar stabilising.',
 'LowFat,HighProtein,Diabetic,LowSugar,GeneralWellness'),

('Avocado & Tomato on Rye Crispbread',
 'Breakfast', 310, 7.0, 34.0, 16.0, 180.0,
 'Two rye crispbreads topped with smashed avocado, sliced tomato and lemon juice.',
 'LowCholesterol,HighFiber,LowSodium,HeartHealthy,WeightLoss'),

('Boiled Eggs with Cucumber & Whole-Wheat Bread',
 'Breakfast', 290, 20.0, 26.0, 10.0, 240.0,
 'Two boiled eggs served with sliced cucumber and one slice of whole-wheat bread. Simple and protein-rich.',
 'HighProtein,LowFat,GeneralWellness,WeightLoss');

-- ── Lunches ───────────────────────────────────────────────────────────────
INSERT INTO AppointmentDishes (DishName, MealType, Calories, ProteinG, CarbsG, FatG, SodiumMg, Description, Tags) VALUES
('Grilled Chicken Breast with Quinoa & Roasted Vegetables',
 'Lunch', 480, 44.0, 38.0, 11.0, 260.0,
 'Grilled skinless chicken breast over quinoa with cherry tomatoes, cucumber and lemon dressing.',
 'HighProtein,LowSodium,WeightLoss,HeartHealthy,GeneralWellness'),

('Red Lentil & Vegetable Soup',
 'Lunch', 300, 17.0, 46.0, 4.0, 340.0,
 'Red lentil soup with carrot, celery, tomato and mild spices.',
 'HighFiber,LowFat,LowCholesterol,Hypertension,Diabetic'),

('Steamed Fish with Brown Rice & Broccoli',
 'Lunch', 420, 36.0, 44.0, 7.0, 160.0,
 'Steamed tilapia with brown rice and steamed broccoli, seasoned with lemon and herbs.',
 'LowSodium,LowFat,LowCholesterol,HeartHealthy,WeightLoss'),

('Chickpea & Roasted Vegetable Bowl',
 'Lunch', 370, 15.0, 52.0, 10.0, 210.0,
 'Roasted chickpeas and vegetables over bulgur wheat.',
 'HighFiber,LowCholesterol,Diabetic,WeightLoss,Hypertension'),

('Tuna & White Bean Salad',
 'Lunch', 390, 34.0, 32.0, 8.0, 300.0,
 'Canned tuna in water with white beans, red onion, parsley and a lemon-olive oil dressing.',
 'HighProtein,LowFat,LowCholesterol,HeartHealthy,Diabetic'),

('Turkey Meatballs in Tomato Broth with Whole-Wheat Pasta',
 'Lunch', 440, 36.0, 46.0, 9.0, 310.0,
 'Lean turkey meatballs in light tomato broth over whole-wheat pasta.',
 'HighProtein,LowFat,WeightLoss,GeneralWellness'),

('Vegetable & Tofu Stir-Fry with Brown Rice',
 'Lunch', 360, 20.0, 44.0, 12.0, 290.0,
 'Firm tofu with broccoli, snap peas and carrot in low-sodium tamari sauce over brown rice.',
 'LowSodium,LowFat,LowCholesterol,Diabetic,Hypertension,WeightLoss');

-- ── Dinners ───────────────────────────────────────────────────────────────
INSERT INTO AppointmentDishes (DishName, MealType, Calories, ProteinG, CarbsG, FatG, SodiumMg, Description, Tags) VALUES
('Baked Salmon with Asparagus & Quinoa',
 'Dinner', 510, 44.0, 30.0, 18.0, 220.0,
 'Oven-baked salmon fillet with roasted asparagus and a side of quinoa.',
 'HighProtein,LowSodium,HeartHealthy,LowCholesterol,GeneralWellness'),

('Herb-Baked Chicken Breast with Steamed Greens',
 'Dinner', 430, 42.0, 16.0, 15.0, 240.0,
 'Skinless chicken breast baked with herbs, served with steamed kale and green beans.',
 'HighProtein,LowSodium,HeartHealthy,WeightLoss,LowCholesterol'),

('Grilled Lean Beef with Roasted Sweet Potato',
 'Dinner', 470, 42.0, 26.0, 16.0, 280.0,
 'Lean beef sirloin grilled plain with roasted sweet potato and carrot.',
 'HighProtein,LowSodium,HeartHealthy'),

('Black Bean & Sweet Potato Bowl',
 'Dinner', 400, 14.0, 68.0, 5.0, 190.0,
 'Spiced black beans and roasted sweet potato over brown rice with salsa and lime.',
 'HighFiber,LowFat,LowCholesterol,Diabetic,WeightLoss'),

('Steamed White Fish with Couscous & Cucumber Salad',
 'Dinner', 390, 30.0, 44.0, 7.0, 170.0,
 'Poached white fish on fluffy couscous with cucumber, tomato and lemon.',
 'LowSodium,LowFat,LowCholesterol,HeartHealthy,WeightLoss'),

('Lentil & Spinach Stew with Flatbread',
 'Dinner', 380, 18.0, 52.0, 6.0, 200.0,
 'Hearty red lentil and baby spinach stew served with one warm flatbread.',
 'HighFiber,LowFat,LowCholesterol,Hypertension,Diabetic');
GO