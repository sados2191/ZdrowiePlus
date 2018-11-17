using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ZdrowiePlus
{
    [BroadcastReceiver(Name = "com.zdrowieplus.app.BootBroadcastReceiver", Enabled = true)]
    [IntentFilter(actions: new [] { Intent.ActionBootCompleted } )]
    class BootBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Intent restartAlarmsIntent = new Intent(context, typeof(RestartAlarmsService));

            RestartAlarmsService.EnqueueWork(context, restartAlarmsIntent);
        }
    }
}