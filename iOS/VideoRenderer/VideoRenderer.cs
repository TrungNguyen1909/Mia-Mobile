﻿using System;
using System.IO;
using BackgroundVideo.Controls;
using BackgroundVideo.iOS.Renderers;
using Foundation;
using MediaPlayer;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(Video), typeof(VideoRenderer))]
namespace BackgroundVideo.iOS.Renderers
{
	public class VideoRenderer : ViewRenderer<Video, UIView>
	{
		MPMoviePlayerController videoPlayer;
		NSObject notification = null;

		protected override void OnElementChanged(ElementChangedEventArgs<Video> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
			{
				InitVideoPlayer();
			}
			if (e.OldElement != null)
			{
				// Unsubscribe
				notification?.Dispose();
			}
			if (e.NewElement != null)
			{
				// Subscribe
				notification = MPMoviePlayerController.Notifications.ObservePlaybackDidFinish((sender, args) =>
				{
					/* Access strongly typed args */
					Debug.WriteLine("Notification: {0}", args.Notification);
					Debug.WriteLine("FinishReason: {0}", args.FinishReason);

					Element?.OnFinishedPlaying?.Invoke();
				});
			}
		}

		void InitVideoPlayer()
		{
            string path;
            try
            {
                path = Path.Combine(NSBundle.MainBundle.BundlePath, Element.Source);

				if ((!NSFileManager.DefaultManager.FileExists(path)) || String.IsNullOrWhiteSpace(Element.Source))
				{
                    throw new Exception();
				}
            }
			catch
			{
				Debug.WriteLine("Video not exist");
				videoPlayer = new MPMoviePlayerController();
				videoPlayer.ControlStyle = MPMovieControlStyle.None;
				videoPlayer.ScalingMode = MPMovieScalingMode.AspectFill;
				videoPlayer.RepeatMode = MPMovieRepeatMode.One;
				videoPlayer.View.BackgroundColor = UIColor.Clear;
                videoPlayer.Stop();
				SetNativeControl(videoPlayer.View);
				return;
			}


			// Load the video from the app bundle.
			NSUrl videoURL = new NSUrl(path, false);

			// Create and configure the movie player.
			videoPlayer = new MPMoviePlayerController(videoURL);

			videoPlayer.ControlStyle = MPMovieControlStyle.None;
			videoPlayer.ScalingMode = MPMovieScalingMode.AspectFill;
			videoPlayer.RepeatMode = Element.Loop ? MPMovieRepeatMode.One : MPMovieRepeatMode.None;
			videoPlayer.View.BackgroundColor = UIColor.Clear;
			foreach (UIView subView in videoPlayer.View.Subviews)
			{
				subView.BackgroundColor = UIColor.Clear;
			}

			videoPlayer.PrepareToPlay();
			SetNativeControl(videoPlayer.View);
		}

		protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (Element == null || Control == null)
				return;

			if (e.PropertyName == Video.SourceProperty.PropertyName)
			{
				InitVideoPlayer();
			}
			else if (e.PropertyName == Video.LoopProperty.PropertyName)
			{
				var liveImage = Element as Video;
				if (videoPlayer != null)
					videoPlayer.RepeatMode = Element.Loop ? MPMovieRepeatMode.One : MPMovieRepeatMode.None;
			}
		}
	}
}
