using Aleepartners.CarFleetManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Aleepartners.CarFleetManagement.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            var dataManager = new DataManager();
            dataManager.FileChanged += OnFileChanged; // Subscribe to the file changed event
            DataContext = dataManager;
        }
        private void OnFileChanged()
        {
            Dispatcher.Invoke(() =>
            {
                // Optionally, you can refresh the DataGrid or show a message
                MessageBox.Show("Data has been updated!");
            });
        }

    }
}
