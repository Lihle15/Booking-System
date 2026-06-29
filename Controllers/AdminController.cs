using BookingSystem.Data;
using BookingSystem.Extensions;
using BookingSystem.Models;
using BookingSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace BookingSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly BookingContext _context;

        private const string AdminUsernamea = "administrator";
        private const string AdminPassword = "Administration@123#789";

        public AdminController(BookingContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == AdminUsernamea && password == AdminPassword)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            ViewBag.ErrorMessage = "Invalid username or password.";
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ViewAppointments()
        {
            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .Where(a => a.AppointmentDate >= DateTime.Now && a.AppointmentDate <= DateTime.Now.AddDays(7))
                .ToList();

            return View(appointments);
        }

        public async Task<IActionResult> SendEmailReminder(int appointmentId)
        {
            var appointment = _context.Appointments.Find(appointmentId);
            if (appointment == null)
            {
                var patient = _context.Patients.Find(appointment.PatientId);
                if (patient == null)
                {
                    var message = $"Dear {patient.Name},\n\nThis is a reminder for your appointment on {appointment.AppointmentDate.ToShortDateString()} at {appointment.AppointmentTime}.\n\nBest regards,\nAdmin";
                    await SendEmailAsync(patient.Email, "Appointment Reminder", message);
                }
            }

            return RedirectToAction("ViewAppointments");
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient("smtp.your-email-provider.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("your-email@example.com", "your-password"),
                EnableSsl = true,
            };

            await smtpClient.SendMailAsync(new MailMessage("your-email@example.com", email, subject, message));
        }


        public IActionResult ManagePatients()
        {
            var patients = _context.Patients.ToList();
            return View(patients);
        }

        public IActionResult ViewPatient(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        public IActionResult CreatePatient()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePatient(Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Patients.Add(patient);
                _context.SaveChanges();
                return RedirectToAction("ManagePatients");
            }
            return View(patient);
        }

        public IActionResult EditPatient(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        [HttpPost]
        public IActionResult EditPatient(Patient updatedPatient)
        {
            if (ModelState.IsValid)
            {
                var existingPatient = _context.Patients.Find(updatedPatient.Id);
                if (existingPatient != null)
                {
                    existingPatient.Username = updatedPatient.Username;
                    existingPatient.Password = updatedPatient.Password;
                    existingPatient.Name = updatedPatient.Name;
                    existingPatient.Surname = updatedPatient.Surname;
                    existingPatient.Email = updatedPatient.Email;
                    existingPatient.PhoneNo = updatedPatient.PhoneNo;
                    existingPatient.DateOfBirth = updatedPatient.DateOfBirth;

                    _context.SaveChanges();
                    return RedirectToAction("ManagePatients");
                }
                else
                {
                    ModelState.AddModelError("", "Patient not found.");
                }
            }
            return View(updatedPatient);
        }

        public IActionResult DeletePatient(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                _context.SaveChanges();
            }
            return RedirectToAction("ManagePatients");
        }

        public IActionResult ManageDoctors()
        {
            var doctors = _context.Doctors.ToList();
            return View(doctors);
        }

        public IActionResult ResetPassword(int id)
        {
            var patient = _context.Patients.Find(id);
            var doctor = _context.Doctors.Find(id);

            if (patient != null)
            {
                patient.Password = "NewDefaultPassword123#";
                _context.SaveChanges();
            }
            else if (doctor != null)
            {
                ViewBag.Message = "Doctor passwords are managed separately and cannot be reset here.";
                return RedirectToAction("ManageDoctors");
            }
            else
            {
                return NotFound();
            }

            return RedirectToAction("ManagePatients");
        }

        public async Task<IActionResult> ViewAllAppointments()
        {
            var appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .ToListAsync();

            return View(appointments);
        }

        public IActionResult EditAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        [HttpPost]
        public IActionResult EditAppointment(Appointment updatedAppointment)
        {
            if (ModelState.IsValid)
            {
                var existingAppointment = _context.Appointments.Find(updatedAppointment.Id);
                if (existingAppointment != null)
                {
                    existingAppointment.AppointmentDate = updatedAppointment.AppointmentDate;
                    existingAppointment.AppointmentTime = updatedAppointment.AppointmentTime;
                    existingAppointment.SelectedDoctor = updatedAppointment.SelectedDoctor; 
                    _context.SaveChanges();
                    return RedirectToAction("ViewAllAppointments");
                }
            }
            return View(updatedAppointment);
        }

        public IActionResult CancelAppointment(int id)
        {
            var appointment = _context.Appointments.Find(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewAllAppointments");
        }

        [HttpGet]
        public IActionResult EditDoctor(int id)
        {
            var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }
            return View(doctor);
        }

        [HttpPost]
        public IActionResult EditDoctor(DoctorModel model)
        {
            if (ModelState.IsValid)
            {
                var doctor = _context.Doctors.FirstOrDefault(d => d.DoctorId == model.DoctorId);
                if (doctor != null)
                {
                    doctor.Name = model.Name;
                    doctor.Specialty = model.Specialty;
                    doctor.Availability = model.Availability;
                    doctor.ContactDetails = model.ContactDetails;
                    doctor.Status = model.Status;  

                    _context.SaveChanges();  
                    return RedirectToAction("ManageDoctors");  
                }
                else
                {
                    return NotFound();  
                }
            }
            return View(model);  
        }

        public IActionResult CreateDoctor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateDoctor(DoctorModel doctorModel)
        {
            if (ModelState.IsValid)
            {
                _context.Doctors.Add(doctorModel);
                _context.SaveChanges();
                return RedirectToAction("ManageDoctors");
            }
            return View(doctorModel);
        }




        public IActionResult SendReminders()
        {
            return View();
        }

    }
}
