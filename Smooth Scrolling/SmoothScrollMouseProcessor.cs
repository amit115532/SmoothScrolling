using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.VisualStudio.Text;

namespace Smooth_Scrolling
{
    // זאת המחלקה שמגדירה מה לעשות עם הגלגלת
    internal sealed class SmoothScrollMouseProcessor : MouseProcessorBase
    {
        private readonly object locker = new object();
        private const float DECELERATION_SPEED = 1;
        private const float INITIAL_SPEED_FACTOR = 0.05f;

        private readonly Dispatcher dispatcher;

        private const int UPDATE_MS = 10;
        readonly IWpfTextView wpfTextView;

        private float currentScrollDistance;

        internal SmoothScrollMouseProcessor(IWpfTextView wpfTextView)
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            this.wpfTextView = wpfTextView;
            var thread = new Thread(Update);
            thread.Start();
        }

        // זה גם נקרא כל UPDATE_MS כי מי שקורא לו נקרא כל UPDATE_MS
        private void Scroll(float value)
        {
            // מזיז את הSCROLL BAR לפי הVALUE שנתתי למעלה
            wpfTextView.ViewScroller.ScrollViewportVerticallyByPixels(value);
        }

        private static float Lerp(float a, float b, float amount)
        {
            return a + (b - a) * amount;
        }

        // זה נקרא כל UPDATE_MS
        public void SmoothScrollUpdate()
        {
            float dist;
            lock (locker)
            {
                // אני לוקח את כמות הפיקסלים שצריך להזיז את העורך ומחליק אותה ל0
                currentScrollDistance = Lerp(currentScrollDistance, 0, (1 / (float)UPDATE_MS) * DECELERATION_SPEED);
                dist = currentScrollDistance;
            }
            if (Math.Abs(dist) > 0.01f)
            {
                // וקורא לSCROLL
                dispatcher.Invoke(() => Scroll(dist));
            }
            else
            {
                lock (locker)
                {
                    currentScrollDistance = 0;
                }
            }
        }

        // יצרתי thread שמתעדכן כל UPDATE_MS
        private void Update()
        {
            while (true)
            {
                Thread.Sleep(UPDATE_MS);
                SmoothScrollUpdate();
            }
        }

        // זה קורה כל פעם כשהמשתמש מגלגל
        public override void PreprocessMouseWheel(MouseWheelEventArgs e)
        {
            lock (locker)
            {
                if (e.Delta * currentScrollDistance < 0)
                    currentScrollDistance = 0;
                else
                    currentScrollDistance += e.Delta * INITIAL_SPEED_FACTOR;
            }

            e.Handled = true;
        }
    }
}
