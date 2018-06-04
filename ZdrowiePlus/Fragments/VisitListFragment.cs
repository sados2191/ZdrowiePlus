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
    public class VisitListFragment : Android.App.Fragment
    {
        public static ArrayAdapter<string> visitAdapter;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.VisitList, container, false);

            MainActivity.visitList.Clear();

            if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.ReadExternalStorage) == (int)Permission.Granted)
            {
                // We have permission
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "events.db"));
                db.CreateTable<Event>();
                var events = db.Table<Event>().OrderBy(e => e.Date).ToList();
                foreach (var e in events)
                {
                    MainActivity.visitList.Add($"{e.Date} {e.Description}");
                }

                visitAdapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, MainActivity.visitList);
                ListView visitListView = view.FindViewById<ListView>(Resource.Id.listViewVisits);
                visitListView.Adapter = visitAdapter;
                visitListView.FastScrollEnabled = true;

            }
            else
            {
                // Permission is not granted. If necessary display rationale & request.

                //if (ActivityCompat.ShouldShowRequestPermissionRationale(this.Activity, Manifest.Permission.ReadExternalStorage))
                //{
                //    //Explain to the user why we need permission
                //    Snackbar.Make(View, "Read external storage is required to read the events", Snackbar.LengthIndefinite)
                //            .SetAction("OK", v => ActivityCompat.RequestPermissions(this.Activity, new String[] { Manifest.Permission.ReadExternalStorage}, 2))
                //            .Show();

                //    return;
                //}

                ActivityCompat.RequestPermissions(this.Activity, new String[] { Manifest.Permission.ReadExternalStorage }, 2);

            }

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            //visitAdapter.NotifyDataSetChanged();
        }
    }
}