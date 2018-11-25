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
    public class MeasurementReminderFragment : Android.App.Fragment
    {
        Spinner spinnerMeasurementType;
        SeekBar seekbarFrequency;
        TextView startDate;
        TextView endDate;
        ListView measurementTimesListView;

        public DateTime startDay;
        public DateTime endDay;
        public DateTime dateTime;
        public List<DateTime> measurementTimes = new List<DateTime>();
        public List<string> measurementTimesString = new List<string>();
        MeasurementType measurementType;
        int selected;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            dateTime = DateTime.Now;

            View view = inflater.Inflate(Resource.Layout.AddMeasurementReminder, container, false);

            //measurement type spinner
            spinnerMeasurementType = view.FindViewById<Spinner>(Resource.Id.measurementReminderSpinner);
            //spinnerMeasurementType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerMeasurementType.Adapter = adapter;
            selected = spinnerMeasurementType.SelectedItemPosition;
            measurementType = (MeasurementType)selected;

            //times list
            measurementTimesString.Add(new DateTime(2000, 12, 12, 8, 0, 0).ToString("HH:mm"));
            measurementTimesListView = view.FindViewById<ListView>(Resource.Id.listViewMeasurementReminder);
            measurementTimesListView.FastScrollEnabled = true;
            ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, measurementTimesString);
            measurementTimesListView.Adapter = arrayAdapter;

            //setting measurement frequency per day
            seekbarFrequency = view.FindViewById<SeekBar>(Resource.Id.measurementReminderFrequency);
            TextView labelFrequency = view.FindViewById<TextView>(Resource.Id.labelMeasurementReminderFrequency);
            labelFrequency.Text = $"Ile pomiarów dziennie:  {seekbarFrequency.Progress}";
            seekbarFrequency.Progress = 1;
            seekbarFrequency.ProgressChanged += (s, e) => {
                if (e.Progress < 1) seekbarFrequency.Progress = 1;
                labelFrequency.Text = $"Ile pomiarów dziennie:  {seekbarFrequency.Progress}";

                measurementTimesString.Clear();

                int pillDelay = 0;
                if (seekbarFrequency.Progress == 2)
                    pillDelay = 720;
                if (seekbarFrequency.Progress > 2)
                    pillDelay = 840 / (seekbarFrequency.Progress - 1);

                for (int i = 0; i < seekbarFrequency.Progress; i++)
                {
                    measurementTimesString.Add(new DateTime(2000, 12, 12, 8, 0, 0).AddMinutes(pillDelay * i).ToString("HH:mm"));
                }
                arrayAdapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, measurementTimesString);
                measurementTimesListView.Adapter = arrayAdapter;
            };

            //start date choosing
            startDate = view.FindViewById<TextView>(Resource.Id.measurementReminderStartDate);
            startDate.Text = dateTime.ToLongDateString();
            startDate.Click += DateSelect_OnClick;
            startDate.TextChanged += (s, e) => { startDay = dateTime; };

            //end date choosing
            endDate = view.FindViewById<TextView>(Resource.Id.measurementReminderEndDate);
            endDate.Text = dateTime.ToLongDateString();
            endDate.Click += DateSelect_OnClick;
            endDate.TextChanged += (s,e) => { endDay = dateTime; };

            //add button
            Button buttonAdd = view.FindViewById<Button>(Resource.Id.btnAddMeasurementReminder);
            buttonAdd.Click += AddMeasurementReminder;

            return view;
        }

        private void AddMeasurementReminder(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DateSelect_OnClick(object sender, EventArgs e)
        {
            TextView textView = (TextView)sender;
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                dateTime = time;
                textView.Text = time.ToLongDateString();
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        //private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        //{
        //    selected = e.Position;
        //    measurementType = (MeasurementType)selected;
        //}
    }
}