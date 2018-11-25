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
        public static string pillName;
        public static List<DateTime> pillTimes = new List<DateTime>();
        public static int therapyLength;
        //public static DateTime startDay;

        TextView medicineDate;
        SeekBar seekbarFrequency;
        SeekBar seekbarTimeout;
        EditText medicineName;

        static DateTime currentTime;
        int year, month, day;

        private static MedicineTimeListFragment timeListFragment = new MedicineTimeListFragment();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            currentTime = DateTime.Now;
            year = currentTime.Year;
            month = currentTime.Month;
            day = currentTime.Day;

            View view = inflater.Inflate(Resource.Layout.MedicineTherapy, container, false);

            medicineName = view.FindViewById<EditText>(Resource.Id.medicineName);

            //setting pill frequency per day
            seekbarFrequency = view.FindViewById<SeekBar>(Resource.Id.medicineFrequency);
            TextView labelFrequency = view.FindViewById<TextView>(Resource.Id.labelFrequency);
            labelFrequency.Text = $"Ile razy dziennie:  {seekbarFrequency.Progress}";
            seekbarFrequency.ProgressChanged += (s, e) => {
                if (e.Progress < 1) seekbarFrequency.Progress = 1;
                labelFrequency.Text = $"Ile razy dziennie:  {seekbarFrequency.Progress}";
            };

            //setting length of therapy
            seekbarTimeout = view.FindViewById<SeekBar>(Resource.Id.medicineTimeout);
            TextView labelTimeout = view.FindViewById<TextView>(Resource.Id.labelTimeout);
            labelTimeout.Text = $"Długość kuracji w dniach:  {seekbarTimeout.Progress}";
            seekbarTimeout.ProgressChanged += (s, e) => {
                if (e.Progress < 1) seekbarTimeout.Progress = 1;
                labelTimeout.Text = $"Długość kuracji w dniach:  {seekbarTimeout.Progress}";
            };

            //date choosing
            medicineDate = view.FindViewById<TextView>(Resource.Id.medicineDate);
            medicineDate.Text = currentTime.ToLongDateString();
            medicineDate.Click += DateSelect_OnClick;

            //next screen button
            Button buttonNext = view.FindViewById<Button>(Resource.Id.btnNextMedicineTherapy);
            buttonNext.Click += NextChooseTime;

            return view;
        }

        private void NextChooseTime(object sender, EventArgs e)
        {
            pillName = medicineName.Text;
            therapyLength = seekbarTimeout.Progress;
            //startDay = new DateTime(year, month, day, 0, 0, 0);

            pillTimes.Clear();

            int pillDelay = 0;
            if (seekbarFrequency.Progress == 2)
                pillDelay = 720;
            if (seekbarFrequency.Progress > 2)
               pillDelay = 840 / (seekbarFrequency.Progress - 1);
            
            for (int i = 0; i < seekbarFrequency.Progress; i++)
            {
                pillTimes.Add(new DateTime(year, month, day, 8, 0, 0).AddMinutes(pillDelay * i));
            }

            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, timeListFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        void DateSelect_OnClick(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                medicineDate.Text = time.ToLongDateString();
                year = time.Year;
                month = time.Month;
                day = time.Day;
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        public override void OnResume()
        {
            base.OnResume();

            medicineName.Text = String.Empty;
            medicineDate.Text = currentTime.ToLongDateString();
        }
    }
}