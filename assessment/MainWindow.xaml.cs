using System.Diagnostics;
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

        // Create empty list of all inventory items
        public List<Dictionary<string, dynamic>> inventory = new List<Dictionary<string, dynamic>>(); // Background list for item information

        // TextBox validation - if empty, return true, else return false
        public bool isTbxEmpty(List<TextBox> checklist) // Takes a list of textbox objects as input
        {
            bool empty = false;
            for (int i = 0; i < checklist.Count; i++)
            {
                if (string.IsNullOrEmpty(checklist[i].Text.Trim())) // Remove 
                {
                    empty = true;
                }
            }
            return empty;
        }

        public void populateListbox(ListBox list)
        {
            list.Items.Clear();
            if (inventory.Count > 0) {
                foreach (var dict in inventory)
                {
                    list.Items.Add($"ID: {dict["ID"]}, Name: {dict["Name"]}, Quantity: {dict["Quantity"]}, Price: {dict["Price"]:N2}"); // Price is 2dp
                }
            }

        }

        // Add item to inventory
        private void btnAddInventory_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            // Are any boxes empty?
            bool emptyCheck = isTbxEmpty([tbxProductID, tbxName, tbxQuantity, tbxPrice]);
            if (emptyCheck) {
                MessageBox.Show("You have an empty box, check for any blank boxes and make sure they're filled.", "Failed to add item - Blank attribute");
                valid = false;
            }

            // Check if the empty box error message has occured
            if (!emptyCheck)
            {
                // Do tbxQuanity and tbxPrice contain any letters? [a-zA-Z]
                try { Convert.ToInt32(tbxQuantity.Text); Convert.ToDouble(tbxPrice.Text); }
                catch
                {
                    MessageBox.Show("Please make sure that both Quantity and Price only contain numbers.\nQuanity must be a whole number, while Price can be a decimal.", "Failed to add item - number error.");
                    valid = false;
                }
            }
            // If everything is correct, add to lbxInventorySP
            if (valid)
            {
                // Add to background list for editting later.
                inventory.Add(new Dictionary<string, dynamic>());
                inventory[lbxInventorySP.Items.Count].Add("DictID", Guid.NewGuid().ToString());       // Unique ID to dictionary. Easier deleting.
                inventory[lbxInventorySP.Items.Count].Add("ID", tbxProductID.Text);                   // Add ID
                inventory[lbxInventorySP.Items.Count].Add("Name", tbxName.Text);                      // Add Name
                inventory[lbxInventorySP.Items.Count].Add("Quantity", int.Parse(tbxQuantity.Text));   // Add Quantity
                inventory[lbxInventorySP.Items.Count].Add("Price", double.Parse(tbxPrice.Text));      // Add Price

                // Add to listbox
                populateListbox(lbxInventorySP);

                // Print all saved information - debugging.
                //foreach (var dict in inventory) {
                //    foreach (var keyValue in dict)
                //        Trace.Write($"{keyValue.Key}: {keyValue.Value} ");
                //    Trace.WriteLine("");
                //}

                // print size of inventory, debug
                Trace.WriteLine($"Amount of items in inventory: {inventory.Count}");
            }
        }
        private void btnClearInv_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = lbxInventorySP.SelectedIndex;
            if (selectedIndex != -1)
            {
                var dictSelected = inventory[selectedIndex];
                var dictID = dictSelected["DictID"].ToString();

                for (int i = 0; i < inventory.Count; i++)
                {
                    if (inventory[i]["DictID"]  == dictID)
                    {
                        inventory.RemoveAt(i);
                    }
                }
            }
            else
            {
                inventory.Clear();
            }
            populateListbox(lbxInventorySP);

            // print size of inventory, debug
            Trace.WriteLine($"Amount of items in inventory: {inventory.Count}");

        } 
    }
}