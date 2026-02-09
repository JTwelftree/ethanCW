using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClothesResell
{
    public partial class BuyForm : Form
    {
        public BuyForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }


        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private void LoadBasketData()
        {
            //Clear existing rows
            dgBuy.Rows.Clear();

            //this code runs when the form opens
            try
            {
                //connect to the database placed in bin folder
                // For .NET 6, the exe runs from bin/Debug/net6.0-windows/, but the database is in bin/Debug/
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "dbClothesSimulation.accdb");
                dbPath = Path.GetFullPath(dbPath);
                string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath}";

                //select all records from database - changed to tblBuyItems to show all available items
                string selectString = "SELECT * FROM tblBuyItems";

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    using (OleDbCommand cmd = new OleDbCommand(selectString, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            //Get the schema to see what columns are available
                            DataTable schemaTable = reader.GetSchemaTable();
                            
                            // Clear and setup columns dynamically based on what's in the table
                            dgBuy.Columns.Clear();
                            foreach (DataRow column in schemaTable.Rows)
                            {
                                string columnName = column["ColumnName"].ToString();
                                dgBuy.Columns.Add(columnName, columnName);
                            }
                            
                            // Create and add button column
                            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                            buttonColumn.Name = "btnBuy";
                            buttonColumn.HeaderText = "Action";
                            buttonColumn.Text = "Buy";
                            buttonColumn.UseColumnTextForButtonValue = true;
                            dgBuy.Columns.Add(buttonColumn);

                            //if data has been returned from database 
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    object[] values = new object[reader.FieldCount + 1];
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        values[i] = reader.GetValue(i) ?? "";
                                    }
                                    values[reader.FieldCount] = "Buy";
                                    dgBuy.Rows.Add(values);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading buy items data: {ex.Message}\n\n{ex.StackTrace}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private void BuyForm_Load(object sender, EventArgs e)
        {
            try
            {
                LoadBasketData();
                UpdateBalanceDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Critical error loading BuyForm: {ex.Message}\n\n{ex.StackTrace}", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateBalanceDisplay()
        {
            lblBalance.Text = $"Balance: £{UserBalance.GetBalance():F2}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error returning to main form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SellForm frmSellForm = new SellForm();
                frmSellForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Sell Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Wallet frmWallet = new Wallet();
                frmWallet.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Wallet: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private void AddItemToBasket(int rowIndex, string itemName, decimal priceBought, decimal avp)
        {
            try
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "dbClothesSimulation.accdb");
                dbPath = Path.GetFullPath(dbPath);
                string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath}";

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    // Escape single quotes in itemName to prevent SQL errors
                    string safeName = itemName.Replace("'", "''");
                    
                    // IMPORTANT: Both PriceBought and ARP are Short Text fields in Access, so they need quotes!
                    string insertString = $"INSERT INTO tblbasket (ItemName, PriceBought, ARP) VALUES ('{safeName}', '{priceBought}', '{avp}')";

                    using (OleDbCommand cmd = new OleDbCommand(insertString, conn))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding item to basket: {ex.Message}\n\nInner Exception: {ex.InnerException?.Message}\n\n{ex.StackTrace}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        private void dgBuy_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //Check if the button column (Action/Buy button) was clicked
                if (e.ColumnIndex >= 0 && e.ColumnIndex < dgBuy.Columns.Count && 
                    dgBuy.Columns[e.ColumnIndex].Name == "btnBuy" && e.RowIndex >= 0)
                {
                    // Get all row values - we need ItemName, ItemPrice, and AVP
                    string itemName = "";
                    string itemPriceStr = "0";
                    string avpStr = "0";
                    
                    // Find ItemName, ItemPrice, and AVP columns dynamically
                    for (int i = 0; i < dgBuy.Columns.Count; i++)
                    {
                        // Skip the button column
                        if (dgBuy.Columns[i].Name == "btnBuy")
                            continue;
                            
                        string columnName = dgBuy.Columns[i].Name?.ToLower() ?? "";
                        string cellValue = dgBuy.Rows[e.RowIndex].Cells[i].Value?.ToString() ?? "";
                        
                        if (columnName.Contains("itemname") || columnName == "itemname")
                        {
                            itemName = cellValue;
                        }
                        else if (columnName.Contains("itemprice") || columnName == "itemprice")
                        {
                            itemPriceStr = cellValue;
                        }
                        else if (columnName.Contains("avp") || columnName == "avp")
                        {
                            avpStr = cellValue;
                        }
                    }

                    if (string.IsNullOrEmpty(itemName))
                    {
                        MessageBox.Show("Invalid item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(itemPriceStr, out decimal itemPrice))
                    {
                        MessageBox.Show("Invalid price format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(avpStr, out decimal avp))
                    {
                        avp = itemPrice; // Default AVP to item price if not found
                    }

                    // Check if user has enough balance
                    if (UserBalance.GetBalance() < itemPrice)
                    {
                        MessageBox.Show($"Insufficient funds! You need £{itemPrice:F2} but only have £{UserBalance.GetBalance():F2}", "Purchase Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Confirm the purchase
                    DialogResult result = MessageBox.Show($"Buy {itemName} for £{itemPrice:F2}?", "Confirm Purchase", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        return;

                    // Add the item to user's basket (tblbasket) with ItemPrice as PriceBought and AVP as ARP
                    AddItemToBasket(e.RowIndex, itemName, itemPrice, avp);
                    
                    // Deduct from balance
                    UserBalance.DeductBalance(itemPrice);
                    UpdateBalanceDisplay();

                    MessageBox.Show($"Successfully purchased {itemName}! New balance: £{UserBalance.GetBalance():F2}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBasketData(); // Refresh the grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing purchase: {ex.Message}\n\n{ex.StackTrace}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}