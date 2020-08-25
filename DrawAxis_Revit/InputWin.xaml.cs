using System.Windows;

namespace DrawGrids_Revit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Get data from user
    /// </summary>
    public partial class InputWin : Window
    {
        public InputWin()
        {
            InitializeComponent();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
