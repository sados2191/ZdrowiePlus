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
using ZdrowiePlus.Fragments;
using Android.Content.PM;
using Android.Runtime;
using SQLite;
using System.IO;

namespace ZdrowiePlus
{
    [Activity(Label = "ZdrowiePlus", MainLauncher = true, Theme ="@style/MyTheme")]
    public class MainActivity : AppCompatActivity
    {
        //DODAC - ponowic notifikacje po reboot'cie telefonu

        //list of visits
        public static List<Event> visitList = new List<Event>();
        public static Event eventToEdit = new Event();

        //left menu
        private MyActionBarDrawerToggle drawerToggle;
        private DrawerLayout drawerLayout;
        private ListView leftDrawer;
        private ArrayAdapter leftAdapter;
        private List<string> leftDataSet;
        
        //fragments
        //private Fragment currentFragment; show fragment method
        //private Stack<Fragment> stackFragment;
        private static AddVisitFragment addVisitFragment;
        private static VisitListFragment visitListFragment;

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
            leftDataSet.Add("Dodaj wizytę");
            leftDataSet.Add("Leki");
            leftDataSet.Add("Parametry Zdrowotne");
            leftDataSet.Add("Raport");
            leftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, leftDataSet);
            leftDrawer.Adapter = leftAdapter;
            leftDrawer.ItemClick += leftDrawer_ItemClick;

            //fragments
            //stackFragment = new Stack<Fragment>(); show fragment method
            addVisitFragment = new AddVisitFragment();
            visitListFragment = new VisitListFragment();
            var trans = FragmentManager.BeginTransaction();

            //trans.Add(Resource.Id.fragmentContainer, addVisitFragment, "AddVisit");
            //trans.Hide(addVisitFragment);

            trans.Add(Resource.Id.fragmentContainer, visitListFragment, "VisitList");
            //currentFragment = visitListFragment;
            //stackFragment.Push(currentFragment);
            trans.Commit();

        }

        //show fragment method
        //private void ShowFragment(Fragment fragment)
        //{
        //    if (fragment.IsVisible)
        //    {
        //        return;
        //    }

        //    var trans = FragmentManager.BeginTransaction();

        //    //fragment.View.BringToFront();
        //    //currentFragment.View.BringToFront();

        //    trans.Hide(currentFragment);
        //    trans.Show(fragment);
        //    trans.AddToBackStack(null);
        //    trans.Commit();
        //    stackFragment.Push(currentFragment);
        //    currentFragment = fragment;
        //}

        //replace fragment method
        private void ReplaceFragment(Fragment fragment)
        {
            if (fragment.IsVisible)
            {
                return;
            }

            var trans = FragmentManager.BeginTransaction();

            trans.Replace(Resource.Id.fragmentContainer, fragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        public override void OnBackPressed()
        {
            //if (FragmentManager.BackStackEntryCount > 0) show fragment way
            //{
            //    FragmentManager.PopBackStack();
            //    currentFragment = stackFragment.Pop(); //changing current fragment on back button press
            //}
            //else
            //{
            //    base.OnBackPressed();
            //}

            base.OnBackPressed();
        }

        //left menu item click
        private void leftDrawer_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            switch (e.Position)
            {
                case 0:
                    //ShowFragment(visitListFragment);
                    ReplaceFragment(visitListFragment);
                    break;
                case 1:
                    //ShowFragment(addVisitFragment);
                    ReplaceFragment(addVisitFragment);
                    break;
                default:
                    break;
            }
            Toast.MakeText(this, leftDataSet[e.Position], ToastLength.Short).Show();
            drawerLayout.CloseDrawer((int)GravityFlags.Left);
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
                case Resource.Id.menu_info:
                    Toast.MakeText(this, Resource.String.app_name, ToastLength.Short).Show();
                    break;
                case Resource.Id.menu_visits:
                    ReplaceFragment(visitListFragment);
                    break;
                case Resource.Id.menu_delete_events:
                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "events.db"));
                    db.DeleteAll<Event>();
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