using System;
using System.Collections.Generic;
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

namespace ZdrowiePlus.Fragments
{
    public class AddReminderFragment : Android.App.Fragment
    {
        public AddVisitFragment addVisitFragment;
        public AddMedicineTherapyFragment addMedicineTherapyFragment;
        public AddMeasurementReminderFragment addMeasurementReminderFragment;

        ImageView reminderIcon;
        Spinner spinner;

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

            reminderIcon = view.FindViewById<ImageView>(Resource.Id.reminder_icon);

            //reminder type spinner
            spinner = view.FindViewById<Spinner>(Resource.Id.remindersListSpinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.add_reminder_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            //get reminder type fragment based on spinner position
            GetReminderType(spinner.SelectedItemPosition);

            return view;
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            GetReminderType(e.Position);
        }

        private void GetReminderType(int switch_case)
        {
            var trans = FragmentManager.BeginTransaction();

            switch (switch_case)
            {
                case 0:
                    reminderIcon.SetImageResource(Resource.Drawable.doctor_icon);
                    trans.Replace(Resource.Id.addReminderContainer, addVisitFragment);
                    trans.Commit();
                    break;
                case 1:
                    reminderIcon.SetImageResource(Resource.Drawable.medical_pill);
                    trans.Replace(Resource.Id.addReminderContainer, addMedicineTherapyFragment);
                    trans.Commit();
                    break;
                case 2:
                    reminderIcon.SetImageResource(Resource.Drawable.pulsometer_icon);
                    trans.Replace(Resource.Id.addReminderContainer, addMeasurementReminderFragment);
                    trans.Commit();
                    break;
                default:
                    reminderIcon.SetImageResource(Resource.Drawable.doctor_icon);
                    trans.Replace(Resource.Id.addReminderContainer, addVisitFragment);
                    trans.Commit();
                    break;
            }
        }

        private void AddMeasurementReminder(object sender, EventArgs e)
        {
            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, addMeasurementReminderFragment);
            //trans.AddToBackStack(null);
            trans.Commit();
        }

        private void AddMedicine(object sender, EventArgs e)
        {
            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, addMedicineTherapyFragment);
            //trans.AddToBackStack(null);
            trans.Commit();
        }

        private void AddVisit(object sender, EventArgs e)
        {
            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, addVisitFragment);
            //trans.AddToBackStack(null);
            trans.Commit();
        }
    }
}