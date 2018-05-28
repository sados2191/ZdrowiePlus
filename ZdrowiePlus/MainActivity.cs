using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using System.Collections.Generic;
using Android.Content;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;

namespace ZdrowiePlus
{
    [Activity(Label = "ZdrowiePlus", MainLauncher = true, Theme ="@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {
        TextView dateDisplay;
        Button buttonDateSelect;
        TextView timeDisplay;
        Button buttonTimeSelect;
        static DateTime current = DateTime.Now;
        int year = current.Year, month = current.Month, day = current.Day, hour = current.Hour, minute = current.Minute;
        static readonly List<string> visitDates = new List<string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            //Toolbar takes on default actionbar characteristics
            SetSupportActionBar(toolbar);
            //ActionBar.Title = "Nowy";

            //date choosing
            buttonDateSelect = FindViewById<Button>(Resource.Id.btnDate);
            dateDisplay = FindViewById<TextView>(Resource.Id.textDate);
            dateDisplay.Text = current.ToLongDateString();
            dateDisplay.Click += DateSelect_OnClick;
            buttonDateSelect.Click += DateSelect_OnClick;

            //time choosing
            buttonTimeSelect = FindViewById<Button>(Resource.Id.btnTime);
            timeDisplay = FindViewById<TextView>(Resource.Id.textTime);
            timeDisplay.Text = current.ToShortTimeString();
            buttonTimeSelect.Click += TimeSelectOnClick;

            //Adding term
            Button buttonAddVisit = FindViewById<Button>(Resource.Id.btnAddVisit);
            buttonAddVisit.Click += AddVisit;
        }

        private void AddVisit(object sender, EventArgs e)
        {
            DateTime visit = new DateTime(year, month, day, hour, minute, 0);
            string description = FindViewById<EditText>(Resource.Id.visitDescription).Text;
            if (description != string.Empty)
            {
                visitDates.Add($"{visit.ToString("dd.MM.yyyy HH:mm")} {description}");
                //Toast.MakeText(this, $"{day}-{month}-{year} {hour}:{minute}", ToastLength.Long).Show();
                Toast.MakeText(this, $"Dodano\n{visit.ToString("dd.MM.yyyy HH:mm")}\n{description}", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Opis nie może być pusty", ToastLength.Short).Show();
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

        //toolbar menu initialization
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        //toolbar menu select
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            string textToShow = string.Empty;

            switch (item.ItemId)
            {
                case Resource.Id.menu_info:
                    textToShow = "Info";
                    break;
                case Resource.Id.menu_visits:
                    textToShow = "Zapisane wizyty";
                    var intent = new Intent(this, typeof(VisitDatesActivity));
                    intent.PutStringArrayListExtra("terms", visitDates);
                    StartActivity(intent);
                    break;
            }

            Toast.MakeText(this, item.TitleFormatted + ": " + textToShow, ToastLength.Short).Show();

            return base.OnOptionsItemSelected(item);
        }
    }
}