using System;
using System.Linq;
using System.Collections.Generic;
using Xamarin.Forms;
using ApiAiSDK;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using Mia.DataServices;
using System.Reflection;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Plugin.TextToSpeech;
using Plugin.LocalNotifications;
using Plugin.Connectivity;
using Plugin.SpeechToText;
using Plugin.SpeechToText.Abstractions;
using Plugin.Contacts;
using Plugin.Contacts.Abstractions;
using Mia.Helpers;
using XFGloss;
using DeviceOrientation.Forms.Plugin.Abstractions;

namespace Mia
{
    public partial class MiaPage : ContentPage
    {
        #region Private field
        private ApiAi lus;
        private bool IsDone = false;
        private Random Choices = new Random();
        private Reminder item = new Reminder();
        private ISpeechToText micClient = CrossSpeechToText.Current;
        private bool isRecording = false;
        private IDeviceOrientation osvc;
        bool isThinking = false;
        TapGestureRecognizer StartSpeakTapRecognizer = new TapGestureRecognizer();
        private string wvp;
        #endregion
        #region Override method
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            osvc = DependencyService.Get<IDeviceOrientation>();
            #region Request Permission
            #region Location
            StartSpeak.Source = ImageSource.FromResource("Mia.Assets.microMute.png");
            var locstatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (locstatus != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await DisplayAlert("Need location", "Gunna need that location", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Location });
                locstatus = results[Permission.Location];
            }
            if (locstatus != PermissionStatus.Granted)
            {
                await DisplayAlert("Location Denied", "Can not continue, try again.", "OK");
            }
            #endregion
            #region Microphone
            var micstatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);
            if (micstatus != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Microphone))
                {
                    await DisplayAlert("Need Microphone access", "Please allow microphone access", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Microphone });
                micstatus = results[Permission.Microphone];
            }
            if (micstatus != PermissionStatus.Granted)
            {
                await DisplayAlert("Microphone Denied", "Can not continue, try again.", "OK");
            }
            #endregion
            #region Contacts
            await CrossContacts.Current.RequestPermission();
            #endregion
            #endregion

            var config = new AIConfiguration("6bb0cdf023e54c868895091294ae2a0e", SupportedLanguage.English);
            lus = new ApiAi(config);

            if (String.IsNullOrWhiteSpace(Settings.UserInfo)&&(Device.RuntimePlatform!=Device.Android))
            {
                await DisplayAlert("Setup", "Please choose your contact info", "OK");
                var MyContactPicker = new ContactPicker();

                MyContactPicker.OnContactPicked += (sender, e) => { Settings.UserInfo = e.Id; ClearResult(); };
                await Navigation.PushAsync(MyContactPicker, true);
            }
            else
                ClearResult();
            StartSpeak.HeightRequest = Request.Height;
            Request.Completed += OnRequestCompleted;
            Request.Focused += OnRequestFocused;
            micClient.OnRecognized += OnMicRecognized;
            micClient.OnConversationError += OnMicConversationError;

            StartSpeakTapRecognizer.Tapped += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Start Speak button tapped");
                if (!isRecording)
                    StartMicAndRecognition();
                else
                    StopMicAndRecognition();
            };
            Request.TextChanged += OnRequestTextChanged;
            StartSpeak.GestureRecognizers.Add(StartSpeakTapRecognizer);
            CrossConnectivity.Current.ConnectivityChanged += OnConnectivityChanged;
            video.PropertyChanged += Video_PropertyChanged;
            MessagingCenter.Subscribe<DeviceOrientationChangeMessage>(this, DeviceOrientationChangeMessage.MessageId, OnDeviceOrientationChanged);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            video.Source = null;
            wvp = null;
            Request.Completed -= OnRequestCompleted;
            Request.Focused -= OnRequestFocused;
            micClient.OnRecognized -= OnMicRecognized;
            micClient.OnConversationError -= OnMicConversationError;
            Request.TextChanged -= OnRequestTextChanged;
            StartSpeak.GestureRecognizers.Remove(StartSpeakTapRecognizer);
            CrossConnectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
            MessagingCenter.Unsubscribe<DeviceOrientationChangeMessage>(this, DeviceOrientationChangeMessage.MessageId);
            video.PropertyChanged -= Video_PropertyChanged;
        }
        #endregion
        #region ResultUI Update
        public async Task WriteResult(string text, bool speech = true, bool write = true)
        {
            if(isThinking)
            {
                Result.Content = new StackLayout();
                isThinking = false;
            }
            if (write)
            {
                StackLayout newContent;
                try
                {
                    newContent = (StackLayout)Result.Content;
                }
                catch
                {
                    newContent = new StackLayout();
                }
                newContent.Children.Add(new Label { Text = text, TextColor = Color.White });
                Result.Content = newContent;
            }
            if (speech)
            {
                var Lang = await CrossTextToSpeech.Current.GetInstalledLanguages();
                var enLang=Lang.FirstOrDefault(a => a.Language == "en_US");
                await Task.Run(() => CrossTextToSpeech.Current.Speak(text, enLang));
            }

        }
        public void WriteResult(Uri imageUrl)
		{
			if (isThinking)
			{
				Result.Content = new StackLayout();
				isThinking = false;
			}
            StackLayout newContent;
            try
            {
                newContent = (StackLayout)Result.Content;
            }
            catch
            {
                newContent = new StackLayout();
            }
            ImageEx image = new ImageEx
            {
                Source = ImageSource.FromUri(imageUrl),
                AspectEx = AspectEx.Uniform

            };
            newContent.Children.Add(image);

            Result.Content = newContent;
        }
        public void WriteResult(ImageSource imagesource)
		{
			if (isThinking)
			{
				Result.Content = new StackLayout();
				isThinking = false;
			}
            StackLayout newContent;
            try
            {
                newContent = (StackLayout)Result.Content;
            }
            catch
            {
                newContent = new StackLayout();
            }
            ImageEx image = new ImageEx
            {
                Source = imagesource,
                AspectEx = AspectEx.Uniform

            };
            newContent.Children.Add(image);
            Result.Content = newContent;
        }
        public void ClearResult()
        {
            string name=String.Empty;
            if (Device.RuntimePlatform != Device.Android)
            {
                CrossContacts.Current.PreferContactAggregation = false;
                name = " " + CrossContacts.Current.LoadContact(Settings.UserInfo).FirstName.Trim();
            }
            var Content = new StackLayout();
            Content.Children.Add(new Label
            {
                Text = "Hi" + name + ", What can I help you today?",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White
            });
            Result.Content = Content;
            video.Source = null;
            wvp = null;
            IsDone = true;
            Request.BackgroundColor = Color.FromHex("#2592AA");
        }
        private void StatusThinking()
        {
            video.Source = null;
            wvp = "";
            Request.Placeholder = "Thinking...";
            var Content = new StackLayout();
            Content.Children.Add(new Label
            {
                Text = "Thinking...",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                TextColor = Color.White
            });
            Result.Content = Content;
            isThinking = true;
        }
        #endregion
        private async void NLP(string sentence, bool speech = true)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

			if (sentence != null && sentence != "")
            {
                #region Pre-Processing
                if (IsDone)
                {
                    Result.Content = new StackLayout();
                    IsDone = false;
                }
                StatusThinking();
                //StopCommandWaiting.Invoke(this, new EventArgs());
                var Received = await lus.TextRequest(sentence);
                #endregion
                if (Received.Result.ActionIncomplete)
                {
                    #region ActionIncomplete
                    await WriteResult(Received.Result.Fulfillment.Speech, speech);
                    if (speech)
                    {
                        StartMicAndRecognition();
                    }
                    return;
                }
                #endregion
                else
                {
                    #region ActionComplete
                    this.IsBusy = true;
                    switch (Received.Result.Action)
                    {
                        #region WebSearch
                        case "web.search":
                            if (Received.Result.Parameters.ContainsKey("q")) Device.OpenUri(new Uri(String.Concat("https://www.google.com/search?q=" + Received.Result.Parameters["q"])));
                            break;
                        #endregion

                        #region Clock
                        case "clock.date":
                            if (Received.Result.Parameters["date"].ToString() != null)
                            {
                                DateTime ClockDate;
                                DateTime.TryParseExact(Received.Result.Parameters["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ClockDate);
                                var dateori = Received.Result.Parameters["dateori"].ToString().ToCharArray().Where(c => char.IsLetterOrDigit(c)).ToString();
                                WriteResult(dateori.ToUpper() + " is " + ClockDate.ToString("D"), speech);
                            }
                            else
                                WriteResult("It's " + DateTime.Now.ToString("D"), speech);
                            break;
                        case "clock.time":
                            WriteResult("It's " + DateTime.Now.ToString("t"), speech);
                            break;
                        #endregion
                        #region Weather
                        case "weather.search":
                            DateTime Now = DateTime.Now;
                            DateTime Converted = DateTime.Now;
                            string location = "autoip";
                            if (Received.Result.Parameters.ContainsKey("geo-city") && Received.Result.Parameters["geo-city"].ToString() != "") { location = Received.Result.Parameters["geo-city"].ToString(); }
                            else { location = "autoip"; }
                            string time = "present";
                            if (Received.Result.Parameters.ContainsKey("date"))
                            {
                                DateTime.TryParseExact(Received.Result.Parameters["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out Converted);
                                if (Now < Converted) time = "future";
                            }
                            if (time == "present")
                            {
                                var data = await Weather.GetCurrentWeather(location);
                                if (data == null)
                                {
                                    WriteResult(Received.Result.Fulfillment.Speech, speech);
                                }
                                else
                                {
                                    if (location == "autoip")
                                        WriteResult("The current weather at current location is " + data["weather"].ToString().ToLower() + " with the temperature of " + data["temp_c"].ToString() + " C.", speech);
                                    else
                                        WriteResult("The current weather in " + location + " is " + data["weather"].ToString().ToLower() + " with the temperature of " + data["temp_c"].ToString() + " C.", speech);
                                    if (osvc.GetOrientation() == DeviceOrientations.Landscape)
                                        video.Source = "Weather/weather_l_" + data["icon"] + ".mp4";
                                    else
                                        video.Source = "Weather/weather_" + data["icon"] + ".mp4";
                                    wvp = data["icon"].ToString();

                                }
                            }
                            if (time == "future")
                            {
                                var data = await Weather.GetForecastWeather(Converted, location);
                                if (data == null)
                                    WriteResult(Received.Result.Fulfillment.Speech, speech);
                                else
                                {
                                    if (location == "autoip")
                                        WriteResult("The weather at current location on " + Converted.ToString("D") + " will be " + data["conditions"].ToString().ToLower() + " with the temperature of " + data["low_c"].ToString() + " C at lowest and " + data["high_c"] + " C at highest.", speech);
                                    else
                                        WriteResult("The weather in " + location + " on " + Converted.ToString("D") + " will be " + data["conditions"].ToString().ToLower() + " with the temperature of " + data["low_c"].ToString() + " C at lowest and " + data["high_c"] + " C at highest.", speech);

									if (osvc.GetOrientation() == DeviceOrientations.Landscape)
										video.Source = "Weather/weather_l_" + data["icon"] + ".mp4";
									else
										video.Source = "Weather/weather_" + data["icon"] + ".mp4";
                                    wvp = data["icon"].ToString();
                                }
                            }
                            break;
                        #endregion

                        #region Reminder
                        case "reminder.add":

                            string summary = Received.Result.Parameters["summary"].ToString();
                            time = Received.Result.Parameters["time"].ToString();
                            DateTime converted = new DateTime();

                            if (time.Contains("/"))
                            {
                                /*if (!time.Contains("T"))
                                    time = time.Substring(time.IndexOf('/') + 1);
                                else */
                                time = time.Remove(time.IndexOf('/'));
                                time = time.Trim('/');
                            }
                            if (time.Contains("T"))
                            {
                                DateTime.TryParseExact(time, "yyyy-MM-dd\\THH:mm:ss\\Z", CultureInfo.InvariantCulture, DateTimeStyles.None, out converted);
                            }
                            else
                            {
                                if (time.Contains("-"))
                                {
                                    DateTime.TryParseExact(time, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out converted);
                                }
                                else
                                    DateTime.TryParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out converted);
                            }
                            while (converted < DateTime.Now) converted = converted.AddDays(1);
                            summary = summary.Trim();
                            item.reminder = summary;
                            item.datetime = converted;
                            item.Notified = false;
                            string cvt;
                            if (converted.Date > DateTime.Now.Date)
                                cvt = converted.ToString("f");
                            else cvt = converted.ToString("t");

                            await WriteResult("I'll remind you to " + summary + " on " + cvt + ". Ready to confirm?", speech);
                            WriteResult(ImageSource.FromResource("Mia.Assets.Reminder.png"));
                            IsDone = false;
                            Request.Text = null;
                            if (speech)
                            {
                                StartMicAndRecognition();
                            }
                            return;
                        case "reminder.confirm":
                            if (item != new Reminder())
                            {
                                int ID = await ReminderManager.SaveReminderAsync(item);
                                item.ID = ID;
                                CrossLocalNotifications.Current.Show("Mia Reminder", item.reminder, item.ID, item.datetime);
                                WriteResult(Received.Result.Fulfillment.Speech, speech);

                                item = new Reminder();

                            }
                            else
                            {
                                WriteResult("I'm not sure what you said.", speech);
                            }
                            IsDone = true;
                            break;
                        case "reminder.cancel":
                            item = new Reminder();
                            WriteResult(Received.Result.Fulfillment.Speech, speech);

                            break;
                        #endregion
                        #region Location
                        case "location.current":
                            string loc = await Location.GetCurrentAddressAsync();
                            if (loc != null)
                            {
                                WriteResult("We are at " + loc + " .", speech);
                            }
                            else
                            {
                                WriteResult(Received.Result.Fulfillment.Speech, speech);
                            }
                            break;
                        #endregion
                        #region History
                        case "history.event":
                            List<Dictionary<string, object>> result = null;
                            DateTime ChosenDate = new DateTime();
                            bool IsToday = true;
                            if (Received.Result.Parameters["date"].ToString() == "")
                            {
                                result = await History.GetHistory(DateTime.Now);
                            }
                            else
                            {
                                DateTime.TryParseExact(Received.Result.Parameters["date"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ChosenDate);
                                result = await History.GetHistory(ChosenDate);
                                IsToday = false;
                            }
                            if (result == null)
                            {
                                WriteResult(Received.Result.Fulfillment.Speech, speech);
                            }
                            else
                            {
                                var Choice = Choices.Next() % result.Count - 1;
                                var picked = result[Choice];
                                if (IsToday)
                                    WriteResult("On this day in " + picked["year"] + ", " + AccentsRemover.RemoveAccents(picked["text"].ToString()), speech);
                                else
                                    WriteResult("On " + ChosenDate.ToString("M") + " in " + picked["year"] + ", " + AccentsRemover.RemoveAccents(picked["text"].ToString()), speech);
                            }
                            break;
                        #endregion
                        #region Knowledge & Dictionary
                        case "dictionary.search":
                            string q = Received.Result.Parameters["q"].ToString();
                            var definition = await Dictionary.Define(q);
                            if (definition == null)
                            {
                                WriteResult(Received.Result.Fulfillment.Speech, speech);
                            }
                            else
                            {
                                var First = definition[0];
                                WriteResult(q + " has a " + First.lexicalCategory + " meaning, " + First.entries[0].senses[0].definitions[0], speech, false);
                                WriteResult(q, false, true);
                                foreach (var entry in definition)
                                {
                                    WriteResult(entry.lexicalCategory, false, true);
                                    foreach (var def in entry.entries[0].senses)
                                    {
                                        WriteResult("-" + def.definitions[0], false, true);
                                    }
                                }
                            }
                            break;
                        case "knowledge.search":

                            var knowledgeresult = await Knowledge.GetKnowledge(Received.Result.Parameters["questionword"] + " " + Received.Result.Parameters["q"], Device.GetNamedSize(NamedSize.Small, typeof(Label)), Result.Width);
                            if (knowledgeresult != null)
                            {

                                try
                                {
                                    WriteResult(knowledgeresult["response"], speech);
                                }
                                catch (KeyNotFoundException knfe) { }
                                try
                                {
                                    //WriteResult(new Uri(knowledgeresult["imageurl"], UriKind.Absolute));
                                }
                                catch (KeyNotFoundException knfe) { }
                                try
                                {
                                    WriteResult(new Uri(knowledgeresult["simpleurl"], UriKind.Absolute));
                                }
                                catch (KeyNotFoundException knfe) { }


                            }
                            else
                            {
                                Device.OpenUri(new Uri(String.Concat("https://www.google.com/search?q=" + Received.Result.Parameters["q"])));
                            }

                            break;
                        #endregion

                        default:
                            WriteResult(Received.Result.Fulfillment.Speech, speech);
                            break;
                    }

                    #endregion
                }
            }
            this.IsBusy = false;
            IsDone = true;
            Request.Placeholder = "Ask me anything";
            StartSpeak.GestureRecognizers.Add(StartSpeakTapRecognizer);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

		}
        #region Mic Controller
        void StartMicAndRecognition()
        {
            Request.Text = null;
            isRecording = true;
            StartSpeak.Source = ImageSource.FromResource("Mia.Assets.microSpeak.png");
            Request.Placeholder = "Listening...";
            Request.IsEnabled = false;
            micClient.Start();
        }
        void StopMicAndRecognition()
        {
            Request.Text = null;
            isRecording = false;
            StartSpeak.Source = ImageSource.FromResource("Mia.Assets.microMute.png");
            Request.Placeholder = "Ask me anything";
            Request.IsEnabled = true;
            micClient.Stop();
        }
        #endregion
        #region Event Handler
        private void OnMicRecognized(object sender, EventArgsVoiceRecognition e)
		{
            Device.BeginInvokeOnMainThread(delegate
            {


                Request.Text = e.RecognizedPhrase;
                if (e.IsFinal)
                {
                    StartSpeak.Source = ImageSource.FromResource("Mia.Assets.microMute.png");
                    Request.Placeholder = "Ask me anything";
                    isRecording = false;
                    Request.IsEnabled = true;
                    NLP(e.RecognizedPhrase, true);
                }
            });
		}
		private void OnMicConversationError(object sender, string e)
		{
            Device.BeginInvokeOnMainThread(delegate
            {
                this.DisplayAlert("Conversation Error", e, "OK");
                Request.Placeholder = "Ask me anything";
                StartSpeak.Source = ImageSource.FromResource("Mia.Assets.microMute.png");
                isRecording = false;
                Request.IsEnabled = true;
            });
		}
		private void OnRequestFocused(object sender, FocusEventArgs e)
		{
			Request.Text = null;
		}
		private async void OnConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
		{
			if (!e.IsConnected)
			{
				await DisplayAlert("Internet connection required!", "Please check your internet connection to continue!", "OK");
				Request.IsEnabled = false;
				StartSpeak.GestureRecognizers.Remove(StartSpeakTapRecognizer);
			}
			else
			{
				Request.IsEnabled = true;
				StartSpeak.GestureRecognizers.Add(StartSpeakTapRecognizer);
			}
		}
		private void OnRequestTextChanged(object sender, TextChangedEventArgs e)
		{
			if ((!isRecording) && (!String.IsNullOrWhiteSpace(e.NewTextValue)))
				StartSpeak.GestureRecognizers.Remove(StartSpeakTapRecognizer);
			else
				StartSpeak.GestureRecognizers.Add(StartSpeakTapRecognizer);
		}
		private void OnDeviceOrientationChanged(DeviceOrientationChangeMessage obj)
		{
            if(!String.IsNullOrWhiteSpace(wvp))
            {
                if(obj.Orientation==DeviceOrientations.Landscape)
                {
                    video.Source = "Weather/weather_l_" + wvp + ".mp4";
                }
                else
                    video.Source = "Weather/weather_" + wvp + ".mp4";
            }
		}

        void Video_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName=="Source")
            {
                if (!String.IsNullOrWhiteSpace(video.Source))
                    Request.BackgroundColor = Color.Transparent;
                else
                    Request.BackgroundColor = Color.FromHex("#2592AA");
                    
            }
        }

        private void OnRequestCompleted(object sender, EventArgs e)
        {
            NLP(Request.Text, false);
        }
        #endregion
    }
}
