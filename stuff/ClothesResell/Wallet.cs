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
    public partial class Wallet : Form
    {
        public Wallet()
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


        private void Wallet_Load(object sender, EventArgs e)
        {
            //this code runs when the form opens
            try
            {
                // Update balance display with current user balance from UserBalance class
                lblBalance.Text = $"Current Balance: £{UserBalance.GetBalance():F2}";

                //connect to the database placed in bin folder
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "dbClothesSimulation.accdb");
                dbPath = Path.GetFullPath(dbPath);
                string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath}";

                decimal totalInvested = 0;
                decimal totalSoldFor = 0;
                decimal cumulativeProfit = 0;
                
                // List to track sales for graphing
                List<decimal> profitHistory = new List<decimal>();
                List<string> itemNames = new List<string>();
                
                //select all records from database - Past Transactions
                string selectString = "SELECT * FROM tblPastTransactions";

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    using (OleDbCommand cmd = new OleDbCommand(selectString, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Safely get values, handling cases where PriceBought might be null or missing
                                string priceBoughtStr = "0";
                                string priceSoldStr = "0";
                                string itemName = "";
                                
                                try
                                {
                                    itemName = reader["ItemName"]?.ToString() ?? "";
                                }
                                catch
                                {
                                    itemName = "Unknown";
                                }
                                
                                try
                                {
                                    // Check if PriceBought column exists and has a value
                                    int priceBoughtIndex = reader.GetOrdinal("PriceBought");
                                    if (!reader.IsDBNull(priceBoughtIndex))
                                    {
                                        priceBoughtStr = reader["PriceBought"]?.ToString() ?? "0";
                                    }
                                }
                                catch
                                {
                                    // PriceBought column might not exist in old records
                                    priceBoughtStr = "0";
                                }
                                
                                try
                                {
                                    priceSoldStr = reader["PriceSold"]?.ToString() ?? "0";
                                }
                                catch
                                {
                                    priceSoldStr = "0";
                                }
                                
                                decimal.TryParse(priceBoughtStr, out decimal priceBought);
                                decimal.TryParse(priceSoldStr, out decimal priceSold);
                                
                                decimal profit = priceSold - priceBought;
                                cumulativeProfit += profit;
                                
                                // Track totals for sold items
                                totalInvested += priceBought;
                                totalSoldFor += priceSold;
                                
                                // Store for graph
                                profitHistory.Add(cumulativeProfit);
                                itemNames.Add(itemName);
                            }
                        }
                    }
                }

                //select all records from database - Basket (current wardrobe)
                string selectString2 = "SELECT * FROM tblbasket";

                using (OleDbConnection conn2 = new OleDbConnection(connString))
                {
                    conn2.Open();

                    using (OleDbCommand cmd2 = new OleDbCommand(selectString2, conn2))
                    {
                        using (OleDbDataReader reader2 = cmd2.ExecuteReader())
                        {
                            //Create datatable for wardrobe
                            DataTable dt = new DataTable();
                            dt.Clear();
                            dt.Columns.Add("Item Name");
                            dt.Columns.Add("Price Bought");
                            dt.Columns.Add("Resell Price");

                            //for each row returned from database add to datagridview
                            while (reader2.Read())
                            {
                                DataRow UserRow = dt.NewRow();
                                UserRow["Item Name"] = reader2["ItemName"]?.ToString() ?? "";
                                UserRow["Price Bought"] = reader2["PriceBought"]?.ToString() ?? "0";
                                UserRow["Resell Price"] = reader2["ARP"]?.ToString() ?? "0";
                                dt.Rows.Add(UserRow);
                            }
                            
                            // Bind to the dgBasket DataGridView to show wardrobe
                            dgBasket.DataSource = dt;
                        }
                    }
                }
                
                // Calculate returns from sold items
                decimal totalReturn = totalSoldFor - totalInvested;
                decimal percentageReturn = 0;
                
                if (totalInvested > 0)
                {
                    percentageReturn = (totalReturn / totalInvested) * 100;
                }
                
                // Update the return labels
                if (totalReturn >= 0)
                {
                    label3.Text = $"+£{totalReturn:F2}";
                    label3.ForeColor = System.Drawing.Color.GreenYellow;
                }
                else
                {
                    label3.Text = $"-£{Math.Abs(totalReturn):F2}";
                    label3.ForeColor = System.Drawing.Color.Red;
                }
                
                if (percentageReturn >= 0)
                {
                    label5.Text = $"+{percentageReturn:F2}%";
                    label5.ForeColor = System.Drawing.Color.GreenYellow;
                }
                else
                {
                    label5.Text = $"{percentageReturn:F2}%";
                    label5.ForeColor = System.Drawing.Color.Red;
                }
                
                // Create revenue graph if there are any sales
                CreateSalesHistoryDisplay(profitHistory, itemNames);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading wallet data: {ex.Message}\n\n{ex.StackTrace}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void CreateSalesHistoryDisplay(List<decimal> profitHistory, List<string> itemNames)
        {
            try
            {
                // Try to find an existing ListBox or create one
                ListBox salesHistoryBox = null;
                
                // Search for existing ListBox
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is ListBox && ctrl.Name == "lstSalesHistory")
                    {
                        salesHistoryBox = (ListBox)ctrl;
                        break;
                    }
                }
                
                // If no ListBox found, create one dynamically
                if (salesHistoryBox == null)
                {
                    salesHistoryBox = new ListBox();
                    salesHistoryBox.Name = "lstSalesHistory";
                    salesHistoryBox.Location = new System.Drawing.Point(20, 322);  // Moved up 100px from 422
                    salesHistoryBox.Size = new System.Drawing.Size(320, 250);
                    salesHistoryBox.BackColor = System.Drawing.Color.Black;
                    salesHistoryBox.ForeColor = System.Drawing.Color.White;
                    salesHistoryBox.Font = new System.Drawing.Font("Consolas", 9);
                    salesHistoryBox.BorderStyle = BorderStyle.FixedSingle;
                    this.Controls.Add(salesHistoryBox);
                    
                    // Add a label above it
                    Label lblHistory = new Label();
                    lblHistory.Text = "Sales History:";
                    lblHistory.Location = new System.Drawing.Point(20, 292);  // Moved up 100px from 392
                    lblHistory.Size = new System.Drawing.Size(150, 25);
                    lblHistory.ForeColor = System.Drawing.Color.White;
                    lblHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10, System.Drawing.FontStyle.Bold);
                    this.Controls.Add(lblHistory);
                }
                
                // Clear existing items
                salesHistoryBox.Items.Clear();
                
                // Add header
                salesHistoryBox.Items.Add("═══════════════════════════════");
                salesHistoryBox.Items.Add("  PROFIT TRACKER");
                salesHistoryBox.Items.Add("═══════════════════════════════");
                salesHistoryBox.Items.Add("");
                
                if (profitHistory.Count > 0)
                {
                    decimal runningTotal = 0;
                    
                    for (int i = 0; i < profitHistory.Count; i++)
                    {
                        decimal currentProfit = i == 0 ? profitHistory[0] : profitHistory[i] - profitHistory[i - 1];
                        runningTotal = profitHistory[i];
                        
                        string profitSymbol = currentProfit >= 0 ? "+" : "";
                        string itemDisplay = itemNames[i].Length > 15 ? itemNames[i].Substring(0, 15) : itemNames[i];
                        
                        salesHistoryBox.Items.Add($"Sale {i + 1}: {itemDisplay}");
                        salesHistoryBox.Items.Add($"  Profit: {profitSymbol}£{currentProfit:F2}");
                        salesHistoryBox.Items.Add($"  Total:  £{runningTotal:F2}");
                        salesHistoryBox.Items.Add("───────────────────────────────");
                    }
                    
                    // Summary
                    salesHistoryBox.Items.Add("");
                    salesHistoryBox.Items.Add("OVERALL PERFORMANCE:");
                    string finalSymbol = runningTotal >= 0 ? "+" : "";
                    salesHistoryBox.Items.Add($"Final Profit: {finalSymbol}£{runningTotal:F2}");
                    
                    // Scroll to bottom to show most recent
                    if (salesHistoryBox.Items.Count > 0)
                    {
                        salesHistoryBox.TopIndex = Math.Max(0, salesHistoryBox.Items.Count - 1);
                    }
                }
                else
                {
                    salesHistoryBox.Items.Add("  No sales yet!");
                    salesHistoryBox.Items.Add("");
                    salesHistoryBox.Items.Add("  Buy items from the BUY page");
                    salesHistoryBox.Items.Add("  and sell them to start");
                    salesHistoryBox.Items.Add("  tracking your profit!");
                }
            }
            catch (Exception ex)
            {
                // Display creation failed, but don't crash the app
                MessageBox.Show($"Could not create sales history: {ex.Message}", "Display Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}