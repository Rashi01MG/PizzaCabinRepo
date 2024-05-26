namespace PCIMeetingTool.API.Models
{
    public class ScheduleRoot
    {
        public ScheduleResult ScheduleResult { get; set; }
    }
    public class ScheduleResult
    {
        public List<Schedule> Schedules { get; set; }
    }
    public class Schedule
    {
        public int ContractTimeMinutes { get; set; }
        public DateTime Date { get; set; }
        public bool IsFullDayAbsence { get; set; }
        public string Name { get; set; }
        public string PersonId { get; set; }                        
        public List<Projection> Projection { get; set; }
    }
    public class Projection
    {
        public string Color { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public int Minutes { get; set; }
    }
}
