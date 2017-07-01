using System;
using Xamarin.Forms;

namespace Mia.DataServices
{
	public class ImageEx : Image
	{
        public static readonly BindableProperty AspectExProperty =
            BindableProperty.Create(nameof(AspectEx), typeof(AspectEx), typeof(ImageEx), AspectEx.AspectFit, BindingMode.OneWay, null, OnAspectExPropertyChanged, null, null, null);
                                            
        
		static void OnAspectExPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var aspectEx = (AspectEx)newValue;
			Aspect aspect;
			if (Enum.TryParse(aspectEx.ToString(), out aspect))
			{
				(bindable as ImageEx).Aspect = aspect;
			}
		}


        public AspectEx AspectEx
		{
			get { return (AspectEx)base.GetValue(AspectExProperty); }
			set { base.SetValue(AspectExProperty, value); }
		}

		protected override SizeRequest OnMeasure(Double widthConstraint, Double heightConstraint)
		{
			// the returned sizeRequest contains the dimensions of the image
            SizeRequest sizeRequest = base.OnMeasure(double.PositiveInfinity, double.PositiveInfinity);

			switch (AspectEx)
			{
				case AspectEx.Uniform:
					var innerAspectRatio = sizeRequest.Request.Width / sizeRequest.Request.Height;

					if (Double.IsInfinity(heightConstraint))
					{
						if (Double.IsInfinity(widthConstraint))
						{
							// both destination constraints are infinity
							// use the view's size request dimensions
							widthConstraint = sizeRequest.Request.Width;
							heightConstraint = sizeRequest.Request.Height;
						}
						else
						{
							// destination height constraint is infinity
							heightConstraint = widthConstraint * sizeRequest.Request.Height / sizeRequest.Request.Width;
						}
					}
					else if (Double.IsInfinity(widthConstraint))
					{
						// destination width constraint is infity
						widthConstraint = heightConstraint * sizeRequest.Request.Width / sizeRequest.Request.Height;
					}
					else
					{
						// both of the destination width and height constraints are non-infinity
						var outerAspectRatio = widthConstraint / heightConstraint;

						var resizeFactor = (innerAspectRatio >= outerAspectRatio) ?
							(widthConstraint / sizeRequest.Request.Width) :
							(heightConstraint / sizeRequest.Request.Height);

						widthConstraint = sizeRequest.Request.Width * resizeFactor;
						heightConstraint = sizeRequest.Request.Height * resizeFactor;
					}
                    try
                    {
                        sizeRequest = new SizeRequest(new Size(widthConstraint, heightConstraint));
                    }
                    catch
                    {
                        sizeRequest = new SizeRequest(new Size(sizeRequest.Request.Width / 2, sizeRequest.Request.Height / 2));
                    }
                    break;
				case AspectEx.None:
					sizeRequest = new SizeRequest(new Size(sizeRequest.Request.Width / 2, sizeRequest.Request.Height / 2));
					break;
			}

			return sizeRequest;
		}
	}

	public enum AspectEx
	{
		/// <summary>Scale the image to fit the view. Some parts may be left empty (letter boxing).</summary>
		AspectFit,
		/// <summary>Scale the image to fill the view. Some parts may be clipped in order to fill the view.</summary>
		/// <remarks />
		AspectFill,
		/// <summary>Scale the image so it exactly fill the view. Scaling may not be uniform in X and Y.</summary>
		Fill,
		/// <summary>Scale the image to fill the view while it preserves its native aspect ratio.</summary>
		Uniform,
		/// <summary>The image preserves its original size.</summary>
		None
	}
}
