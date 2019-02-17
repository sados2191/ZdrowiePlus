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
        private List<Reminder> mItems;

        public event EventHandler<int> ItemClick;

        private int skipped;

        public ListReminderAdapter(List<Reminder> items)
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

            if (mItems[position].Skipped == 1)
            {
                reminder.cardView.SetCardBackgroundColor(Android.Graphics.Color.ParseColor("#ffe6eb"));
                skipped = 1; //reminder skipped
            }
            else if (mItems[position].Skipped == 2 && mItems[position].Date < DateTime.Now)
            {
                reminder.cardView.SetCardBackgroundColor(Android.Graphics.Color.ParseColor("#e6ffee"));
                skipped = 2; //reminder confirmed
            }
            else
            {
                reminder.cardView.SetCardBackgroundColor(Android.Graphics.Color.ParseColor("#ffffff"));
                skipped = 0; //no action
            }

            switch (mItems[position].ReminderType)
            {
                case ReminderType.Visit:
                    reminder.reminderTitle.SetTextColor(Android.Graphics.Color.ParseColor("#e54d03"));
                    reminder.reminderType.SetImageResource(Resource.Drawable.doctor_icon);
                    if (skipped == 1)
                    {
                        reminder.medicalCount.Text = $"Odwołana";
                    }
                    else if (skipped == 2)
                    {
                        reminder.medicalCount.Text = $"Odbyta";
                    }
                    else
                    {
                        reminder.medicalCount.Text = $"";
                    }
                    break;
                case ReminderType.Medicine:
                    reminder.reminderTitle.SetTextColor(Android.Graphics.Color.ParseColor("#33cc33"));
                    reminder.reminderType.SetImageResource(Resource.Drawable.medical_pill);
                    if (skipped == 1)
                    {
                        reminder.medicalCount.Text = $"Pominięty. Dawka: {mItems[position].Count}";
                    }
                    else if (skipped == 2)
                    {
                        reminder.medicalCount.Text = $"Zażyty. Dawka: {mItems[position].Count}";
                    }
                    else
                    {
                        reminder.medicalCount.Text = $"Dawka: {mItems[position].Count}";
                    }
                    break;
                case ReminderType.Measurement:
                    reminder.reminderTitle.SetTextColor(Android.Graphics.Color.ParseColor("#be03e5"));
                    reminder.reminderType.SetImageResource(Resource.Drawable.pulsometer_icon);
                    if (skipped == 1)
                    {
                        reminder.medicalCount.Text = $"Pominięty";
                    }
                    else if (skipped == 2)
                    {
                        reminder.medicalCount.Text = $"Zrobiony";
                    }
                    else
                    {
                        reminder.medicalCount.Text = $"";
                    }
                    break;
                default:
                    break;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.RowReminder, parent, false);
            MyViewHolder holder = new MyViewHolder(view, OnClick);
            return holder;
        }

        internal class MyViewHolder : RecyclerView.ViewHolder
        {
            public ImageView reminderType;
            public TextView reminderDate;
            public TextView reminderTitle;
            public TextView medicalCount;
            public CardView cardView;

            public MyViewHolder(View itemView, Action<int> listener)
                : base(itemView)
            {
                reminderType = itemView.FindViewById<ImageView>(Resource.Id.imageView);
                reminderDate = itemView.FindViewById<TextView>(Resource.Id.textDate);
                reminderTitle = itemView.FindViewById<TextView>(Resource.Id.textTitle);
                medicalCount = itemView.FindViewById<TextView>(Resource.Id.textCount);
                cardView = itemView.FindViewById<CardView>(Resource.Id.cardView);

                itemView.Click += (sender, e) => listener (base.LayoutPosition);
            }
        }
    }
}