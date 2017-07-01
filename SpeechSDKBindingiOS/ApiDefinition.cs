using System;

using UIKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
using Com.Microsoft.Cognitiveservices.Speechrecognition;

namespace Com.Microsoft.Cognitiveservices.Speechrecognition
{
	// The first step to creating a binding is to add your native library ("libNativeLibrary.a")
	// to the project by right-clicking (or Control-clicking) the folder containing this source
	// file and clicking "Add files..." and then simply select the native library (or libraries)
	// that you want to bind.
	//
	// When you do that, you'll notice that MonoDevelop generates a code-behind file for each
	// native library which will contain a [LinkWith] attribute. MonoDevelop auto-detects the
	// architectures that the native library supports and fills in that information for you,
	// however, it cannot auto-detect any Frameworks or other system libraries that the
	// native library may depend on, so you'll need to fill in that information yourself.
	//
	// Once you've done that, you're ready to move on to binding the API...
	//
	//
	// Here is where you'd define your API definition for the native Objective-C library.
	//
	// For example, to bind the following Objective-C class:
	//
	//     @interface Widget : NSObject {
	//     }
	//
	// The C# binding would look like this:
	//
	//     [BaseType (typeof (NSObject))]
	//     interface Widget {
	//     }
	//
	// To bind Objective-C properties, such as:
	//
	//     @property (nonatomic, readwrite, assign) CGPoint center;
	//
	// You would add a property definition in the C# interface like so:
	//
	//     [Export ("center")]
	//     CGPoint Center { get; set; }
	//
	// To bind an Objective-C method, such as:
	//
	//     -(void) doSomething:(NSObject *)object atIndex:(NSInteger)index;
	//
	// You would add a method definition to the C# interface like so:
	//
	//     [Export ("doSomething:atIndex:")]
	//     void DoSomething (NSObject object, int index);
	//
	// Objective-C "constructors" such as:
	//
	//     -(id)initWithElmo:(ElmoMuppet *)elmo;
	//
	// Can be bound as:
	//
	//     [Export ("initWithElmo:")]
	//     IntPtr Constructor (ElmoMuppet elmo);
	//
	// For more information, see http://developer.xamarin.com/guides/ios/advanced_topics/binding_objective-c/
	//


	// @interface Preferences : NSObject
	[BaseType (typeof (NSObject))]
	interface Preferences
	{
		// @property (copy, nonatomic) NSString * Locale;
		[Export ("Locale")]
		string Locale { get; set; }

		// @property (copy, nonatomic) NSString * ServiceUri;
		[Export ("ServiceUri")]
		string ServiceUri { get; set; }

		// @property (nonatomic, strong) NSNumber * MicrophoneTimeout;
		[Export ("MicrophoneTimeout", ArgumentSemantic.Strong)]
		NSNumber MicrophoneTimeout { get; set; }

		// @property (nonatomic, strong) NSNumber * LoggingLevel;
		[Export ("LoggingLevel", ArgumentSemantic.Strong)]
		NSNumber LoggingLevel { get; set; }
	}

	// @interface AdmRecoOnlyPreferences : Preferences
	[BaseType (typeof (Preferences))]
	interface AdmRecoOnlyPreferences
	{
		// @property (copy, nonatomic) NSString * ClientId;
		[Export ("ClientId")]
		string ClientId { get; set; }

		// @property (copy, nonatomic) NSString * ClientSecret;
		[Export ("ClientSecret")]
		string ClientSecret { get; set; }

		// @property (copy, nonatomic) NSString * LuisAppId;
		[Export ("LuisAppId")]
		string LuisAppId { get; set; }

		// @property (copy, nonatomic) NSString * LuisSubscriptionId;
		[Export ("LuisSubscriptionId")]
		string LuisSubscriptionId { get; set; }
	}

	// @interface RecognizedPhrase : NSObject
	[BaseType (typeof (NSObject))]
	interface RecognizedPhrase
	{
		// @property (nonatomic, strong) NSString * LexicalForm;
		[Export ("LexicalForm", ArgumentSemantic.Strong)]
		string LexicalForm { get; set; }

		// @property (nonatomic, strong) NSString * DisplayText;
		[Export ("DisplayText", ArgumentSemantic.Strong)]
		string DisplayText { get; set; }

		// @property (nonatomic, strong) NSString * InverseTextNormalizationResult;
		[Export ("InverseTextNormalizationResult", ArgumentSemantic.Strong)]
		string InverseTextNormalizationResult { get; set; }

