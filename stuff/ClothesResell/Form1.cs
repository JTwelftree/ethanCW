using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClothesResell
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SellForm frmSellForm = new SellForm();
                frmSellForm.ShowDialog();
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
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Wallet: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblBalance.Text = $"";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                BuyForm frmBuyForm = new BuyForm();
                frmBuyForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Buy Form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
