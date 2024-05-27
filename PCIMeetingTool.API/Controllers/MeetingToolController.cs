using Microsoft.AspNetCore.Mvc;
using PCIMeetingTool.API.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;

namespace PCIMeetingTool.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MeetingToolController : ControllerBase
    {
        public MeetingToolController()
        {
        }

        [Route("GetDeserializedTeamSchedule")]
        [HttpGet]
        public ScheduleRoot GetDeserializedTeamSchedule()
        {
            //Assuming the Json object from the external REST API is stored in 'Data' folder         
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string relativePath = Path.Combine(basePath, @"..\..\..\Data\TeamSchedule.json");
            var jsonData = System.IO.File.ReadAllText(relativePath);

            var teamMemberSchedule = JsonConvert.DeserializeObject<ScheduleRoot>(jsonData,
                   new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });

            return teamMemberSchedule;
        }

        [Route("GetWorkingHoursForTeamMembers")]
        [HttpGet]
        public List<TeamMemberWorkSchedule> GetWorkingHoursForTeamMembers()
        {
            var teamSchedule = GetDeserializedTeamSchedule();
            var teamMemberWorkScheduleList = new List<TeamMemberWorkSchedule>();

            if (teamSchedule.ScheduleResult.Schedules.Count > 0)
            {
                foreach (var data in teamSchedule.ScheduleResult.Schedules)
                {
                    var teamMemberWorkSchedule = new TeamMemberWorkSchedule();
                    teamMemberWorkSchedule.WorkDuration = new List<WorkDuration>();

                    teamMemberWorkSchedule.Id = data.PersonId;
                    foreach (var item in data.Projection)
                    {
                        if (item.Description.ToLower().Contains("short break") || item.Description.ToLower().Contains("lunch"))
                        {
                            continue;
                        }
                        else
                        {
                            teamMemberWorkSchedule.WorkDuration.Add(new WorkDuration
                            {
                                StartTime = item.Start,
                                EndTime = item.Start.AddMinutes(item.Minutes)
                            });
                        }
                    }
                    teamMemberWorkScheduleList.Add(teamMemberWorkSchedule);
                }
            }
            return teamMemberWorkScheduleList;
        }

        [Route("GetStandUptimingsForTeamMembers/{requiredTeamMembersCount}")]
        [HttpGet]
        public IActionResult GetStandUptimingsForTeamMembers(int requiredTeamMembersCount)
        {
            try
            {
                var teamMembersWorkDuration = GetWorkingHoursForTeamMembers();
                var availableMeetingTimes = new List<WorkDuration>();

                var workingTimes = teamMembersWorkDuration
                    .SelectMany(x => x.WorkDuration.OrderBy(y => y.StartTime))
                    .Select(x => new WorkDuration
                    {
                        StartTime = x.StartTime,
                        EndTime = x.EndTime
                    })
                    .ToArray();

                if (!workingTimes.Any())
                {
                    return NotFound("No suitable team member data found.");
                }

                //Get the overall start and end time from the team schedule
                var overallStartTime = workingTimes.Min(x => x.StartTime);
                var overallEndTime = workingTimes.Max(x => x.EndTime);
                var teamMemberCount = 0;
                var standUpDataList = new List<StandUp>();

                //Divide the total time in 15 min span and check team member's availability
                for (var startTime = overallStartTime; startTime <= overallEndTime; startTime = startTime.AddMinutes(15))
                {
                    var endTime = startTime.AddMinutes(15);

                    foreach (var item in teamMembersWorkDuration)
                    {
                        if (item.WorkDuration.Any(x => startTime >= x.StartTime && endTime <= x.EndTime))
                        {
                            teamMemberCount++;
                        }
                    }

                    if (teamMemberCount > 0)
                    {
                        var standUpData = new StandUp
                        {
                            StartTime = startTime,
                            EndTime = endTime,
                            TeamMembercount = teamMemberCount
                        };
                        standUpDataList.Add(standUpData);
                        teamMemberCount = 0;
                    }
                }

                if (!standUpDataList.Any())
                {
                    return NotFound("No suitable stand up time found.");
                }
                else
                {
                    var finalStandUpList = standUpDataList.Where(x => x.TeamMembercount == requiredTeamMembersCount).ToList();

                    if (!finalStandUpList.Any())
                    {
                        return NotFound($"No suitable stand up timings found where {requiredTeamMembersCount} team members are available.");
                    }
                    else
                    {
                        return Ok(finalStandUpList);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong." + ex.Message);
                return null;
            }
        }
    }
}
