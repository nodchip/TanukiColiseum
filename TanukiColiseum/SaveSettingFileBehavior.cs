using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;

namespace TanukiColiseum
{
    public class SaveSettingFileBehavior : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closed += WindowClosed;
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            (AssociatedObject.DataContext as MainViewModel)?.SaveSettingFile();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closed -= WindowClosed;
        }
    }
}
