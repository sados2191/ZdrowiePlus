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
        TextView eventStatus;
        EditText eventTitle;
        EditText eventDescription;
        Reminder eventToEdit;
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
                if (!Arguments.IsEmpty)
                {
                    int id = Arguments.GetInt("id", 0);
                    //Arguments = null;
                    Arguments.Clear();

                    SelectEvent(id);
                }
            }

            year = eventToEdit.Date.Year;
            month = eventToEdit.Date.Month;
            day = eventToEdit.Date.Day;
            hour = eventToEdit.Date.Hour;
            minute = eventToEdit.Date.Minute;

            View view = inflater.Inflate(Resource.Layout.EditReminder, container, false);

            //type
            eventType = view.FindViewById<TextView>(Resource.Id.textEditType);
            iconType = view.FindViewById<ImageView>(Resource.Id.imageEditType);

            //title
            eventTitle = view.FindViewById<EditText>(Resource.Id.eventEditTitle);

            eventStatus = view.FindViewById<TextView>(Resource.Id.eventStatus);

            //for Visit reminder edit
            remindBeforeLayout = view.FindViewById<LinearLayout>(Resource.Id.remindBeforeLayout);
            remindBeforeSpinner = view.FindViewById<Spinner>(Resource.Id.addVisitReminderSpinner);
            remindBeforeValue = view.FindViewById<EditText>(Resource.Id.textAddVisitReminder);
            remindBeforeValue.SetSelectAllOnFocus(true);

            //for Measurement reminder edit
            measurementLayout = view.FindViewById<LinearLayout>(Resource.Id.layoutMeasurement);
            measurementSpinner = view.FindViewById<Spinner>(Resource.Id.editSpinner);

            //for Medicine reminder edit
            medicineLayout = view.FindViewById<LinearLayout>(Resource.Id.layoutMedicine);
            medicineCount = view.FindViewById<EditText>(Resource.Id.medicineCount);

            if (eventToEdit.ReminderType == ReminderType.Measurement)
            {
                eventType.Text = "Przypomienie o pomiarze";
                iconType.SetImageResource(Resource.Drawable.pulsometer_icon);

                if (eventToEdit.Skipped == 1)
                {
                    eventStatus.Text = "Pominięty";
                    eventStatus.SetTextColor(Android.Graphics.Color.Red);
                }
                else if (eventToEdit.Skipped == 2)
                {
                    eventStatus.Text = "Zrobiony";
                    eventStatus.SetTextColor(Android.Graphics.Color.ParseColor("#00a300"));
                }
                else
                {
                    eventStatus.Text = string.Empty;
                }

                eventTitle.Visibility = ViewStates.Gone;
                measurementLayout.Visibility = ViewStates.Visible;
                remindBeforeLayout.Visibility = ViewStates.Gone;
                medicineLayout.Visibility = ViewStates.Gone;
            }
            else if (eventToEdit.ReminderType == ReminderType.Visit)
            {
                eventTitle.Visibility = ViewStates.Visible;
                measurementLayout.Visibility = ViewStates.Gone;
                medicineLayout.Visibility = ViewStates.Gone;

                eventType.Text = "Przypomienie o wizycie";
                iconType.SetImageResource(Resource.Drawable.doctor_icon);

                if (eventToEdit.Skipped == 1)
                {
                    eventStatus.Text = "Odwołana";
                    eventStatus.SetTextColor(Android.Graphics.Color.Red);
                }
                else if (eventToEdit.Skipped == 2)
                {
                    eventStatus.Text = "Odbyta";
                    eventStatus.SetTextColor(Android.Graphics.Color.Green);
                }
                else
                {
                    eventStatus.Text = string.Empty;
                }

                remindBeforeLayout.Visibility = ViewStates.Visible;
            }
            else if (eventToEdit.ReminderType == ReminderType.Medicine)
            {
                if (eventToEdit.Skipped == 1)
                {
                    eventStatus.Text = "Pominięty";
                    eventStatus.SetTextColor(Android.Graphics.Color.Red);
                }
                else if (eventToEdit.Skipped == 2)
                {
                    eventStatus.Text = "Zażyty";
                    eventStatus.SetTextColor(Android.Graphics.Color.Green);
                }
                else
                {
                    eventStatus.Text = string.Empty;
                }

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
            if (eventToEdit.ReminderType == ReminderType.Medicine || eventToEdit.ReminderType == ReminderType.Measurement)
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
                //trans.AddToBackStack(null);
                trans.Commit();
            };

            return view;
        }

        private void SelectEvent(int id)
        {
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            eventToEdit = db.Get<Reminder>(id);
        }

        private void DeleteVisit(object sender, EventArgs e)
        {
            //delete from database
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Reminder>();
            db.Delete(eventToEdit);

            //cancel alarm manager
            Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, eventToEdit.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
            AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(pendingIntent);

            //go to list after delete
            var trans = FragmentManager.BeginTransaction();
            trans.Replace(Resource.Id.fragmentContainer, reminderListFragment);
            //trans.AddToBackStack(null);
            trans.Commit();
        }

        private void DeleteSeries(object sender, EventArgs e)
        {
            //selecting events to delete
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Reminder>();
            var events = db.Table<Reminder>().Where(x => x.Title == eventToEdit.Title && x.Description == eventToEdit.Description)
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
            //trans.AddToBackStack(null);
            trans.Commit();
        }

        private void SaveVisit(object sender, EventArgs e)
        {
            eventToEdit.Date = new DateTime(year, month, day, hour, minute, 0);

            if (eventToEdit.ReminderType == ReminderType.Measurement)
            {
                eventToEdit.Title = measurementSpinner.SelectedItem.ToString();
            }
            else if (eventToEdit.ReminderType == ReminderType.Visit)
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
                
                eventToEdit.MinutesBefore = remindMinutesBefore;
            }
            else if (eventToEdit.ReminderType == ReminderType.Medicine)
            {
                eventToEdit.Title = eventTitle.Text;

                if (!int.TryParse(medicineCount.Text, out int count)) //jesli sie nie uda (pole puste)
                {
                    Toast.MakeText(this.Activity, $"Dawka leku nie może być pusta", ToastLength.Short).Show();
                    return;
                }

                if (count < 1)
                {
                    Toast.MakeText(this.Activity, $"Minimalna dawka to 1", ToastLength.Short).Show();
                    return;
                }

                eventToEdit.Count = count;
            }

            eventToEdit.Description = eventDescription.Text;

            

            if (eventToEdit.Title != string.Empty)
            {
                //database connection
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                db.CreateTable<Reminder>();
                db.Update(eventToEdit);

                Toast.MakeText(this.Activity, $"Zapisano", ToastLength.Short).Show();

                if (eventToEdit.Date > DateTime.Now)
                {
                    //Notification
                    Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));

                    if (eventToEdit.ReminderType == ReminderType.Visit)
                    {
                        notificationIntent.PutExtra("title", "Wizyta");
                        notificationIntent.PutExtra("message", $"{eventToEdit.Title}. {eventToEdit.Date.ToString("dd.MM.yyyy HH:mm")}");
                        eventToEdit.Date = eventToEdit.Date.AddMinutes(remindMinutesBefore * (-1)); //change the date to publish notification by chosen minutes before
                    }
                    else if (eventToEdit.ReminderType == ReminderType.Medicine)
                    {
                        notificationIntent.PutExtra("title", "Leki");
                        notificationIntent.PutExtra("message", $"{eventToEdit.Title} dawka: {eventToEdit.Count}. {eventToEdit.Date.ToString("HH:mm")}");
                    }
                    else if (eventToEdit.ReminderType == ReminderType.Measurement)
                    {
                        notificationIntent.PutExtra("title", "Pomiar");
                        notificationIntent.PutExtra("message", $"{eventToEdit.Title}. {eventToEdit.Date.ToString("HH:mm")}");
                        notificationIntent.PutExtra("type", measurementSpinner.SelectedItemPosition);
                    }
                    notificationIntent.PutExtra("id", eventToEdit.Id);

                    var timer = (long)eventToEdit.Date.ToUniversalTime().Subtract(
                        new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                        ).TotalMilliseconds;

                    PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, eventToEdit.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                    AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                    alarmManager.Set(AlarmType.RtcWakeup, timer, pendingIntent);
                }

                //go to list after save
                var trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.fragmentContainer, reminderListFragment);
                //trans.AddToBackStack(null);
                trans.Commit();
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

            this.Activity.Title = "Edycja przypomnienia";

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
            remindMinutesBefore = eventToEdit.MinutesBefore;
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