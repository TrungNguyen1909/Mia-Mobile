using Plugin.Contacts.Abstractions;
using System;

namespace Plugin.Contacts
{
    /// <summary>
    /// Cross platform Contacts implemenations
    /// </summary>
    public class CrossContacts
    {
        static IContacts Implementation = Xamarin.Forms.DependencyService.Get<IContacts>();

        /// <summary>
        /// Current settings to use
        /// </summary>
        public static IContacts Current
        {
            get
            {
                if (Implementation == null)
                {
                    throw new NotImplementedException();
                }
                return Implementation;
            }
        }

    }
}
