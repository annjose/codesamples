using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace MyXam.iOS
{
	public partial class CourseDetailsViewController : UIViewController
	{
		public CourseDetailsViewController () : base ("CourseDetailsViewController", null)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
            buttonPrev.TouchUpInside += delegate
                {
                    labelTitle.Text = "Prev clicked!";
                    textViewDescription.Text = "Detailed description for PREV button. Does this looks good? It is supposed to be split " +
                                                "into multiple lines. Hope it is displayed that way!";
                    imgCourse.Image = UIImage.FromBundle("flower1");
                };
            buttonNext.TouchUpInside += 
                delegate 
                { 
                    labelTitle.Text = "Next clicked!";
                    textViewDescription.Text = "Detailed description for NEXT button. Does this looks good? It is supposed to be split " +
                                                "into multiple lines. Hope it is displayed that way!";
                    imgCourse.Image = UIImage.FromBundle("flower2");
                };
		}
	}
}

