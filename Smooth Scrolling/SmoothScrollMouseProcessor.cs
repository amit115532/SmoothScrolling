using Microsoft.VisualStudio.Text.Editor;
using SmoothScrolling;
using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;

namespace SmoothScrollingExtension
{
    /// <summary>
    /// Inheriting MouseProcessorBase in order to override the normal behaviour
    /// of the mouse scrollwheel
    /// </summary>
    internal sealed class SmoothScrollMouseProcessor : MouseProcessorBase
    {
        /// <summary>
        /// For thread syncronization
        /// </summary>
        private readonly object locker = new object();

        /// <summary>
        /// Used for scrolling the text editor
        /// </summary>
        private readonly IWpfTextView wpfTextView;

        /// <summary>
        /// The current scroll distance, used for decelerating
        /// </summary>
        private double currentScrollDistance;

        /// <summary>
        /// The current time in seconds
        /// </summary>
        private double time;
        /// <summary>
        /// The time the last update took
        /// </summary>
        private double deltaTime;

        /// <summary>
        /// A dispatcher that is owned by the main thread
        /// </summary>
        private readonly Dispatcher providerThreadDispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothScrollMouseProcessor"/> class.
        /// </summary>
        /// <param name="wpfTextView">The WPF text view.</param>
        internal SmoothScrollMouseProcessor(IWpfTextView wpfTextView)
        {
            providerThreadDispatcher = Dispatcher.CurrentDispatcher;
            this.wpfTextView = wpfTextView;
            var thread = new Thread(Engine);
            thread.Start();
        }

        /// <summary>
        /// Scrolls the screen by the specified value.
        /// This method must be invoked from the thread of the provider
        /// </summary>
        /// <param name="value">The how much to scroll (in pixels)</param>
        private void Scroll(double value)
        {
            wpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(value);
        }

        /// <summary>
        /// This is called every <see cref="UpdateMs"/>
        /// </summary>
        public void Update()
        {
            if (!SmoothScrollingPackage.Options.Enabled)
                return;

            double dist;
            lock (locker)
            {
                currentScrollDistance = Utils.Lerp(currentScrollDistance, 0, deltaTime * SmoothScrollingPackage.Options.DecelerationSpeed);
                dist = currentScrollDistance;
            }

            var isScrollingValueTooLow = Math.Abs(dist) < SmoothScrollingPackage.Options.MinimumScrollValue;
            if (!isScrollingValueTooLow)
            {
                providerThreadDispatcher.Invoke(() => Scroll(dist * deltaTime));
            }
            else
            {
                currentScrollDistance = 0;
            }
        }

        /// <summary>
        /// Engine is used to call <see cref="Update"/> every <see cref="UpdateMs"/>
        /// </summary>
        private void Engine()
        {
            time = Utils.GetCurrentTime();

            while (true)
            {
                Thread.Sleep(SmoothScrollingPackage.Options.UpdateMs);
                var currentTime = Utils.GetCurrentTime();
                deltaTime = currentTime - time;
                time = currentTime;
                Update();
            }
        }

        /// <summary>
        /// Handles the mouse wheel event before the default handler.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseWheelEventArgs" /> event arguments.</param>
        public override void PreprocessMouseWheel(MouseWheelEventArgs e)
        {
            if (!SmoothScrollingPackage.Options.Enabled)
                return;

            var intensity = SmoothScrollingPackage.Options.ScrollIntensity;

            if (SmoothScrollingPackage.Options.PauseWhenPressingCtrl)
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    return;
                }
            }
            if (SmoothScrollingPackage.Options.UseShiftForPageScrolling)
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) ||
                    Keyboard.IsKeyDown(Key.RightShift))
                {
                    intensity = SmoothScrollingPackage.Options.ShiftScrollIntensity;
                }
            }

            var delta = e.Delta;
            lock (locker)
            {
                if (SmoothScrollingPackage.Options.InterruptScrollingWhenInDifferentDirection && ShouldInterrupt(delta))
                {
                    InterruptScrolling();
                }
                else
                {
                    AddScrollingDistance(delta, intensity);
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Should we interrupt scrolling based on the delta mouse scrollwheel
        /// </summary>
        private bool ShouldInterrupt(int delta)
        {
            if (Math.Abs(currentScrollDistance) < SmoothScrollingPackage.Options.MinimumScrollValue)
            {
                return false;
            }

            var isDifferentSide = delta * currentScrollDistance < 0;
            return isDifferentSide;
        }

        /// <summary>
        /// Interrupts the scrolling.
        /// </summary>
        private void InterruptScrolling()
        {
            currentScrollDistance = 0;
        }

        /// <summary>
        /// Adds the delta from the mouse scrollwheel
        /// </summary>
        /// <param name="delta">The delta from the mouse scrollwheel</param>
        private void AddScrollingDistance(int delta, double intensity)
        {
            var scrollAdd = delta * intensity;
            currentScrollDistance += scrollAdd;
        }
    }
}
