using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace SmoothScrolling
{
    public class OptionPageGrid : DialogPage
    {
        private double scrollIntensity = 4;
        private double decelerationSpeed = 6;
        private double minimumScrollValue = 0.1;
        private bool useShiftForPageScrollingForPageScrolling = true;
        private double shiftScrollIntensity = 30;
        private bool pauseWhenPressingCtrl = true;
        private bool interruptScrollingWhenInDifferentDirection = true;
        private int updateMs = 5;
        private bool enabled = true;

        [Category("Smooth Scrolling")]
        [DisplayName("Enabled")]
        [Description("Enable or disable this feature")]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Deceleration Speed")]
        [Description("How fast will the smooth scroll decelerate")]
        public double DecelerationSpeed
        {
            get { return decelerationSpeed; }
            set
            {
                decelerationSpeed = value;
                if (decelerationSpeed <= 0.01)
                    decelerationSpeed = 0.01;
            }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Scroll Intensity")]
        [Description("The intensity of the scrolling.\nBigger value means faster scrolling (not smoother)")]
        public double ScrollIntensity
        {
            get { return scrollIntensity; }
            set
            {
                scrollIntensity = value;
                if (scrollIntensity < 0.01) scrollIntensity = 0.01;
            }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Minimum Scroll Value")]
        [Description("Scrolling values below this will not be considered")]
        public double MinimumScrollValue
        {
            get { return minimumScrollValue; }
            set
            {
                minimumScrollValue = value;
                if (minimumScrollValue < 0.01) minimumScrollValue = 0.01f;
            }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Interrupt Scrolling")]
        [Description("Should we interrupt the scrolling if the user scrolling in a different direction?")]
        public bool InterruptScrollingWhenInDifferentDirection
        {
            get { return interruptScrollingWhenInDifferentDirection; }
            set { interruptScrollingWhenInDifferentDirection = value; }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Use Shift for page scrolling")]
        [Description("When pressing shift will scroll in a different amount")]
        public bool UseShiftForPageScrolling
        {
            get { return useShiftForPageScrollingForPageScrolling; }
            set { useShiftForPageScrollingForPageScrolling = value; }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Shift Scroll Intensity")]
        [Description("When pressing shift will scroll using this value")]
        public double ShiftScrollIntensity
        {
            get { return shiftScrollIntensity; }
            set { shiftScrollIntensity = value; }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Ctrl Pause")]
        [Description("Should we pause smooth scrolling if the user holds ctrl key?")]
        public bool PauseWhenPressingCtrl
        {
            get { return pauseWhenPressingCtrl; }
            set { pauseWhenPressingCtrl = value; }
        }

        [Category("Smooth Scrolling")]
        [DisplayName("Update Milliseconds")]
        [Description("How much time between engine updates (in miliseconds)\nIf you dont know what that means, do not change this value. It might reduce overall visual studio performance")]
        public int UpdateMs
        {
            get { return updateMs; }
            set
            {
                updateMs = value;
                if (updateMs < 1) updateMs = 1;
                if (updateMs > 1000) updateMs = 1000;
            }
        }
    }
}