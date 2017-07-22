using System;
using Xamarin.Forms;
using XFGloss;
namespace Mia.GradientTheme
{
    public class Theme
    {
        public Theme(string name,Gradient gradient)
        {
            Name = name;
            Gradient = gradient;
        }
        public string Name { get; internal set; }
        public Gradient Gradient { get; internal set; }
        public static readonly Theme Default = new Theme("Default",
            new Gradient()
            {
                Rotation = 150,
                Steps = new GradientStepCollection()
                {
                    new GradientStep(Color.FromHex("#2592AA"), 0),
                    new GradientStep(Color.FromHex("#5DAFAA"), .67),
                    new GradientStep(Color.FromHex("#9DBBB2"), 1)
                }

            });
        public static readonly Theme Summer = new Theme("Summer",new Gradient()
        {
            Rotation = 150,
            Steps = new GradientStepCollection()
            {
                new GradientStep(Color.FromHex("#22c1c3"),0),
                new GradientStep(Color.FromHex("#fdbb2d"),1)
            }
        });
        public static readonly Theme Dracula =new Theme("Dracula", new Gradient()
		{
			Rotation = 150,
			Steps = new GradientStepCollection()
			{
				new GradientStep(Color.FromHex("#DC2424"),0),
				new GradientStep(Color.FromHex("#4A569D"),1)
			}
        });
		public static readonly Theme Peach = new Theme("Peach",new Gradient()
		{
			Rotation = 150,
			Steps = new GradientStepCollection()
			{
				new GradientStep(Color.FromHex("#ED4264"),0),
				new GradientStep(Color.FromHex("#FFEDBC"),1)
			}
        });
        public static Theme[] ThemeList = { Default, Summer,Dracula,Peach };
        public static void SetBackgroundTheme(BindableObject bindable,Theme theme)
        {
            ContentPageGloss.SetBackgroundGradient(bindable, theme.Gradient);
        }
        public void SetBackgroundTheme(BindableObject bindable)
        {
            ContentPageGloss.SetBackgroundGradient(bindable,this.Gradient);
        }
    }
}
