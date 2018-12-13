using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

namespace ZdrowiePlus.Fragments
{
    public class EventFromNotificationFragment : Android.App.Fragment
    {
        TextView eventTitle;
        TextView eventDate;
        TextView eventTime;
        TextView eventDescription;
        Event selectedEvent;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (Arguments != null)
            {
                int id = Arguments.GetInt("id", 0);
                Toast.MakeText(this.Activity, $"{id}", ToastLength.Short).Show();
                //Arguments.Clear();
                Arguments = null;

                SelectEvent(id);
            }

            View view = inflater.Inflate(Resource.Layout.EventFormNotification, container, false);
            
            eventDate = view.FindViewById<TextView>(Resource.Id.eventDate);
            eventDate.Text = selectedEvent.Date.ToLongDateString();

            eventTime = view.FindViewById<TextView>(Resource.Id.eventTime);
            eventTime.Text = selectedEvent.Date.ToShortTimeString();

            eventTitle = view.FindViewById<TextView>(Resource.Id.eventTitle);
            eventTitle.Text = selectedEvent.Title;

            eventDescription = view.FindViewById<TextView>(Resource.Id.eventDescription);
            eventDescription.Text = selectedEvent.Description;

            return view;
        }

        private void SelectEvent(int id)
        {
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            //db.CreateTable<Event>();
            selectedEvent = db.Get<Event>(id);
        }
    }
}