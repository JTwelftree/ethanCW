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
                // Update balance display
                lblBalance.Text = $"Current Balance: £{UserBalance.GetBalance():F2}";

                //connect to the database placed in bin folder
                // For .NET 6, the exe runs from bin/Debug/net6.0-windows/, but the database is in bin/Debug/
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "dbClothesSimulation.accdb");
                dbPath = Path.GetFullPath(dbPath);
                string connString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbPath}";

                //select all records from database - Past Transactions
                string selectString = "SELECT * FROM tblPastTransactions";

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    using (OleDbCommand cmd = new OleDbCommand(selectString, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                DataTable dt = new DataTable();
                                dt.Clear();
                                dt.Columns.Add("ItemName");
                                dt.Columns.Add("PriceSold");

                                while (reader.Read())
                                {
                                    DataRow UserRow = dt.NewRow();
                                    UserRow["ItemName"] = (string)reader["ItemName"];
                                    UserRow["PriceSold"] = (string)reader["PriceSold"].ToString();
                                    dt.Rows.Add(UserRow);
                                }
                            }
                        }
                    }
                }

                //select all records from database - Basket
                string selectString2 = "SELECT * FROM tblbasket";

                using (OleDbConnection conn2 = new OleDbConnection(connString))
                {
                    conn2.Open();

                    using (OleDbCommand cmd2 = new OleDbCommand(selectString2, conn2))
                    {
                        using (OleDbDataReader reader2 = cmd2.ExecuteReader())
                        {
                            if (reader2.HasRows)
                            {
                                //Create datatable 
                                DataTable dt = new DataTable();
                                dt.Clear();
                                dt.Columns.Add("ItemName");
                                dt.Columns.Add("PriceBought");
                                dt.Columns.Add("ARP");

                                //for each row returned from database add to datagridview
                                while (reader2.Read())
                                {
                                    DataRow UserRow = dt.NewRow();
                                    UserRow["ItemName"] = (string)reader2["ItemName"];
                                    UserRow["PriceBought"] = (string)reader2["PriceBought"].ToString();
                                    UserRow["ARP"] = (string)reader2["ARP"].ToString();
                                    dt.Rows.Add(UserRow);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading wallet data: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

