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

                //select all records from tblbasket - items the user has bought
                string selectString = "SELECT * FROM tblbasket";

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    using (OleDbCommand cmd = new OleDbCommand(selectString, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            // Setup columns dynamically based on table structure
                            DataTable schemaTable = reader.GetSchemaTable();
                            
                            dataGridView1.Columns.Clear();
                            foreach (DataRow column in schemaTable.Rows)
                            {
                                string columnName = column["ColumnName"].ToString();
                                dataGridView1.Columns.Add(columnName, columnName);
                            }
                            
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
                                    object[] values = new object[reader.FieldCount + 1];
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        values[i] = reader.GetValue(i) ?? "";
                                    }
                                    values[reader.FieldCount] = "Sell";
                                    dataGridView1.Rows.Add(values);
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
                    // Get all row values
                    string itemName = "";
                    string arpStr = "0";
                    
                    // Find ItemName and ARP columns dynamically
                    for (int i = 0; i < dataGridView1.Columns.Count - 1; i++) // -1 to skip the button column
                    {
                        string columnName = dataGridView1.Columns[i].Name.ToLower();
                        string cellValue = dataGridView1.Rows[e.RowIndex].Cells[i].Value?.ToString() ?? "";
                        
                        if (columnName.Contains("itemname") || columnName.Contains("item name"))
                        {
                            itemName = cellValue;
                        }
                        else if (columnName.Contains("arp"))
                        {
                            arpStr = cellValue;
                        }
                    }

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
