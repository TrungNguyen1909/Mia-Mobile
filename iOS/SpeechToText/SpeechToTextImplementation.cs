using System;
using AVFoundation;
using Foundation;
using UIKit;
using Mia.iOS;
using Mia;
using Plugin.SpeechToText;
using Plugin.SpeechToText.Abstractions;
using Com.Microsoft.Cognitiveservices.Speechrecognition;
using System.Diagnostics;
[assembly: Xamarin.Forms.Dependency(typeof(SpeechToTextImplementation))]
namespace Plugin.SpeechToText
{
    public class SpeechToTextImplementation : NSObject,ISpeechToText,ISpeechRecognitionProtocol
    {
        public event EventHandler<EventArgsVoiceRecognition> OnRecognized;
        public event EventHandler<string> OnConversationError;
        private AVAudioSession AudioSession=AVAudioSession.SharedInstance();
        private MicrophoneRecognitionClient micClient;
        public void InitializeProperties()
        {
            AudioSession.SetCategory(AVAudioSessionCategory.Record);
			var v = SpeechRecognitionServiceFactory.APIVersion;
			micClient = SpeechRecognitionServiceFactory.
                                            CreateMicrophoneClient(SpeechRecognitionMode.ShortPhrase, "en-us", "f1301065313743309c0a865358712543",this);
            UIApplication.SharedApplication.InvokeOnMainThread(() => micClient.StartMicAndRecognition());
        }

        public void Start()
        {
            InitializeProperties();
        }
        public void Stop()
        {
            StopRecording();
        }

        public void StopRecording()
        {
            micClient.EndMicAndRecognition();
            AudioSession.SetCategory(AVAudioSessionCategory.Playback);
        }
		public void OnError(string errorMessage, int errorCode)
		{
            OnConversationError?.Invoke(this, errorMessage);
		}

		public void OnFinalResponseReceived(RecognitionResult result)
		{
            OnRecognized?.Invoke(this,new EventArgsVoiceRecognition (result.RecognizedPhrase[0].DisplayText,true));
            Stop();
		}

        public void OnMicrophoneStatus (byte recording)
        {
            Debug.WriteLine ("MICROPHONE STATUS: " + recording);
        }

		public void OnPartialResponseReceived(string partialResult)
		{
            OnRecognized?.Invoke(this, new EventArgsVoiceRecognition(partialResult, false));
		}

    }
}
