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
        public DateTime Date { get; set; }
        public EventType EventType { get; set; }
        public string Description { get; set; }
    }

    public enum EventType
    {
        Visit,
        Medicine
    }
}