		// @property (nonatomic, strong) NSString * MaskedInverseTextNormalizationResult;
		[Export ("MaskedInverseTextNormalizationResult", ArgumentSemantic.Strong)]
		string MaskedInverseTextNormalizationResult { get; set; }

		// @property (assign, nonatomic) Confidence Confidence;
		[Export ("Confidence", ArgumentSemantic.Assign)]
		Confidence Confidence { get; set; }
	}

	// @interface RecognitionResult : NSObject
	[BaseType (typeof (NSObject))]
	interface RecognitionResult
	{
		// @property (assign, nonatomic) RecognitionStatus RecognitionStatus;
		[Export ("RecognitionStatus", ArgumentSemantic.Assign)]
		RecognitionStatus RecognitionStatus { get; set; }

		// @property (nonatomic, strong) NSArray * RecognizedPhrase;
		[Export ("RecognizedPhrase", ArgumentSemantic.Strong)]
        RecognizedPhrase [] RecognizedPhrase { get; set; }
	}

	// @interface IntentResult : NSObject
	/*[BaseType(typeof(NSObject))]
	interface IntentResult
	{
		// @property (nonatomic, strong) NSURL * Url;
		[Export("Url", ArgumentSemantic.Strong)]
		NSUrl Url { get; set; }

		// @property (nonatomic, strong) NSDictionary * Headers;
		[Export("Headers", ArgumentSemantic.Strong)]
		NSDictionary Headers { get; set; }

		// @property (nonatomic, strong) NSString * Body;
		[Export("Body", ArgumentSemantic.Strong)]
		string Body { get; set; }
	}*/

	// @protocol SpeechRecognitionProtocol
	[Protocol, Model]
	interface SpeechRecognitionProtocol
	{
		// @required -(void)onPartialResponseReceived:(NSString *)partialResult;
		[Abstract]
		[Export ("onPartialResponseReceived:")]
		void OnPartialResponseReceived (string partialResult);

		// @required -(void)onFinalResponseReceived:(RecognitionResult *)result;
		[Abstract]
		[Export ("onFinalResponseReceived:")]
		void OnFinalResponseReceived (RecognitionResult result);

		// @required -(void)onError:(NSString *)errorMessage withErrorCode:(int)errorCode;
		[Abstract]
		[Export ("onError:withErrorCode:")]
		void OnError (string errorMessage, int errorCode);

		// @required -(void)onMicrophoneStatus:(Boolean)recording;
		[Abstract]
		[Export ("onMicrophoneStatus:")]
		void OnMicrophoneStatus (byte recording);

		// @optional -(void)onIntentReceived:(IntentResult *)intent;
		/*[Export("onIntentReceived:")]
		void OnIntentReceived(IntentResult intent);*/
	}

	// @interface SpeechAudioFormat : NSObject
	[BaseType (typeof (NSObject))]
	interface SpeechAudioFormat
	{
		// @property (assign, nonatomic) int AverageBytesPerSecond;
		[Export ("AverageBytesPerSecond")]
		int AverageBytesPerSecond { get; set; }

		// @property (assign, nonatomic) short BitsPerSample;
		[Export ("BitsPerSample")]
		short BitsPerSample { get; set; }

		// @property (assign, nonatomic) short BlockAlign;
		[Export ("BlockAlign")]
		short BlockAlign { get; set; }

		// @property (assign, nonatomic) short ChannelCount;
		[Export ("ChannelCount")]
		short ChannelCount { get; set; }

		// @property (assign, nonatomic) AudioCompressionType EncodingFormat;
		[Export ("EncodingFormat", ArgumentSemantic.Assign)]
		AudioCompressionType EncodingFormat { get; set; }

		// @property (nonatomic, strong) NSData * FormatSpecificData;
		[Export ("FormatSpecificData", ArgumentSemantic.Strong)]
		NSData FormatSpecificData { get; set; }

		// @property (assign, nonatomic) int SamplesPerSecond;
		[Export ("SamplesPerSecond")]
		int SamplesPerSecond { get; set; }

		// +(SpeechAudioFormat *)createSiren7Format:(int)sampleRate;
		[Static]
		[Export ("createSiren7Format:")]
		SpeechAudioFormat CreateSiren7Format (int sampleRate);

