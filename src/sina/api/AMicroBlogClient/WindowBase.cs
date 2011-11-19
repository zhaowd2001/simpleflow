using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Effects;

namespace AMicroblogAPISample
{
    public class WindowBase : Window
    {
        private BlurEffect blurEffect;
        protected WindowBase()
        {
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = null;

           blurEffect =  new BlurEffect() { Radius = 0 };
           this.Effect = blurEffect;

            this.MouseLeftButtonDown += HandleDragMove;
            this.KeyDown += HandleWindKeyDown;
        }

        private void HandleWindKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void HandleDragMove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        public bool EnableBlurEffect
        {
            get { return (bool)GetValue(EnableBlurEffectProperty); }
            set { SetValue(EnableBlurEffectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableBlurEffect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableBlurEffectProperty =
            DependencyProperty.Register("EnableBlurEffect", typeof(bool), typeof(WindowBase), new UIPropertyMetadata(false, HandleBlurEffectPropChanged));

        private static void HandleBlurEffectPropChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
        {
            var winBase = dpo as WindowBase;
            winBase.HandleBlurEffectPropChanged();
        }

        private void HandleBlurEffectPropChanged()
        {
            if (EnableBlurEffect)
            {
                this.Activated += HandleWindowBaseActivated;
                this.Deactivated += HandleWindowBaseDeactivated;
            }
            else
            {
                this.Activated -= HandleWindowBaseActivated;
                this.Deactivated -= HandleWindowBaseDeactivated;
            }
        }

        private void HandleWindowBaseActivated(object sender, EventArgs e)
        {
            UndoBlur();
        }

        private void HandleWindowBaseDeactivated(object sender, EventArgs e)
        {
            DoBlur();
        }

        public void DoBlur(double blurRadius = 2)
        {
            blurEffect.Radius = blurRadius;           
        }

        public void UndoBlur()
        {
            blurEffect.Radius = 0;
        }
    }
}
