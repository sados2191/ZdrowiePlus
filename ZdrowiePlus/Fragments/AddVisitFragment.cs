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
    public class AddVisitFragment : Android.App.Fragment
    {
        private static ListRemindersFragment visitListFragment = new ListRemindersFragment();

        TextView dateDisplay;
        TextView timeDisplay;
        Spinner remindBeforeSpinner;
        EditText reminderBeforeValue;
        int reminderMinutesBefore;
        int reminderBeforeMultiplier;
        DateTime currentTime;
        int year, month, day, hour, minute;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.AddVisit, container, false);

            //date choosing
            dateDisplay = view.FindViewById<TextView>(Resource.Id.textDate);
            dateDisplay.Click += DateSelect_OnClick;

            //time choosing
            timeDisplay = view.FindViewById<TextView>(Resource.Id.textTime);
            timeDisplay.Click += TimeSelectOnClick;

            //reminder time before visit
            reminderBeforeValue = view.FindViewById<EditText>(Resource.Id.textAddVisitReminder);
            reminderBeforeValue.SetSelectAllOnFocus(true);
            reminderBeforeValue.Text = "0";

            //spinner set remind time before visit
            remindBeforeSpinner = view.FindViewById<Spinner>(Resource.Id.addVisitReminderSpinner);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.visits_reminder_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            remindBeforeSpinner.Adapter = adapter;

            //Add visit button
            Button buttonAddVisit = view.FindViewById<Button>(Resource.Id.buttonAdd);
            buttonAddVisit.Click += AddVisit;

            Button buttonCancel = view.FindViewById<Button>(Resource.Id.buttonCancel);
            buttonCancel.Click += (s, e) =>
            {
                var trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.fragmentContainer, visitListFragment);
                //trans.AddToBackStack(null);
                trans.Commit();
            };

            return view;
        }

        void AddVisit(object sender, EventArgs e)
        {
            DateTime visitTime = new DateTime(year, month, day, hour, minute, 0);
            string title = this.Activity.FindViewById<EditText>(Resource.Id.visitTitle).Text.Trim();
            string description = this.Activity.FindViewById<EditText>(Resource.Id.visitDescription).Text;

            if (DateTime.Now > visitTime)
            {
                Toast.MakeText(this.Activity, $"Nie można zaplanować w przeszłości", ToastLength.Short).Show();
                return;
            }

            if (!int.TryParse(reminderBeforeValue.Text, out reminderMinutesBefore))
            {
                reminderMinutesBefore = 0;
            }
            else
            {
                switch (remindBeforeSpinner.SelectedItemPosition)
                {
                    case 0:
                        reminderBeforeMultiplier = 1;
                        break;
                    case 1:
                        reminderBeforeMultiplier = 60;
                        break;
                    case 2:
                        reminderBeforeMultiplier = 60 * 24;
                        break;
                    default:
                        break;
                }

                reminderMinutesBefore = reminderMinutesBefore * reminderBeforeMultiplier;
            }

            if (title != string.Empty)
            {
                //database connection
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),"zdrowieplus.db"));
                db.CreateTable<Reminder>();
                var reminder = new Reminder();
                reminder.Date = visitTime;
                reminder.MinutesBefore = reminderMinutesBefore;
                reminder.Title = title;
                reminder.Description = description;
                reminder.ReminderType = ReminderType.Visit;
                db.Insert(reminder);

                this.Activity.FindViewById<EditText>(Resource.Id.visitTitle).Text = string.Empty;
                this.Activity.FindViewById<EditText>(Resource.Id.visitDescription).Text = string.Empty;
                Toast.MakeText(this.Activity, $"Dodano", ToastLength.Short).Show();

                //Notification
                Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                notificationIntent.PutExtra("message", $"{title}. {visitTime.ToString("dd.MM.yyyy HH:mm")}");
                notificationIntent.PutExtra("title", "Wizyta");
                notificationIntent.PutExtra("id", reminder.Id);

                //notificate time
                DateTime reminderTime = visitTime.AddMinutes(reminderMinutesBefore * (-1));

                var timer = (long)reminderTime.ToUniversalTime().Subtract(
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    ).TotalMilliseconds;

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, reminder.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                alarmManager.Set(AlarmType.RtcWakeup, timer, pendingIntent);

                //go to list after save
                var trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.fragmentContainer, visitListFragment);
                //trans.AddToBackStack(null);
                trans.Commit();
            }
            else
            {
                Toast.MakeText(this.Activity, "Tytuł nie może być pusty", ToastLength.Short).Show();
                return;
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

            this.Activity.Title = "Dodaj przypomnienie";

            reminderMinutesBefore = 0;
            reminderBeforeMultiplier = 1;

            currentTime = DateTime.Now;
            year = currentTime.Year;
            month = currentTime.Month;
            day = currentTime.Day;
            hour = currentTime.Hour;
            minute = currentTime.Minute;

            dateDisplay.Text = currentTime.ToLongDateString();
            timeDisplay.Text = currentTime.ToShortTimeString();
        }
    }
}