		// +(SpeechAudioFormat *)create16BitPCMFormat:(int)sampleRate;
		[Static]
		[Export ("create16BitPCMFormat:")]
		SpeechAudioFormat Create16BitPCMFormat (int sampleRate);
	}

	public interface ISpeechRecognitionProtocol { }
	// @interface Conversation : NSObject <SpeechRecognitionProtocol>
	[BaseType (typeof (NSObject))]
	interface Conversation : ISpeechRecognitionProtocol
	{
		// -(void)initWithPrefs:(Preferences *)prefs withProtocol:(id<SpeechRecognitionProtocol>)delegate;
		[Export ("initWithPrefs:withProtocol:")]
		void InitWithPrefs (Preferences prefs, ISpeechRecognitionProtocol @delegate);

		// -(void)createConversation;
		[Export ("createConversation")]
		void CreateConversation ();

		// -(OSStatus)audioStart;
		[Export ("audioStart")]
		int AudioStart { get; }

		// -(void)audioStop;
		[Export ("audioStop")]
		void AudioStop ();

		// -(void)sendText:(NSString *)textQuery;
		[Export ("sendText:")]
		void SendText (string textQuery);

		// -(void)synthesizeText:(NSString *)text;
		[Export ("synthesizeText:")]
		void SynthesizeText (string text);

		// -(void)synthesizeText:(NSString *)input withMimeType:(NSString *)mimeType;
		[Export ("synthesizeText:withMimeType:")]
		void SynthesizeText (string input, string mimeType);

		// -(void)setLocationLatitude:(double)latitude withLongitude:(double)longitude;
		[Export ("setLocationLatitude:withLongitude:")]
		void SetLocationLatitude (double latitude, double longitude);
	}

	// @interface DataRecognitionClient : Conversation
	[BaseType(typeof(Conversation))]
	interface DataRecognitionClient
	{
		// -(id)initWithSpeechRecoParams:(SpeechRecognitionMode)speechRecognitionMode withPrefs:(AdmRecoOnlyPreferences *)prefs withIntent:(_Bool)wantIntent withProtocol:(id<SpeechRecognitionProtocol>)delegate;
		[Export("initWithSpeechRecoParams:withPrefs:withIntent:withProtocol:")]
		IntPtr Constructor(SpeechRecognitionMode speechRecognitionMode, AdmRecoOnlyPreferences prefs, bool wantIntent, ISpeechRecognitionProtocol @delegate);

		// -(void)sendAudioFormat:(SpeechAudioFormat *)audioFormat;
		[Export("sendAudioFormat:")]
		void SendAudioFormat(SpeechAudioFormat audioFormat);

		// -(void)sendAudio:(NSData *)buffer withLength:(int)actualAudioBytesInBuffer;
		[Export("sendAudio:withLength:")]
		void SendAudio(NSData buffer, int actualAudioBytesInBuffer);

		// -(void)endAudio;
		[Export("endAudio")]
		void EndAudio();

		// -(_Bool)waitForFinalResponse:(int)timeoutInSeconds;
		[Export("waitForFinalResponse:")]
		bool WaitForFinalResponse(int timeoutInSeconds);
	}

	// @interface DataRecognitionClientWithIntent : DataRecognitionClient
	[BaseType(typeof(DataRecognitionClient))]
	interface DataRecognitionClientWithIntent
	{
		// -(id)initWithSpeechRecoParams:(AdmRecoOnlyPreferences *)prefs withProtocol:(id<SpeechRecognitionProtocol>)delegate;
		[Export("initWithSpeechRecoParams:withProtocol:")]
		IntPtr Constructor(AdmRecoOnlyPreferences prefs, ISpeechRecognitionProtocol @delegate);
	}

	// @interface MicrophoneRecognitionClient : Conversation
	[BaseType (typeof (Conversation))]
	interface MicrophoneRecognitionClient
	{
		// -(id)initWithSpeechRecoParams:(SpeechRecognitionMode)speechRecognitionMode withPrefs:(AdmRecoOnlyPreferences *)prefs withIntent:(_Bool)wantIntent withProtocol:(id<SpeechRecognitionProtocol>)delegate;
		[Export ("initWithSpeechRecoParams:withPrefs:withIntent:withProtocol:")]
		IntPtr Constructor (SpeechRecognitionMode speechRecognitionMode, AdmRecoOnlyPreferences prefs, bool wantIntent, ISpeechRecognitionProtocol @delegate);

