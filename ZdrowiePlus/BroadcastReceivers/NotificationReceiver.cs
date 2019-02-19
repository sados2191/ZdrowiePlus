using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
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
            var id = intent.GetIntExtra("id", 0);

            Bitmap largeIcon = largeIcon = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.icon);

            string channelId = "zdrowieplus-111";
            string channelName = "ZdrowiePlus";

            // When user click notification start a activity
            Intent notifyIntent = new Intent(context, typeof(MainActivity));
            if (title == "Pomiar")
            {
                notifyIntent.PutExtra("notification", "measurement");
                int type = intent.GetIntExtra("type", 0);
                notifyIntent.PutExtra("type", type);

                largeIcon = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.pulsometer_icon);
            }
            if (title == "Wizyta")
            {
                notifyIntent.PutExtra("notification", "visit");

                largeIcon = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.doctor_icon);
            }
            if (title == "Leki")
            {
                notifyIntent.PutExtra("notification", "medicine");

                largeIcon = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.medical_pill);
            }
            notifyIntent.PutExtra("id", id);
            // Set the activity to start in a new, empty task
            notifyIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            PendingIntent contentIntent = PendingIntent.GetActivity(context, id, notifyIntent, PendingIntentFlags.UpdateCurrent);

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
                .SetSmallIcon(Resource.Drawable.round_schedule_24)
                .SetLargeIcon(largeIcon)
                .SetVisibility((int)NotificationVisibility.Private);

            // Build the notification:
            Notification notification = builder.Build();

            // Publish the notification:
            notificationManager.Notify(id, notification);
        }
    }
}