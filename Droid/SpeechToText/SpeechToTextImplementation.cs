using System;
using Android.App;
using Android.Content;
using Android.Speech;
using Android.Views;
using Mia;
using Mia.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.Linq;
using Plugin.SpeechToText.Abstractions;
using Plugin.SpeechToText;
using Com.Microsoft.Cognitiveservices.Speechrecognition;
using Android.Runtime;

[assembly: Dependency (typeof (SpeechToTextImplementation))]
namespace Plugin.SpeechToText
{
    public class SpeechToTextImplementation : ISpeechToText,ISpeechRecognitionServerEvents
    {
        private MicrophoneRecognitionClient micClient;
        public event EventHandler<string> OnConversationError;
        private bool isRecording = false;

        IntPtr IJavaObject.Handle => throw new NotImplementedException();

        public event EventHandler<EventArgsVoiceRecognition> OnRecognized;
        public void Start()
        {
            string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
            if (rec != "android.hardware.microphone")
            {
                // no microphone, no recording. Disable the button and output an alert

                OnConversationError?.Invoke(this, "MicrophoneUnavailable");
            }
            else
            {
                micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(SpeechRecognitionMode.ShortPhrase, "en-us", this, "f1301065313743309c0a865358712543");
                micClient.StartMicAndRecognition();
                isRecording = true;
            }

        }
        public void Stop()
        {
            if (isRecording)
            {
                micClient.EndMicAndRecognition();
                isRecording = false;
            }
        }

        public void OnAudioEvent(bool p0)
        {
        }

        public void OnError(int p0, string p1)
        {
            OnConversationError?.Invoke(this, p1);
        }

        public void OnFinalResponseReceived(RecognitionResult p0)
        {
            OnRecognized?.Invoke(this, new EventArgsVoiceRecognition(p0.Results.FirstOrDefault().DisplayText, true));
            Stop();
        }

        public void OnIntentReceived(string p0)
        {
        }

        public void OnPartialResponseReceived(string p0)
        {
            OnRecognized?.Invoke(this, new EventArgsVoiceRecognition(p0, false));
        }

        public void Dispose()
        {
        }
    }
}
