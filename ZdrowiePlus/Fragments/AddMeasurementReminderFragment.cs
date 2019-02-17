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
    public class AddMeasurementReminderFragment : Android.App.Fragment
    {
        private static ListRemindersFragment remindersListFragment = new ListRemindersFragment();

        Spinner spinnerMeasurementType;
        SeekBar seekbarFrequency;
        TextView startDate;
        TextView endDate;
        ListView measurementTimesListView;
        TextView Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday;

        public DateTime startDay;
        public DateTime endDay;
        public DateTime dateTime;
        public List<DateTime> measurementTimes = new List<DateTime>();
        public static ListViewTimeAdapter timeListAdapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            startDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            endDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);

            View view = inflater.Inflate(Resource.Layout.AddMeasurementReminder, container, false);

            measurementTimes.Clear();

            //measurement type spinner
            spinnerMeasurementType = view.FindViewById<Spinner>(Resource.Id.measurementReminderSpinner);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinnerMeasurementType.Adapter = adapter;

            //times list
            measurementTimes.Add(new DateTime(2000, 12, 12, 8, 0, 0));
            measurementTimesListView = view.FindViewById<ListView>(Resource.Id.listViewMeasurementReminder);
            measurementTimesListView.FastScrollEnabled = true;
            timeListAdapter = new ListViewTimeAdapter(this.Activity, measurementTimes);
            measurementTimesListView.Adapter = timeListAdapter;
            measurementTimesListView.ItemClick += MeasurementTimesListView_ItemClick;

            //setting measurement frequency per day
            seekbarFrequency = view.FindViewById<SeekBar>(Resource.Id.measurementReminderFrequency);
            TextView labelFrequency = view.FindViewById<TextView>(Resource.Id.labelMeasurementReminderFrequency);
            seekbarFrequency.Progress = 1;
            labelFrequency.Text = $"Ile pomiarów dziennie:  {seekbarFrequency.Progress}";
            seekbarFrequency.ProgressChanged += (s, e) => {
                if (e.Progress < 1) seekbarFrequency.Progress = 1;
                labelFrequency.Text = $"Ile pomiarów dziennie:  {seekbarFrequency.Progress}";
                
                measurementTimes.Clear();

                int delay = 0;
                if (seekbarFrequency.Progress == 2)
                    delay = 720;
                if (seekbarFrequency.Progress > 2)
                    delay = 840 / (seekbarFrequency.Progress - 1);

                for (int i = 0; i < seekbarFrequency.Progress; i++)
                {
                    measurementTimes.Add(new DateTime(2000, 12, 12, 8, 0, 0).AddMinutes(delay * i));
                }
                timeListAdapter.NotifyDataSetChanged();
            };

            //start date choosing
            startDate = view.FindViewById<TextView>(Resource.Id.measurementReminderStartDate);
            startDate.Click += DateSelect_OnClick;
            startDate.TextChanged += (s, e) => { startDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0); };

            //end date choosing
            endDate = view.FindViewById<TextView>(Resource.Id.measurementReminderEndDate);
            endDate.Click += DateSelect_OnClick;
            endDate.TextChanged += (s,e) => { endDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59); };

            //repeat days choosing
            Monday = view.FindViewById<TextView>(Resource.Id.labelMeasurementReminderMonday);
            Monday.Selected = true;
            Monday.Click += ChangeDayStatus;

            Tuesday = view.FindViewById<TextView>(Resource.Id.labelMeasurementReminderTuesday);
            Tuesday.Selected = true;
            Tuesday.Click += ChangeDayStatus;

            Wednesday = view.FindViewById<TextView>(Resource.Id.labelMeasurementReminderWednesday);
            Wednesday.Selected = true;
            Wednesday.Click += ChangeDayStatus;

            Thursday = view.FindViewById<TextView>(Resource.Id.labelMeasurementReminderThursday);
            Thursday.Selected = true;
            Thursday.Click += ChangeDayStatus;

            Friday = view.FindViewById<TextView>(Resource.Id.labelMeasurementReminderFriday);
            Friday.Selected = true;
            Friday.Click += ChangeDayStatus;

            Saturday = view.FindViewById<TextView>(Resource.Id.labelMeasurementReminderSaturday);
            Saturday.Selected = true;
            Saturday.Click += ChangeDayStatus;

            Sunday = view.FindViewById<TextView>(Resource.Id.labelMeasurementReminderSunday);
            Sunday.Selected = true;
            Sunday.Click += ChangeDayStatus;

            //add button
            Button buttonAdd = view.FindViewById<Button>(Resource.Id.buttonAdd);
            buttonAdd.Click += AddMeasurementReminder;

            Button buttonCancel = view.FindViewById<Button>(Resource.Id.buttonCancel);
            buttonCancel.Click += (s, e) =>
            {
                var trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.fragmentContainer, remindersListFragment);
                //trans.AddToBackStack(null);
                trans.Commit();
            };

            return view;
        }

        private void ChangeDayStatus(object sender, EventArgs e)
        {
            TextView textView = (TextView)sender;

            if (textView.Selected == true)
            {
                textView.SetTextColor(Android.Graphics.Color.ParseColor("#808080"));
                textView.Selected = false;
                return;
            }
            if (textView.Selected == false)
            {
                textView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.active_text)));
                textView.Selected = true;
                return;
            }
        }

        private void MeasurementTimesListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                measurementTimes[e.Position] = time;
                timeListAdapter.NotifyDataSetChanged();
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        private void AddMeasurementReminder(object sender, EventArgs e)
        {
            if (startDay > endDay)
            {
                Toast.MakeText(this.Activity, $"Data końca nie może być wcześniej niż data początku", ToastLength.Short).Show();
                return;
            }

            if (DateTime.Now > endDay)
            {
                Toast.MakeText(this.Activity, $"Nie można zaplanować w przeszłości", ToastLength.Short).Show();
                return;
            }

            //database connection
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Reminder>();

            List<int> weekDays = new List<int>();
            if (Sunday.Selected) weekDays.Add(0);
            if (Monday.Selected) weekDays.Add(1);
            if (Tuesday.Selected) weekDays.Add(2);
            if (Wednesday.Selected) weekDays.Add(3);
            if (Thursday.Selected) weekDays.Add(4);
            if (Friday.Selected) weekDays.Add(5);
            if (Saturday.Selected) weekDays.Add(6);

            if(weekDays.Count == 0)
            {
                Toast.MakeText(this.Activity, $"Nie wybrano żadnego dnia dygodnia", ToastLength.Short).Show();
                return;
            }

            int i = 0;

            DateTime eventDay = startDay;

            while (eventDay <= endDay)
            {
                if (weekDays.Contains((int)eventDay.DayOfWeek))
                {
                    foreach (var item in measurementTimes)
                    {
                        DateTime date = new DateTime(eventDay.Year, eventDay.Month, eventDay.Day, item.Hour, item.Minute, 0);
                        if (date >= DateTime.Now)
                        {
                            var newEvent = new Reminder();
                            newEvent.Date = date;
                            newEvent.Title = spinnerMeasurementType.SelectedItem.ToString();
                            newEvent.ReminderType = ReminderType.Measurement;
                            db.Insert(newEvent);

                            //Notification
                            Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                            notificationIntent.PutExtra("message", $"{newEvent.Title}. {newEvent.Date.ToString("HH:mm")}");
                            notificationIntent.PutExtra("title", "Pomiar");
                            notificationIntent.PutExtra("id", newEvent.Id);
                            notificationIntent.PutExtra("type", spinnerMeasurementType.SelectedItemPosition);

                            var timer = (long)newEvent.Date.ToUniversalTime().Subtract(
                                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                                ).TotalMilliseconds;

                            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, newEvent.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                            AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                            alarmManager.Set(AlarmType.RtcWakeup, timer, pendingIntent);

                            //go to list after save
                            var trans = FragmentManager.BeginTransaction();
                            trans.Replace(Resource.Id.fragmentContainer, remindersListFragment);
                            //trans.AddToBackStack(null);
                            trans.Commit();

                            i++;
                        }
                    }
                }
                eventDay = eventDay.AddDays(1);
            }

            if (i == 0)
            {
                Toast.MakeText(this.Activity, $"Wybrane dni tygodnia nie zawierają się w przedziale dat.", ToastLength.Short).Show();
                return;
            }
            else
            {
                Toast.MakeText(this.Activity, $"Zaplanowano {i} pomiarów.", ToastLength.Short).Show();
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

        public override void OnResume()
        {
            base.OnResume();

            this.Activity.Title = "Dodaj przypomnienie";

            dateTime = DateTime.Now;
            startDate.Text = dateTime.ToLongDateString();
            endDate.Text = dateTime.ToLongDateString();
            
            startDate.Text = dateTime.ToLongDateString();
            startDate.Text = dateTime.ToLongDateString();
        }
    }
}