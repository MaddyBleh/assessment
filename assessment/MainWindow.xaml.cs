using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace assessment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    // Programming for Assessment - Madeline.
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        // TextBox validation - if empty, return true, else return false
        public bool isTbxEmpty(TextBox[] checklist)
        {
            bool empty = false;
            for (int i = 0; i < checklist.Length; i++)
            {
                if (checklist[i].Text == "")
                {
                    empty = true;
                }
            }
            return empty;
        }

        private void btnAddInventory_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}