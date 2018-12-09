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
    public class ListViewReminderAdapter : BaseAdapter<Event>
    {
        private List<Event> mItems;
        private Context mContext;

        public ListViewReminderAdapter(Context context, List<Event> items)
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
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowReminderList, null, false);
            }

            TextView txtDate = row.FindViewById<TextView>(Resource.Id.txtDate);
            txtDate.Text = mItems[position].Date.ToString("dd.MM.yyyy");

            TextView txtTime = row.FindViewById<TextView>(Resource.Id.txtTime);
            txtTime.Text = mItems[position].Date.ToString("HH:mm");

            TextView txtDesc = row.FindViewById<TextView>(Resource.Id.txtDesc);
            txtDesc.Text = mItems[position].Title;

            TextView txtType = row.FindViewById<TextView>(Resource.Id.txtType);
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