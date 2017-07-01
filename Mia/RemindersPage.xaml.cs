using System;
using System.Collections.Generic;
using Mia.DataServices;
using Xamarin.Forms;
using Plugin.LocalNotifications;
namespace Mia
{
    public partial class RemindersPage : ContentPage
    {
        public RemindersPage()
        {
            InitializeComponent();
            RemindersList.ItemsSource = ReminderManager.GetReminder();
            RemindersList.IsPullToRefreshEnabled = false;

        }
		public void OnDelete(object sender, EventArgs e)
		{
			var mi = ((MenuItem)sender);
            var TaskID = (int)mi.CommandParameter;
            try
            {
                CrossLocalNotifications.Current.Cancel(TaskID);
            }
            catch{}
            ReminderManager.DeleteReminderAsync(TaskID);

            RemindersList.ItemsSource = ReminderManager.GetReminder();
		}
    }
}
