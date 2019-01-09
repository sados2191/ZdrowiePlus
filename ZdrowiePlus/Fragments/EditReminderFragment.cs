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
    public class EditReminderFragment : Android.App.Fragment
    {
        private static ListRemindersFragment reminderListFragment = new ListRemindersFragment();

        TextView eventDate;
        TextView eventTime;
        EditText eventTitle;
        EditText eventDescription;
        Event eventToEdit;
        TextView eventType;
        Spinner measurementSpinner;
        int year, month, day, hour, minute;

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

            year = eventToEdit.Date.Year;
            month = eventToEdit.Date.Month;
            day = eventToEdit.Date.Day;
            hour = eventToEdit.Date.Hour;
            minute = eventToEdit.Date.Minute;

            View view = inflater.Inflate(Resource.Layout.EditReminder, container, false);
            
            //type
            eventType = view.FindViewById<TextView>(Resource.Id.textEditType);
            //eventType.Text = eventToEdit.EventType.ToString();
            if (eventToEdit.EventType == EventType.Measurement)
            {
                eventType.Text = "Pomiar";
            }
            else if (eventToEdit.EventType == EventType.Visit)
            {
                eventType.Text = "Wizyta";
            }
            else if (eventToEdit.EventType == EventType.Medicine)
            {
                eventType.Text = "Leki";
            }

            //title
            eventTitle = view.FindViewById<EditText>(Resource.Id.visitEditTitle);
            eventTitle.Text = eventToEdit.Title;

            //measurement type spinner
            measurementSpinner = view.FindViewById<Spinner>(Resource.Id.editSpinner);
            //measurementSpinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            measurementSpinner.Adapter = adapter;
            //measurementSpinner.SetSelection(4, true);

            if (eventToEdit.EventType == EventType.Measurement)
            {
                eventTitle.Visibility = ViewStates.Gone;
                measurementSpinner.Visibility = ViewStates.Visible;

                measurementSpinner.SetSelection(adapter.GetPosition(eventToEdit.Title), true);
            }
            else
            {
                eventTitle.Visibility = ViewStates.Visible;
                measurementSpinner.Visibility = ViewStates.Gone;
            }

            //date choosing
            eventDate = view.FindViewById<TextView>(Resource.Id.textEditDate);
            eventDate.Text = eventToEdit.Date.ToLongDateString();
            eventDate.Click += DateSelect_OnClick;

            //time choosing
            eventTime = view.FindViewById<TextView>(Resource.Id.textEditTime);
            eventTime.Text = eventToEdit.Date.ToShortTimeString();
            eventTime.Click += TimeSelectOnClick;

            //description
            eventDescription = view.FindViewById<EditText>(Resource.Id.visitEditDescription);
            eventDescription.Text = eventToEdit.Description;

            //Save visit button
            Button buttonSaveVisit = view.FindViewById<Button>(Resource.Id.btnSaveVisit);
            buttonSaveVisit.Click += SaveVisit;

            //Delete visit button
            Button buttonDeleteVisit = view.FindViewById<Button>(Resource.Id.btnDeleteVisit);
            buttonDeleteVisit.Click += DeleteVisit;

            //Delete series button
            Button buttonDeleteSeries = view.FindViewById<Button>(Resource.Id.btnDeleteSeries);
            buttonDeleteSeries.Click += DeleteSeries;
            if (eventToEdit.EventType == EventType.Medicine || eventToEdit.EventType == EventType.Measurement)
            {
                buttonDeleteSeries.Visibility = ViewStates.Visible;
            }

            return view;
        }

        private void SelectEvent(int id)
        {
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            //db.CreateTable<Event>();
            eventToEdit = db.Get<Event>(id);
        }

        private void DeleteVisit(object sender, EventArgs e)
        {
            //delete from database
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Event>();
            db.Delete(eventToEdit);

            //cancel alarm manager
            Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, eventToEdit.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(pendingIntent);

            //go to list after delete
            var trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.fragmentContainer, reminderListFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        private void DeleteSeries(object sender, EventArgs e)
        {
            //selecting events to delete
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Event>();
            var events = db.Table<Event>().Where(x => x.Title == eventToEdit.Title && x.Description == eventToEdit.Description)
                                          .OrderBy(x => x.Date).ToList();
            foreach (var item in events)
            {
                db.Delete(item);

                //canceling alarm manager
                Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, item.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                alarmManager.Cancel(pendingIntent);
            }

            //go to list after delete
            var trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.fragmentContainer, reminderListFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        private void SaveVisit(object sender, EventArgs e)
        {
            eventToEdit.Date = new DateTime(year, month, day, hour, minute, 0);
            if (eventToEdit.EventType == EventType.Measurement)
            {
                //eventToEdit.Title = measurementSpinner.GetItemAtPosition(measurementSpinner.SelectedItemPosition).ToString();
                eventToEdit.Title = measurementSpinner.SelectedItem.ToString();
            }
            else
            {
                eventToEdit.Title = eventTitle.Text;
            }
            eventToEdit.Description = eventDescription.Text;
            if (eventToEdit.Title != string.Empty)
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                {
                    // We have permission

                    //database connection
                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                    db.CreateTable<Event>();
                    db.Update(eventToEdit);

                    Toast.MakeText(this.Activity, $"Zapisano\n{eventToEdit.Date.ToString("dd.MM.yyyy HH:mm")}\n{eventToEdit.Id}", ToastLength.Short).Show();

                    //Notification
                    Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                    notificationIntent.PutExtra("message", $"{eventToEdit.Date.ToString("dd.MM.yyyy HH:mm")} {eventToEdit.Title}");
                    if (eventToEdit.EventType == EventType.Visit)
                    {
                        notificationIntent.PutExtra("title", "Wizyta");
                    }
                    else if (eventToEdit.EventType == EventType.Medicine)
                    {
                        notificationIntent.PutExtra("title", "Leki");
                    }
                    else if (eventToEdit.EventType == EventType.Measurement)
                    {
                        notificationIntent.PutExtra("title", "Pomiar");
                        notificationIntent.PutExtra("type", measurementSpinner.SelectedItemPosition);
                    }
                    notificationIntent.PutExtra("id", eventToEdit.Id);

                    var timer = (long)eventToEdit.Date.ToUniversalTime().Subtract(
                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        ).TotalMilliseconds;

                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, eventToEdit.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
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
                    // Permission is not granted. If necessary display rationale & request.

                    //if (ActivityCompat.ShouldShowRequestPermissionRationale(this.Activity, Manifest.Permission.WriteExternalStorage))
                    //{
                    //    //Explain to the user why we need permission
                    //    Snackbar.Make(View, "Write external storage is required to save a visit", Snackbar.LengthIndefinite)
                    //            .SetAction("OK", v => ActivityCompat.RequestPermissions(this.Activity, new String[] { Manifest.Permission.WriteExternalStorage}, 1))
                    //            .Show();

                    //    return;
                    //}

                    ActivityCompat.RequestPermissions(this.Activity, new String[] { Manifest.Permission.WriteExternalStorage }, 1);

                }
            }
            else
            {
                Toast.MakeText(this.Activity, "Tytuł nie może być pusty", ToastLength.Short).Show();
            }
        }

        void TimeSelectOnClick(object sender, EventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                eventTime.Text = time.ToShortTimeString();
                hour = time.Hour;
                minute = time.Minute;
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        void DateSelect_OnClick(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                eventDate.Text = time.ToLongDateString();
                year = time.Year;
                month = time.Month;
                day = time.Day;
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        public override void OnResume()
        {
            base.OnResume();

            eventTitle.Text = eventToEdit.Title;
            eventDescription.Text = eventToEdit.Description;

            var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            measurementSpinner.Adapter = adapter;
            measurementSpinner.SetSelection(adapter.GetPosition(eventToEdit.Title), true);
        }
    }
}