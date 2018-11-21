using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

namespace ZdrowiePlus.Fragments
{
    public class AddMeasurementFragment : Android.App.Fragment
    {
        TextView dateDisplay;
        TextView timeDisplay;
        Spinner spinner;
        static DateTime currentTime = DateTime.Now;
        int year, month, day, hour, minute;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            year = currentTime.Year;
            month = currentTime.Month;
            day = currentTime.Day;
            hour = currentTime.Hour;
            minute = currentTime.Minute;

            View view = inflater.Inflate(Resource.Layout.AddMeasurement, container, false);

            //measurement type spinner
            spinner = view.FindViewById<Spinner>(Resource.Id.measurementSpinner);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            //date choosing
            dateDisplay = view.FindViewById<TextView>(Resource.Id.textMeasurementDate);
            dateDisplay.Text = currentTime.ToLongDateString();
            dateDisplay.Click += DateSelect_OnClick;

            //time choosing
            timeDisplay = view.FindViewById<TextView>(Resource.Id.textMeasurementTime);
            timeDisplay.Text = currentTime.ToShortTimeString();
            timeDisplay.Click += TimeSelectOnClick;

            //Add measurement button
            Button buttonAddMeasurement = view.FindViewById<Button>(Resource.Id.btnAddMeasurement);
            buttonAddMeasurement.Click += AddMeasurement;

            return view;
        }

        void AddMeasurement(object sender, EventArgs e)
        {
            int measurementType = spinner.SelectedItemPosition;
            DateTime measurementTime = new DateTime(year, month, day, hour, minute, 0);
            string value = this.Activity.FindViewById<EditText>(Resource.Id.measurementValue).Text;
            //string description = this.Activity.FindViewById<EditText>(Resource.Id.).Text;

            var newMeasurement = new Measurement();
            newMeasurement.Date = measurementTime;
            newMeasurement.Value = value;
            switch (measurementType)
            {
                case 0:
                    newMeasurement.MeasurementType = MeasurementType.BloodPressure;
                    Toast.MakeText(this.Activity, $"{newMeasurement.MeasurementType} {measurementType}", ToastLength.Short).Show();
                    break;
                case 1:
                    newMeasurement.MeasurementType = MeasurementType.GlucoseLevel;
                    Toast.MakeText(this.Activity, $"{newMeasurement.MeasurementType} {measurementType}", ToastLength.Short).Show();
                    break;
                case 2:
                    newMeasurement.MeasurementType = MeasurementType.Temperature;
                    Toast.MakeText(this.Activity, $"{newMeasurement.MeasurementType} {measurementType}", ToastLength.Short).Show();
                    break;
                case 3:
                    newMeasurement.MeasurementType = MeasurementType.HeartRate;
                    Toast.MakeText(this.Activity, $"{newMeasurement.MeasurementType} {measurementType}", ToastLength.Short).Show();
                    break;
                case 4:
                    newMeasurement.MeasurementType = MeasurementType.BodyWeight;
                    Toast.MakeText(this.Activity, $"{newMeasurement.MeasurementType} {measurementType}", ToastLength.Short).Show();
                    break;
                default:
                    break;
            }
            if (value != string.Empty)
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                {
                    // We have permission

                    //database connection
                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                    db.CreateTable<Measurement>();
                    db.Insert(newMeasurement); //change to GUID

                    this.Activity.FindViewById<EditText>(Resource.Id.measurementValue).Text = String.Empty;
                    Toast.MakeText(this.Activity, $"Dodano\n{measurementTime.ToString("dd.MM.yyyy HH:mm")}\n{newMeasurement.Id}", ToastLength.Short).Show();

                    //go to list after save
                }
                else
                {
                    // Permission is not granted. If necessary display rationale & request.

                    //if (ActivityCompat.ShouldShowRequestPermissionRationale(this.Activity, Manifest.Permission.WriteExternalStorage))
                    //{
                    //    //Explain to the user why we need permission
                    //    Snackbar.Make(View, "Write external storage is required to save a visit", Snackbar.LengthIndefinite)
                    //            .SetAction("OK", v => ActivityCompat.RequestPermissions(this.Activity, new String[] { Manifest.Permission.WriteExternalStorage}, 1))
                    //            .Show();

                    //    return;
                    //}

                    ActivityCompat.RequestPermissions(this.Activity, new String[] { Manifest.Permission.WriteExternalStorage }, 1);

                }
            }
            else
            {
                Toast.MakeText(this.Activity, "Wartość pomiaru nie może być pusta", ToastLength.Short).Show();
            }
        }    

        void TimeSelectOnClick(object sender, EventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                timeDisplay.Text = time.ToShortTimeString();
                hour = time.Hour;
                minute = time.Minute;
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        void DateSelect_OnClick(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                dateDisplay.Text = time.ToLongDateString();
                year = time.Year;
                month = time.Month;
                day = time.Day;
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        public override void OnResume()
        {
            base.OnResume();

            dateDisplay.Text = currentTime.ToLongDateString();
            timeDisplay.Text = currentTime.ToShortTimeString();
        }
    }
}