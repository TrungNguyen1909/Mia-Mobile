using System;
using Xamarin.Forms;
namespace Mia
{
    public class MasterPageItem
    {
		public string Title { get; set; }

		public ImageSource IconSource { get; set; }

        public Type TargetPage { get; set; }
    }
    
}
