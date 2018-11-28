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

namespace ZdrowiePlus
{
    public class MyListViewCalendarAdapter : BaseAdapter<Event>
    {
        private List<Event> mItems;
        private Context mContext;

        public MyListViewCalendarAdapter(Context context, List<Event> items)
        {
            mItems = items;
            mContext = context;
        }

        public override Event this[int position]
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
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ListViewCalendarRow, null, false);
            }

            TextView txtTime = row.FindViewById<TextView>(Resource.Id.txtTimeCalendar);
            txtTime.Text = mItems[position].Date.ToString("HH:mm");

            TextView txtDesc = row.FindViewById<TextView>(Resource.Id.txtTitleCalendar);
            txtDesc.Text = mItems[position].Title;

            TextView txtType = row.FindViewById<TextView>(Resource.Id.txtTypeCalendar);
            switch (mItems[position].EventType)
            {
                case EventType.Visit:
                    txtType.Text = "Wizyta";
                    txtDesc.SetTextColor(Android.Graphics.Color.ParseColor("#cc6600"));
                    break;
                case EventType.Medicine:
                    txtType.Text = "Leki";
                    txtDesc.SetTextColor(Android.Graphics.Color.ParseColor("#33cc33"));
                    break;
                case EventType.Measurement:
                    txtType.Text = "Pomiar";
                    txtDesc.SetTextColor(Android.Graphics.Color.ParseColor("#3700b3"));
                    break;
                default:
                    break;
            }

            return row;
        }
    }
}