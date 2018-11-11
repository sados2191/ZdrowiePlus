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
    public class VisitListFragment : Android.App.Fragment
    {
        public static MyListViewAdapter visitAdapter;
        private static EditVisitFragment editVisitFragment = new EditVisitFragment();
        private static AddVisitFragment addVisitFragment = new AddVisitFragment();
        private static MedicineTherapyFragment medicineTherapyFragment = new MedicineTherapyFragment();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.VisitList, container, false);

            MainActivity.visitList.Clear();

            if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {
                // We have permission

                //database connection
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "events.db"));
                db.CreateTable<Event>();

                List<Event> events = new List<Event>();
                switch (MainActivity.listFilter)
                {
                    case 0:
                        events = db.Table<Event>().OrderBy(e => e.Date).ToList();
                        break;
                    case 1:
                        events = db.Table<Event>().Where(e => e.EventType == EventType.Visit).OrderBy(e => e.Date).ToList();
                        break;
                    case 2:
                        events = db.Table<Event>().Where(e => e.EventType == EventType.Medicine).OrderBy(e => e.Date).ToList();
                        break;
                    default:
                        events = db.Table<Event>().OrderBy(e => e.Date).ToList();
                        break;
                }

                foreach (var visit in events)
                {
                    MainActivity.visitList.Add(visit);
                }

                visitAdapter = new MyListViewAdapter(this.Activity, MainActivity.visitList);
                ListView visitListView = view.FindViewById<ListView>(Resource.Id.listViewVisits);
                visitListView.Adapter = visitAdapter;
                visitListView.FastScrollEnabled = true;

                visitListView.ItemClick += visitListView_ItemClick;

                // Event list is empty
                if (events.Count == 0)
                {
                    Button buttonAddVisit = view.FindViewById<Button>(Resource.Id.btnAddVisit_list);
                    buttonAddVisit.Click += AddVisit;
                    buttonAddVisit.Visibility = ViewStates.Visible;

                    Button buttonAddMedicine = view.FindViewById<Button>(Resource.Id.btnAddMedicine_list);
                    buttonAddMedicine.Click += AddMedicine;
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

            return view;
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
            MainActivity.eventToEdit = MainActivity.visitList[e.Position];

            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, editVisitFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        public override void OnResume()
        {
            base.OnResume();
            
            //visitAdapter.NotifyDataSetChanged();
        }
    }
}