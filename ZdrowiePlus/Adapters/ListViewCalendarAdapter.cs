﻿using System;
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
    public class ListViewCalendarAdapter : BaseAdapter<Reminder>
    {
        private List<Reminder> mItems;
        private Context mContext;

        public ListViewCalendarAdapter(Context context, List<Reminder> items)
        {
            mItems = items;
            mContext = context;
        }

        public override Reminder this[int position]
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
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.RowCalendarList, null, false);
            }

            TextView txtTime = row.FindViewById<TextView>(Resource.Id.txtTimeCalendar);
            txtTime.Text = mItems[position].Date.ToString("HH:mm");

            TextView txtDesc = row.FindViewById<TextView>(Resource.Id.txtTitleCalendar);
            txtDesc.Text = mItems[position].Title;

            ImageView iconType = row.FindViewById<ImageView>(Resource.Id.iconType);
            switch (mItems[position].ReminderType)
            {
                case ReminderType.Visit:
                    iconType.SetImageResource(Resource.Drawable.doctor_icon);
                    txtDesc.SetTextColor(Android.Graphics.Color.ParseColor("#e54d03"));
                    break;
                case ReminderType.Medicine:
                    iconType.SetImageResource(Resource.Drawable.medical_pill);
                    txtDesc.SetTextColor(Android.Graphics.Color.ParseColor("#33cc33"));
                    break;
                case ReminderType.Measurement:
                    iconType.SetImageResource(Resource.Drawable.pulsometer_icon);
                    txtDesc.SetTextColor(Android.Graphics.Color.ParseColor("#be03e5"));
                    break;
                default:
                    break;
            }

            return row;
        }
    }
}