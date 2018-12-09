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
    public class ListMeasurementsFragment : Android.App.Fragment
    {
        public static ListViewMeasurementAdapter measurementAdapter;

        ListView measurementListView;
        Spinner spinner;

        List<Measurement> measurements;

        MeasurementType measurementType;
        int selected;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.ListMeasurements, container, false);

            measurementListView = view.FindViewById<ListView>(Resource.Id.listViewMeasurements);

            //measurement type spinner
            spinner = view.FindViewById<Spinner>(Resource.Id.measurementsListSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            selected = spinner.SelectedItemPosition;
            measurementType = (MeasurementType)selected;
            loadData(measurementType);

            //measurementListView.ItemClick += measurementListView_ItemClick;

            return view;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            selected = e.Position;
            measurementType = (MeasurementType)selected;
            loadData(measurementType);
        }

        public void loadData(MeasurementType measurementType)
        {
            if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {
                // We have permission

                //database connection
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                db.CreateTable<Measurement>();

                measurements = new List<Measurement>();

                measurements = db.Table<Measurement>().Where(e => e.MeasurementType == measurementType && e.Date >= DateTime.Today).OrderByDescending(e => e.Date).ToList();

                measurementAdapter = new ListViewMeasurementAdapter(this.Activity, measurements);
                measurementListView.Adapter = measurementAdapter;
                measurementListView.FastScrollEnabled = true;

                // Measurements list is empty
                if (measurements.Count == 0)
                {
                    
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
    }
}