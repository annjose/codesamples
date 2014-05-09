// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace MyXam.iOS
{
	[Register ("CourseDetailsViewController")]
	partial class CourseDetailsViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton buttonNext { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton buttonPrev { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel labelTitle { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (buttonPrev != null) {
				buttonPrev.Dispose ();
				buttonPrev = null;
			}

			if (buttonNext != null) {
				buttonNext.Dispose ();
				buttonNext = null;
			}

			if (labelTitle != null) {
				labelTitle.Dispose ();
				labelTitle = null;
			}
		}
	}
}
