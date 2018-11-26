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
        private static VisitListFragment visitListFragment = new VisitListFragment();

        Spinner spinnerMeasurementType;
        SeekBar seekbarFrequency;
        TextView startDate;
        TextView endDate;
        //TextView repeatDays;
        ListView measurementTimesListView;

        public DateTime startDay;
        public DateTime endDay;
        public DateTime dateTime;
        public List<DateTime> measurementTimes = new List<DateTime>();
        public List<string> measurementTimesString = new List<string>(); //add custom list adapter
        ArrayAdapter<string> arrayTimeAdapter;
        MeasurementType measurementType;
        //static string[] weekDays;
        //bool[] checkedItems;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            dateTime = DateTime.Now;
            startDay = DateTime.Now;
            endDay = DateTime.Now;

            View view = inflater.Inflate(Resource.Layout.AddMeasurementReminder, container, false);

            measurementTimesString.Clear();
            measurementTimes.Clear();

            //measurement type spinner
            spinnerMeasurementType = view.FindViewById<Spinner>(Resource.Id.measurementReminderSpinner);
            //spinnerMeasurementType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerMeasurementType.Adapter = adapter;
            measurementType = (MeasurementType)spinnerMeasurementType.SelectedItemPosition;

            //times list
            measurementTimesString.Add(new DateTime(2000, 12, 12, 8, 0, 0).ToString("HH:mm"));
            measurementTimes.Add(new DateTime(2000, 12, 12, 8, 0, 0));
            measurementTimesListView = view.FindViewById<ListView>(Resource.Id.listViewMeasurementReminder);
            measurementTimesListView.FastScrollEnabled = true;
            arrayTimeAdapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, measurementTimesString);
            measurementTimesListView.Adapter = arrayTimeAdapter;
            measurementTimesListView.ItemClick += MeasurementTimesListView_ItemClick;

            //setting measurement frequency per day
            seekbarFrequency = view.FindViewById<SeekBar>(Resource.Id.measurementReminderFrequency);
            TextView labelFrequency = view.FindViewById<TextView>(Resource.Id.labelMeasurementReminderFrequency);
            labelFrequency.Text = $"Ile pomiarów dziennie:  {seekbarFrequency.Progress}";
            seekbarFrequency.Progress = 1;
            seekbarFrequency.ProgressChanged += (s, e) => {
                if (e.Progress < 1) seekbarFrequency.Progress = 1;
                labelFrequency.Text = $"Ile pomiarów dziennie:  {seekbarFrequency.Progress}";

                measurementTimesString.Clear();
                measurementTimes.Clear();

                int delay = 0;
                if (seekbarFrequency.Progress == 2)
                    delay = 720;
                if (seekbarFrequency.Progress > 2)
                    delay = 840 / (seekbarFrequency.Progress - 1);

                for (int i = 0; i < seekbarFrequency.Progress; i++)
                {
                    measurementTimes.Add(new DateTime(2000, 12, 12, 8, 0, 0).AddMinutes(delay * i));
                    measurementTimesString.Add(new DateTime(2000, 12, 12, 8, 0, 0).AddMinutes(delay * i).ToString("HH:mm"));
                }
                arrayTimeAdapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, measurementTimesString);
                measurementTimesListView.Adapter = arrayTimeAdapter;
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

            //repeat days choosing
            //weekDays = Resources.GetStringArray(Resource.Array.week_days_array);
            //checkedItems = new bool[weekDays.Length];
            //repeatDays = view.FindViewById<TextView>(Resource.Id.measurementReminderRepeatDays);
            //repeatDays.Click += WeekDaysDialog;

            //add button
            Button buttonAdd = view.FindViewById<Button>(Resource.Id.btnAddMeasurementReminder);
            buttonAdd.Click += AddMeasurementReminder;

            return view;
        }

        //private void WeekDaysDialog(object sender, EventArgs e)
        //{
        //    AlertDialog.Builder mBuilder = new AlertDialog.Builder(this.Activity);
        //    mBuilder.SetTitle("Dni tygodnia");
        //    //mBuilder.SetMultiChoiceItems(weekDays, checkedItems,IDialogInterfaceOnMultiChoiceClickListener);
        //    mBuilder.Create();
        //}

        private void MeasurementTimesListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                //property hour and minute are read only
                measurementTimes[e.Position] = measurementTimes[e.Position].AddHours(time.Hour - measurementTimes[e.Position].Hour);
                measurementTimes[e.Position] = measurementTimes[e.Position].AddMinutes(time.Minute - measurementTimes[e.Position].Minute);
                measurementTimesString[e.Position] = measurementTimes[e.Position].ToString("HH:mm");
                //arrayAdapter.NotifyDataSetChanged();
                arrayTimeAdapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, measurementTimesString);
                measurementTimesListView.Adapter = arrayTimeAdapter;
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        private void AddMeasurementReminder(object sender, EventArgs e)
        {
            if (startDay > endDay)
            {
                Toast.MakeText(this.Activity, $"Data końca nie może być wcześniej nić data początku", ToastLength.Short).Show();
                return;
            }
            //List<DateTime> times = new List<DateTime>();

            if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
            {
                // We have permission

                //database connection
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                db.CreateTable<Event>();

                while (startDay <= endDay)
                {
                    foreach (var item in measurementTimes)
                    {
                        DateTime date = new DateTime(startDay.Year, startDay.Month, startDay.Day, item.Hour, item.Minute, 0);
                        if (date >= DateTime.Now)
                        {
                            var newEvent = new Event();
                            newEvent.Date = date;
                            newEvent.Title = spinnerMeasurementType.GetItemAtPosition(spinnerMeasurementType.SelectedItemPosition).ToString();
                            newEvent.EventType = EventType.Measurement;
                            db.Insert(newEvent);

                            //Notification
                            Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                            notificationIntent.PutExtra("message", $"{newEvent.Date.ToString("dd.MM.yyyy HH:mm")} {newEvent.Title}");
                            notificationIntent.PutExtra("title", "Pomiar");
                            notificationIntent.PutExtra("id", newEvent.Id);

                            var timer = (long)newEvent.Date.ToUniversalTime().Subtract(
                                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                                ).TotalMilliseconds;

                            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, newEvent.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                            AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                            alarmManager.Set(AlarmType.RtcWakeup, timer, pendingIntent);

                            //go to list after save
                            var trans = FragmentManager.BeginTransaction();
                            trans.Replace(Resource.Id.fragmentContainer, visitListFragment);
                            trans.AddToBackStack(null);
                            trans.Commit();
                        }
                    }
                    startDay = startDay.AddDays(1);
                }
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

        public override void OnResume()
        {
            base.OnResume();

            measurementTimesString.Clear();
            measurementTimes.Clear();
        }
    }
}