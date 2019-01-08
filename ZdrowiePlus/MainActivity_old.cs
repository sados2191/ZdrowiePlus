﻿//using Android.App;
//using Android.Widget;
//using Android.OS;
//using Android.Views;
//using System;
//using System.Collections.Generic;
//using Android.Content;
//using SupportToolbar = Android.Support.V7.Widget.Toolbar;
//using Android.Support.V7.App;
//using Android.Support.V4.Widget;
//using ZdrowiePlus.Fragments;
//using Android.Content.PM;
//using Android.Runtime;
//using SQLite;
//using System.IO;
//using System.Linq;

//namespace ZdrowiePlus
//{
//    [Activity(Label = "Zdrowie Plus", MainLauncher = true, Theme ="@style/MyTheme", ScreenOrientation = ScreenOrientation.Portrait)]
//    public class MainActivity : AppCompatActivity
//    {
//        //sprawdzić czy OnNewIntent jest potrzebne
//        //usunąć toolbar menu z prawego rogu
//        //edytowanie pomiaru, zamiast edycji tytułu spiner - zrobione
//        //po kliknięciu na powiadomienie otwiera dodanie pomiaru / widok wizyty / leki - zrobione pomiar
//        //po kliknieciu na powiadomienie tworzy sie nowa instancja aplikacji, poprawić
//        //czy wszystkie fragmenty powinny miec AddToBackstack?
//        //wczytywanie z bazy w nowym Thred i progress bar (kręcące się kółko? Resource.Id.progressBar1 ? )
//        //maksymalnie 500 alarmów - error, dodać alarmy jako powtarzające (set repeating)
//        //zmiana czcionki w zależności od dpi
//        //sprawdzić czy wymagane uprawnienia zapisu/odczytu sd, wywala błąd - wcześniej nie było
//        //okrągły przycisk do dodawania
//        //kolory pomiarów w zależności od norm - czesciowo zrobione
//        //cukier na czczo czy po posiłku
//        //tętno w spoczynku

//        //list filter
//        public static int listFilter = 0;//zmienic tak jak w liscie pomiarów

//        //left menu
//        private MyActionBarDrawerToggle drawerToggle;
//        private DrawerLayout drawerLayout;
//        private ListView leftDrawer;
//        private ArrayAdapter leftAdapter;
//        private List<string> leftDataSet;

//        //fragments
//        //private Fragment currentFragment; show fragment method
//        //private Stack<Fragment> stackFragment;
//        private AddReminderFragment addReminderFragment;
//        //private static AddVisitFragment addVisitFragment;
//        private static ListRemindersFragment reminderListFragment;
//        //private static AddMedicineTherapyFragment medicineTherapyFragment;
//        private static ListHistoryFragment historyListFragment;
//        private static CalendarFragment calendarFragment;
//        private static AddMeasurementFragment addMeasurementFragment;
//        private static ListMeasurementsFragment measurementsListFragment;
//        //private static AddMeasurementReminderFragment measurementReminderFragment;

//        protected override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);

//            // Set our view from the "main" layout resource
//            SetContentView(Resource.Layout.Main);

//            //Toolbar takes on default actionbar characteristics
//            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
//            SetSupportActionBar(toolbar);

//            //left menu
//            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
//            leftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
//            drawerToggle = new MyActionBarDrawerToggle(this, drawerLayout, Resource.String.openDrawer, Resource.String.closeDrawer);
//            drawerLayout.AddDrawerListener(drawerToggle);
//            SupportActionBar.SetHomeButtonEnabled(true);
//            SupportActionBar.SetDisplayShowTitleEnabled(true);
//            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
//            drawerToggle.SyncState();
//            leftDataSet = new List<string>();
//            leftDataSet.Add("Dodaj przypomnienie");
//            leftDataSet.Add("Przypomnienia");
//            leftDataSet.Add("Dodaj pomiar");
//            leftDataSet.Add("Lista pomiarów");
//            leftDataSet.Add("Historia");
//            leftDataSet.Add("Kalendarz");
//            leftDataSet.Add("Zamknij aplikację");
//            leftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, leftDataSet);
//            leftDrawer.Adapter = leftAdapter;
//            leftDrawer.ItemClick += leftDrawer_ItemClick;

