# PizzaCabinRepo
### The PCIMeetingTool.API checks the JSON file (TeamSchedule.json) and calculates the available working hours of the team members. Using the working hours for each team member and an input of the number of people required for daily standup , calculates the possible meeting timings. 

- PCIMeetingTool.API contains folders Models,Data and Controllers
- PCIMeetingTool.UnitTests contains a Data folder with the Json and the unit test file.
- The Unit tests test the scenarios from the start of deserializing the Json input to testing the available meeting hours for the team.
- API specification available at : https://localhost:44393/swagger/index.html

We are assuming that the TeamSchedule.json is received from an external REST API and stored in our solution in the Data folder.
Adding to this solution, we can also make the actual HTTP calls and mock calls to the REST API which return the JSON.
