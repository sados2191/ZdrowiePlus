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
    public class AddVisitFragment : Fragment
    {
        TextView dateDisplay;
        TextView timeDisplay;
        static DateTime currentTime = DateTime.Now;
        int year = currentTime.Year, month = currentTime.Month, day = currentTime.Day, hour = currentTime.Hour, minute = currentTime.Minute;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.AddVisit, container, false);

            //date choosing
            dateDisplay = view.FindViewById<TextView>(Resource.Id.textDate);
            dateDisplay.Text = currentTime.ToLongDateString();
            dateDisplay.Click += DateSelect_OnClick;

            //time choosing
            timeDisplay = view.FindViewById<TextView>(Resource.Id.textTime);
            timeDisplay.Text = currentTime.ToShortTimeString();
            timeDisplay.Click += TimeSelectOnClick;

            //Adding visit
            Button buttonAddVisit = view.FindViewById<Button>(Resource.Id.btnAddVisit);
            buttonAddVisit.Click += AddVisit;

            return view;
        }

        void AddVisit(object sender, EventArgs e)
        {
            DateTime visit = new DateTime(year, month, day, hour, minute, 0);
            string description = this.Activity.FindViewById<EditText>(Resource.Id.visitDescription).Text;
            if (description != string.Empty)
            {
                MainActivity.visitList.Add($"{visit.ToString("dd.MM.yyyy HH:mm")} {description}");
                this.Activity.FindViewById<EditText>(Resource.Id.visitDescription).Text = String.Empty;
                Toast.MakeText(this.Activity, $"Dodano\n{visit.ToString("dd.MM.yyyy HH:mm")}\n{description}", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this.Activity, "Opis nie może być pusty", ToastLength.Short).Show();
            }
        }

        void TimeSelectOnClick(object sender, EventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                timeDisplay.Text = time.ToShortTimeString();
                hour = time.Hour;
                minute = time.Minute;
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        void DateSelect_OnClick(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                dateDisplay.Text = time.ToLongDateString();
                year = time.Year;
                month = time.Month;
                day = time.Day;
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }
    }
}