		// -(OSStatus)startMicAndRecognition;
		[Export ("startMicAndRecognition")]
		int StartMicAndRecognition ();

		// -(void)endMicAndRecognition;
		[Export ("endMicAndRecognition")]
		void EndMicAndRecognition ();

		// -(_Bool)waitForFinalResponse:(int)timeoutInSeconds;
		[Export ("waitForFinalResponse:")]
		bool WaitForFinalResponse (int timeoutInSeconds);
	}

	// @interface MicrophoneRecognitionClientWithIntent : MicrophoneRecognitionClient
	[BaseType (typeof (MicrophoneRecognitionClient))]
	interface MicrophoneRecognitionClientWithIntent
	{
		// -(id)initWithSpeechRecoParams:(AdmRecoOnlyPreferences *)prefs withProtocol:(id<SpeechRecognitionProtocol>)delegate;
		[Export ("initWithSpeechRecoParams:withProtocol:")]
		IntPtr Constructor (AdmRecoOnlyPreferences prefs, ISpeechRecognitionProtocol @delegate);
	}

	// @interface SpeechRecognitionServiceFactory : NSObject
	[BaseType (typeof (NSObject))]
	interface SpeechRecognitionServiceFactory
	{
		// +(NSString *)getAPIVersion;
		[Static]
		[Export ("getAPIVersion")]
		string APIVersion { get; }
		
		// +(DataRecognitionClient *)createDataClient:(SpeechRecognitionMode)speechRecognitionMode withLanguage:(NSString *)language withKey:(NSString *)primaryOrSecondaryKey withProtocol:(id<SpeechRecognitionProtocol>)delegate __attribute__((deprecated("Use alternative method using both primary and secondary.")));
		[Static]
		[Export("createDataClient:withLanguage:withKey:withProtocol:")]
		DataRecognitionClient CreateDataClient(SpeechRecognitionMode speechRecognitionMode, string language, string primaryOrSecondaryKey, ISpeechRecognitionProtocol @delegate);

		// +(DataRecognitionClient *)createDataClient:(SpeechRecognitionMode)speechRecognitionMode withLanguage:(NSString *)language withPrimaryKey:(NSString *)primaryKey withSecondaryKey:(NSString *)secondaryKey withProtocol:(id<SpeechRecognitionProtocol>)delegate;
		[Static]
		[Export("createDataClient:withLanguage:withPrimaryKey:withSecondaryKey:withProtocol:")]
		DataRecognitionClient CreateDataClient(SpeechRecognitionMode speechRecognitionMode, string language, string primaryKey, string secondaryKey, ISpeechRecognitionProtocol @delegate);

		// +(DataRecognitionClient *)createDataClient:(SpeechRecognitionMode)speechRecognitionMode withLanguage:(NSString *)language withKey:(NSString *)primaryOrSecondaryKey withProtocol:(id<SpeechRecognitionProtocol>)delegate withUrl:(NSString *)url __attribute__((deprecated("Use alternative method using both primary and secondary keys.")));
		[Static]
		[Export("createDataClient:withLanguage:withKey:withProtocol:withUrl:")]
		DataRecognitionClient CreateDataClient(SpeechRecognitionMode speechRecognitionMode, string language, string primaryOrSecondaryKey, ISpeechRecognitionProtocol @delegate, string url);

		// +(DataRecognitionClient *)createDataClient:(SpeechRecognitionMode)speechRecognitionMode withLanguage:(NSString *)language withPrimaryKey:(NSString *)primaryKey withSecondaryKey:(NSString *)secondaryKey withProtocol:(id<SpeechRecognitionProtocol>)delegate withUrl:(NSString *)url;
		[Static]
		[Export("createDataClient:withLanguage:withPrimaryKey:withSecondaryKey:withProtocol:withUrl:")]
		DataRecognitionClient CreateDataClient(SpeechRecognitionMode speechRecognitionMode, string language, string primaryKey, string secondaryKey, ISpeechRecognitionProtocol @delegate, string url);

		// +(DataRecognitionClientWithIntent *)createDataClientWithIntent:(NSString *)language withKey:(NSString *)primaryOrSecondaryKey withLUISAppID:(NSString *)luisAppID withLUISSecret:(NSString *)luisSubscriptionID withProtocol:(id<SpeechRecognitionProtocol>)delegate __attribute__((deprecated("Use alternative method using both primary and secondary keys.")));
		[Static]
		[Export("createDataClientWithIntent:withKey:withLUISAppID:withLUISSecret:withProtocol:")]
		DataRecognitionClientWithIntent CreateDataClientWithIntent(string language, string primaryOrSecondaryKey, string luisAppID, string luisSubscriptionID, ISpeechRecognitionProtocol @delegate);

