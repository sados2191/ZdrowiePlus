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
        const string SHARED_PREFS = "shared preferences";
        const string HEIGHT = "height";

        ListMeasurementsFragment measurementListFragment;

        TextView dateDisplay;
        TextView timeDisplay;
        TextView valueUnit;
        TextView valueUnit2;
        Spinner spinner;
        EditText measurementValue;
        EditText measurementValue2;
        LinearLayout linearLayout2;
        Button buttonCancel;
        DateTime currentTime;
        int year, month, day, hour, minute;
        int reminderId = 0;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            measurementListFragment = new ListMeasurementsFragment();

            View view = inflater.Inflate(Resource.Layout.AddMeasurement, container, false);
            
            //measurement type spinner
            spinner = view.FindViewById<Spinner>(Resource.Id.measurementSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            //additional for blood presure and height
            linearLayout2 = view.FindViewById<LinearLayout>(Resource.Id.measurementLayout2);

            //Edit text boxes for values
            measurementValue = view.FindViewById<EditText>(Resource.Id.measurementValue);
            measurementValue2 = view.FindViewById<EditText>(Resource.Id.measurementValue2);

            //value unit
            valueUnit = view.FindViewById<TextView>(Resource.Id.measurementValueUnit);
            valueUnit2 = view.FindViewById<TextView>(Resource.Id.measurementValueUnit2);

            //set measurement type based on spinner position
            SetMeasurementType(spinner.SelectedItemPosition);

            //date choosing
            dateDisplay = view.FindViewById<TextView>(Resource.Id.textMeasurementDate);
            dateDisplay.Click += DateSelect_OnClick;

            //time choosing
            timeDisplay = view.FindViewById<TextView>(Resource.Id.textMeasurementTime);
            timeDisplay.Click += TimeSelectOnClick;

            //Add measurement button
            Button buttonAddMeasurement = view.FindViewById<Button>(Resource.Id.buttonAdd);
            buttonAddMeasurement.Click += AddMeasurement;

            buttonCancel = view.FindViewById<Button>(Resource.Id.buttonCancel);
            buttonCancel.Click += (s, e) =>
            {
                if (reminderId > 0)
                {
                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                    Reminder fromNotification = db.Get<Reminder>(reminderId);
                    fromNotification.Skipped = 1;
                    db.Update(fromNotification);
                }

                var trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.fragmentContainer, measurementListFragment);
                //trans.AddToBackStack(null);
                trans.Commit();
            };

            return view;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //set measurement type based on spinner position
            SetMeasurementType(e.Position);
        }

        private void SetMeasurementType(int switch_case)
        {
            switch (switch_case)
            {
                case 0://ciśnienie
                    valueUnit.Text = "mmHG";
                    valueUnit2.Text = "mmHG";
                    measurementValue.Text = string.Empty;
                    measurementValue2.Text = string.Empty;
                    measurementValue.Hint = "Ciśnienie skurczowe";
                    measurementValue2.Hint = "Ciśnienie rozkurczowe";
                    linearLayout2.Visibility = ViewStates.Visible;
                    break;
                case 1://poziom glukozy
                    valueUnit.Text = "mg/dL";
                    measurementValue.Text = string.Empty;
                    measurementValue.Hint = "Poziom glukozy";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                case 2://temperatura
                    valueUnit.Text = "°C";
                    measurementValue.Text = string.Empty;
                    measurementValue.Hint = "temperatura";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                case 3://tętno
                    valueUnit.Text = "ud/min";
                    measurementValue.Text = string.Empty;
                    measurementValue.Hint = "tętno";
                    linearLayout2.Visibility = ViewStates.Gone;
                    break;
                case 4://waga
                    ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences(SHARED_PREFS, FileCreationMode.Private);
                    ISharedPreferencesEditor editor = sharedPreferences.Edit();

                    valueUnit.Text = "kg";
                    measurementValue.Text = string.Empty;
                    measurementValue.Hint = "Waga";

                    valueUnit2.Text = "cm";
                    measurementValue2.Text = sharedPreferences.GetString(HEIGHT, string.Empty);
                    measurementValue2.Hint = "Wzrost";
                    linearLayout2.Visibility = ViewStates.Visible;
                    break;
                default:
                    break;
            }
        }


        void AddMeasurement(object sender, EventArgs e)
        {
            int measurementType = spinner.SelectedItemPosition;
            DateTime measurementTime = new DateTime(year, month, day, hour, minute, 0);
            string value = string.Empty;

            var newMeasurement = new Measurement();
            newMeasurement.Date = measurementTime;
            switch (measurementType)
            {
                case 0:
                    newMeasurement.MeasurementType = MeasurementType.BloodPressure;
                    break;
                case 1:
                    newMeasurement.MeasurementType = MeasurementType.GlucoseLevel;
                    break;
                case 2:
                    newMeasurement.MeasurementType = MeasurementType.Temperature;
                    break;
                case 3:
                    newMeasurement.MeasurementType = MeasurementType.HeartRate;
                    break;
                case 4:
                    newMeasurement.MeasurementType = MeasurementType.BodyWeight;

                    ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences(SHARED_PREFS, FileCreationMode.Private);
                    ISharedPreferencesEditor editor = sharedPreferences.Edit();
                    editor.PutString(HEIGHT, measurementValue2.Text.Trim());
                    editor.Apply();
                    
                    break;
                default:
                    break;
            }
            if (newMeasurement.MeasurementType == MeasurementType.BloodPressure || newMeasurement.MeasurementType == MeasurementType.BodyWeight)
            {
                if (measurementValue.Text.Trim() != string.Empty && measurementValue2.Text.Trim() != string.Empty)
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
                //database connection
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                db.CreateTable<Measurement>();
                db.Insert(newMeasurement);

                measurementValue.Text = string.Empty;
                measurementValue2.Text = string.Empty;
                Toast.MakeText(this.Activity, $"Dodano", ToastLength.Short).Show();

                //if opened from notification
                if (reminderId > 0)
                {
                    Reminder fromNotification = db.Get<Reminder>(reminderId);
                    fromNotification.Skipped = 2;
                    db.Update(fromNotification);
                }

                //go to list after save
                var trans = FragmentManager.BeginTransaction();
                Bundle bundle = new Bundle();
                bundle.PutInt("type", measurementType);
                measurementListFragment.Arguments = bundle;
                trans.Replace(Resource.Id.fragmentContainer, measurementListFragment);
                //trans.AddToBackStack(null);
                trans.Commit();
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

            this.Activity.Title = "Dodaj pomiar";

            currentTime = DateTime.Now;
            year = currentTime.Year;
            month = currentTime.Month;
            day = currentTime.Day;
            hour = currentTime.Hour;
            minute = currentTime.Minute;

            dateDisplay.Text = currentTime.ToLongDateString();
            timeDisplay.Text = currentTime.ToShortTimeString();

            dateDisplay.Text = currentTime.ToLongDateString();
            timeDisplay.Text = currentTime.ToShortTimeString();
            measurementValue.Text = string.Empty;
            measurementValue2.Text = string.Empty;
        }

        public override void OnViewStateRestored(Bundle savedInstanceState)
        {
            base.OnViewStateRestored(savedInstanceState);

            //if opened by notification get measurement type position that was passed
            if (Arguments != null)
            {
                if (!Arguments.IsEmpty)
                {
                    reminderId = Arguments.GetInt("id", 0);
                    int spinnerPosition = Arguments.GetInt("type", 0);
                    spinner.SetSelection(spinnerPosition, true);
                    //Arguments = null;
                    Arguments.Clear();

                    if (reminderId > 0)
                    {
                        buttonCancel.Text = "Pomiń";
                    }
                    else
                    {
                        buttonCancel.Text = "Anuluj";
                    }
                }
            }
        }
    }
}