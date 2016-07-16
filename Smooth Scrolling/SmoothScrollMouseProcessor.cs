using Microsoft.VisualStudio.Text.Editor;
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
        /// How fast will the smooth scroll decelerate
        /// </summary>
        public double DecelerationSpeed { get; set; }

        /// <summary>
        /// The intensity of the scrolling.
        /// Bigger value means faster scrolling (not smoother)
        /// </summary>
        public double ScrollIntensity { get; set; }

        /// <summary>
        /// Scrolling value below this will not be considered
        /// </summary>
        public double MinimumScrollValue { get; set; }

        /// <summary>
        /// When interrupting, the scrolling speed will be reset to this value
        /// </summary>
        public double InterruptionValue { get; set; }

        /// <summary>
        /// Should we interrupt the scrolling if the user scrolling in a
        /// different direction?
        /// </summary>
        public bool InterruptScrollingWhenInDifferentDirection { get; set; }

        /// <summary>
        /// How much time between engine updates (in miliseconds)
        /// </summary>
        public int UpdateMs { get; set; }
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
            DecelerationSpeed = 10;
            ScrollIntensity = 0.05;
            MinimumScrollValue = 0.01;
            InterruptScrollingWhenInDifferentDirection = true;
            UpdateMs = 10;
            InterruptionValue = 1;

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
            double dist;
            lock (locker)
            {
                currentScrollDistance = Utils.Lerp(currentScrollDistance, 0, deltaTime * DecelerationSpeed);
                dist = currentScrollDistance;
            }

            var isScrollingValueTooLow = Math.Abs(dist) < MinimumScrollValue;
            if (!isScrollingValueTooLow)
            {
                providerThreadDispatcher.Invoke(() => Scroll(dist));
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
                Thread.Sleep(UpdateMs);
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
            var delta = e.Delta;
            lock (locker)
            {
                if (InterruptScrollingWhenInDifferentDirection && ShouldInterrupt(delta))
                {
                    InterruptScrolling();
                }
                else
                {
                    AddScrollingDistance(delta);
                }
            }

            e.Handled = true;
        }

        /// <summary>
        /// Should we interrupt scrolling based on the delta mouse scrollwheel
        /// </summary>
        private bool ShouldInterrupt(int delta)
        {
            if (currentScrollDistance != 0)
            {
                var shouldInterrupt = delta * currentScrollDistance <= 0;
                if (shouldInterrupt)
                {
                    if (currentScrollDistance > 0)
                        shouldInterrupt = currentScrollDistance > InterruptionValue;
                    else
                        shouldInterrupt = currentScrollDistance < -InterruptionValue;

                    return shouldInterrupt;
                }
            }

            return false;
        }

        /// <summary>
        /// Interrupts the scrolling.
        /// </summary>
        private void InterruptScrolling()
        {
            if (currentScrollDistance > 0)
            {
                currentScrollDistance = Math.Min(InterruptionValue, currentScrollDistance);
            }
            else
            {
                currentScrollDistance = Math.Max(-InterruptionValue, currentScrollDistance);
            }
        }

        /// <summary>
        /// Adds the delta from the mouse scrollwheel
        /// </summary>
        /// <param name="delta">The delta from the mouse scrollwheel</param>
        private void AddScrollingDistance(int delta)
        {
            var scrollAdd = delta * ScrollIntensity;
            currentScrollDistance += scrollAdd;
        }
    }
}
