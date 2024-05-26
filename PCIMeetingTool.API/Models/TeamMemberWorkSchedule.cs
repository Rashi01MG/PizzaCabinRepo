namespace PCIMeetingTool.API.Models
{
    public class TeamMemberWorkSchedule
    {
        public string Id { get; set; }
        public List<WorkDuration> WorkDuration { get; set; }
    }
}
