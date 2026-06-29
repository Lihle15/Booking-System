using BookingSystem.Models;

namespace BookingSystem.ViewModels
{
    public class BookAppointmentViewModel
    {
        public Appointment Appointment { get; set; }
        public List<DoctorModel> Doctors { get; set; }
        public string SelectedDoctor { get; set; }
    }
}
