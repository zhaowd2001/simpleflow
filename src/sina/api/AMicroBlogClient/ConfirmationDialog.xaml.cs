using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AMicroblogAPISample
{
    /// <summary>
    /// ConfirmationDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConfirmationDialog : WindowBase
    {
        private double? requiredLocationX;
        private double? requiredLocationY;
        public ConfirmationDialog()
        {
            InitializeComponent();

            this.container.DataContext = this;
        }

        public ConfirmationDialog(double requiredLocationX, double requiredLocationY) : this()
        {
            this.requiredLocationX = requiredLocationX;
            this.requiredLocationY = requiredLocationY;
            this.Loaded += HandleConfirmationDialogLoaded;
        }

        private void HandleConfirmationDialogLoaded(object sender, RoutedEventArgs e)
        {
            if (requiredLocationX.HasValue)
                this.Left = requiredLocationX.Value - this.ActualWidth / 2;

            if (requiredLocationY.HasValue)
                this.Top = requiredLocationY.Value - this.ActualHeight - 20;
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(ConfirmationDialog), new UIPropertyMetadata(null));

        public string ConfirmButtonText
        {
            get { return (string)GetValue(ConfirmButtonTextProperty); }
            set { SetValue(ConfirmButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConfirmButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfirmButtonTextProperty =
            DependencyProperty.Register("ConfirmButtonText", typeof(string), typeof(ConfirmationDialog), new UIPropertyMetadata(null));

        public string CancelButtonText
        {
            get { return (string)GetValue(CancelButtonTextProperty); }
            set { SetValue(CancelButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CancelButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelButtonTextProperty =
            DependencyProperty.Register("CancelButtonText", typeof(string), typeof(ConfirmationDialog), new UIPropertyMetadata(null));

        private void HandleConfirmBtnClicked(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
