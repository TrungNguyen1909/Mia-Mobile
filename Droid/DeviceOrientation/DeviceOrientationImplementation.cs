using DeviceOrientation.Forms.Plugin.Abstractions;
using System;
using Xamarin.Forms;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Runtime;
using Android.App;

using DeviceOrientation.Forms.Plugin.Droid;
using Android.Hardware;

[assembly: Dependency(typeof(DeviceOrientationImplementation))]
namespace DeviceOrientation.Forms.Plugin.Droid
{
    /// <summary>
    /// DeviceOrientation Implementation
    /// </summary>
    public class DeviceOrientationImplementation : IDeviceOrientation
    {
      
        /// <summary>
        /// Used for registration with dependency service
        /// </summary>
        public static void Init() 
        { 
        }
        
        /// <summary>
        /// Send orientation change message through MessagingCenter
        /// </summary>
        /// <param name="newConfig">New configuration</param>
        public static void NotifyOrientationChange(global::Android.Content.Res.Configuration newConfig)
        {
            bool isLandscape = newConfig.Orientation == global::Android.Content.Res.Orientation.Landscape;
            var msg = new DeviceOrientationChangeMessage()
            {
                Orientation = isLandscape ? DeviceOrientations.Landscape : DeviceOrientations.Portrait
            };
            MessagingCenter.Send<DeviceOrientationChangeMessage>(msg, DeviceOrientationChangeMessage.MessageId);           
        }
            
        #region IDeviceOrientation implementation

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <returns>The orientation.</returns>
        public DeviceOrientations GetOrientation()
        {
            IWindowManager windowManager = Android.App.Application.Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

            var rotation = windowManager.DefaultDisplay.Rotation;
            bool isLandscape = rotation == SurfaceOrientation.Rotation90 || rotation == SurfaceOrientation.Rotation270;
            return isLandscape ? DeviceOrientations.Landscape : DeviceOrientations.Portrait;
        }

        #endregion
    }
}
