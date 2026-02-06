namespace ClothesResell
{
    partial class BuyForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.Cl = new System.Windows.Forms.Label();
            this.lblBalance = new System.Windows.Forms.Label();
            this.performanceCounter1 = new System.Diagnostics.PerformanceCounter();
            this.performanceCounter2 = new System.Diagnostics.PerformanceCounter();
            this.performanceCounter3 = new System.Diagnostics.PerformanceCounter();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.dgBuy = new System.Windows.Forms.DataGridView();
            this.btnBuy = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.performanceCounter1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.performanceCounter2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.performanceCounter3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgBuy)).BeginInit();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(1041, 18);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(141, 68);
            this.button3.TabIndex = 4;
            this.button3.Text = "Wallet";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button2.Location = new System.Drawing.Point(188, 114);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(141, 68);
            this.button2.TabIndex = 6;
            this.button2.Text = "SELL";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.Highlight;
            this.button1.Location = new System.Drawing.Point(18, 114);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(141, 68);
            this.button1.TabIndex = 5;
            this.button1.Text = "BUY";
            this.button1.UseVisualStyleBackColor = false;
            // 
            // Cl
            // 
            this.Cl.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Cl.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cl.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Cl.Location = new System.Drawing.Point(18, 14);
            this.Cl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Cl.Name = "Cl";
            this.Cl.Size = new System.Drawing.Size(184, 86);
            this.Cl.TabIndex = 7;
            this.Cl.Text = "Resell Simulation";
            this.Cl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBalance
            // 
            this.lblBalance.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblBalance.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBalance.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblBalance.Location = new System.Drawing.Point(800, 114);
            this.lblBalance.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBalance.Name = "lblBalance";
            this.lblBalance.Size = new System.Drawing.Size(200, 68);
            this.lblBalance.TabIndex = 9;
            this.lblBalance.Text = "Balance: £100.00";
            this.lblBalance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgBuy
            // 
            this.dgBuy.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgBuy.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgBuy.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.btnBuy});
            this.dgBuy.Location = new System.Drawing.Point(24, 220);
            this.dgBuy.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgBuy.Name = "dgBuy";
            this.dgBuy.RowHeadersWidth = 62;
            this.dgBuy.Size = new System.Drawing.Size(1152, 450);
            this.dgBuy.TabIndex = 8;
            this.dgBuy.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgBuy_CellContentClick);
            // 
            // btnBuy
            // 
            this.btnBuy.HeaderText = "Action";
            this.btnBuy.MinimumWidth = 8;
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Text = "Buy";
            this.btnBuy.UseColumnTextForButtonValue = true;
            this.btnBuy.Width = 150;
            // 
            // BuyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(1200, 692);
            this.Controls.Add(this.lblBalance);
            this.Controls.Add(this.dgBuy);
            this.Controls.Add(this.Cl);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button3);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "BuyForm";
            this.Load += new System.EventHandler(this.BuyForm_Load);
            ///((System.ComponentModel.ISupportInitialize)(this.performanceCounter1)).EndInit();
            ///((System.ComponentModel.ISupportInitialize)(this.performanceCounter2)).EndInit();
            ///((System.ComponentModel.ISupportInitialize)(this.performanceCounter3)).EndInit();
            ///((System.ComponentModel.ISupportInitialize)(this.dgBuy)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label Cl;
        private System.Windows.Forms.Label lblBalance;
        private System.Diagnostics.PerformanceCounter performanceCounter1;
        private System.Diagnostics.PerformanceCounter performanceCounter2;
        private System.Diagnostics.PerformanceCounter performanceCounter3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.DataGridView dgBuy;
        private System.Windows.Forms.DataGridViewButtonColumn btnBuy;
    }
}