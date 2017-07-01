using System;
using Xamarin.Forms.Platform.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;
using CoreGraphics;
using KeyboardOverlap.Forms.Plugin.iOSUnified;
using System.Diagnostics;
using Mia;
using XFGloss.iOS.Renderers;

[assembly: ExportRenderer(typeof(MiaPage), typeof(KeyboardOverlapRenderer))]
namespace KeyboardOverlap.Forms.Plugin.iOSUnified
{
	[Preserve(AllMembers = true)]
	public class KeyboardOverlapRenderer : XFGlossContentPageRenderer
	{
		NSObject _keyboardShowObserver;
		NSObject _keyboardHideObserver;
        NSObject _keyboardWillChangeObserver;
		private bool _pageWasShiftedUp;
		private double _activeViewBottom;
		private bool _isKeyboardShown;

		public static void Init()
		{
			var now = DateTime.Now;
			Debug.WriteLine("Keyboard Overlap plugin initialized {0}", now);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			var page = Element as ContentPage;

			if (page != null)
			{
				var contentScrollView = page.Content as ScrollView;

				if (contentScrollView != null)
					return;

				RegisterForKeyboardNotifications();
			}
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			UnregisterForKeyboardNotifications();
		}

		void RegisterForKeyboardNotifications()
		{
            if (_keyboardShowObserver == null)
                _keyboardShowObserver = UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShow);
            if (_keyboardHideObserver == null)
                _keyboardHideObserver = UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHide);
            if (_keyboardWillChangeObserver == null)
                _keyboardWillChangeObserver = UIKeyboard.Notifications.ObserveWillChangeFrame(OnKeyboardChangeFrame);
		}


        void UnregisterForKeyboardNotifications()
		{
			_isKeyboardShown = false;
			if (_keyboardShowObserver != null)
			{
				_keyboardShowObserver.Dispose();
				_keyboardShowObserver = null;
			}

			if (_keyboardHideObserver != null)
			{
				_keyboardHideObserver.Dispose();
				_keyboardHideObserver = null;
			}
            if(_keyboardWillChangeObserver!=null)
            {
                _keyboardWillChangeObserver.Dispose();
                _keyboardWillChangeObserver = null;
            }
		}

        private void OnKeyboardShow(object sender,UIKeyboardEventArgs notification)
		{
			if (!IsViewLoaded || _isKeyboardShown)
				return;

			_isKeyboardShown = true;
			var activeView = View.FindFirstResponder();

			if (activeView == null)
				return;

            var keyboardFrame = notification.FrameEnd;
			var isOverlapping = activeView.IsKeyboardOverlapping(View, keyboardFrame);

			if (!isOverlapping)
				return;

			if (isOverlapping)
			{
				_activeViewBottom = activeView.GetViewRelativeBottom(View);
				ShiftPageUp(keyboardFrame.Height, _activeViewBottom);
			}
		}

        private void OnKeyboardHide(object sender, UIKeyboardEventArgs notification)
		{
			if (!IsViewLoaded)
				return;

			_isKeyboardShown = false;
            var keyboardFrame = notification.FrameEnd;

			if (_pageWasShiftedUp)
			{
				var activeView = View.FindFirstResponder();

				if (activeView == null)
					return;
                _activeViewBottom = activeView.GetViewRelativeBottom(View);
				ShiftPageDown(keyboardFrame.Height, _activeViewBottom);
			}
		}
        private void OnKeyboardChangeFrame(object sender, UIKeyboardEventArgs notification)
        {
            var keyboardFrame = notification.FrameEnd;
            if ((_isKeyboardShown) && (!keyboardFrame.Y.Equals(Element.Bounds.Height)))
            {
				if (!IsViewLoaded)
					return;

				var activeView = View.FindFirstResponder();

				if (activeView == null)
					return;
                var isOverlapping = activeView.IsKeyboardOverlapping(View, keyboardFrame);
                if(isOverlapping)
                {
                    _activeViewBottom = activeView.GetViewRelativeBottom(View);
                    if (keyboardFrame.Height > notification.FrameBegin.Height)
                        ShiftPageUp(keyboardFrame.Height - notification.FrameBegin.Height, Element.Bounds.Height, false);
                    else
                        ShiftPageDown(notification.FrameBegin.Height - keyboardFrame.Height, Element.Bounds.Height, false);
                }
			}
            else return;

        }
		private void ShiftPageUp(nfloat keyboardHeight, double activeViewBottom,bool willResetLater=true)
		{
			var pageFrame = Element.Bounds;

			var newY = pageFrame.Y + CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

			Element.LayoutTo(new Rectangle(pageFrame.X, newY,
				pageFrame.Width, pageFrame.Height));
            if (willResetLater)
                _pageWasShiftedUp = true;
		}

		private void ShiftPageDown(nfloat keyboardHeight, double activeViewBottom,bool willResetLater=true)
		{
			var pageFrame = Element.Bounds;

			var newY = pageFrame.Y - CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

			Element.LayoutTo(new Rectangle(pageFrame.X, newY,
				pageFrame.Width, pageFrame.Height));
            if (willResetLater)
                _pageWasShiftedUp = false;
		}

		private double CalculateShiftByAmount(double pageHeight, nfloat keyboardHeight, double activeViewBottom)
		{
			return (pageHeight - activeViewBottom) - keyboardHeight;
		}
	}
}