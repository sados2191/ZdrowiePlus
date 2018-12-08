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
        TextView valueUnit;
        TextView valueUnit2;
        Spinner spinner;
        EditText measurementValue;
        EditText measurementValue2;
        LinearLayout linearLayout2;
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
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            linearLayout2 = view.FindViewById<LinearLayout>(Resource.Id.measurementLayout2);

            measurementValue = view.FindViewById<EditText>(Resource.Id.measurementValue);
            measurementValue2 = view.FindViewById<EditText>(Resource.Id.measurementValue2);
            //value unit
            valueUnit = view.FindViewById<TextView>(Resource.Id.measurementValueUnit);
            valueUnit2 = view.FindViewById<TextView>(Resource.Id.measurementValueUnit2);
            switch (spinner.SelectedItemPosition)
            {
                case 0://ciśnienie
                    valueUnit.Text = "mmHG";
                    valueUnit2.Text = "mmHG";
                    //measurementValue.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationFilter;
                    measurementValue.Text = String.Empty;
                    measurementValue2.Text = String.Empty;
                    measurementValue.Hint = "Ciśnienie skurczowe";
                    measurementValue2.Hint = "Ciśnienie rozkurczowe";
                    linearLayout2.Visibility = ViewStates.Visible;
                    break;
                case 1://poziom glukozy
                    valueUnit.Text = "mg/dL";
                    //measurementValue.InputType = Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.ClassNumber;
                    measurementValue.Text = String.Empty;
                    measurementValue.Hint = "poziom glukozy";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                case 2://temperatura
                    valueUnit.Text = "°C";
                    //measurementValue.InputType = Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.ClassNumber;
                    measurementValue.Text = String.Empty;
                    measurementValue.Hint = "temperatura";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                case 3://tętno
                    valueUnit.Text = "ud/min";
                    //measurementValue.InputType = Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.ClassNumber;
                    measurementValue.Text = String.Empty;
                    measurementValue.Hint = "tętno";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                case 4://waga
                    valueUnit.Text = "kg";
                    //measurementValue.InputType = Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.ClassNumber;
                    measurementValue.Text = String.Empty;
                    measurementValue.Hint = "waga";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                default:
                    break;
            }

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

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch (e.Position)
            {
                case 0://ciśnienie
                    valueUnit.Text = "mmHG";
                    valueUnit2.Text = "mmHG";
                    //measurementValue.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextVariationFilter;
                    measurementValue.Text = String.Empty;
                    measurementValue2.Text = String.Empty;
                    measurementValue.Hint = "Ciśnienie skurczowe";
                    measurementValue2.Hint = "Ciśnienie rozkurczowe";
                    linearLayout2.Visibility = ViewStates.Visible;
                    break;
                case 1://poziom glukozy
                    valueUnit.Text = "mg/dL";
                    //measurementValue.InputType = Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.ClassNumber;
                    measurementValue.Text = String.Empty;
                    measurementValue.Hint = "poziom glukozy";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                case 2://temperatura
                    valueUnit.Text = "°C";
                    //measurementValue.InputType = Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.ClassNumber;
                    measurementValue.Text = String.Empty;
                    measurementValue.Hint = "temperatura";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                case 3://tętno
                    valueUnit.Text = "ud/min";
                    //measurementValue.InputType = Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.ClassNumber;
                    measurementValue.Text = String.Empty;
                    measurementValue.Hint = "tętno";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                case 4://waga
                    valueUnit.Text = "kg";
                    //measurementValue.InputType = Android.Text.InputTypes.NumberFlagDecimal | Android.Text.InputTypes.ClassNumber;
                    measurementValue.Text = String.Empty;
                    measurementValue.Hint = "waga";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                default:
                    break;
            }
        }

        void AddMeasurement(object sender, EventArgs e)
        {
            int measurementType = spinner.SelectedItemPosition;
            DateTime measurementTime = new DateTime(year, month, day, hour, minute, 0);
            string value = String.Empty;
            //string description = this.Activity.FindViewById<EditText>(Resource.Id.).Text;

            var newMeasurement = new Measurement();
            newMeasurement.Date = measurementTime;
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
            if (newMeasurement.MeasurementType == MeasurementType.BloodPressure)
            {
                if (measurementValue.Text.Trim() != String.Empty && measurementValue2.Text.Trim() != String.Empty)
                {
                    value = $"{measurementValue.Text.Trim()}/{measurementValue2.Text.Trim()}";
                }
            }
            else
            {
                value = measurementValue.Text.Trim();
            }
            newMeasurement.Value = value;
            if (value != string.Empty)
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                {
                    // We have permission

                    //database connection
                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                    db.CreateTable<Measurement>();
                    db.Insert(newMeasurement); //change to GUID

                    measurementValue.Text = String.Empty;
                    measurementValue2.Text = String.Empty;
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
            measurementValue.Text = String.Empty;
            measurementValue2.Text = String.Empty;
        }
    }
}