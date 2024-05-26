namespace PCIMeetingTool.API.Models
{
    public class WorkDuration
    {
        public WorkDuration()
        {
            
        }
        public WorkDuration(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }      
    }
}