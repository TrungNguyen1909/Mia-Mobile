using System;
using Android.App;
using Android.Content;
using Mia.DataServices;
using Plugin.LocalNotifications;

namespace Mia.Droid.Services
{
	[BroadcastReceiver]
	[IntentFilter(new[] { Intent.ActionBootCompleted })]
    public class ReminderLoader:BroadcastReceiver
    {
		public override void OnReceive(Context context, Intent intent)
		{
            var Reminders = ReminderManager.GetReminder();
            foreach(var item in Reminders)
            {
                CrossLocalNotifications.Current.Show("Mia Reminder",item.reminder,item.ID,item.datetime);
            }
		}
    }
}
