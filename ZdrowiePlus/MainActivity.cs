using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using System.Collections.Generic;
using Android.Content;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;

namespace ZdrowiePlus
{
    [Activity(Label = "ZdrowiePlus", MainLauncher = true, Theme ="@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {
        TextView dateDisplay;
        TextView timeDisplay;
        static DateTime current = DateTime.Now;
        int year = current.Year, month = current.Month, day = current.Day, hour = current.Hour, minute = current.Minute;
        static readonly List<string> visitDates = new List<string>();

        //left menu
        private MyActionBarDrawerToggle drawerToggle;
        private DrawerLayout drawerLayout;
        private ListView leftDrawer;
        private ArrayAdapter leftAdapter;
        private List<string> leftDataSet;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Toolbar takes on default actionbar characteristics
            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //left menu
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            leftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
            drawerToggle = new MyActionBarDrawerToggle(this, drawerLayout, Resource.String.openDrawer, Resource.String.closeDrawer);
            drawerLayout.AddDrawerListener(drawerToggle);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            drawerToggle.SyncState();
            leftDataSet = new List<string>();
            leftDataSet.Add("Terminy wizyt");
            leftDataSet.Add("Item 2");
            leftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, leftDataSet);
            leftDrawer.Adapter = leftAdapter;
            leftDrawer.ItemClick += leftDrawer_ItemClick;

            //date choosing
            dateDisplay = FindViewById<TextView>(Resource.Id.textDate);
            dateDisplay.Text = current.ToLongDateString();
            dateDisplay.Click += DateSelect_OnClick;

            //time choosing
            timeDisplay = FindViewById<TextView>(Resource.Id.textTime);
            timeDisplay.Text = current.ToShortTimeString();
            timeDisplay.Click += TimeSelectOnClick;

            //Adding visit
            Button buttonAddVisit = FindViewById<Button>(Resource.Id.btnAddVisit);
            buttonAddVisit.Click += AddVisit;
        }

        //left menu item click
        private void leftDrawer_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    var intent = new Intent(this, typeof(VisitDatesActivity));
                    intent.PutStringArrayListExtra("visits", visitDates);
                    StartActivity(intent);
                    break;
                case 1:
                    Toast.MakeText(this, leftDataSet[e.Position], ToastLength.Short).Show();
                    break;
                default:
                    break;
            }
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
            drawerToggle.OnOptionsItemSelected(item);

            string textToShow = string.Empty;

            switch (item.ItemId)
            {
                case Resource.Id.menu_info:
                    textToShow = "Info";
                    break;
                case Resource.Id.menu_visits:
                    textToShow = "Zapisane wizyty";
                    var intent = new Intent(this, typeof(VisitDatesActivity));
                    intent.PutStringArrayListExtra("visits", visitDates);
                    StartActivity(intent);
                    break;
            }

            //Toast.MakeText(this, item.TitleFormatted + ": " + textToShow, ToastLength.Short).Show();

            return base.OnOptionsItemSelected(item);
        }
    }
}