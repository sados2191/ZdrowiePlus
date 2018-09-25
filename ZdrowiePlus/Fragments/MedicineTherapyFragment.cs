using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

namespace ZdrowiePlus.Fragments
{
    public class MedicineTherapyFragment : Android.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.MedicineTherapy, container, false);

            EditText medicineName = view.FindViewById<EditText>(Resource.Id.medicineName);

            SeekBar seekbarFrequency = view.FindViewById<SeekBar>(Resource.Id.medicineFrequency);
            TextView labelFrequency = view.FindViewById<TextView>(Resource.Id.labelFrequency);
            labelFrequency.Text = $"Ile razy dziennie:  {seekbarFrequency.Progress}";
            seekbarFrequency.ProgressChanged += (s, e) => {
                if (e.Progress < 1) seekbarFrequency.Progress = 1;
                labelFrequency.Text = $"Ile razy dziennie:  {seekbarFrequency.Progress}";
            };

            SeekBar seekbarTimeout = view.FindViewById<SeekBar>(Resource.Id.medicineTimeout);
            TextView labelTimeout = view.FindViewById<TextView>(Resource.Id.labelTimeout);
            labelTimeout.Text = $"Długość kuracji w dniach:  {seekbarTimeout.Progress}";
            seekbarTimeout.ProgressChanged += (s, e) => {
                if (e.Progress < 1) seekbarTimeout.Progress = 1;
                labelTimeout.Text = $"Długość kuracji w dniach:  {seekbarTimeout.Progress}";
            };

            TextView medicineDate = view.FindViewById<TextView>(Resource.Id.medicineDate);

            return view;
        }
    }
}