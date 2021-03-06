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
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

namespace ZdrowiePlus.Fragments
{
    public class ListMeasurementsFragment : Android.App.Fragment
    {
        AddMeasurementFragment addMeasurementFragment = new AddMeasurementFragment();

        ListMeasurementAdapter measurementAdapter;
        RecyclerView measurementRecyclerView;
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

            measurementRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.recyclerViewMeasurements);
            measurementRecyclerView.SetLayoutManager(new LinearLayoutManager(this.Activity));
            measurementRecyclerView.HasFixedSize = true;

            //measurement type spinner
            spinner = view.FindViewById<Spinner>(Resource.Id.measurementsListSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            selected = spinner.SelectedItemPosition;
            measurementType = (MeasurementType)selected;
            loadData(measurementType);

            var fabAdd = view.FindViewById<FloatingActionButton>(Resource.Id.fab_add);
            fabAdd.Click += (s, e) =>
            {
                var trans = FragmentManager.BeginTransaction();
                Bundle bundle = new Bundle();
                bundle.PutInt("type", selected);
                addMeasurementFragment.Arguments = bundle;
                trans.Replace(Resource.Id.fragmentContainer, addMeasurementFragment);
                //trans.AddToBackStack(null);
                trans.Commit();
            };

            return view;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selected = e.Position;
            measurementType = (MeasurementType)selected;
            loadData(measurementType);
        }

        public void loadData(MeasurementType measurementType)
        {
            //database connection
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Measurement>();

            measurements = new List<Measurement>();

            measurements = db.Table<Measurement>().Where(e => e.MeasurementType == measurementType).OrderByDescending(e => e.Date).ToList();

            measurementAdapter = new ListMeasurementAdapter(measurements);
            measurementRecyclerView.SetAdapter(measurementAdapter);
        }

        public override void OnViewStateRestored(Bundle savedInstanceState)
        {
            base.OnViewStateRestored(savedInstanceState);

            //if opened by AddMeasurementFragment
            if (Arguments != null)
            {
                if (!Arguments.IsEmpty)
                {
                    int spinnerPosition = Arguments.GetInt("type", 0);
                    spinner.SetSelection(spinnerPosition, true);
                    //Arguments = null;
                    Arguments.Clear();
                }
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            this.Activity.Title = "Lista pomiarów";
        }
    }
}