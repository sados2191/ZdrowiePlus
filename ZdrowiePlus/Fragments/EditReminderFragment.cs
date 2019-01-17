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
        ImageView iconType;

        Spinner measurementSpinner;
        LinearLayout measurementLayout;

        LinearLayout medicineLayout;
        EditText medicineCount;

        LinearLayout remindBeforeLayout;
        EditText remindBeforeValue;
        Spinner remindBeforeSpinner;
        int remindMinutesBefore;
        int remindBeforeMultiplier;

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
                //Toast.MakeText(this.Activity, $"{id}", ToastLength.Short).Show();
                //Arguments.Clear();
                Arguments = null;

                SelectEvent(id);
            }

            //remindMinutesBefore = eventToEdit.ReminderMinutesBefore;
            //remindBeforeMultiplier = 1;

            Toast.MakeText(this.Activity, $"{eventToEdit.ReminderMinutesBefore} min", ToastLength.Short).Show();

            year = eventToEdit.Date.Year;
            month = eventToEdit.Date.Month;
            day = eventToEdit.Date.Day;
            hour = eventToEdit.Date.Hour;
            minute = eventToEdit.Date.Minute;

            View view = inflater.Inflate(Resource.Layout.EditReminder, container, false);

            //type
            eventType = view.FindViewById<TextView>(Resource.Id.textEditType);
            iconType = view.FindViewById<ImageView>(Resource.Id.imageEditType);
            //eventType.Text = eventToEdit.EventType.ToString();

            //title
            eventTitle = view.FindViewById<EditText>(Resource.Id.eventEditTitle);
            //eventTitle.Text = eventToEdit.Title;

            //for Visit reminder edit
            remindBeforeLayout = view.FindViewById<LinearLayout>(Resource.Id.remindBeforeLayout);
            remindBeforeSpinner = view.FindViewById<Spinner>(Resource.Id.addVisitReminderSpinner);
            remindBeforeValue = view.FindViewById<EditText>(Resource.Id.textAddVisitReminder);

            //for Measurement reminder edit
            measurementLayout = view.FindViewById<LinearLayout>(Resource.Id.layoutMeasurement);
            measurementSpinner = view.FindViewById<Spinner>(Resource.Id.editSpinner);

            //for Medicine reminder edit
            medicineLayout = view.FindViewById<LinearLayout>(Resource.Id.layoutMedicine);
            medicineCount = view.FindViewById<EditText>(Resource.Id.medicineCount);

            if (eventToEdit.EventType == EventType.Measurement)
            {
                eventType.Text = "Przypomienie o pomiarze";
                iconType.SetImageResource(Resource.Drawable.pulsometer_icon);

                //spinner behavior
                //var adapterM = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
                //adapterM.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                //measurementSpinner.Adapter = adapterM;

                eventTitle.Visibility = ViewStates.Gone;
                measurementLayout.Visibility = ViewStates.Visible;
                remindBeforeLayout.Visibility = ViewStates.Gone;
                medicineLayout.Visibility = ViewStates.Gone;

                //measurementSpinner.SetSelection(adapterM.GetPosition(eventToEdit.Title), true);
            }
            else if (eventToEdit.EventType == EventType.Visit)
            {
                eventTitle.Visibility = ViewStates.Visible;
                measurementLayout.Visibility = ViewStates.Gone;
                medicineLayout.Visibility = ViewStates.Gone;

                eventType.Text = "Przypomienie o wizycie";
                iconType.SetImageResource(Resource.Drawable.doctor_icon);

                remindBeforeLayout.Visibility = ViewStates.Visible;

                //Edit text value behavior
                remindBeforeValue.SetSelectAllOnFocus(true);
                //remindBeforeValue.TextChanged += (s, e) => {
                //    //EditText value = (EditText)s;
                //    //if (!int.TryParse(value.Text, out int x)) x = 0;
                //    //remindMinutesBefore = x * remindBeforeMultiplier;
                //    //Toast.MakeText(this.Activity, $"{remindMinutesBefore} minut przed", ToastLength.Short).Show();
                //};

                //spinner set remind time before visit
                //var adapterV = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.visits_reminder_array, Android.Resource.Layout.SimpleSpinnerItem);
                //adapterV.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                //remindBeforeSpinner.Adapter = adapterV;
                //remindBeforeSpinner.ItemSelected += (s, e) => {
                //    //remindMinutesBefore = int.Parse(remindBeforeValue.Text.ToString());
                //    switch (e.Position)
                //    {
                //        case 0:
                //            remindBeforeMultiplier = 1;
                //            break;
                //        case 1:
                //            remindBeforeMultiplier = 60;
                //            break;
                //        case 2:
                //            remindBeforeMultiplier = 60 * 24;
                //            break;
                //        default:
                //            break;
                //    }

                //    //remindMinutesBefore *= remindBeforeMultiplier;
                //    //Toast.MakeText(this.Activity, $"{remindMinutesBefore} minut przed", ToastLength.Short).Show();
                //};

                //set remindBefore views
                //int z = 0;
                //if (remindMinutesBefore % 1440 == 0)
                //{
                //    remindBeforeMultiplier = 60 * 24;

                //    z = remindMinutesBefore / 1440;
                //    remindBeforeValue.Text = z.ToString();

                //    remindBeforeSpinner.SetSelection(2, true);
                //}
                //else if (remindMinutesBefore % 60 == 0)
                //{
                //    remindBeforeMultiplier = 60;

                //    z = remindMinutesBefore / 60;
                //    remindBeforeValue.Text = z.ToString();

                //    remindBeforeSpinner.SetSelection(1, true);
                //}
                //else
                //{
                //    remindBeforeMultiplier = 1;

                //    z = remindMinutesBefore;
                //    remindBeforeValue.Text = z.ToString();

                //    remindBeforeSpinner.SetSelection(0, true);
                //}
            }
            else if (eventToEdit.EventType == EventType.Medicine)
            {
                medicineLayout.Visibility = ViewStates.Visible;
                eventTitle.Visibility = ViewStates.Visible;
                measurementLayout.Visibility = ViewStates.Gone;
                remindBeforeLayout.Visibility = ViewStates.Gone;

                eventType.Text = "Przypomienie o lekach";
                iconType.SetImageResource(Resource.Drawable.medical_pill);
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
            eventDescription = view.FindViewById<EditText>(Resource.Id.eventEditDescription);
            eventDescription.Text = eventToEdit.Description;

            //Save visit button
            Button buttonSaveVisit = view.FindViewById<Button>(Resource.Id.buttonSave);
            buttonSaveVisit.Click += SaveVisit;

            //Delete visit button
            Button buttonDeleteVisit = view.FindViewById<Button>(Resource.Id.buttonDelete);
            buttonDeleteVisit.Click += DeleteVisit;

            //Delete series button
            Button buttonDeleteSeries = view.FindViewById<Button>(Resource.Id.buttonDeleteSeries);
            buttonDeleteSeries.Click += DeleteSeries;
            if (eventToEdit.EventType == EventType.Medicine || eventToEdit.EventType == EventType.Measurement)
            {
                buttonDeleteSeries.Visibility = ViewStates.Visible;
            }
            else
            {
                buttonDeleteSeries.Visibility = ViewStates.Gone;
            }

            //Cancel button
            Button buttonCancel = view.FindViewById<Button>(Resource.Id.buttonCancel);
            buttonCancel.Click += (s, e) =>
            {
                var trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.fragmentContainer, reminderListFragment);
                trans.AddToBackStack(null);
                trans.Commit();
            };

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
            else if (eventToEdit.EventType == EventType.Visit)
            {
                eventToEdit.Title = eventTitle.Text;

                if (!int.TryParse(remindBeforeValue.Text, out remindMinutesBefore))
                {
                    remindMinutesBefore = 0;
                }
                else
                {
                    switch (remindBeforeSpinner.SelectedItemPosition)
                    {
                        case 0:
                            remindBeforeMultiplier = 1;
                            break;
                        case 1:
                            remindBeforeMultiplier = 60;
                            break;
                        case 2:
                            remindBeforeMultiplier = 60 * 24;
                            break;
                        default:
                            break;
                    }

                    remindMinutesBefore = remindMinutesBefore * remindBeforeMultiplier;
                }
                
                eventToEdit.ReminderMinutesBefore = remindMinutesBefore;
            }
            else if (eventToEdit.EventType == EventType.Medicine)
            {
                eventToEdit.Title = eventTitle.Text;

                if (!int.TryParse(medicineCount.Text, out int count)) //jesli sie nie uda (pole puste)
                {
                    count = 1;
                }
                eventToEdit.Count = count;
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
                    
                    if (eventToEdit.EventType == EventType.Visit)
                    {
                        notificationIntent.PutExtra("title", "Wizyta");
                        notificationIntent.PutExtra("message", $"{eventToEdit.Date.ToString("dd.MM.yyyy HH:mm")} {eventToEdit.Title}");
                        eventToEdit.Date = eventToEdit.Date.AddMinutes(remindMinutesBefore * (-1)); //change date to save notification date earlier than visit date
                    }
                    else if (eventToEdit.EventType == EventType.Medicine)
                    {
                        notificationIntent.PutExtra("title", "Leki");
                        notificationIntent.PutExtra("message", $"{eventToEdit.Date.ToString("dd.MM.yyyy HH:mm")} {eventToEdit.Title} dawka: {eventToEdit.Count}");
                    }
                    else if (eventToEdit.EventType == EventType.Measurement)
                    {
                        notificationIntent.PutExtra("title", "Pomiar");
                        notificationIntent.PutExtra("message", $"{eventToEdit.Date.ToString("dd.MM.yyyy HH:mm")} {eventToEdit.Title}");
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

            //medicine
            medicineCount.Text = eventToEdit.Count.ToString();

            //measurement
            var adapterM = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.measurements_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterM.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            measurementSpinner.Adapter = adapterM;
            measurementSpinner.SetSelection(adapterM.GetPosition(eventToEdit.Title), true);

            //set remindBefore views fields
            remindMinutesBefore = eventToEdit.ReminderMinutesBefore;
            var adapterV = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.visits_reminder_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapterV.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            remindBeforeSpinner.Adapter = adapterV;
            int z = 0;
            if (remindMinutesBefore % 1440 == 0)
            {
                remindBeforeMultiplier = 60 * 24;

                z = remindMinutesBefore / 1440;
                remindBeforeValue.Text = z.ToString();

                remindBeforeSpinner.SetSelection(2, true);
            }
            else if (remindMinutesBefore % 60 == 0)
            {
                remindBeforeMultiplier = 60;

                z = remindMinutesBefore / 60;
                remindBeforeValue.Text = z.ToString();

                remindBeforeSpinner.SetSelection(1, true);
            }
            else
            {
                remindBeforeMultiplier = 1;

                z = remindMinutesBefore;
                remindBeforeValue.Text = z.ToString();

                remindBeforeSpinner.SetSelection(0, true);
            }
        }
    }
}