using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;

namespace ZdrowiePlus
{
    public class Reminder
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int MinutesBefore { get; set; }
        public int Skipped { get; set; }
        public int Count { get; set; } //for medicine reminder
        public ReminderType ReminderType { get; set; }
        public string Description { get; set; }
    }

    public enum ReminderType
    {
        Visit,
        Medicine,
        Measurement
    }
}