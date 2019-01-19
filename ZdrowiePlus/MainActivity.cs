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
using Android.Support.Design.Widget;
using ZdrowiePlus.Fragments;
using Android.Content.PM;
using Android.Runtime;
using SQLite;
using System.IO;
using System.Linq;

namespace ZdrowiePlus
{
    [Activity(Label = "@string/app_name", MainLauncher = true, Theme ="@style/MyTheme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        //ogarnąć cofanie back button
        //spinner type dialog?
        //usuwanie serii - sprawdzać dateAdded?
        //usuwanie serii w aktywnych czy w historii
        //potwierdzenie usuwania
        //jak jest w OnResume nie musi byc w OnCreate
        //edycja pomiarów
        //inicjalizacja fragmentów w OnCreateView
        //wczytywanie z bazy w nowym Thred i progress bar (kręcące się kółko? Resource.Id.progressBar1 ? )
        //maksymalnie 500 alarmów - error, dodać alarmy jako powtarzające? (set repeating)
        //sprawdzić czy wymagane uprawnienia zapisu/odczytu sd, wywala błąd - wcześniej nie było
        //cukier na czczo czy po posiłku
        //tętno w spoczynku czy w czasie aktywności
        //ogarnąć nazwy class itd
        //ogarnąć dostępy (private, public)
        //powiadomienie otweiera dialog?
        //czy wszystkie fragmenty powinny miec AddToBackstack? - usunieto z menu po lewej, skomentowano wszystkie

        //list filter
        public static int listFilter = 0;//zmienic tak jak w liscie pomiarów

        bool doubleBackExit = false;

        ////left menu
        private ActionBarDrawerToggle drawerToggle;
        private DrawerLayout drawerLayout;
        NavigationView navigationView;

        //fragments
        private AddReminderFragment addReminderFragment;
        private static ListRemindersFragment reminderListFragment;
        private static ListHistoryFragment historyListFragment;
        private static CalendarFragment calendarFragment;
        private static AddMeasurementFragment addMeasurementFragment;
        private static ListMeasurementsFragment measurementsListFragment;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.MyTheme);

            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            //Toolbar takes on default actionbar characteristics
            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //left menu
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerToggle = new ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.openDrawer, Resource.String.closeDrawer);
            drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            //fragments
            addReminderFragment = new AddReminderFragment();
            reminderListFragment = new ListRemindersFragment();
            historyListFragment = new ListHistoryFragment();
            calendarFragment = new CalendarFragment();
            addMeasurementFragment = new AddMeasurementFragment();
            measurementsListFragment = new ListMeasurementsFragment();

            var trans = FragmentManager.BeginTransaction();

