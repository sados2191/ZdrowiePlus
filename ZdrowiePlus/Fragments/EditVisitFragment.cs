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
    public class EditVisitFragment : Android.App.Fragment
    {
        private static VisitListFragment visitListFragment = new VisitListFragment();

        TextView eventDate;
        TextView eventTime;
        EditText eventTitle;
        EditText eventDescription;
        int year, month, day, hour, minute;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            year = MainActivity.eventToEdit.Date.Year;
            month = MainActivity.eventToEdit.Date.Month;
            day = MainActivity.eventToEdit.Date.Day;
            hour = MainActivity.eventToEdit.Date.Hour;
            minute = MainActivity.eventToEdit.Date.Minute;

            View view = inflater.Inflate(Resource.Layout.EditVisit, container, false);

            //date choosing
            eventDate = view.FindViewById<TextView>(Resource.Id.textEditDate);
            eventDate.Text = MainActivity.eventToEdit.Date.ToLongDateString();
            eventDate.Click += DateSelect_OnClick;

            //time choosing
            eventTime = view.FindViewById<TextView>(Resource.Id.textEditTime);
            eventTime.Text = MainActivity.eventToEdit.Date.ToShortTimeString();
            eventTime.Click += TimeSelectOnClick;

            //title
            eventTitle = view.FindViewById<EditText>(Resource.Id.visitEditTitle);
            eventTitle.Text = MainActivity.eventToEdit.Title;

            //description
            eventDescription = view.FindViewById<EditText>(Resource.Id.visitEditDescription);
            eventDescription.Text = MainActivity.eventToEdit.Description;

            //Save visit button
            Button buttonSaveVisit = view.FindViewById<Button>(Resource.Id.btnSaveVisit);
            buttonSaveVisit.Click += SaveVisit;

            //Delete visit button
            Button buttonDeleteVisit = view.FindViewById<Button>(Resource.Id.btnDeleteVisit);
            buttonDeleteVisit.Click += DeleteVisit;

            return view;
        }

        private void DeleteVisit(object sender, EventArgs e)
        {
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "events.db"));
            db.CreateTable<Event>();
            db.Delete(MainActivity.eventToEdit);

            //go to list after delete
            var trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.fragmentContainer, visitListFragment);
            trans.AddToBackStack(null);
            trans.Commit();
        }

        private void SaveVisit(object sender, EventArgs e)
        {
            MainActivity.eventToEdit.Date = new DateTime(year, month, day, hour, minute, 0);
            MainActivity.eventToEdit.Title = eventTitle.Text;
            MainActivity.eventToEdit.Description = eventDescription.Text;
            if (MainActivity.eventToEdit.Title != string.Empty)
            {
                if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                {
                    // We have permission

                    //database connection
                    var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "events.db"));
                    db.CreateTable<Event>();
                    db.Update(MainActivity.eventToEdit);

                    Toast.MakeText(this.Activity, $"Zapisano\n{MainActivity.eventToEdit.Date.ToString("dd.MM.yyyy HH:mm")}\n{MainActivity.eventToEdit.Id}", ToastLength.Short).Show();

                    //Notification
                    Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                    notificationIntent.PutExtra("message", $"{MainActivity.eventToEdit.Date.ToString("dd.MM.yyyy HH:mm")} {MainActivity.eventToEdit.Title}");
                    notificationIntent.PutExtra("title", "Wizyta");
                    notificationIntent.PutExtra("id", MainActivity.eventToEdit.Id);

                    var timer = (long)MainActivity.eventToEdit.Date.ToUniversalTime().Subtract(
                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        ).TotalMilliseconds;

                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, MainActivity.eventToEdit.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                    AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                    alarmManager.Set(AlarmType.RtcWakeup, timer, pendingIntent);

                    //go to list after save
                    var trans = FragmentManager.BeginTransaction();
                    trans.Replace(Resource.Id.fragmentContainer, visitListFragment);
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

            eventTitle.Text = MainActivity.eventToEdit.Title;
            eventDescription.Text = MainActivity.eventToEdit.Description;
        }
    }
}