		// +(DataRecognitionClientWithIntent *)createDataClientWithIntent:(NSString *)language withPrimaryKey:(NSString *)primaryKey withSecondaryKey:(NSString *)secondaryKey withLUISAppID:(NSString *)luisAppID withLUISSecret:(NSString *)luisSubscriptionID withProtocol:(id<SpeechRecognitionProtocol>)delegate;
		[Static]
		[Export("createDataClientWithIntent:withPrimaryKey:withSecondaryKey:withLUISAppID:withLUISSecret:withProtocol:")]
		DataRecognitionClientWithIntent CreateDataClientWithIntent(string language, string primaryKey, string secondaryKey, string luisAppID, string luisSubscriptionID, ISpeechRecognitionProtocol @delegate);

		// +(DataRecognitionClientWithIntent *)createDataClientWithIntent:(NSString *)language withKey:(NSString *)primaryOrSecondaryKey withLUISAppID:(NSString *)luisAppID withLUISSecret:(NSString *)luisSubscriptionID withProtocol:(id<SpeechRecognitionProtocol>)delegate withUrl:(NSString *)url __attribute__((deprecated("Use alternative method using both primary and secondary keys.")));
		[Static]
		[Export("createDataClientWithIntent:withKey:withLUISAppID:withLUISSecret:withProtocol:withUrl:")]
		DataRecognitionClientWithIntent CreateDataClientWithIntent(string language, string primaryOrSecondaryKey, string luisAppID, string luisSubscriptionID, ISpeechRecognitionProtocol @delegate, string url);

		// +(DataRecognitionClientWithIntent *)createDataClientWithIntent:(NSString *)language withPrimaryKey:(NSString *)primaryKey withSecondaryKey:(NSString *)secondaryKey withLUISAppID:(NSString *)luisAppID withLUISSecret:(NSString *)luisSubscriptionID withProtocol:(id<SpeechRecognitionProtocol>)delegate withUrl:(NSString *)url;
		[Static]
		[Export("createDataClientWithIntent:withPrimaryKey:withSecondaryKey:withLUISAppID:withLUISSecret:withProtocol:withUrl:")]
		DataRecognitionClientWithIntent CreateDataClientWithIntent(string language, string primaryKey, string secondaryKey, string luisAppID, string luisSubscriptionID, ISpeechRecognitionProtocol @delegate, string url);
				
		// +(MicrophoneRecognitionClient *)createMicrophoneClient:(SpeechRecognitionMode)speechRecognitionMode withLanguage:(NSString *)language withKey:(NSString *)primaryOrSecondaryKey withProtocol:(id<SpeechRecognitionProtocol>)delegate __attribute__((deprecated("Use alternative method using both primary and secondary keys.")));
		[Static]
		[Export ("createMicrophoneClient:withLanguage:withKey:withProtocol:")]
		MicrophoneRecognitionClient CreateMicrophoneClient (SpeechRecognitionMode speechRecognitionMode, string language, string primaryOrSecondaryKey, ISpeechRecognitionProtocol @delegate);

		// +(MicrophoneRecognitionClient *)createMicrophoneClient:(SpeechRecognitionMode)speechRecognitionMode withLanguage:(NSString *)language withPrimaryKey:(NSString *)primaryKey withSecondaryKey:(NSString *)secondaryKey withProtocol:(id<SpeechRecognitionProtocol>)delegate;
		[Static]
		[Export ("createMicrophoneClient:withLanguage:withPrimaryKey:withSecondaryKey:withProtocol:")]
		MicrophoneRecognitionClient CreateMicrophoneClient (SpeechRecognitionMode speechRecognitionMode, string language, string primaryKey, string secondaryKey, ISpeechRecognitionProtocol @delegate);

		// +(MicrophoneRecognitionClient *)createMicrophoneClient:(SpeechRecognitionMode)speechRecognitionMode withLanguage:(NSString *)language withKey:(NSString *)primaryOrSecondaryKey withProtocol:(id<SpeechRecognitionProtocol>)delegate withUrl:(NSString *)url __attribute__((deprecated("Use alternative method using both primary and secondary keys.")));
		[Static]
		[Export ("createMicrophoneClient:withLanguage:withKey:withProtocol:withUrl:")]
		MicrophoneRecognitionClient CreateMicrophoneClient (SpeechRecognitionMode speechRecognitionMode, string language, string primaryOrSecondaryKey, ISpeechRecognitionProtocol @delegate, string url);

