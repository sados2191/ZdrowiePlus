using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace ZdrowiePlus.Fragments
{
    public class AddReminderFragment : Android.App.Fragment
    {
        public AddVisitFragment addVisitFragment;
        public AddMedicineTherapyFragment addMedicineTherapyFragment;
        public AddMeasurementReminderFragment addMeasurementReminderFragment;

        Button btnAddVisit;
        Button btnAddMedicineTherapy;
        Button btnAddMeasurementReminder;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.AddReminder, container, false);

            addVisitFragment = new AddVisitFragment();
            addMedicineTherapyFragment = new AddMedicineTherapyFragment();
            addMeasurementReminderFragment = new AddMeasurementReminderFragment();

            btnAddVisit = view.FindViewById<Button>(Resource.Id.AddVisit_reminder);
            btnAddMedicineTherapy = view.FindViewById<Button>(Resource.Id.AddMedicineTherapy_reminder);
            btnAddMeasurementReminder = view.FindViewById<Button>(Resource.Id.AddMeasurement_reminder);

            btnAddVisit.Click += AddVisit;
            btnAddMedicineTherapy.Click += AddMedicine;
            btnAddMeasurementReminder.Click += AddMeasurementReminder;

            return view;
        }

        private void AddMeasurementReminder(object sender, EventArgs e)
        {
            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, addMeasurementReminderFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        private void AddMedicine(object sender, EventArgs e)
        {
            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, addMedicineTherapyFragment);
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
    }
}