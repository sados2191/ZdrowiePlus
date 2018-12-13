using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace ZdrowiePlus
{
    [BroadcastReceiver]
    class NotificationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var message = intent.GetStringExtra("message");
            var title = intent.GetStringExtra("title");
            var id = intent.GetIntExtra("id", 1);

            string channelId = "zdrowieplus-111";
            string channelName = "ZdrowiePlus";

            // When user click notification start a activity
            Intent notifyIntent = new Intent(context, typeof(MainActivity));
            if (title == "Pomiar")
            {
                notifyIntent.PutExtra("notification", "measurement");
                int type = intent.GetIntExtra("type", 0);
                notifyIntent.PutExtra("type", type);
            }
            if (title == "Wizyta")
            {
                notifyIntent.PutExtra("notification", "visit");
                notifyIntent.PutExtra("id", id);
            }
            if (title == "Leki")
            {
                notifyIntent.PutExtra("notification", "medicine");
                notifyIntent.PutExtra("id", id);
            }
            // Set the activity to start in a new, empty task
            notifyIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            PendingIntent contentIntent = PendingIntent.GetActivity(context, 0, notifyIntent, PendingIntentFlags.UpdateCurrent);
            // Create the TaskStackBuilder and add the intent, which inflates the back stack
            //Android.App.TaskStackBuilder stackBuilder = Android.App.TaskStackBuilder.Create(context);
            //stackBuilder.AddNextIntentWithParentStack(notifyIntent);
            //PendingIntent contentIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

            // Get the notification manager:
            NotificationManager notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;

            // Check if android API is >= 26 (Oreo 8.0)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                // Creating notification channel
                var channel = new NotificationChannel(channelId, channelName, NotificationImportance.High)
                {
                    Description = "ZdrowiePlus"
                };

                notificationManager.CreateNotificationChannel(channel);
            }

            // Instantiate the builder and set notification elements:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(context, channelId)
                .SetPriority(2) //NotificationPriority.Max = 2, .Min = -2
                .SetContentTitle(title)
                .SetContentText(message)
                .SetColor(Android.Graphics.Color.Blue)
                .SetContentIntent(contentIntent)
                .SetCategory(NotificationCompat.CategoryReminder)
                .SetAutoCancel(true)
                .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                .SetSmallIcon(Resource.Drawable.outline_info_white_24);

            // Build the notification:
            Notification notification = builder.Build();

            // Publish the notification:
            notificationManager.Notify(id, notification);
        }
    }
}