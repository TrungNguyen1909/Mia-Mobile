using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Mia
{
    public partial class MasterPage : ContentPage
    {
		public ListView ListView { get { return listView; } }

		public MasterPage()
		{
			InitializeComponent();

			var masterPageItems = new List<MasterPageItem>();
			masterPageItems.Add(new MasterPageItem
			{
				Title = "Mia",
                TargetPage= typeof(MiaPage)
			});
            masterPageItems.Add(new MasterPageItem { 
                Title="Reminders",
                TargetPage=typeof(RemindersPage),
                IconSource=ImageSource.FromResource("Mia.Assets.Reminder.png")
            });
            masterPageItems.Add(new MasterPageItem{
                Title="Settings",
                IconSource=ImageSource.FromResource("Mia.Assets.Settings.png"),
                TargetPage=typeof(SettingsPage)
            });
			listView.ItemsSource = masterPageItems;
		}
    }
}
