using BookingSystem.Data;
using BookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookingSystem.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly BookingContext _context;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(BookingContext context, ILogger<AppointmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        
        [HttpGet]
        public IActionResult BookAppointment()
        {
            var allDoctors = _context.Doctors
                .Where(d => d.Status == "Active")
                .Select(d => new { d.Name, d.Specialty })
                .ToList();

            ViewBag.Doctors = allDoctors;
            return View();
        }

        
        [HttpPost]
        public IActionResult BookAppointment(int patientId, DateTime appointment_date, TimeSpan appointment_time, string doctorName)
        {
            if (!ModelState.IsValid)
            {
                // If validation fails, reload doctors to repopulate the dropdown
                ViewBag.Doctors = _context.Doctors
                    .Where(d => d.Status == "Active")
                    .Select(d => new { d.Name, d.Specialty })
                    .ToList();

                return View();
            }

            var appointment = new Appointment
            {
                PatientId = patientId,
                AppointmentDate = appointment_date,
                AppointmentTime = appointment_time,
                SelectedDoctor = doctorName,
                Status = "Confirmed"
            };

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("Confirmation");
        }

        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
