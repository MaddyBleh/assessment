using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
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
using Microsoft.Win32;

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
                // Remove leading and trailing white spaces. Stops strings only consisting of spaces from being skipped.
                if (string.IsNullOrEmpty(checklist[i].Text.Trim()))
                {
                    empty = true;
                }
            }
            return empty;
        }

        // Fill listboxes
        public void populateListbox(ListBox list)
        {
            // Refresh
            list.Items.Clear();

            // If there is 1 or more item in inventory
            if (inventory.Count > 0)
            {

                // Loop through each item in inventory
                foreach (var dict in inventory)
                {
                    // Display in listbox
                    list.Items.Add($"ID: {dict["ID"]}, Name: {dict["Name"]}, Quantity: {dict["Quantity"]}, Price: ${dict["Price"]:N2}"); // Price is 2dp
                }
            }
        }

        // View selected item's details in textboxes.
        public void updateItem(ListBox list, List<TextBox> textbox)
        {
            // Find the chosen dictionary
            int selectedIndex = lbxInventorySP.SelectedIndex;

            // If an item is actually selected
            if (list.SelectedIndex != -1)
            {

                // Set the selected dictionary
                var selectedDict = inventory[selectedIndex];
                // Update as needed
                textbox[0].Text = selectedDict["ID"];
                textbox[1].Text = selectedDict["Name"];
                textbox[2].Text = selectedDict["Quantity"].ToString();
                textbox[3].Text = selectedDict["Price"].ToString();
            }
        }

        // Save to .csv file
        public void saveCsvFile()
        {
            bool canSave = true;
            if (inventory.Count == 0)
            {
                MessageBox.Show("You appear to have an empty inventory list, so this file can not be saved.", "Empty Inventory!");
                canSave = false;
            }

            if (canSave)
            {
                // Set up file saving dialog box
                SaveFileDialog savetocsv = new SaveFileDialog();
                savetocsv.Title = "Save your file";
                savetocsv.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                savetocsv.DefaultExt = "csv";

                if (savetocsv.ShowDialog() == true)
                {
                    // File will be saved here
                    string filePath = savetocsv.FileName;
                    MessageBox.Show($"Your inventory list will been saved to the following location\n{filePath}", "Saving file");

                    if (lbxInventorySP.SelectedIndex != -1)
                    {
                        // Chose the selected item
                        var selectedItem = lbxInventorySP.SelectedIndex;
                        var selectedDict = inventory[selectedItem];

                        try
                        {
                            // Begin writing file - only using the item chosen
                            using (var writer = new StreamWriter(filePath))
                            {
                                writer.WriteLine("ID,Name,Quantity,Price");
                                writer.WriteLine($"{selectedDict["ID"]},{selectedDict["Name"]},{selectedDict["Quantity"]},{selectedDict["Price"]}");
                            }

                            MessageBox.Show("File saved successfully!", "File saved");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occured: {ex}");
                        }

                    }
                    else
                    {
                        try
                        {
                            // Begin writing file - whole file
                            using (var writer = new StreamWriter(filePath))
                            {
                                writer.WriteLine("ID,Name,Quantity,Price");
                                foreach (var item in inventory)
                                {
                                    writer.WriteLine($"{item["ID"]},{item["Name"]},{item["Quantity"]},{item["Price"]}");
                                }
                            }

                            MessageBox.Show("File saved successfully!", "File saved");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occured: {ex}");
                        }
                    }
                }

            }
        }

        // Load from .csv file
        public void loadCsvFile()
        {
            // Set up dialog box
            OpenFileDialog loadfromcsv = new OpenFileDialog();
            loadfromcsv.Title = "Load your file";
            loadfromcsv.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            loadfromcsv.FilterIndex = 0;

            // If a dialog box appears
            if (loadfromcsv.ShowDialog() == true)
            {
                // Get file name
                string filePath = loadfromcsv.FileName;

                try
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        // Read headings
                        string header = reader.ReadLine();
                        var headers = header.Split(',');

                        // If the file still has content
                        while (!reader.EndOfStream)
                        {
                            // Read the line, and get each attribute (ID, Name, Quantity, Price)
                            string line = reader.ReadLine();
                            var details = line.Split(',');

                            // Create item dictionary - will be added to inventory
                            Dictionary<string, dynamic> item = new Dictionary<string, dynamic>();
                            
                            // This runs 4 times - one per header (ID, Name, Quantity, Price)
                            for (int i = 0; i < headers.Length; i++)
                            {
                                // If in Quantity column, turn data into an int
                                if (headers[i] == "Quantity")
                                {
                                    item.Add(headers[i], int.Parse(details[i]));
                                }
                                // If in Price column, turn data into a double
                                else if (headers[i] == "Price")
                                {
                                    item.Add(headers[i], double.Parse(details[i]));
                                }
                                // If in ID or Name column, turn into string
                                else
                                {
                                    item.Add(headers[i], details[i]);
                                }
                            }
                            // Add the newly created item into inventory
                            inventory.Add(item);
                        }

                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"An error occured: {ex}");
                    Trace.Write(ex.ToString());
                }
            }
        }

        // Add item to inventory - sales page
        private void btnAddInventory_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;

            // Are any boxes empty?
            bool emptyCheck = isTbxEmpty([tbxProductID, tbxName, tbxQuantity, tbxPrice]);
            if (emptyCheck)
            {
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
                    MessageBox.Show("Please make sure that both Quantity and Price only contain numbers.\nQuanity must be a whole number, while Price can be a decimal.", "Failed to add item - Invalid data type");
                    valid = false;
                }
            }
            // If everything is correct, add to lbxInventorySP
            if (valid)
            {
                // If user has selected an item to update
                if (lbxInventorySP.SelectedIndex != -1)
                {
                    var selection = inventory[lbxInventorySP.SelectedIndex];    // Get selected item to edit
                    selection["ID"] = tbxProductID.Text.Trim();                 // Change ID
                    selection["Name"] = tbxName.Text.Trim();                    // Change Name
                    selection["Quantity"] = int.Parse(tbxQuantity.Text);        // Change Quantity
                    selection["Price"] = double.Parse(tbxPrice.Text);           // Chaange Price
                }
                else
                {
                    // Add to background list for editting later.
                    inventory.Add(new Dictionary<string, dynamic>());
                    inventory[lbxInventorySP.Items.Count].Add("ID", tbxProductID.Text.Trim());            // Add ID, remove extra spaces
                    inventory[lbxInventorySP.Items.Count].Add("Name", tbxName.Text.Trim());               // Add Name, remove extra spaces
                    inventory[lbxInventorySP.Items.Count].Add("Quantity", int.Parse(tbxQuantity.Text));   // Add Quantity
                    inventory[lbxInventorySP.Items.Count].Add("Price", double.Parse(tbxPrice.Text));      // Add Price
                }

                // Add to / update listbox
                populateListbox(lbxInventorySP);

                // Clear input boxes
                tbxProductID.Clear();
                tbxName.Clear();
                tbxQuantity.Clear();
                tbxPrice.Clear();

                // Print all saved information - debugging.
                foreach (var dict in inventory)
                {
                    foreach (var keyValue in dict)
                        Trace.Write($"{keyValue.Key}: {keyValue.Value} ");
                    Trace.WriteLine("");
                }

                // print size of inventory, debug
                Trace.WriteLine($"Amount of items in inventory: {inventory.Count}");
            }
        }

        // Clear inventory - sales page
        private void btnClearInv_Click(object sender, RoutedEventArgs e)
        {
            // Find selected item
            int selectedIndex = lbxInventorySP.SelectedIndex;

            // If an item is seleted
            if (selectedIndex != -1)
            {
                // Remove selected item
                inventory.RemoveAt(selectedIndex);
            }
            // If nothing is selected
            else
            {
                // Clear all items from inventory list
                inventory.Clear();
            }

            // Update listbox
            populateListbox(lbxInventorySP);

            // print size of inventory, debug
            Trace.WriteLine($"Amount of items in inventory: {inventory.Count}");
        }

        // Only exists to load selected item details into textboxes
        private void lbxInventorySP_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Takes the Inventory from Sales page, as well as the textboxes from the sales page
            updateItem(lbxInventorySP, [tbxProductID, tbxName, tbxQuantity, tbxPrice]);
        }

        // Save button
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            saveCsvFile();
        }

        // Load button
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            loadCsvFile();
            populateListbox(lbxInventorySP);
        }

        // CUSTOMER PAGE
        // Let user see entire inventory
        private void btnViewProducts_Click(object sender, RoutedEventArgs e)
        {
            // If the inventory has atleast 1 item.
            if (inventory.Count > 0)
            {
                populateListbox(lbxInventoryCust);
            }
            else
            {
                MessageBox.Show("Inventory is currently empty, please load a list on the first page, or input items manually.", "Empty Inventory.");
            }
        }
    }
}