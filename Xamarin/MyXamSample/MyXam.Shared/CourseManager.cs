using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyXam.Shared
{
    public class CourseManager
    {
        IList<Course> courses;
        int currentIndex = 0;

        public CourseManager()
        {
            courses = new List<Course>()
            {
                new Course(){ Title = "How to be become an outlier", Description = "Detailed course on how you can become an outlier developer", Image = "flower1"},
                new Course(){ Title = "iOS Programming Part 1", Description = "Become an iOS programmer easy and fast", Image = "flower1"},
            };
        }

        public void MoveToPrev()
        {
            if (currentIndex > 0) currentIndex--;
        }
        public void MoveToNext()
        {
            if (currentIndex < courses.Count - 1) currentIndex++;
        }
    }
}
