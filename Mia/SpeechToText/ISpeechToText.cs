using System;

using Xamarin.Forms;

namespace Plugin.SpeechToText.Abstractions
{
	public interface ISpeechToText
	{
		void Start ();
		void Stop ();
		event EventHandler<EventArgsVoiceRecognition> OnRecognized;
        event EventHandler<string> OnConversationError;
	}
	public class EventArgsVoiceRecognition : EventArgs
	{
		public EventArgsVoiceRecognition(string recognizedphrase, bool isFinal)
		{
			this.RecognizedPhrase = recognizedphrase;
			this.IsFinal = isFinal;
		}
		public string RecognizedPhrase { get; set; }
		public bool IsFinal { get; set; }

	}
}

