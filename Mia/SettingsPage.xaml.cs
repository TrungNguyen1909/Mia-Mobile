using System;
using System.Collections.Generic;
using Plugin.Contacts;
using Plugin.Contacts.Abstractions;
using Xamarin.Forms;
using Mia.Helpers;

namespace Mia
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            GetContactInfo();
            MyInfo.Tapped+= delegate {
				var MyContactPicker = new ContactPicker();

                MyContactPicker.OnContactPicked += (sender, e) => { Settings.UserInfo=e.Id;GetContactInfo(); };
				Navigation.PushAsync(MyContactPicker,true);
            };
            Theme.Text = Settings.Theme;
            MyTheme.Tapped+=delegate {
                var MyThemePicker = new GradientTheme.ThemePreview();
                Navigation.PushAsync(MyThemePicker,true);
            };
            Helpers.Settings.SettingsChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => 
			{
                if (e.PropertyName == "Theme")
                    Theme.Text = Settings.Theme;
			};
        }
        void GetContactInfo()
        {
            if (!String.IsNullOrWhiteSpace(Settings.UserInfo))
            {
                Contact MyContactInfo = CrossContacts.Current.LoadContact(Settings.UserInfo);
                ContactInfo.Text = String.IsNullOrWhiteSpace(MyContactInfo.DisplayName) ? "None" : MyContactInfo.DisplayName;
            }
        }
    }
}
