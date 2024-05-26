using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PCIMeetingTool.API.Controllers;
using PCIMeetingTool.API.Models;

namespace PCIMeetingTool.UnitTests
{
    public class TeamScheduleTests
    {
        [Fact]
        public void CanGetDeserializedTeamSchedule()
        {
            //Arrange
            var sut = new MeetingToolController();

            //Act
            var result = sut.GetDeserializedTeamSchedule();

            //Assert
            result.Should().BeOfType<ScheduleRoot>();
        }

        [Fact]
        public void CanGetWorkDurationForTeamMembers()
        {
            //Arrange
            var sut = new MeetingToolController();

            //Act
            var result = sut.GetWorkingHoursForTeamMembers();

            //Asserts
            result.Should().NotBeNull();
            result.Should().Contain(x => x.WorkDuration.Count() > 0);
        }

        [Fact]
        public void CanGetResponseForFifteenMinStandUpTime()
        {
            //Arrange
            var sut = new MeetingToolController();
            var standUpTimeInMinutes = 15;

            //Acts
            var result = sut.GetStandUptimingsForTeamMembers(4);

            //Assert
            var okObjectResult = (OkObjectResult)result;

            okObjectResult.Should().NotBeNull();
            okObjectResult.Value.Should().BeOfType<List<StandUp>>();
            var standUpTimes = okObjectResult.Value as List<StandUp>;

            standUpTimes.Select(x => (x.EndTime - x.StartTime).Minutes).FirstOrDefault().Should().Be(standUpTimeInMinutes);
        }

        [Theory]
        [InlineData(8, 15)]
        [InlineData(4, 30)]
        public void CanGetStandUpTimeForSpecifiedTeamMembers(int teamMemberRequired, int availableTimeDurations)
        {
            //Arrange
            var sut = new MeetingToolController();

            //Act
            var result = sut.GetStandUptimingsForTeamMembers(teamMemberRequired);

            //Assert
            var okObjectResult = (OkObjectResult)result;
            var standUpTimes = okObjectResult.Value as List<StandUp>;

            standUpTimes.Should().HaveCount(availableTimeDurations);
        }

        [Fact]
        public void CanGetResponseForNoTeamMembersFound()
        {
            //Arrange
            var sut = new MeetingToolController();

            //Act
            var result = sut.GetStandUptimingsForTeamMembers(5);

            //Assert
            var notFoundResult = (NotFoundObjectResult)result;
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