//            //fragments
//            //stackFragment = new Stack<Fragment>(); show fragment method
//            addReminderFragment = new AddReminderFragment();
//            //addVisitFragment = new AddVisitFragment();
//            reminderListFragment = new ListRemindersFragment();
//            //medicineTherapyFragment = new AddMedicineTherapyFragment();
//            historyListFragment = new ListHistoryFragment();
//            calendarFragment = new CalendarFragment();
//            addMeasurementFragment = new AddMeasurementFragment();
//            measurementsListFragment = new ListMeasurementsFragment();
//            //measurementReminderFragment = new AddMeasurementReminderFragment();

//            var trans = FragmentManager.BeginTransaction();

//            //check if notification opened the app
//            string notification = Intent.GetStringExtra("notification");
//            if (notification == "measurement")
//            {
//                Bundle bundle = new Bundle();
//                bundle.PutInt("type", Intent.GetIntExtra("type", 0));
//                addMeasurementFragment.Arguments = bundle;
//                //trans.Replace(Resource.Id.fragmentContainer, addMeasurementFragment);
//                ReplaceFragment(addMeasurementFragment);
//            }
//            else if (notification == "visit" || notification == "medicine")
//            {
//                EventFromNotificationFragment eventFragment = new EventFromNotificationFragment();
//                Bundle bundle = new Bundle();
//                bundle.PutInt("id", Intent.GetIntExtra("id", 0));
//                eventFragment.Arguments = bundle;
//                //trans.Replace(Resource.Id.fragmentContainer, eventFragment);
//                ReplaceFragment(eventFragment);
//            }
//            else
//            {
//                trans.Add(Resource.Id.fragmentContainer, reminderListFragment, "RemaindersList");
//            }

//            //trans.Add(Resource.Id.fragmentContainer, addVisitFragment, "AddVisit");
//            //trans.Hide(addVisitFragment);
//            //currentFragment = visitListFragment;
//            //stackFragment.Push(currentFragment);
//            trans.Commit();

//        }

//        protected override void OnNewIntent(Intent intent)
//        {
//            base.OnNewIntent(intent);

//            //var trans = FragmentManager.BeginTransaction();

//            //check if notification opened the app
//            string notification = intent.GetStringExtra("notification");
//            if (notification == "measurement")
//            {
//                Bundle bundle = new Bundle();
//                bundle.PutInt("type", intent.GetIntExtra("type", 0));
//                addMeasurementFragment.Arguments = bundle;
//                //trans.Replace(Resource.Id.fragmentContainer, addMeasurementFragment);
//                ReplaceFragment(addMeasurementFragment);
//            }
//            else if (notification == "visit" || notification == "medicine")
//            {
//                EventFromNotificationFragment eventFragment = new EventFromNotificationFragment();
//                Bundle bundle = new Bundle();
//                bundle.PutInt("id", intent.GetIntExtra("id", 0));
//                eventFragment.Arguments = bundle;
//                //trans.Replace(Resource.Id.fragmentContainer, eventFragment);
//                ReplaceFragment(eventFragment);
//            }

//            //trans.AddToBackStack(null);
//            //trans.Commit();
//        }

//        //show fragment method
//        //private void ShowFragment(Fragment fragment)
//        //{
//        //    if (fragment.IsVisible)
//        //    {
//        //        return;
//        //    }

//        //    var trans = FragmentManager.BeginTransaction();

//        //    //fragment.View.BringToFront();
//        //    //currentFragment.View.BringToFront();

//        //    trans.Hide(currentFragment);
//        //    trans.Show(fragment);
//        //    trans.AddToBackStack(null);
//        //    trans.Commit();
//        //    stackFragment.Push(currentFragment);
//        //    currentFragment = fragment;
//        //}

//        //replace fragment method
//        private void ReplaceFragment(Fragment fragment)
//        {
//            var trans = FragmentManager.BeginTransaction();

//            if (fragment.IsVisible)
//            {
//                //check if android API is >= 26 (Oreo 8.0)
//                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
//                {
//                    trans.SetReorderingAllowed(false);
//                }
//                trans.Detach(fragment);
//                trans.Attach(fragment);
//                trans.Commit();

//                return;
//            }

//            ////test
//            ////check if android API is >= 26 (Oreo 8.0)
//            //if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
//            //{
//            //    trans.SetReorderingAllowed(false);
//            //}
//            //trans.Detach(fragment);
//            //trans.Attach(fragment);
//            ////test

