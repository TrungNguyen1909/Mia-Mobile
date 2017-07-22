using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Mia
{
    public partial class MasterPage : ContentPage
    {
		public ListView ListView { get { return listView; } }

		public MasterPage()
		{
			InitializeComponent();
			var CT = GradientTheme.Theme.ThemeList.Single(f => f.Name == Helpers.Settings.Theme);
			CT.SetBackgroundTheme(this);
			var masterPageItems = new List<MasterPageItem>();
            listView.BackgroundColor= CT.Gradient.Steps.SingleOrDefault(f => Math.Abs(f.StepPercentage) < 1e-9).StepColor;
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
            Helpers.Settings.SettingsChanged+= (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                if(e.PropertyName=="Theme")
                {
					CT = GradientTheme.Theme.ThemeList.Single(f => f.Name == Helpers.Settings.Theme);
					CT.SetBackgroundTheme(this);
					listView.BackgroundColor = CT.Gradient.Steps.SingleOrDefault(f => Math.Abs(f.StepPercentage) < 1e-9).StepColor;
                }
            };
		}
    }
}
