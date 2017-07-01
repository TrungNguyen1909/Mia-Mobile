using System;
using Plugin.SpeechToText.Abstractions;
namespace Plugin.SpeechToText
{
    public static class CrossSpeechToText
    {
        private static ISpeechToText SpeechToText = Xamarin.Forms.DependencyService.Get<ISpeechToText>();
        public static ISpeechToText Current
        {
            get
            {
                if (SpeechToText == null)
                    throw new NotImplementedException();
                else
                    return SpeechToText;

            }
        }
    }
}
