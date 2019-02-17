using System;
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

        List<Reminder> events = new List<Reminder>();

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

            //database connection
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Reminder>();

            DateTime nextDay = DateTime.Today.AddDays(1);

            events.Clear();
            events = db.Table<Reminder>().Where(e => e.Date >= DateTime.Today && e.Date < nextDay).OrderBy(e => e.Date).ToList();

            listAdapter = new ListViewCalendarAdapter(this.Activity, events);
            remindersListView = view.FindViewById<ListView>(Resource.Id.listViewCalendar);
            remindersListView.Adapter = listAdapter;
            remindersListView.FastScrollEnabled = true;

            remindersListView.ItemClick += reminder_ItemClick;

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

            //database connection
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Reminder>();

            DateTime nextDay = selectedDate.AddDays(1);

            events.Clear();
            events = db.Table<Reminder>().Where(x => x.Date >= selectedDate && x.Date < nextDay).OrderBy(x => x.Date).ToList();

            listAdapter = new ListViewCalendarAdapter(this.Activity, events);
            remindersListView.Adapter = listAdapter;
        }

        public override void OnResume()
        {
            base.OnResume();

            this.Activity.Title = "Kalendarz";
        }
    }
}