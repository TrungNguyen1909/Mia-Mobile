using System;
using XFGloss;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Reflection;
using System.Linq;

namespace Mia.GradientTheme
{
    public class ThemePreview : ContentPage
    {
        public ThemePreview()
        {
            int c = 0;
            bool validTheme = false;
            for (c = 0; c < Theme.ThemeList.Count();c++)
            {
                if(Theme.ThemeList[c].Name==Helpers.Settings.Theme)
                {
                    validTheme = true;
                    Theme.ThemeList[c].SetBackgroundTheme(this);
                    break;
                }
            }
            if(!validTheme)
            {
                Helpers.Settings.Theme = "Default";
                c = 0;
            }
            Button button = new Button()
            {
                Text = "Change Color",
                TextColor=Color.White
            };
            Label CT = new Label()
            {
                Text = Theme.ThemeList[c].Name,
                HorizontalTextAlignment=TextAlignment.Center,
                TextColor=Color.White
            };
            StackLayout tp = new StackLayout();
            tp.VerticalOptions = LayoutOptions.Center;
            tp.Children.Add(new Label()
            {
                Text="Please\nchoose my color.",
                FontSize=70,
                TextColor=Color.White,
                VerticalOptions=LayoutOptions.FillAndExpand
            });
            tp.Children.Add(CT);
            tp.Children.Add(button);
            Content = tp;
            button.Clicked += delegate
            {
                c++;
                c %= Theme.ThemeList.Count();
                Theme.ThemeList[c].SetBackgroundTheme(this);
                Helpers.Settings.Theme = Theme.ThemeList[c].Name;
                CT.Text = Helpers.Settings.Theme;       

            };
        }
    }
}

