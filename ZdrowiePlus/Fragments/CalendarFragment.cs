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
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

namespace ZdrowiePlus.Fragments
{
    public class CalendarFragment : Android.App.Fragment
    {
        private static EditReminderFragment editReminderFragment = new EditReminderFragment();

        public static ListViewCalendarAdapter listAdapter;
        public CalendarView calendarView;
        ListView remindersListView;

        List<Event> events = new List<Event>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.Calendar, container, false);
            calendarView = view.FindViewById<CalendarView>(Resource.Id.calendarView1);

            calendarView.DateChange += DateSelect;

            if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {
                //We have permission

                //MainActivity.visitList.Clear();

                //database connection
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                db.CreateTable<Event>();

                //List<Event> events = new List<Event>();

                DateTime nextDay = DateTime.Today.AddDays(1);

                events.Clear();
                events = db.Table<Event>().Where(e => e.Date >= DateTime.Today && e.Date < nextDay).OrderBy(e => e.Date).ToList();

                //foreach (var visit in events)
                //{
                //    MainActivity.visitList.Add(visit);
                //}

                listAdapter = new ListViewCalendarAdapter(this.Activity, /* MainActivity.visitList */ events);
                remindersListView = view.FindViewById<ListView>(Resource.Id.listViewCalendar);
                remindersListView.Adapter = listAdapter;
                remindersListView.FastScrollEnabled = true;

                remindersListView.ItemClick += reminder_ItemClick;
            }
            else
            {
                // Permission is not granted. If necessary display rationale & request.

                //if (ActivityCompat.ShouldShowRequestPermissionRationale(this.Activity, Manifest.Permission.ReadExternalStorage))
                //{
                //    //Explain to the user why we need permission
                //    Snackbar.Make(View, "Read external storage is required to read the events", Snackbar.LengthIndefinite)
                //            .SetAction("OK", v => ActivityCompat.RequestPermissions(this.Activity, new String[] { Manifest.Permission.ReadExternalStorage}, 2))
                //            .Show();

                //    return;
                //}

                ActivityCompat.RequestPermissions(this.Activity, new String[] { Manifest.Permission.ReadExternalStorage }, 2);

            }

            return view; 
        }

        private void reminder_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            int id = events[e.Position].Id; //przesłać id w bundlu

            Bundle bundle = new Bundle();
            bundle.PutInt("id", id);
            editReminderFragment.Arguments = bundle;

            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, editReminderFragment);
            //trans.AddToBackStack(null);
            trans.Commit();
        }

        private void DateSelect(object sender, CalendarView.DateChangeEventArgs e)
        {
            // Month is a value beetwen 0 and 11
            DateTime selectedDate = new DateTime(e.Year, e.Month + 1, e.DayOfMonth);

            //MainActivity.visitList.Clear();

            //database connection
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Event>();

            DateTime nextDay = selectedDate.AddDays(1);

            events.Clear();
            events = db.Table<Event>().Where(x => x.Date >= selectedDate && x.Date < nextDay).OrderBy(x => x.Date).ToList();

            listAdapter = new ListViewCalendarAdapter(this.Activity, events);
            remindersListView.Adapter = listAdapter;
            //visitAdapter.NotifyDataSetChanged();
        }

        public override void OnResume()
        {
            base.OnResume();

            this.Activity.Title = "Kalendarz";
        }
    }
}