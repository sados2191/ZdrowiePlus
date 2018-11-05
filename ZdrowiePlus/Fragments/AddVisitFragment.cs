﻿using System;
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
        TextView dateDisplay;
        TextView timeDisplay;
        static DateTime currentTime = DateTime.Now;
        int year, month, day, hour, minute;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            year = currentTime.Year; month = currentTime.Month; day = currentTime.Day; hour = currentTime.Hour; minute = currentTime.Minute;

            View view = inflater.Inflate(Resource.Layout.AddVisit, container, false);

            //date choosing
            dateDisplay = view.FindViewById<TextView>(Resource.Id.textDate);
            dateDisplay.Text = currentTime.ToLongDateString();
            dateDisplay.Click += DateSelect_OnClick;

            //time choosing
            timeDisplay = view.FindViewById<TextView>(Resource.Id.textTime);
            timeDisplay.Text = currentTime.ToShortTimeString();
            timeDisplay.Click += TimeSelectOnClick;

            //Add visit button
            Button buttonAddVisit = view.FindViewById<Button>(Resource.Id.btnAddVisit);
            buttonAddVisit.Click += AddVisit;

            return view;
        }

        void AddVisit(object sender, EventArgs e)
        {
            DateTime visitTime = new DateTime(year, month, day, hour, minute, 0);
            string title = this.Activity.FindViewById<EditText>(Resource.Id.visitTitle).Text;
            string description = this.Activity.FindViewById<EditText>(Resource.Id.visitDescription).Text;
            if (title != string.Empty)
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                {
                    // We have permission

                    //database connection
                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),"events.db"));
                    db.CreateTable<Event>();
                    var newEvent = new Event();
                    newEvent.Date = visitTime;
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

                    var timer = (long)visitTime.ToUniversalTime().Subtract(
                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        ).TotalMilliseconds;

                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, newEvent.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                    AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                    alarmManager.Set(AlarmType.RtcWakeup, timer, pendingIntent);
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

            //visitAdapter.NotifyDataSetChanged();
        }
    }
}