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
    public class MedicineTimeListFragment : Android.App.Fragment
    {
        public static MedicineTimeViewAdapter medicineTimeAdapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.VisitList, container, false);

            medicineTimeAdapter = new MedicineTimeViewAdapter(this.Activity, MedicineTherapyFragment.pillTimes);
            ListView medicineTimeListView = view.FindViewById<ListView>(Resource.Id.listViewVisits);
            medicineTimeListView.Adapter = medicineTimeAdapter;
            medicineTimeListView.FastScrollEnabled = true;

            medicineTimeListView.ItemClick += medicineTimeListView_ItemClick;

            return view;
        }

        private void medicineTimeListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                MedicineTherapyFragment.pillTimes[e.Position] = MedicineTherapyFragment.pillTimes[e.Position].AddHours(time.Hour - MedicineTherapyFragment.pillTimes[e.Position].Hour);
                MedicineTherapyFragment.pillTimes[e.Position] = MedicineTherapyFragment.pillTimes[e.Position].AddMinutes(time.Minute - MedicineTherapyFragment.pillTimes[e.Position].Minute);
                medicineTimeAdapter.NotifyDataSetChanged();
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        public override void OnResume()
        {
            base.OnResume();

            //medicineTimeAdapter.NotifyDataSetChanged();
        }
    }
}