//            trans.Replace(Resource.Id.fragmentContainer, fragment);
//            trans.AddToBackStack(null);
//            trans.Commit();
//        }

//        public override void OnBackPressed()
//        {
//            //if (FragmentManager.BackStackEntryCount > 0) show fragment way
//            //{
//            //    FragmentManager.PopBackStack();
//            //    currentFragment = stackFragment.Pop(); //changing current fragment on back button press
//            //}
//            //else
//            //{
//            //    base.OnBackPressed();
//            //}

//            base.OnBackPressed();
//        }

//        //left menu item click
//        private void leftDrawer_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
//        {
//            switch (e.Position)
//            {
//                case 0:
//                    this.Title = leftDataSet[e.Position];
//                    ReplaceFragment(addReminderFragment);
//                    break;
//                case 1:
//                    this.Title = leftDataSet[e.Position];
//                    //ShowFragment(visitListFragment);
//                    ReplaceFragment(reminderListFragment);
//                    break;
//                case 2:
//                    this.Title = leftDataSet[e.Position];
//                    ReplaceFragment(addMeasurementFragment);
//                    break;
//                case 3:
//                    this.Title = leftDataSet[e.Position];
//                    ReplaceFragment(measurementsListFragment);
//                    break;
//                case 4:
//                    this.Title = leftDataSet[e.Position];
//                    ReplaceFragment(historyListFragment);
//                    break;
//                case 5:
//                    this.Title = leftDataSet[e.Position];
//                    ReplaceFragment(calendarFragment);
//                    break;
//                case 6:
//                    this.Finish();
//                    break;
//                default:
//                    break;
//            }
//            //Toast.MakeText(this, leftDataSet[e.Position], ToastLength.Short).Show();
//            drawerLayout.CloseDrawer((int)GravityFlags.Left);
//        }

//        //toolbar menu initialization
//        public override bool OnCreateOptionsMenu(IMenu menu)
//        {
//            MenuInflater.Inflate(Resource.Menu.toolbar_menu, menu);
//            return base.OnCreateOptionsMenu(menu);
//        }

//        //toolbar menu select
//        public override bool OnOptionsItemSelected(IMenuItem item)
//        {
//            drawerToggle.OnOptionsItemSelected(item);

//            switch (item.ItemId)
//            {
//                case Resource.Id.menu_info:
//                    Toast.MakeText(this, Resource.String.app_name, ToastLength.Short).Show();
//                    break;
//                case Resource.Id.menu_all:
//                    listFilter = 0;
//                    ReplaceFragment(reminderListFragment);
//                    break;
//                case Resource.Id.menu_visit:
//                    listFilter = 1;
//                    ReplaceFragment(reminderListFragment);
//                    break;
//                case Resource.Id.menu_medicine:
//                    listFilter = 2;
//                    ReplaceFragment(reminderListFragment);
//                    break;
//                case Resource.Id.menu_delete_events:
//                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
//                    db.CreateTable<Event>();
//                    var events = db.Table<Event>().ToList();
//                    foreach (var x in events)
//                    {
//                        //canceling alarm manager
//                        Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
//                        PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, x.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
//                        AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
//                        alarmManager.Cancel(pendingIntent);
//                    }
//                    db.DeleteAll<Event>();
//                    ReplaceFragment(reminderListFragment);
//                    break;
//            }

//            return base.OnOptionsItemSelected(item);
//        }

//        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
//        {
//            switch (requestCode)
//            {
//                case 1: //write_external_storage
//                    if (grantResults[0] == Permission.Granted)
//                    {
//                        Toast.MakeText(this, "Permission granted", ToastLength.Short).Show();
//                    }
//                    else
//                    {
//                        Toast.MakeText(this, "Permission denied", ToastLength.Short).Show();
//                    }
//                    break;
//                case 2: //read_external_storage
//                    if (grantResults[0] == Permission.Granted)
//                    {
//                        Toast.MakeText(this, "Permission granted", ToastLength.Short).Show();
//                    }
//                    else
//                    {
//                        Toast.MakeText(this, "Permission denied", ToastLength.Short).Show();
//                    }
//                    break;
//                default:
//                    break;
//            }
//            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
//        }
//    }
//}