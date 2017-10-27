namespace HBP_Graph_Checker
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(552, 35);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 22);
            this.numericUpDown1.TabIndex = 3;
            this.numericUpDown1.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Max Number Of Emails";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(553, 391);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(119, 32);
            this.button1.TabIndex = 4;
            this.button1.Text = "Go";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(26, 111);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(647, 190);
            this.dataGridView1.TabIndex = 5;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(155, 70);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(517, 22);
            this.textBox3.TabIndex = 11;
            this.textBox3.Text = resources.GetString("textBox3.Text");
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 70);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "Query Filter";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 466);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "HBP Outlook Checker UI";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label5;
    }
}

