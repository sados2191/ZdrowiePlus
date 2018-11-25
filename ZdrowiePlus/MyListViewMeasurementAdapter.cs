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
    public class MyListViewMeasurementAdapter : BaseAdapter<Measurement>
    {
        private List<Measurement> mItems;
        private Context mContext;

        public MyListViewMeasurementAdapter(Context context, List<Measurement> items)
        {
            mItems = items;
            mContext = context;
        }

        public override Measurement this[int position]
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
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ListViewMeasurementRow, null, false);
            }

            TextView txtDate = row.FindViewById<TextView>(Resource.Id.listMeasurementDate);
            txtDate.Text = mItems[position].Date.ToString("dd.MM.yyyy");

            TextView txtTime = row.FindViewById<TextView>(Resource.Id.listMeasurementTime);
            txtTime.Text = mItems[position].Date.ToString("HH:mm");

            TextView txtValue = row.FindViewById<TextView>(Resource.Id.listMeasurementValue);
            txtValue.Text = mItems[position].Value;
            txtValue.SetTextColor(Android.Graphics.Color.ParseColor("#3700b3"));

            TextView txtType = row.FindViewById<TextView>(Resource.Id.listMeasurementType);
            //switch (mItems[position].MeasurementType)
            //{
            //    case MeasurementType.BloodPressure:
            //        txtType.Text = "Ciśnienie krwi";
            //        txtValue.SetTextColor(Android.Graphics.Color.ParseColor("#cc6600"));
            //        break;
            //    case EventType.Medicine:
            //        txtType.Text = "Leki";
            //        txtDesc.SetTextColor(Android.Graphics.Color.ParseColor("#33cc33"));
            //        break;
            //    default:
            //        break;
            //}

            return row;
        }
    }
}