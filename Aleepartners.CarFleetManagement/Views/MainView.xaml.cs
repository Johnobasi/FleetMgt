using FleetMgt.Models;
using FleetMgt.ViewModels;
using System.Windows;

namespace FleetMgt.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            DataContext  = new MainViewModel();

        }
    }
}
