namespace BookingSystem.ViewModels
{
    public class PatientDashboardViewModel
    {
        public List<AppointmentViewModel> Appointments { get; set; }
        public AppointmentViewModel UpcomingAppointment { get; set; }
    }

    public class AppointmentViewModel
    {
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string DoctorName { get; set; }
        public string Status { get; set; }
    }

}
