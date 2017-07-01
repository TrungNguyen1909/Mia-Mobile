using System;
using System.Threading;
using Plugin.LocalNotifications.Abstractions;

namespace Plugin.LocalNotifications
{
	/// <summary>
	/// Access Cross Local Notifictions
	/// </summary>
	public static class CrossLocalNotifications
	{
        private static ILocalNotifications LocalNotifications = Xamarin.Forms.DependencyService.Get<ILocalNotifications>();

		/// <summary>
		/// Gets the current platform specific ILocalNotifications implementation.
		/// </summary>
		public static ILocalNotifications Current
		{
			get
			{
                if (LocalNotifications == null)
                    throw new NotImplementedException();
                else return LocalNotifications;
			}
		}
	}
}
