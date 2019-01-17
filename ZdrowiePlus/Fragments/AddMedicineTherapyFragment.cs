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
    public class AddMedicineTherapyFragment : Android.App.Fragment
    {
        private static ListRemindersFragment visitListFragment = new ListRemindersFragment();

        EditText medicineName;
        EditText medicineCount;
        SeekBar seekbarFrequency;
        TextView startDate;
        TextView endDate;
        ListView pillTimesListView;
        TextView Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday;

        public DateTime startDay;
        public DateTime endDay;
        public DateTime dateTime;
        public static string pillName;
        public static List<DateTime> pillTimes = new List<DateTime>();
        public static ListViewTimeAdapter timeListAdapter;
        //public List<string> pillTimesString = new List<string>(); //add custom list adapter
        //ArrayAdapter<string> arrayTimeAdapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            dateTime = DateTime.Now;
            startDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            endDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);

            View view = inflater.Inflate(Resource.Layout.AddMedicineTherapy, container, false);

            //pillTimesString.Clear();
            pillTimes.Clear();

            medicineName = view.FindViewById<EditText>(Resource.Id.medicineName);
            medicineCount = view.FindViewById<EditText>(Resource.Id.medicineCount);

            //times list
            //pillTimesString.Add(new DateTime(2000, 12, 12, 8, 0, 0).ToString("HH:mm"));
            pillTimes.Add(new DateTime(2000, 12, 12, 8, 0, 0));
            //pillTimesString.Add(new DateTime(2000, 12, 12, 15, 0, 0).ToString("HH:mm"));
            pillTimes.Add(new DateTime(2000, 12, 12, 15, 0, 0));
            //pillTimesString.Add(new DateTime(2000, 12, 12, 22, 0, 0).ToString("HH:mm"));
            pillTimes.Add(new DateTime(2000, 12, 12, 22, 0, 0));
            pillTimesListView = view.FindViewById<ListView>(Resource.Id.listViewMedicine);
            pillTimesListView.FastScrollEnabled = true;
            //arrayTimeAdapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, pillTimesString);
            timeListAdapter = new ListViewTimeAdapter(this.Activity, pillTimes);
            pillTimesListView.Adapter = timeListAdapter;
            pillTimesListView.ItemClick += PillTimesListView_ItemClick;

            //setting pill frequency per day
            seekbarFrequency = view.FindViewById<SeekBar>(Resource.Id.medicineFrequency);
            TextView labelFrequency = view.FindViewById<TextView>(Resource.Id.labelFrequency);
            seekbarFrequency.Progress = 3;
            labelFrequency.Text = $"Ile razy dziennie:  {seekbarFrequency.Progress}";
            seekbarFrequency.ProgressChanged += (s, e) => {
                if (e.Progress < 1) seekbarFrequency.Progress = 1;
                labelFrequency.Text = $"Ile razy dziennie:  {seekbarFrequency.Progress}";

                //pillTimesString.Clear();
                pillTimes.Clear();

                int delay = 0;
                if (seekbarFrequency.Progress == 2)
                    delay = 720;
                if (seekbarFrequency.Progress > 2)
                    delay = 840 / (seekbarFrequency.Progress - 1);

                for (int i = 0; i < seekbarFrequency.Progress; i++)
                {
                    pillTimes.Add(new DateTime(2000, 12, 12, 8, 0, 0).AddMinutes(delay * i));
                    //pillTimesString.Add(new DateTime(2000, 12, 12, 8, 0, 0).AddMinutes(delay * i).ToString("HH:mm"));
                }
                //arrayTimeAdapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, pillTimesString);
                //pillTimesListView.Adapter = arrayTimeAdapter;
                timeListAdapter.NotifyDataSetChanged();
            };

            //start date choosing
            startDate = view.FindViewById<TextView>(Resource.Id.medicineStartDate);
            startDate.Text = dateTime.ToLongDateString();
            startDate.Click += DateSelect_OnClick;
            startDate.TextChanged += (s, e) => { startDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0); };

            //end date choosing
            endDate = view.FindViewById<TextView>(Resource.Id.medicineEndDate);
            endDate.Text = dateTime.ToLongDateString();
            endDate.Click += DateSelect_OnClick;
            endDate.TextChanged += (s, e) => { endDay = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59); };

            //repeat days choosing
            Monday = view.FindViewById<TextView>(Resource.Id.labelMedicineMonday);
            Monday.Selected = true;
            Monday.Click += ChangeDayStatus;

            Tuesday = view.FindViewById<TextView>(Resource.Id.labelMedicineTuesday);
            Tuesday.Selected = true;
            Tuesday.Click += ChangeDayStatus;

            Wednesday = view.FindViewById<TextView>(Resource.Id.labelMedicineWednesday);
            Wednesday.Selected = true;
            Wednesday.Click += ChangeDayStatus;

            Thursday = view.FindViewById<TextView>(Resource.Id.labelMedicineThursday);
            Thursday.Selected = true;
            Thursday.Click += ChangeDayStatus;

            Friday = view.FindViewById<TextView>(Resource.Id.labelMedicineFriday);
            Friday.Selected = true;
            Friday.Click += ChangeDayStatus;

            Saturday = view.FindViewById<TextView>(Resource.Id.labelMedicineSaturday);
            Saturday.Selected = true;
            Saturday.Click += ChangeDayStatus;

            Sunday = view.FindViewById<TextView>(Resource.Id.labelMedicineSunday);
            Sunday.Selected = true;
            Sunday.Click += ChangeDayStatus;

            //add button
            Button buttonAdd = view.FindViewById<Button>(Resource.Id.buttonAdd);
            buttonAdd.Click += AddMedicineTherapy;

            Button buttonCancel = view.FindViewById<Button>(Resource.Id.buttonCancel);
            buttonCancel.Click += (s, e) =>
            {
                var trans = FragmentManager.BeginTransaction();
                trans.Replace(Resource.Id.fragmentContainer, visitListFragment);
                trans.AddToBackStack(null);
                trans.Commit();
            };

            return view;
        }

        private void ChangeDayStatus(object sender, EventArgs e)
        {
            TextView textView = (TextView)sender;

            if (textView.Selected == true)
            {
                textView.SetTextColor(Android.Graphics.Color.ParseColor("#808080"));
                textView.Selected = false;
                return;
            }
            if (textView.Selected == false)
            {
                textView.SetTextColor(new Android.Graphics.Color(ContextCompat.GetColor(this.Activity, Resource.Color.active_text)));
                textView.Selected = true;
                return;
            }
        }

        private void PillTimesListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                //property hour and minute are read only
                pillTimes[e.Position] = time;
                //pillTimesString[e.Position] = pillTimes[e.Position].ToString("HH:mm");
                //arrayAdapter.NotifyDataSetChanged();
                //arrayTimeAdapter = new ArrayAdapter<string>(this.Activity, Android.Resource.Layout.SimpleListItem1, pillTimesString);
                //pillTimesListView.Adapter = arrayTimeAdapter;
                timeListAdapter.NotifyDataSetChanged();
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        private void AddMedicineTherapy(object sender, EventArgs e)
        {
            if (startDay > endDay)
            {
                Toast.MakeText(this.Activity, $"Data końca nie może być wcześniej nić data początku", ToastLength.Short).Show();
                return;
            }

            string medicineNameString = medicineName.Text.Trim();
            if (medicineNameString == string.Empty)
            {
                Toast.MakeText(this.Activity, $"Nazwa leku nie może być pusta", ToastLength.Short).Show();
                return;
            }

            if (!int.TryParse(medicineCount.Text, out int count)) //jesli sie nie uda (pole puste)
            {
                count = 1;
            }

            if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
            {
                // We have permission

                //database connection
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                db.CreateTable<Event>();

                List<int> weekDays = new List<int>();
                if (Sunday.Selected) weekDays.Add(0);
                if (Monday.Selected) weekDays.Add(1);
                if (Tuesday.Selected) weekDays.Add(2);
                if (Wednesday.Selected) weekDays.Add(3);
                if (Thursday.Selected) weekDays.Add(4);
                if (Friday.Selected) weekDays.Add(5);
                if (Saturday.Selected) weekDays.Add(6);

                if (weekDays.Count == 0)
                {
                    Toast.MakeText(this.Activity, $"Nie wybrano żadnego dnia dygodnia", ToastLength.Short).Show();
                    return;
                }

                int i = 0;

                DateTime eventDay = startDay;

                while (eventDay <= endDay)
                {
                    if (weekDays.Contains((int)eventDay.DayOfWeek))
                    {
                        foreach (var item in pillTimes)
                        {
                            DateTime date = new DateTime(eventDay.Year, eventDay.Month, eventDay.Day, item.Hour, item.Minute, 0);
                            if (date >= DateTime.Now)
                            {
                                var newEvent = new Event();
                                newEvent.Date = date;
                                newEvent.Title = medicineNameString;
                                newEvent.EventType = EventType.Medicine;
                                newEvent.Count = count;
                                db.Insert(newEvent);

                                //Notification
                                Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                                notificationIntent.PutExtra("message", $"{newEvent.Date.ToString("dd.MM.yyyy HH:mm")} {newEvent.Title} dawka: {count}");
                                notificationIntent.PutExtra("title", "Leki");
                                notificationIntent.PutExtra("id", newEvent.Id);

                                var timer = (long)newEvent.Date.ToUniversalTime().Subtract(
                                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                                    ).TotalMilliseconds;

                                PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, newEvent.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                                AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                                alarmManager.Set(AlarmType.RtcWakeup, timer, pendingIntent);

                                //go to list after save
                                var trans = FragmentManager.BeginTransaction();
                                trans.Replace(Resource.Id.fragmentContainer, visitListFragment);
                                trans.AddToBackStack(null);
                                trans.Commit();

                                i++;
                            }
                        }
                    }
                    eventDay = eventDay.AddDays(1);
                }

                if (i == 0)
                {
                    Toast.MakeText(this.Activity, $"Niewłaściwe ustawienia", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this.Activity, $"Dodano {i} przypomień", ToastLength.Short).Show();
                }
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

        void DateSelect_OnClick(object sender, EventArgs e)
        {
            TextView textView = (TextView)sender;
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                dateTime = time;
                textView.Text = time.ToLongDateString();
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG); ;
        }

        public override void OnResume()
        {
            base.OnResume();

            medicineName.Text = string.Empty;
            startDate.Text = dateTime.ToLongDateString();
            startDate.Text = dateTime.ToLongDateString();
        }
    }
}