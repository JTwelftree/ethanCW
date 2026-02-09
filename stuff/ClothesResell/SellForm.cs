using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClothesResell
{
    public partial class SellForm : Form
    {
        public SellForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                BuyForm frmBuyForm = new BuyForm();
                frmBuyForm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Buy Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void LoadBasketItems()
        {
            //Clear existing rows
            dataGridView1.Rows.Clear();

            try
            {
                //connect to the database
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "dbClothesSimulation.accdb");
                dbPath = Path.GetFullPath(dbPath);
                string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath}";

                //select the columns we need from tblbasket (BasketID, ItemName, PriceBought, ARP)
                string selectString = "SELECT BasketID, ItemName, PriceBought, ARP FROM tblbasket";

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    using (OleDbCommand cmd = new OleDbCommand(selectString, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            // Clear and setup columns explicitly
                            dataGridView1.Columns.Clear();
                            dataGridView1.Columns.Add("BasketID", "ID");
                            dataGridView1.Columns.Add("ItemName", "Item Name");
                            dataGridView1.Columns.Add("PriceBought", "Price Bought");
                            dataGridView1.Columns.Add("ARP", "Resell Price");
                            
                            // Create and add button column
                            DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                            buttonColumn.Name = "btnSell";
                            buttonColumn.HeaderText = "Action";
                            buttonColumn.Text = "Sell";
                            buttonColumn.UseColumnTextForButtonValue = true;
                            dataGridView1.Columns.Add(buttonColumn);

                            //if data has been returned from database 
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    dataGridView1.Rows.Add(
                                        reader["BasketID"], 
                                        reader["ItemName"], 
                                        reader["PriceBought"],
                                        reader["ARP"]
                                    );
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading basket items: {ex.Message}\n\n{ex.StackTrace}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SellForm_Load(object sender, EventArgs e)
        {
            lblBalance.Text = $"Balance: £{UserBalance.GetBalance():F2}";
            LoadBasketItems();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //Check if the button column (Action/Sell button) was clicked
                if (e.ColumnIndex >= 0 && dataGridView1.Columns.Count > e.ColumnIndex && dataGridView1.Columns[e.ColumnIndex].Name == "btnSell" && e.RowIndex >= 0)
                {
                    // Get the item name, price bought, and ARP from the clicked row
                    string itemName = dataGridView1.Rows[e.RowIndex].Cells["ItemName"].Value?.ToString() ?? "";
                    string priceBoughtStr = dataGridView1.Rows[e.RowIndex].Cells["PriceBought"].Value?.ToString() ?? "0";
                    string arpStr = dataGridView1.Rows[e.RowIndex].Cells["ARP"].Value?.ToString() ?? "0";

                    if (string.IsNullOrEmpty(itemName))
                    {
                        MessageBox.Show("Invalid item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(priceBoughtStr, out decimal priceBought))
                    {
                        priceBought = 0;
                    }

                    if (!decimal.TryParse(arpStr, out decimal arp) || arp < 0)
                    {
                        MessageBox.Show("Invalid selling price.", "Invalid Price", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Calculate sell probability based on price
                    // If ARP <= PriceBought: 100% chance to sell
                    // If ARP > PriceBought: chance decreases based on markup percentage
                    int sellProbability = 100;
                    
                    if (arp > priceBought && priceBought > 0)
                    {
                        // Calculate markup percentage
                        decimal markupPercentage = ((arp - priceBought) / priceBought) * 100;
                        
                        // For every 10% markup, reduce sell chance by 15%
                        // Example: 20% markup = 70% sell chance, 50% markup = 25% sell chance
                        sellProbability = Math.Max(10, 100 - (int)(markupPercentage * 1.5m));
                    }

                    // Confirm the sale attempt
                    DialogResult result = MessageBox.Show(
                        $"Attempt to sell {itemName} for £{arp:F2}?\n" +
                        $"(You paid £{priceBought:F2})\n" +
                        $"Sell probability: {sellProbability}%", 
                        "Confirm Sale Attempt", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);
                    
                    if (result != DialogResult.Yes)
                        return;

                    // Roll to see if the item sells
                    Random random = new Random();
                    int roll = random.Next(0, 100);
                    bool itemSold = roll < sellProbability;

                    string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "dbClothesSimulation.accdb");
                    dbPath = Path.GetFullPath(dbPath);
                    string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath}";

                    if (itemSold)
                    {
                        // Item sold successfully - save to transactions, remove from basket, and add money
                        try
                        {
                            using (OleDbConnection conn = new OleDbConnection(connString))
                            {
                                conn.Open();

                                // First, save to tblPastTransactions
                                // Check what columns exist in tblPastTransactions table
                                string insertString = "INSERT INTO tblPastTransactions (ItemName, PriceBought, PriceSold) VALUES (?, ?, ?)";
                                using (OleDbCommand insertCmd = new OleDbCommand(insertString, conn))
                                {
                                    insertCmd.Parameters.Add("p1", OleDbType.VarWChar, 255).Value = itemName;
                                    insertCmd.Parameters.Add("p2", OleDbType.VarWChar, 255).Value = priceBought.ToString();
                                    insertCmd.Parameters.Add("p3", OleDbType.VarWChar, 255).Value = arp.ToString();
                                    insertCmd.ExecuteNonQuery();
                                }

                                // Then remove item from tblbasket
                                string deleteString = "DELETE FROM tblbasket WHERE ItemName = ?";
                                using (OleDbCommand cmd = new OleDbCommand(deleteString, conn))
                                {
                                    cmd.Parameters.AddWithValue("p1", itemName);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        catch (Exception dbEx)
                        {
                            MessageBox.Show($"Database error saving transaction: {dbEx.Message}\n\nThe item will still be removed and you'll get the money, but the transaction won't be recorded.", 
                                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            
                            // Still remove from basket even if transaction save failed
                            using (OleDbConnection conn = new OleDbConnection(connString))
                            {
                                conn.Open();
                                string deleteString = "DELETE FROM tblbasket WHERE ItemName = ?";
                                using (OleDbCommand cmd = new OleDbCommand(deleteString, conn))
                                {
                                    cmd.Parameters.AddWithValue("p1", itemName);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // Add funds to balance
                        UserBalance.AddBalance(arp);
                        lblBalance.Text = $"Balance: £{UserBalance.GetBalance():F2}";

                        decimal profit = arp - priceBought;
                        string profitText = profit > 0 ? $"Profit: £{profit:F2}" : profit < 0 ? $"Loss: £{Math.Abs(profit):F2}" : "Break even";
                        
                        MessageBox.Show(
                            $"Successfully sold {itemName} for £{arp:F2}!\n{profitText}\nNew balance: £{UserBalance.GetBalance():F2}", 
                            "Sale Successful!", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Information);
                        
                        LoadBasketItems(); // Refresh the grid
                    }
                    else
                    {
                        // Item didn't sell - stays in inventory
                        MessageBox.Show(
                            $"Nobody wanted to buy {itemName} for £{arp:F2}.\nThe item remains in your inventory.\nTry lowering the price!", 
                            "Sale Failed", 
                            MessageBoxButtons.OK, 
                            MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}\n\n{ex.StackTrace}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}