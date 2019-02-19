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
    public class Measurement
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public MeasurementType MeasurementType { get; set; }
        public string Value { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

    public enum MeasurementType
    {
        //same order as measurements_array in Strings.xml
        BloodPressure,
        GlucoseLevel,
        Temperature,
        HeartRate,
        BodyWeight
    }
}