            //check if notification opened the app
            string notification = Intent.GetStringExtra("notification");
            if (notification == "measurement")
            {
                Bundle bundle = new Bundle();
                bundle.PutInt("id", Intent.GetIntExtra("id", 0));
                bundle.PutInt("type", Intent.GetIntExtra("type", 0));
                addMeasurementFragment.Arguments = bundle;
                ReplaceFragment(addMeasurementFragment);
            }
            else if (notification == "visit" || notification == "medicine")
            {
                EventFromNotificationFragment eventFragment = new EventFromNotificationFragment();
                Bundle bundle = new Bundle();
                bundle.PutInt("id", Intent.GetIntExtra("id", 0));
                eventFragment.Arguments = bundle;
                ReplaceFragment(eventFragment);
            }
            else
            {
                trans.Add(Resource.Id.fragmentContainer, reminderListFragment);
                trans.Commit();
            }

        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            string notification = intent.GetStringExtra("notification");
            if (notification == "measurement")
            {
                Bundle bundle = new Bundle();
                bundle.PutInt("id", intent.GetIntExtra("id", 0));
                bundle.PutInt("type", intent.GetIntExtra("type", 0));
                addMeasurementFragment.Arguments = bundle;
                ReplaceFragment(addMeasurementFragment);
            }
            else if (notification == "visit" || notification == "medicine")
            {
                EventFromNotificationFragment eventFragment = new EventFromNotificationFragment();
                Bundle bundle = new Bundle();
                bundle.PutInt("id", intent.GetIntExtra("id", 0));
                eventFragment.Arguments = bundle;
                ReplaceFragment(eventFragment);
            }
        }

        //replace fragment method
        private void ReplaceFragment(Fragment fragment)
        {
            var trans = FragmentManager.BeginTransaction();

            if (fragment.IsVisible)
            {
                //check if android API is >= 26 (Oreo 8.0)
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    trans.SetReorderingAllowed(false);
                }
                trans.Detach(fragment);
                trans.Attach(fragment);
                trans.Commit();

                return;
            }

            trans.Replace(Resource.Id.fragmentContainer, fragment);
            //trans.AddToBackStack(null); sprawdzic czy działa, nie chcemy wracac po fragmentach
            trans.Commit();
        }

        public override void OnBackPressed()
        {
            if (drawerLayout.IsDrawerOpen((int)GravityFlags.Start))
            {
                drawerLayout.CloseDrawer((int)GravityFlags.Start);
            }
            else
            {
                if (doubleBackExit)
                {
                    base.OnBackPressed();
                    return;
                }

                doubleBackExit = true;
                Toast.MakeText(this, "Nasiśnij ponownie aby wyjść", ToastLength.Short).Show();

                new Handler().PostDelayed(() => {
                    doubleBackExit = false;
                }, 2000);

            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.nav_addReminder:
                    this.Title = item.TitleFormatted.ToString();
                    ReplaceFragment(addReminderFragment);
                    break;
                case Resource.Id.nav_reminders:
                    this.Title = item.TitleFormatted.ToString();
                    ReplaceFragment(reminderListFragment);
                    break;
                case Resource.Id.nav_addMeasurement:
                    this.Title = item.TitleFormatted.ToString();
                    ReplaceFragment(addMeasurementFragment);
                    break;
                case Resource.Id.nav_measurements:
                    this.Title = item.TitleFormatted.ToString();
                    ReplaceFragment(measurementsListFragment);
                    break;
                case Resource.Id.nav_history:
                    this.Title = item.TitleFormatted.ToString();
                    ReplaceFragment(historyListFragment);
                    break;
                case Resource.Id.nav_calendar:
                    this.Title = item.TitleFormatted.ToString();
                    ReplaceFragment(calendarFragment);
                    break;
                case Resource.Id.nav_close:
                    this.Finish();
                    break;
                case Resource.Id.nav_about:
                    Toast.MakeText(this, Resource.String.app_name, ToastLength.Short).Show();
                    break;
                default:
                    this.Title = item.TitleFormatted.ToString();
                    ReplaceFragment(reminderListFragment);
                    break;
            }

            drawerLayout.CloseDrawer((int)GravityFlags.Start);

            return true;
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

            switch (item.ItemId)
            {
                case Resource.Id.menu_delete_events:
                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                    db.CreateTable<Event>();
                    var events = db.Table<Event>().ToList();
                    foreach (var x in events)
                    {
                        //canceling alarm manager
                        Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                        PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, x.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                        AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                        alarmManager.Cancel(pendingIntent);
                    }
                    db.DeleteAll<Event>();
                    db.DeleteAll<Measurement>();
                    ReplaceFragment(reminderListFragment);
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case 1: //write_external_storage
                    if (grantResults[0] == Permission.Granted)
                    {
                        Toast.MakeText(this, "Permission granted", ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, "Permission denied", ToastLength.Short).Show();
                    }
                    break;
                case 2: //read_external_storage
                    if (grantResults[0] == Permission.Granted)
                    {
                        Toast.MakeText(this, "Permission granted", ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, "Permission denied", ToastLength.Short).Show();
                    }
                    break;
                default:
                    break;
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}