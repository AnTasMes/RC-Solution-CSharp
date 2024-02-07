using Microsoft.VisualBasic;
using System.Text.Json.Serialization;

namespace RC_Solution_1.Models
{
    // Example data:
    // "Id": "25768d9f-502a-4776-8796-c26d76d6a6eb",
    // "EmployeeName": "Abhay Singh",
    // "StarTimeUtc": "2022-02-22T14:15:00",
    // "EndTimeUtc": "2022-02-22T12:01:00",
    // "EntryNotes": "working on project Take-Two Interactive",
    // "DeletedOn": null
    public class EmployeeModel
    {
        private Guid _id;
        private string _employeeName;
        private DateTime _starTimeUtc;
        private DateTime _endTimeUtc;
        private string _entryNotes;
        private string? _deleteon;
        private double? _totalTimeWorked;

        public EmployeeModel()
        {
            EmployeeName = "";
            StarTimeUtc = DateTime.Now;
            EndTimeUtc = DateTime.Now;
            EntryNotes = "";
            Deleteon = null;
        }

        public EmployeeModel(string employeeName, DateTime startTimeUtc, DateTime endTimeUtc, string entryNotes, string? deleteon)
        {
            EmployeeName = employeeName;
            StarTimeUtc = startTimeUtc;
            EndTimeUtc = endTimeUtc;
            EntryNotes = entryNotes;
            Deleteon = deleteon;
        }

        public void calculateTotalTimeWorked()
        {
            TotalTimeWorkedHours = Math.Abs(EndTimeUtc.Subtract(StarTimeUtc).TotalHours); 
        }

        public Guid Id { get => _id; set => _id = value; }
        public string EmployeeName { get => _employeeName; set => _employeeName = value; }
        public DateTime StarTimeUtc { get => _starTimeUtc; set => _starTimeUtc = value; }
        public DateTime EndTimeUtc { get => _endTimeUtc; set => _endTimeUtc = value; }
        public string EntryNotes { get => _entryNotes; set => _entryNotes = value; }
        public string? Deleteon { get => _deleteon; set => _deleteon = value; }
        public double? TotalTimeWorkedHours { get => _totalTimeWorked; set => _totalTimeWorked = value; }
    }
}