		// +(MicrophoneRecognitionClient *)createMicrophoneClient:(SpeechRecognitionMode)speechRecognitionMode withLanguage:(NSString *)language withPrimaryKey:(NSString *)primaryKey withSecondaryKey:(NSString *)secondaryKey withProtocol:(id<SpeechRecognitionProtocol>)delegate withUrl:(NSString *)url;
		[Static]
		[Export ("createMicrophoneClient:withLanguage:withPrimaryKey:withSecondaryKey:withProtocol:withUrl:")]
		MicrophoneRecognitionClient CreateMicrophoneClient (SpeechRecognitionMode speechRecognitionMode, string language, string primaryKey, string secondaryKey, ISpeechRecognitionProtocol @delegate, string url);

		// +(MicrophoneRecognitionClientWithIntent *)createMicrophoneClientWithIntent:(NSString *)language withKey:(NSString *)primaryOrSecondaryKey withLUISAppID:(NSString *)luisAppID withLUISSecret:(NSString *)luisSubscriptionID withProtocol:(id<SpeechRecognitionProtocol>)delegate __attribute__((deprecated("Use alternative method using both primary and secondary keys.")));
		[Static]
		[Export("createMicrophoneClientWithIntent:withKey:withLUISAppID:withLUISSecret:withProtocol:")]
		MicrophoneRecognitionClientWithIntent CreateMicrophoneClientWithIntent(string language, string primaryOrSecondaryKey, string luisAppID, string luisSubscriptionID, ISpeechRecognitionProtocol @delegate);

		// +(MicrophoneRecognitionClientWithIntent *)createMicrophoneClientWithIntent:(NSString *)language withPrimaryKey:(NSString *)primaryKey withSecondaryKey:(NSString *)secondaryKey withLUISAppID:(NSString *)luisAppID withLUISSecret:(NSString *)luisSubscriptionID withProtocol:(id<SpeechRecognitionProtocol>)delegate;
		[Static]
		[Export("createMicrophoneClientWithIntent:withPrimaryKey:withSecondaryKey:withLUISAppID:withLUISSecret:withProtocol:")]
		MicrophoneRecognitionClientWithIntent CreateMicrophoneClientWithIntent(string language, string primaryKey, string secondaryKey, string luisAppID, string luisSubscriptionID, ISpeechRecognitionProtocol @delegate);

		// +(MicrophoneRecognitionClientWithIntent *)createMicrophoneClientWithIntent:(NSString *)language withKey:(NSString *)primaryOrSecondaryKey withLUISAppID:(NSString *)luisAppID withLUISSecret:(NSString *)luisSubscriptionID withProtocol:(id<SpeechRecognitionProtocol>)delegate withUrl:(NSString *)url __attribute__((deprecated("Use alternative method using both primary and secondary keys.")));
		[Static]
		[Export("createMicrophoneClientWithIntent:withKey:withLUISAppID:withLUISSecret:withProtocol:withUrl:")]
		MicrophoneRecognitionClientWithIntent CreateMicrophoneClientWithIntent(string language, string primaryOrSecondaryKey, string luisAppID, string luisSubscriptionID, ISpeechRecognitionProtocol @delegate, string url);

		// +(MicrophoneRecognitionClientWithIntent *)createMicrophoneClientWithIntent:(NSString *)language withPrimaryKey:(NSString *)primaryKey withSecondaryKey:(NSString *)secondaryKey withLUISAppID:(NSString *)luisAppID withLUISSecret:(NSString *)luisSubscriptionID withProtocol:(id<SpeechRecognitionProtocol>)delegate withUrl:(NSString *)url;
		[Static]
		[Export("createMicrophoneClientWithIntent:withPrimaryKey:withSecondaryKey:withLUISAppID:withLUISSecret:withProtocol:withUrl:")]
		MicrophoneRecognitionClientWithIntent CreateMicrophoneClientWithIntent(string language, string primaryKey, string secondaryKey, string luisAppID, string luisSubscriptionID, ISpeechRecognitionProtocol @delegate, string url);
		
	}
}