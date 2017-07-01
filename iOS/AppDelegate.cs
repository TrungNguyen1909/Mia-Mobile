using System;
using System.Collections.Generic;
using System.Linq;
using DeviceOrientation.Forms.Plugin.iOS;
using Foundation;
using Speech;
using UIKit;
using UserNotifications;

namespace Mia.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				// Ask the user for permission to get notifications on iOS 10.0+
				UNUserNotificationCenter.Current.RequestAuthorization(
						UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
						(approved, error) => { });
			}
			else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				// Ask the user for permission to get notifications on iOS 8.0+
				var settings = UIUserNotificationSettings.GetSettingsForTypes(
						UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
						new NSSet());

				UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
			}

            global::Xamarin.Forms.Forms.Init();
			KeyboardOverlap.Forms.Plugin.iOSUnified.KeyboardOverlapRenderer.Init();
			XFGloss.iOS.Library.Init();
            DeviceOrientationImplementation.Init();
            LoadApplication(new App());
			
            return base.FinishedLaunching(app, options);
        }
    }
}
