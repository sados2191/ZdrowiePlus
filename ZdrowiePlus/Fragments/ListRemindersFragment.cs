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
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

namespace ZdrowiePlus.Fragments
{
    public class ListRemindersFragment : Android.App.Fragment
    {
        private static EditReminderFragment editReminderFragment = new EditReminderFragment();
        AddReminderFragment addReminderFragment = new AddReminderFragment();

        ListReminderAdapter reminderAdapter;
        RecyclerView reminderRecyclerView;
        Spinner spinner;

        List<Reminder> events;
        
        ReminderType reminderType;

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

            //floating add button
            var fabAdd = view.FindViewById<FloatingActionButton>(Resource.Id.fab_add);
            fabAdd.Click += (s, e) =>
            {
                var trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.fragmentContainer, addReminderFragment);
                //trans.AddToBackStack(null);
                trans.Commit();
            };

            return view;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            int position = e.Position - 1; //pozycja 0 spinnera to wybór wszystkich przypomnień, enum o wartosci 0 to wizyty

            if (position < 0)
            {
                loadData();
            }
            else
            {
                reminderType = (ReminderType)position;
                loadData(reminderType);
            }
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

        public void loadData(ReminderType? reminderType = null)
        {
            //database connection
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Reminder>();

            events = new List<Reminder>();

            if (reminderType != null)
            {
                events = db.Table<Reminder>().Where(e => e.ReminderType == reminderType && e.Date >= DateTime.Today).OrderBy(e => e.Date).ToList();
            }
            else
            {
                events = db.Table<Reminder>().Where(e => e.Date >= DateTime.Today).OrderBy(e => e.Date).ToList();
            }

            reminderAdapter = new ListReminderAdapter(events);
            reminderAdapter.ItemClick += reminderCard_ItemClick;
            reminderRecyclerView.SetAdapter(reminderAdapter);
        }

        public override void OnResume()
        {
            base.OnResume();

            this.Activity.Title = "Przypomnienia";
        }
    }
}