using System;
using System.Collections.Generic;
using Plugin.Contacts;
using Xamarin.Forms;
using Plugin.Contacts.Abstractions;
using System.Linq;

namespace Mia
{
    public partial class ContactPicker : ContentPage
    {
        public ContactPicker()
        {
            InitializeComponent();
            CrossContacts.Current.PreferContactAggregation = false;
            Contacts=CrossContacts.Current.Contacts.OrderBy(x=>x.DisplayName).ToList();
            Contacts.RemoveAll(x => x == null);
            ContactList.ItemsSource = Contacts;
            ContactList.ItemSelected+= (sender, e) => { OnContactPicked.Invoke(this,e.SelectedItem as Contact);Navigation.PopAsync(true);};

            Searcher.TextChanged+= (sender, e) => 
            { 
                if (String.IsNullOrWhiteSpace(e.NewTextValue)) ContactList.ItemsSource = Contacts;
                else
                {
                    var FilteredContacts = Contacts.Where(x =>
                    {
                        try
                        {
                            return x.DisplayName.ToLowerInvariant().Contains(Searcher.Text.ToLowerInvariant());
                        }
                        catch { return false; }
                    }).ToList();
					ContactList.ItemsSource = FilteredContacts;
                }
            };
        }
        List<Contact> Contacts;
        public event EventHandler<Contact> OnContactPicked;
    }
}
