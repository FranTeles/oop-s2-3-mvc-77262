# VGC College Management System

## Overview

This project is a College Management System built with ASP.NET Core MVC. It allows administrators, faculty, and students to manage academic data such as courses, enrolments, attendance, assignments, and exam results in a structured and secure way.

---

## Features

* CRUD for Branches, Courses, Students, Enrolments, Assignments, and Exams
* Role-based access control (Admin, Faculty, Student)
* Faculty can only access their assigned courses and students
* Student can only view their own data
* Gradebook management (Assignment Results)
* Exam results with controlled visibility (released vs provisional)
* Attendance tracking (weekly records)
* Prevents duplicate enrolments
* Server-side validation (dates, scores, business rules)
* Seeded database with sample data
* Friendly error handling (no raw exceptions)

---

## Technologies Used

* ASP.NET Core MVC (.NET 8)
* Entity Framework Core (SQLite)
* ASP.NET Identity

---

## How to Run

Clone the repository
Open in Visual Studio

Run the following command in Package Manager Console:

```powershell
Update-Database
```

Run the project (F5)

---

## Logins

### Admin

Email: [admin@vgc.ie](mailto:admin@vgc.ie)
Password: Admin123!

### Faculty

Email: [faculty@vgc.ie](mailto:faculty@vgc.ie)
Password: Faculty123!

### Student

Student 1
Email: [student1@vgc.ie](mailto:student1@vgc.ie)
Password: Student123!

Student 2
Email: [student2@vgc.ie](mailto:student2@vgc.ie)
Password: Student123!

---

## Notes

* Faculty is assigned to courses using `FacultyProfileId`
* Faculty access is restricted to relevant data (courses, students, gradebook)
* Students can only view their own results
* Exam results are hidden until released (`ResultsReleased = true`)

---

## Testing

Basic server-side validation is implemented, including:

* Date validation (EndDate > StartDate)
* Score limits (cannot exceed max score)
* Duplicate enrolment prevention

---

## CI/CD

(Can be extended with GitHub Actions for build and test automation)
