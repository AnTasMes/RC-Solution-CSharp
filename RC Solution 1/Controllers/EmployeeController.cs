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
        /// Generates a pie chart of the total time worked by each employee
        /// And sends the image as a response
        /// </summary>
        /// <returns></returns>
        public IActionResult GeneratePieChart()
        {
            List<EmployeeModel>? employees = GroupRecordsByEmployeeName(GetAllRecords());

            var bitmap = new Bitmap(900, 900);
            var graphics = Graphics.FromImage(bitmap);

            // Isolate data
            var names = employees.Select(e => e.EmployeeName).ToArray();
            var hoursWorked = employees.Select(e => e.TotalTimeWorkedHours).ToArray();
            var totalHoursWorked = hoursWorked.Sum();

            // Generate colors
            var colors = new Color[names.Length];
            var random = new Random();
            for (int i = 0; i < names.Length; i++)
            {
                colors[i] = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
            }

            // Draw pie chart
            var startAngle = 0.0f;

            // Add background
            graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, 1000, 1000);

            for (var i = 0; i < hoursWorked.Length; i++)
            {
                // Data points
                var sweepAngle = Convert.ToSingle((hoursWorked[i] / totalHoursWorked) * 360.0f);
                graphics.FillPie(new SolidBrush(colors[i]), 250, 250, 400, 400, startAngle, sweepAngle);
                startAngle += sweepAngle;

                // Legend
                var percentage = (hoursWorked[i] / totalHoursWorked) * 100;
                graphics.FillRectangle(new SolidBrush(colors[i]), 100, 100 + (i * 20), 10, 10);
                graphics.DrawString($"{names[i]}: {percentage:0.00}%", new Font("Arial", 12), new SolidBrush(Color.Black), 120, 100 + (i * 20));
            }

            // Save image
            var stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            stream.Position = 0;

            graphics.Dispose();

            return File(stream, "image/png", "work_hours.png");
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
