// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
namespace Mia.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string SettingsKey = "settings_key";
        private static readonly string SettingsDefault = string.Empty;
        private const string UserInfoKey = "UserInfo_key";
        private static readonly string UserInfoDefault = string.Empty;
        private const string ThemeKey = "Theme_key";
        private static readonly string ThemeDefault = "Default";
        #endregion


        public static string GeneralSettings
        {
            get
            {
                return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(SettingsKey, value);

            }
        }
        public static string UserInfo
        {
            get
            {
                return AppSettings.GetValueOrDefault(UserInfoKey, UserInfoDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(UserInfoKey, value);
            }
        }
        public static string Theme
        {
            get
            {
                return AppSettings.GetValueOrDefault(ThemeKey, ThemeDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(ThemeKey, value);
                SettingsChanged.Invoke(null, new System.ComponentModel.PropertyChangedEventArgs("Theme"));
            }
        }
        public static event EventHandler<System.ComponentModel.PropertyChangedEventArgs> SettingsChanged;
    }

}