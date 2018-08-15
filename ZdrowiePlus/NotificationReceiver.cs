using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
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

            // Instantiate the builder and set notification elements:
            Notification.Builder builder = new Notification.Builder(context)
                .SetPriority(2) //NotificationPriority.Max = 2, .Min = -2
                .SetContentTitle(title)
                .SetContentText(message)
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Alarm))
                .SetSmallIcon(Resource.Drawable.outline_info_white_24);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                context.GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            notificationManager.Notify(1, notification);
        }
    }
}