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
    public class VisitListFragment : Fragment
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

            visitAdapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, MainActivity.visitList);
            ListView visitListView = view.FindViewById<ListView>(Resource.Id.listViewVisits);
            visitListView.Adapter = visitAdapter;
            visitListView.FastScrollEnabled = true;

            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            visitAdapter.NotifyDataSetChanged();
        }
    }
}