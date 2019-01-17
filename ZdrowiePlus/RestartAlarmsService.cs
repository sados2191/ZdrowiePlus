using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using SQLite;

namespace ZdrowiePlus
{
    [Service(Name = "com.zdrowieplus.app.RestartAlarmsService", Permission = "android.permission.BIND_JOB_SERVICE", Exported = true)]
    class RestartAlarmsService : JobIntentService
    {
        private static int JOB_ID = 1;

        public static void EnqueueWork(Context context, Intent work)
        {
            EnqueueWork(context, Java.Lang.Class.FromType(typeof(RestartAlarmsService)), JOB_ID, work);
        }

        protected override void OnHandleWork(Intent intent)
        {
            //database connection
            var db = new SQLiteConnection(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "zdrowieplus.db"));
            db.CreateTable<Event>();

            List<Event> eventsToRestart = new List<Event>();
            eventsToRestart = db.Table<Event>().Where(e => e.Date > DateTime.Now).OrderBy(e => e.Date).ToList();

            foreach (var item in eventsToRestart)
            {
                //Notification
                Intent notificationIntent = new Intent(Application.Context, typeof(NotificationReceiver));
                
                if (item.EventType == EventType.Visit)
                {
                    notificationIntent.PutExtra("title", "Wizyta");
                    notificationIntent.PutExtra("message", $"{item.Date.ToString("dd.MM.yyyy HH:mm")} {item.Title}");
                }
                else if (item.EventType == EventType.Medicine)
                {
                    notificationIntent.PutExtra("title", "Leki");
                    notificationIntent.PutExtra("message", $"{item.Date.ToString("dd.MM.yyyy HH:mm")} {item.Title} dawka: {item.Count}");
                }
                else if (item.EventType == EventType.Measurement)
                {
                    notificationIntent.PutExtra("title", "Pomiar");
                    notificationIntent.PutExtra("message", $"{item.Date.ToString("dd.MM.yyyy HH:mm")} {item.Title}");

                    if (item.Title == "Ciśnienie")
                        notificationIntent.PutExtra("type", 0);
                    if (item.Title == "Poziom glukozy")
                        notificationIntent.PutExtra("type", 1);
                    if (item.Title == "Temperatura")
                        notificationIntent.PutExtra("type", 2);
                    if (item.Title == "Tętno")
                        notificationIntent.PutExtra("type", 3);
                    if (item.Title == "Waga")
                        notificationIntent.PutExtra("type", 4);
                }
                notificationIntent.PutExtra("id", item.Id);

                if (item.ReminderMinutesBefore != 0)
                {
                    item.Date = item.Date.AddMinutes(item.ReminderMinutesBefore * (-1));
                }

                var timer = (long)item.Date.ToUniversalTime().Subtract(
                    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    ).TotalMilliseconds;

                PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, item.Id, notificationIntent, PendingIntentFlags.UpdateCurrent);
                AlarmManager alarmManager = (AlarmManager)Application.Context.GetSystemService(Context.AlarmService);
                alarmManager.Set(AlarmType.RtcWakeup, timer, pendingIntent);
            }
        }
    }
}