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
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

namespace ZdrowiePlus.Fragments
{
    public class ListHistoryFragment : Android.App.Fragment
    {
        private static EditReminderFragment editReminderFragment = new EditReminderFragment();

        ListReminderAdapter reminderAdapter;
        RecyclerView reminderRecyclerView;
        Spinner spinner;

        List<Reminder> events;

        int listFilter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ListReminders, container, false);

            reminderRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewReminders);
            reminderRecyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity));
            reminderRecyclerView.HasFixedSize = true;

            //event type spinner
            spinner = view.FindViewById<Spinner>(Resource.Id.reminderSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.reminders_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            loadData();

            return view;
        }

        private void reminderCard_ItemClick(object sender, int position)
        {
            int id = events[position].Id; //przesłać id w bundlu

            Bundle bundle = new Bundle();
            bundle.PutInt("id", id);
            editReminderFragment.Arguments = bundle;

            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, editReminderFragment);
            //trans.AddToBackStack(null);
            trans.Commit();
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            listFilter = e.Position;
            loadData();
        }

        public void loadData()
        {
            //database connection
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Reminder>();

            events = new List<Reminder>();
            switch (listFilter)
            {
                case 0:
                    events = db.Table<Reminder>().Where(e => e.Date < DateTime.Now).OrderByDescending(e => e.Date).ToList();
                    break;
                case 1:
                    events = db.Table<Reminder>().Where(e => e.ReminderType == ReminderType.Visit && e.Date < DateTime.Now).OrderByDescending(e => e.Date).ToList();
                    break;
                case 2:
                    events = db.Table<Reminder>().Where(e => e.ReminderType == ReminderType.Medicine && e.Date < DateTime.Now).OrderByDescending(e => e.Date).ToList();
                    break;
                case 3:
                    events = db.Table<Reminder>().Where(e => e.ReminderType == ReminderType.Measurement && e.Date < DateTime.Now).OrderByDescending(e => e.Date).ToList();
                    break;
                default:
                    events = db.Table<Reminder>().Where(e => e.Date < DateTime.Now).OrderByDescending(e => e.Date).ToList();
                    break;
            }

            reminderAdapter = new ListReminderAdapter(events);
            reminderAdapter.ItemClick += reminderCard_ItemClick;
            reminderRecyclerView.SetAdapter(reminderAdapter);
        }

        public override void OnResume()
        {
            base.OnResume();

            this.Activity.Title = "Historia";
        }
    }
}