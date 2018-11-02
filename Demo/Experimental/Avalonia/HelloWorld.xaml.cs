using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HelloWorld
{
    public class HelloWorld : Window
    {
        public HelloWorld()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}