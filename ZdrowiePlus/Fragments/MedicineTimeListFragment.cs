using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using SQLite;

namespace ZdrowiePlus.Fragments
{
    public class MedicineTimeListFragment : Android.App.Fragment
    {
        private static VisitListFragment visitListFragment = new VisitListFragment();

        public static MedicineTimeViewAdapter medicineTimeAdapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.MedicineTimeList, container, false);

            medicineTimeAdapter = new MedicineTimeViewAdapter(this.Activity, MedicineTherapyFragment.pillTimes);
            ListView medicineTimeListView = view.FindViewById<ListView>(Resource.Id.listViewMedicine);
            medicineTimeListView.Adapter = medicineTimeAdapter;
            medicineTimeListView.FastScrollEnabled = true;

            //next screen button
            Button buttonAdd = view.FindViewById<Button>(Resource.Id.btnAddMedicineTherapy);
            buttonAdd.Click += AddMedicineTherapy;

            medicineTimeListView.ItemClick += MedicineTimeListView_ItemClick;

            return view;
        }

        private void MedicineTimeListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (DateTime time)
            {
                //property hour and minute are read only
                MedicineTherapyFragment.pillTimes[e.Position] = MedicineTherapyFragment.pillTimes[e.Position].AddHours(time.Hour - MedicineTherapyFragment.pillTimes[e.Position].Hour);
                MedicineTherapyFragment.pillTimes[e.Position] = MedicineTherapyFragment.pillTimes[e.Position].AddMinutes(time.Minute - MedicineTherapyFragment.pillTimes[e.Position].Minute);
                medicineTimeAdapter.NotifyDataSetChanged();
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }

        private void AddMedicineTherapy(object sender, EventArgs e)
        {
            List<DateTime> medicineTimes = new List<DateTime>();

            for (int i = 0; i < MedicineTherapyFragment.therapyLength; i++)
            {
                foreach (DateTime date in MedicineTherapyFragment.pillTimes)
                {
                    medicineTimes.Add(date.AddDays(i));
                }
            }

            if (ContextCompat.CheckSelfPermission(this.Activity, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
            {
                // We have permission

                //database connection
                var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
                db.CreateTable<Event>();

                foreach (DateTime date in medicineTimes)
                {
                    if (date >= DateTime.Now)
                    {
                        var newEvent = new Event();
                        newEvent.Date = date;
                        newEvent.Title = MedicineTherapyFragment.pillName;
                        newEvent.EventType = EventType.Medicine;
                        db.Insert(newEvent); //change to GUID

                        //this.Activity.FindViewById<EditText>(Resource.Id.visitDescription).Text = String.Empty;
                        //Toast.MakeText(this.Activity, $"Dodano\n{visitTime.ToString("dd.MM.yyyy HH:mm")}\n{description}", ToastLength.Short).Show();

                        //Notification
                        Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                        notificationIntent.PutExtra("message", $"{date.ToString("dd.MM.yyyy HH:mm")} {MedicineTherapyFragment.pillName}");
                        notificationIntent.PutExtra("title", "Leki");
                        notificationIntent.PutExtra("id", newEvent.Id);

                        var timer = (long)date.ToUniversalTime().Subtract(
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
                    }
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

        public override void OnResume()
        {
            base.OnResume();

            //medicineTimeAdapter.NotifyDataSetChanged();
        }
    }
}