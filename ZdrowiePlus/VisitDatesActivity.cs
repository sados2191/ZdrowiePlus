using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;

namespace ZdrowiePlus
{
    [Activity(Label = "Umówione wizyty", MainLauncher = false, Theme = "@style/MyTheme")]
    public class VisitDatesActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.VisitDates);

            var toolbar = FindViewById<SupportToolbar>(Resource.Id.toolbar);
            //Toolbar takes on default actionbar characteristics
            SetSupportActionBar(toolbar);
            //ActionBar.Title = "Nowy";

            var visits = Intent.Extras.GetStringArrayList("terms") ?? new string[0];
            ArrayAdapter<string> visitAdapter  = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, visits);
            ListView visitListView = FindViewById<ListView>(Resource.Id.listViewVisits);
            visitListView.Adapter = visitAdapter;
            visitListView.FastScrollEnabled = true;
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
                //case Resource.Id.menu_visits:
                //    textToShow = "Zapisane wizyty";
                //    var intent = new Intent(this, typeof(TermHistoryActivity));
                //    intent.PutStringArrayListExtra("terms", terms);
                //    StartActivity(intent);
                //    break;
            }

            Toast.MakeText(this, item.TitleFormatted + ": " + textToShow, ToastLength.Short).Show();

            return base.OnOptionsItemSelected(item);
        }
    }
}