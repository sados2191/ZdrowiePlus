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
    public class Event
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } //change to GUID
        public int Skipped { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int ReminderMinutesBefore { get; set; } //for visit reminder
        public int Count { get; set; } //foe medicine reminder
        public EventType EventType { get; set; }
        public string Description { get; set; }
        //public MeasurementType MeasurementType { get; set; } // if event = measurement
    }

    public enum EventType
    {
        Visit,
        Medicine,
        Measurement
    }
}