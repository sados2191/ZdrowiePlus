using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZdrowiePlus.Fragments;

namespace ZdrowiePlus
{
    public class TimeListViewAdapter : BaseAdapter<DateTime>
    {
        private List<DateTime> mItems;
        private Context mContext;

        public TimeListViewAdapter(Context context, List<DateTime> items)
        {
            mItems = items;
            mContext = context;
        }

        public override DateTime this[int position]
        {
            get { return mItems[position]; }
        }

        public override int Count
        {
            get
            {
                return mItems.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.TimeListRow, null, false);
            }

            TextView pillTime = row.FindViewById<TextView>(Resource.Id.time);
            pillTime.Text = mItems[position].ToString("HH:mm");

            return row;
        }
    }
}