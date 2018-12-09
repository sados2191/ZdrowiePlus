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
        Spinner reminderBefore;
        EditText reminderBeforeValue;
        int reminderMinutesBefore;
        int reminderBeforeMultiplier;
        static DateTime currentTime;
        int year, month, day, hour, minute;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            currentTime = DateTime.Now;
            year = currentTime.Year;
            month = currentTime.Month;
            day = currentTime.Day;
            hour = currentTime.Hour;
            minute = currentTime.Minute;

            reminderMinutesBefore = 0;
            reminderBeforeMultiplier = 1;

            View view = inflater.Inflate(Resource.Layout.AddVisit, container, false);

            //date choosing
            dateDisplay = view.FindViewById<TextView>(Resource.Id.textDate);
            dateDisplay.Text = currentTime.ToLongDateString();
            dateDisplay.Click += DateSelect_OnClick;

            //time choosing
            timeDisplay = view.FindViewById<TextView>(Resource.Id.textTime);
            timeDisplay.Text = currentTime.ToShortTimeString();
            timeDisplay.Click += TimeSelectOnClick;

            //spinner set reminder time before visit
            reminderBefore = view.FindViewById<Spinner>(Resource.Id.addVisitReminderSpinner);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.visits_reminder_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            reminderBefore.Adapter = adapter;
            reminderBefore.ItemSelected += (s, e) => {
                reminderMinutesBefore = int.Parse(reminderBeforeValue.Text.ToString());
                switch (e.Position)
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

                reminderMinutesBefore *= reminderBeforeMultiplier;
                Toast.MakeText(this.Activity, $"{reminderMinutesBefore} minut przed", ToastLength.Short).Show();
            };

            //value reminder time before visit
            reminderBeforeValue = view.FindViewById<EditText>(Resource.Id.textAddVisitReminder);
            reminderBeforeValue.SetSelectAllOnFocus(true);
            reminderBeforeValue.Text = "0";
            reminderBeforeValue.TextChanged += (s, e) => {
                EditText value = (EditText)s;
                //if (e.Text.Count() != 0)
                //{
                //if (value.Text == String.Empty) value.Text = "0";
                if (!int.TryParse(value.Text, out int x)) x = 0;
                reminderMinutesBefore = x * reminderBeforeMultiplier;
                    Toast.MakeText(this.Activity, $"{reminderMinutesBefore} minut przed", ToastLength.Short).Show();
                //}
            };

            //Add visit button
            Button buttonAddVisit = view.FindViewById<Button>(Resource.Id.btnAddVisit);
            buttonAddVisit.Click += AddVisit;

            return view;
        }

        void AddVisit(object sender, EventArgs e)
        {
            DateTime visitTime = new DateTime(year, month, day, hour, minute, 0);
            DateTime reminderTime = visitTime.AddMinutes(reminderMinutesBefore * (-1));
            string title = this.Activity.FindViewById<EditText>(Resource.Id.visitTitle).Text.Trim();
            string description = this.Activity.FindViewById<EditText>(Resource.Id.visitDescription).Text;
            if (title != string.Empty)
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                {
                    // We have permission

                    //database connection
                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),"zdrowieplus.db"));
                    db.CreateTable<Event>();
                    var newEvent = new Event();
                    newEvent.Date = visitTime;
                    newEvent.ReminderMinutesBefore = reminderMinutesBefore;
                    newEvent.Title = title;
                    newEvent.Description = description;
                    newEvent.EventType = EventType.Visit;
                    db.Insert(newEvent); //change to GUID

                    this.Activity.FindViewById<EditText>(Resource.Id.visitTitle).Text = String.Empty;
                    this.Activity.FindViewById<EditText>(Resource.Id.visitDescription).Text = String.Empty;
                    Toast.MakeText(this.Activity, $"Dodano\n{visitTime.ToString("dd.MM.yyyy HH:mm")}\n{newEvent.Id}", ToastLength.Short).Show();

                    //Notification
                    Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                    notificationIntent.PutExtra("message", $"{visitTime.ToString("dd.MM.yyyy HH:mm")} {title}");
                    notificationIntent.PutExtra("title", "Wizyta");
                    notificationIntent.PutExtra("id", newEvent.Id);

                    var timer = (long)reminderTime.ToUniversalTime().Subtract(
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
                Toast.MakeText(this.Activity, "Tytuł nie może być pusty", ToastLength.Short).Show();
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

            //reminderBeforeValue.SetSelectAllOnFocus(true);
            dateDisplay.Text = currentTime.ToLongDateString();
            timeDisplay.Text = currentTime.ToShortTimeString();
        }
    }
}