using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;

namespace AMicroblogAPISample
{
    /// <summary>
    /// Provides methods to resize a specified target.
    /// </summary>
    public class ResizeHelper
    {
        public event EventHandler<ResizableStateChangedEventArgs> ResizableStateChanged;

        private bool _resizeable = false;
        private bool resizeable
        {
            get
            {
                return _resizeable;
            }
            set
            {
                if (value != _resizeable)
                {
                    _resizeable = value;
                }
            }
        }

        private double initialX = 0d;
        private double initialY = 0d;

        public ResizeHelper()
        { }

        public ResizeHelper(FrameworkElement target)
        {
            this.Target = target;
        }

        public FrameworkElement Target
        {
            get;
            set;
        }

        public void DoResize()
        {
            Target.MouseMove -= HandleTargetMouseMove;
            Target.MouseMove += HandleTargetMouseMove;

            Target.MouseLeftButtonUp -= HandleTargetMouseLeftButtonUp;
            Target.MouseLeftButtonUp += HandleTargetMouseLeftButtonUp;

            BeginResize();
        }

        private void HandleTargetMouseMove(object sender, MouseEventArgs e)
        {
            PerformResize();
        }

        private void HandleTargetMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EndResize();
        }

        private void BeginResize()
        {
            var initialPosition = Mouse.GetPosition(Target);

            initialX = initialPosition.X;
            initialY = initialPosition.Y;

            resizeable = true;
        }

        private void PerformResize()
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                resizeable = false;
                return;
            }

            if (resizeable)
            {
                var pos = Mouse.GetPosition(Target);
                var xChange = pos.X - initialX;
                var yChange = pos.Y - initialY;

                var newWidth = Target.Width + xChange;
                var newHeight = Target.Height + yChange;

                if (newWidth > Target.MaxWidth || newHeight > Target.MaxHeight
                    || newWidth < Target.MinWidth || newHeight < Target.MinHeight)
                {
                    return;
                }

                Target.Width += xChange;
                Target.Height += yChange;

                initialX = pos.X;
                initialY = pos.Y;
            }
        }

        private void EndResize()
        {
            resizeable = false;
            Target.MouseMove -= HandleTargetMouseMove;
            Target.MouseLeftButtonUp -= HandleTargetMouseLeftButtonUp;
        }

        private void NotifyResizableChanged(bool resizable)
        {
            if (null != ResizableStateChanged)
            {
                ResizableStateChanged(this, new ResizableStateChangedEventArgs(resizable));
            }
        }

        public class ResizableStateChangedEventArgs : EventArgs
        {
            public ResizableStateChangedEventArgs(bool resizable)
            {
                this.Resizable = resizable;
            }

            public bool Resizable
            {
                get;
                private set;
            }
        }
    }
}
