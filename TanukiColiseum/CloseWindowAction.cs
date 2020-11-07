using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace TanukiColiseum
{
    public class CloseWindowAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
            => Window.GetWindow(AssociatedObject).Close();
    }
}
