using System;
using System.Linq;
using Xamarin.Forms;
using XFGloss;
using Xamarin.Forms.Xaml;
[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Mia
{
    public class MainPage : MasterDetailPage
    {
        MasterPage masterPage;
        public MainPage()
        {
            masterPage = new MasterPage();
			var CT = GradientTheme.Theme.ThemeList.Single(f => f.Name == Helpers.Settings.Theme);
			CT.SetBackgroundTheme(this);
            Master = masterPage;
            var homepage = new MiaPage();
            NavigationPage.SetHasNavigationBar(homepage, false);
            Detail = new NavigationPage(homepage);
            masterPage.ListView.ItemSelected += OnItemSelected;
            Helpers.Settings.SettingsChanged+= (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>  {
                if (e.PropertyName == "Theme")
                {
                    CT = GradientTheme.Theme.ThemeList.Single(f => f.Name == Helpers.Settings.Theme);
                    CT.SetBackgroundTheme(this);
                }
            };
        }
		void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			var item = e.SelectedItem as MasterPageItem;
			if (item != null)
			{
                Detail =new NavigationPage((Page)Activator.CreateInstance(item.TargetPage));
				masterPage.ListView.SelectedItem = null;
				IsPresented = false;
			}
		}
    }
}

