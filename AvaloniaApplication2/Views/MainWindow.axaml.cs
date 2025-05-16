using Avalonia.Controls;
using AvaloniaApplication2.ViewModels;

namespace AvaloniaApplication2.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
