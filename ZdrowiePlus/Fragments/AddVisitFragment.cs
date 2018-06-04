﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
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
        int year = currentTime.Year, month = currentTime.Month, day = currentTime.Day, hour = currentTime.Hour, minute = currentTime.Minute;

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
            dateDisplay.Text = currentTime.ToLongDateString();
            dateDisplay.Click += DateSelect_OnClick;

            //time choosing
            timeDisplay = view.FindViewById<TextView>(Resource.Id.textTime);
            timeDisplay.Text = currentTime.ToShortTimeString();
            timeDisplay.Click += TimeSelectOnClick;

            //Adding visit
            Button buttonAddVisit = view.FindViewById<Button>(Resource.Id.btnAddVisit);
            buttonAddVisit.Click += AddVisit;

            return view;
        }

        void AddVisit(object sender, EventArgs e)
        {
            DateTime visitTime = new DateTime(year, month, day, hour, minute, 0);
            string description = this.Activity.FindViewById<EditText>(Resource.Id.visitDescription).Text;
            if (description != string.Empty)
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                {
                    // We have permission
                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),"events.db"));
                    db.CreateTable<Event>();
                    var newEvent = new Event();
                    newEvent.Date = visitTime;
                    newEvent.Description = description;
                    newEvent.EventType = EventType.Visit;
                    db.Insert(newEvent);

                    //MainActivity.visitList.Add($"{visitTime.ToString("dd.MM.yyyy HH:mm")} {description}");
                    this.Activity.FindViewById<EditText>(Resource.Id.visitDescription).Text = String.Empty;
                    Toast.MakeText(this.Activity, $"Dodano\n{visitTime.ToString("dd.MM.yyyy HH:mm")}\n{description}", ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, "Opis nie może być pusty", ToastLength.Short).Show();
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
    }
}