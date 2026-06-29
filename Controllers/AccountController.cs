using BookingSystem.Data;
using BookingSystem.Models;
using BookingSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static BookingSystem.ViewModels.PatientDashboardViewModel;

namespace BookingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly BookingContext _context;

        public AccountController(BookingContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Patients.Add(patient);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(patient);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username == "administrator" && password == "Administration@123#789")
            {
                // Admin credentials are valid
                var adminClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin") 
                };

                var adminIdentity = new ClaimsIdentity(adminClaims, "AdminLogin");
                var adminPrincipal = new ClaimsPrincipal(adminIdentity);

                
                await HttpContext.SignInAsync(adminPrincipal);

                
                return RedirectToAction("Dashboard", "Admin");
            }

            var patient = _context.Patients.FirstOrDefault(p => p.Username == username && p.Password == password);

            if (patient != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, patient.Id.ToString()),
            new Claim(ClaimTypes.Name, patient.Username)
        };

                var claimsIdentity = new ClaimsIdentity(claims, "PatientLogin");
                await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Dashboard", "Account");
            }

            ViewBag.ErrorMessage = "Invalid username or password";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Dashboard()
        {
            var patientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(patientIdClaim))
            {
                return RedirectToAction("Login");
            }

            int patientId = int.Parse(patientIdClaim);
            var patient = _context.Patients.Find(patientId);

            if (patient == null)
            {
                return RedirectToAction("Login");
            }

            var upcomingAppointment = _context.Appointments
                .Where(a => a.PatientId == patientId && a.AppointmentDate >= DateTime.Now)
                .OrderBy(a => a.AppointmentDate)
                .Select(a => new AppointmentViewModel
                {
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status,
                    DoctorName = a.SelectedDoctor
                })
                .FirstOrDefault();

            ViewBag.PatientName = patient.Name;

            return View(new BookingSystem.ViewModels.PatientDashboardViewModel
            {
                UpcomingAppointment = upcomingAppointment
            });
        }


        public IActionResult AppointmentConfirmed()
        {
            return View();
        }

        public IActionResult BookAppointment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmAppointment(DateTime appointment_date, TimeSpan appointment_time, string selected_doctor)
        {
            var patientId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var appointment = new Appointment
            {
                PatientId = int.Parse(patientId),
                AppointmentDate = appointment_date,
                AppointmentTime = appointment_time,
                SelectedDoctor = selected_doctor,
                Status = "Confirmed"
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("AppointmentConfirmed");
        }

        public IActionResult ViewAppointments()
        {
            var patientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(patientIdClaim))
            {
                return RedirectToAction("Login");
            }

            int patientId = int.Parse(patientIdClaim);

            var appointments = _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.AppointmentDate)
                .ToList();

            return View(appointments);
        }

        public IActionResult ViewDetails()
        {
            var patientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(patientIdClaim))
            {
                return RedirectToAction("Login");
            }

            int patientId = int.Parse(patientIdClaim);
            var patient = _context.Patients.Find(patientId);

            if (patient == null)
            {
                return RedirectToAction("Login");
            }

            return View(patient);
        }

        public IActionResult VisitHistory()
        {
            var patientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(patientIdClaim))
            {
                return RedirectToAction("Login");
            }

            int patientId = int.Parse(patientIdClaim);

            var pastVisits = _context.Appointments
                .Where(a => a.PatientId == patientId && a.AppointmentDate < DateTime.Now)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new Appointment
                {
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    SelectedDoctor = a.SelectedDoctor,
                    Status = "Completed"
                })
                .ToList();

            return View(pastVisits);
        }

        public IActionResult EditDetails()
        {
            var patientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(patientIdClaim))
            {
                return RedirectToAction("Login");
            }

            int patientId = int.Parse(patientIdClaim);
            var patient = _context.Patients.Find(patientId);

            if (patient == null)
            {
                return RedirectToAction("Login");
            }

            return View(patient);
        }

        [HttpPost]
        public IActionResult UpdateDetails(Patient updatedPatient)
        {
            if (ModelState.IsValid)
            {
                var patientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int patientId = int.Parse(patientIdClaim);

                var existingPatient = _context.Patients.Find(patientId);
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
                    return RedirectToAction("ViewDetails");
                }
                else
                {
                    ModelState.AddModelError("", "Patient not found.");
                }
            }

            return View(updatedPatient);
        }

    }
}
