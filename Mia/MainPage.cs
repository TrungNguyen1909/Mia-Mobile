using System;

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
			var bkgrndGradient = new Gradient()
			{
				Rotation = 150,
				Steps = new GradientStepCollection()
				{
                    new GradientStep(Color.FromHex("#2592AA"), 0),
					new GradientStep(Color.FromHex("#5DAFAA"), .5),
					new GradientStep(Color.FromHex("#9DBBB2"), 1)
				}
			};

			ContentPageGloss.SetBackgroundGradient(this, bkgrndGradient);
            Master = masterPage;
            var homepage = new MiaPage();
            NavigationPage.SetHasNavigationBar(homepage, false);
            Detail = new NavigationPage(homepage);
            masterPage.ListView.ItemSelected += OnItemSelected;

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

