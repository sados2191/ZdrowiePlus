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
        ListRemindersFragment reminderListFragment;

        ImageView eventIcon;
        TextView eventType;
        TextView eventTitle;
        TextView eventDate;
        TextView eventTime;
        EditText eventDescription;
        Event selectedEvent;
        LinearLayout medicineLayout;
        TextView medicineCount;
        EditText eventLaterValue;
        TextView eventLaterSpan;
        Button buttonSkip;
        Button buttonLater;
        Button buttonConfirm;

        int laterMultiplier;

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

            reminderListFragment = new ListRemindersFragment();

            View view = inflater.Inflate(Resource.Layout.EventFormNotification, container, false);

            eventIcon = view.FindViewById<ImageView>(Resource.Id.iconEventType);
            eventType = view.FindViewById<TextView>(Resource.Id.eventType);
            eventDate = view.FindViewById<TextView>(Resource.Id.eventDate);
            eventTime = view.FindViewById<TextView>(Resource.Id.eventTime);
            eventTitle = view.FindViewById<TextView>(Resource.Id.eventTitle);
            eventDescription = view.FindViewById<EditText>(Resource.Id.eventDescription);
            medicineCount = view.FindViewById<TextView>(Resource.Id.medicineCount);
            medicineLayout = view.FindViewById<LinearLayout>(Resource.Id.layoutMedicine);
            eventLaterValue = view.FindViewById<EditText>(Resource.Id.eventLaterValue);
            eventLaterSpan = view.FindViewById<TextView>(Resource.Id.eventLaterSpan);
            buttonSkip = view.FindViewById<Button>(Resource.Id.buttonSkip);
            buttonLater = view.FindViewById<Button>(Resource.Id.buttonLater);
            buttonConfirm = view.FindViewById<Button>(Resource.Id.buttonConfirm);

            buttonSkip.Click += SkipReminder;
            buttonLater.Click += PostponeReminder;
            buttonConfirm.Click += ConfirmReminder;

            return view;
        }

        private void ConfirmReminder(object sender, EventArgs e)
        {
            selectedEvent.Skipped = 2;
            selectedEvent.Description = eventDescription.Text;

            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.Update(selectedEvent);

            //go to list after save
            var trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.fragmentContainer, reminderListFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        private void PostponeReminder(object sender, EventArgs e)
        {
            selectedEvent.Description = eventDescription.Text;

            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.Update(selectedEvent);

            if (int.TryParse(eventLaterValue.Text, out int x))
            {
                Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));

                if (selectedEvent.EventType == EventType.Visit)
                {
                    notificationIntent.PutExtra("title", "Wizyta");
                    notificationIntent.PutExtra("message", $"{selectedEvent.Date.ToString("dd.MM.yyyy HH:mm")} {selectedEvent.Title}");
                }
                else if (selectedEvent.EventType == EventType.Medicine)
                {
                    notificationIntent.PutExtra("title", "Leki");
                    notificationIntent.PutExtra("message", $"{selectedEvent.Date.ToString("dd.MM.yyyy HH:mm")} {selectedEvent.Title} dawka: {selectedEvent.Count}");
                }
                notificationIntent.PutExtra("id", selectedEvent.Id);

                var timer = (long)DateTime.Now.AddMinutes(x * laterMultiplier).ToUniversalTime().Subtract(
                            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                            ).TotalMilliseconds;

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, selectedEvent.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                alarmManager.Set(AlarmType.RtcWakeup, timer, pendingIntent);

                //go to list after save
                var trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.fragmentContainer, reminderListFragment);
                trans.AddToBackStack(null);
                trans.Commit();
            }
            else
            {
                Toast.MakeText(this.Activity, $"Czas opóźnienia nie może byc pusty", ToastLength.Short).Show();
            }
        }

        private void SkipReminder(object sender, EventArgs e)
        {
            selectedEvent.Skipped = 1;
            selectedEvent.Description = eventDescription.Text;

            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.Update(selectedEvent);

            //go to list after save
            var trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.fragmentContainer, reminderListFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        private void SelectEvent(int id)
        {
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            //db.CreateTable<Event>();
            selectedEvent = db.Get<Event>(id);
        }

        public override void OnResume()
        {
            base.OnResume();

            eventDate.Text = selectedEvent.Date.ToLongDateString();
            eventTime.Text = selectedEvent.Date.ToShortTimeString();
            eventTitle.Text = selectedEvent.Title;
            eventDescription.Text = selectedEvent.Description;

            if (selectedEvent.EventType == EventType.Medicine)
            {
                eventIcon.SetBackgroundResource(Resource.Drawable.medical_pill);
                eventType.Text = "Przypomnienie o leku";
                eventLaterSpan.Text = "minut.";
                buttonConfirm.Text = "Weź";

                laterMultiplier = 1;
                eventLaterValue.Text = "30";

                medicineLayout.Visibility = ViewStates.Visible;
                medicineCount.Text = selectedEvent.Count.ToString();
            }
            else if (selectedEvent.EventType == EventType.Visit)
            {
                eventIcon.SetBackgroundResource(Resource.Drawable.doctor_icon);
                eventType.Text = "Przypomnienie o wizycie";
                eventLaterSpan.Text = "godzin.";
                buttonConfirm.Text = "Potwierdź";

                laterMultiplier = 60;
                eventLaterValue.Text = "1";

                medicineLayout.Visibility = ViewStates.Gone;
            }
        }
    }
}