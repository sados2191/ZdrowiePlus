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
                notificationIntent.PutExtra("message", $"{item.Date.ToString("dd.MM.yyyy HH:mm")} {item.Title}");
                if (item.EventType == EventType.Visit)
                {
                    notificationIntent.PutExtra("title", "Wizyta");
                }
                else if (item.EventType == EventType.Medicine)
                {
                    notificationIntent.PutExtra("title", "Leki");
                }
                else if (item.EventType == EventType.Measurement)
                {
                    notificationIntent.PutExtra("title", "Pomiar");
                }
                notificationIntent.PutExtra("id", item.Id);

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