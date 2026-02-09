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
                    // Get the item name and price from the clicked row
                    string itemName = dataGridView1.Rows[e.RowIndex].Cells["ItemName"].Value?.ToString() ?? "";
                    string arpStr = dataGridView1.Rows[e.RowIndex].Cells["ARP"].Value?.ToString() ?? "0";

                    if (string.IsNullOrEmpty(itemName))
                    {
                        MessageBox.Show("Invalid item selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(arpStr, out decimal arp) || arp <= 0)
                    {
                        MessageBox.Show("Please enter a valid selling price (ARP must be greater than 0).", "Invalid Price", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Confirm the sale
                    DialogResult result = MessageBox.Show($"Sell {itemName} for £{arp:F2}?", "Confirm Sale", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        return;

                    string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "dbClothesSimulation.accdb");
                    dbPath = Path.GetFullPath(dbPath);
                    string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath}";

                    using (OleDbConnection conn = new OleDbConnection(connString))
                    {
                        conn.Open();

                        // Remove item from tblbasket using parameterized query
                        string deleteString = "DELETE FROM tblbasket WHERE ItemName = @ItemName";
                        using (OleDbCommand cmd = new OleDbCommand(deleteString, conn))
                        {
                            cmd.Parameters.AddWithValue("@ItemName", itemName);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    // Add funds to balance
                    UserBalance.AddBalance(arp);
                    lblBalance.Text = $"Balance: £{UserBalance.GetBalance():F2}";

                    MessageBox.Show($"Successfully sold {itemName} for £{arp:F2}! New balance: £{UserBalance.GetBalance():F2}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadBasketItems(); // Refresh the grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing sale: {ex.Message}\n\n{ex.StackTrace}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}