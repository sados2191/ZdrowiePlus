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
    public class ListRemindersFragment : Android.App.Fragment
    {
        public static ListViewReminderAdapter visitAdapter;
        private static EditVisitFragment editVisitFragment = new EditVisitFragment();
        private static AddVisitFragment addVisitFragment = new AddVisitFragment();
        private static AddMedicineTherapyFragment medicineTherapyFragment = new AddMedicineTherapyFragment();

        ListView visitListView;
        Button buttonAddMedicine;
        Button buttonAddVisit;
        Spinner spinner;

        List<Event> events;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ListReminders, container, false);

            visitListView = view.FindViewById<ListView>(Resource.Id.listViewVisits);
            buttonAddMedicine = view.FindViewById<Button>(Resource.Id.btnAddMedicine_list);
            buttonAddVisit = view.FindViewById<Button>(Resource.Id.btnAddVisit_list);

            //event type spinner
            spinner = view.FindViewById<Spinner>(Resource.Id.visitSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.visits_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            //MainActivity.visitList.Clear();
            loadData();

            visitListView.ItemClick += visitListView_ItemClick;
            buttonAddMedicine.Click += AddMedicine;
            buttonAddVisit.Click += AddVisit;

            return view;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            MainActivity.listFilter = e.Position;
            loadData();
        }

        private void AddMedicine(object sender, EventArgs e)
        {
            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, medicineTherapyFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        private void AddVisit(object sender, EventArgs e)
        {
            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, addVisitFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        private void visitListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            MainActivity.eventToEdit = events[e.Position];

            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, editVisitFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        public void loadData() //zmienić na przekazywanie enuma
        {
            if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {
                // We have permission

                //database connection
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                db.CreateTable<Event>();

                events = new List<Event>();
                switch (MainActivity.listFilter)
                {
                    case 0:
                        events = db.Table<Event>().Where(e => e.Date >= DateTime.Today).OrderBy(e => e.Date).ToList();
                        break;
                    case 1:
                        events = db.Table<Event>().Where(e => e.EventType == EventType.Visit && e.Date >= DateTime.Today).OrderBy(e => e.Date).ToList();
                        break;
                    case 2:
                        events = db.Table<Event>().Where(e => e.EventType == EventType.Medicine && e.Date >= DateTime.Today).OrderBy(e => e.Date).ToList();
                        break;
                    case 3:
                        events = db.Table<Event>().Where(e => e.EventType == EventType.Measurement && e.Date >= DateTime.Today).OrderBy(e => e.Date).ToList();
                        break;
                    default:
                        events = db.Table<Event>().Where(e => e.Date >= DateTime.Today).OrderBy(e => e.Date).ToList();
                        break;
                }

                //foreach (var visit in events)
                //{
                //    MainActivity.visitList.Add(visit);
                //}

                visitAdapter = new ListViewReminderAdapter(this.Activity, /* MainActivity.visitList */ events);
                visitListView.Adapter = visitAdapter;
                visitListView.FastScrollEnabled = true;

                // Event list is empty
                if (events.Count == 0)
                {
                    buttonAddVisit.Visibility = ViewStates.Visible;
                    buttonAddMedicine.Visibility = ViewStates.Visible;
                }

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
        }

        public override void OnResume()
        {
            base.OnResume();
            
            //visitAdapter.NotifyDataSetChanged();
        }
    }
}