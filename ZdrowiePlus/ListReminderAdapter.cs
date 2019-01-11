using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace ZdrowiePlus
{
    public class ListReminderAdapter : RecyclerView.Adapter
    {
        private List<Event> mItems;

        public event EventHandler<int> ItemClick;

        public ListReminderAdapter(List<Event> items)
        {
            mItems = items;
        }

        public override int ItemCount
        {
            get
            {
                return mItems.Count;
            }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MyViewHolder reminder = holder as MyViewHolder;

            reminder.reminderDate.Text = mItems[position].Date.ToString("dd/MM/yyyy HH:mm");
            reminder.reminderTitle.Text = mItems[position].Title;

            switch (mItems[position].EventType)
            {
                case EventType.Visit:
                    //txtType.Text = "Wizyta";
                    //txtDesc.SetTextColor(Android.Graphics.Color.ParseColor("#cc6600"));
                    reminder.reminderTitle.SetTextColor(Android.Graphics.Color.ParseColor("#cc6600"));
                    reminder.reminderType.SetImageResource(Resource.Drawable.doctor_icon);
                    break;
                case EventType.Medicine:
                    //txtType.Text = "Leki";
                    //txtDesc.SetTextColor(Android.Graphics.Color.ParseColor("#33cc33"));
                    reminder.reminderTitle.SetTextColor(Android.Graphics.Color.ParseColor("#33cc33"));
                    reminder.reminderType.SetImageResource(Resource.Drawable.medical_pill);
                    break;
                case EventType.Measurement:
                    //txtType.Text = "Pomiar";
                    //txtDesc.SetTextColor(Android.Graphics.Color.ParseColor("#3700b3"));
                    reminder.reminderTitle.SetTextColor(Android.Graphics.Color.ParseColor("#3700b3"));
                    reminder.reminderType.SetImageResource(Resource.Drawable.pulsometer_icon);
                    break;
                default:
                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ReminderCard, parent, false);
            MyViewHolder holder = new MyViewHolder(view, OnClick);
            return holder;
        }

        internal class MyViewHolder : RecyclerView.ViewHolder
        {
            public ImageView reminderType;
            public TextView reminderDate;
            public TextView reminderTitle;

            public MyViewHolder(View itemView, Action<int> listener)
                : base(itemView)
            {
                reminderType = itemView.FindViewById<ImageView>(Resource.Id.imageView);
                reminderDate = itemView.FindViewById<TextView>(Resource.Id.textDate);
                reminderTitle = itemView.FindViewById<TextView>(Resource.Id.textTitle);

                itemView.Click += (sender, e) => listener (base.LayoutPosition);
            }
        }
    }
}