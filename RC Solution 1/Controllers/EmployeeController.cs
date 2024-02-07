using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RC_Solution_1.Models;
using System.Drawing;
using System.Text.Json.Serialization;
using System.Windows.Forms.DataVisualization.Charting;

namespace RC_Solution_1.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            // Fetch all records from the API and group them by employee name
            List<EmployeeModel>? employees = GroupRecordsByEmployeeName(GetAllRecords());

            // Order the records by the total time worked in descending order
            employees = OrderRecordsByTimeWorked(employees, "desc");

            return View(employees);
        }

        /// <summary>
        /// Fetch all records from the API
        /// </summary>
        /// <returns>Full list of employee records</returns>
        private List<EmployeeModel>? GetAllRecords()
        {
            HttpClient client = new HttpClient();
            string url = "https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code=vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
            HttpResponseMessage result = client.GetAsync(url).Result;
            List<EmployeeModel>? allEmployees = JsonConvert.DeserializeObject<List<EmployeeModel>>(result.Content.ReadAsStringAsync().Result);

            if (allEmployees != null)
                allEmployees.ForEach(e => e.calculateTotalTimeWorked());

            return allEmployees;
        }

        /// <summary>
        /// Groups the records by employee name and sums the total time worked
        /// </summary>
        /// <param name="employees">Full list of employee records</param>
        /// <returns>Grouped list</returns>
        private List<EmployeeModel> GroupRecordsByEmployeeName(List<EmployeeModel>? employees)
        {
            if (employees == null)
                return new List<EmployeeModel>();

            return employees.GroupBy(e => e.EmployeeName).Select(e => new EmployeeModel
            {
                Id = e.First().Id,
                StarTimeUtc = e.First().StarTimeUtc,
                EndTimeUtc = e.First().EndTimeUtc,
                EntryNotes = e.First().EntryNotes,
                EmployeeName = e.First().EmployeeName,
                TotalTimeWorkedHours = e.Sum(e => e.TotalTimeWorkedHours)
            })
                .Where(e => !String.IsNullOrEmpty(e.EmployeeName))
                .ToList();
        }

        /// <summary>
        /// Orders the records by the total time worked
        /// </summary>
        /// <param name="employees"></param>
        /// <param name="direction">asc | desc</param>
        /// <returns></returns>
        private List<EmployeeModel> OrderRecordsByTimeWorked(List<EmployeeModel>? employees, string direction)
        {
            if (employees == null)
                return new List<EmployeeModel>();

            if (direction == "asc")
                return employees.OrderBy(e => e.TotalTimeWorkedHours).ToList();
            else
                return employees.OrderByDescending(e => e.TotalTimeWorkedHours).ToList();
        }
    }
}
