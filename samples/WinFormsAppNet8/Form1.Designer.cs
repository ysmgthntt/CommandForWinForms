namespace WinFormsAppNet8
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            toolStrip1 = new ToolStrip();
            toolStripButtonClose = new ToolStripButton();
            toolStripButtonCommand1 = new ToolStripButton();
            toolStripButtonCommand2 = new ToolStripButton();
            toolStripContainer1 = new ToolStripContainer();
            button2 = new Button();
            button1 = new Button();
            textBox4 = new TextBox();
            panel1 = new Panel();
            label1 = new Label();
            panel3 = new Panel();
            label3 = new Label();
            textBox3 = new TextBox();
            panel2 = new Panel();
            label2 = new Label();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            toolStrip1.SuspendLayout();
            toolStripContainer1.ContentPanel.SuspendLayout();
            toolStripContainer1.TopToolStripPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            panel1.SuspendLayout();
            panel3.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = DockStyle.None;
            toolStrip1.ImageScalingSize = new Size(24, 24);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonClose, toolStripButtonCommand1, toolStripButtonCommand2 });
            toolStrip1.Location = new Point(4, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(282, 34);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonClose
            // 
            toolStripButtonClose.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonClose.ImageTransparentColor = Color.Magenta;
            toolStripButtonClose.Name = "toolStripButtonClose";
            toolStripButtonClose.Size = new Size(59, 29);
            toolStripButtonClose.Text = "Close";
            // 
            // toolStripButtonCommand1
            // 
            toolStripButtonCommand1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonCommand1.ImageTransparentColor = Color.Magenta;
            toolStripButtonCommand1.Name = "toolStripButtonCommand1";
            toolStripButtonCommand1.Size = new Size(75, 29);
            toolStripButtonCommand1.Text = "Execute";
            // 
            // toolStripButtonCommand2
            // 
            toolStripButtonCommand2.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButtonCommand2.ImageTransparentColor = Color.Magenta;
            toolStripButtonCommand2.Name = "toolStripButtonCommand2";
            toolStripButtonCommand2.Size = new Size(130, 29);
            toolStripButtonCommand2.Text = "Execute(.NET7)";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Controls.Add(button2);
            toolStripContainer1.ContentPanel.Controls.Add(button1);
            toolStripContainer1.ContentPanel.Controls.Add(textBox4);
            toolStripContainer1.ContentPanel.Controls.Add(panel1);
            toolStripContainer1.ContentPanel.Size = new Size(800, 416);
            toolStripContainer1.Dock = DockStyle.Fill;
            toolStripContainer1.Location = new Point(0, 0);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(800, 450);
            toolStripContainer1.TabIndex = 1;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            toolStripContainer1.TopToolStripPanel.Controls.Add(toolStrip1);
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.System;
            button2.Location = new Point(353, 124);
            button2.Name = "button2";
            button2.Size = new Size(112, 34);
            button2.TabIndex = 3;
            button2.Text = "GC";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.FlatStyle = FlatStyle.System;
            button1.Location = new Point(353, 84);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 2;
            button1.Text = "Open New";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(353, 32);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(150, 31);
            textBox4.TabIndex = 1;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(label1);
            panel1.Controls.Add(panel3);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(textBox1);
            panel1.Location = new Point(16, 16);
            panel1.Name = "panel1";
            panel1.Size = new Size(320, 388);
            panel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 0);
            label1.Name = "label1";
            label1.Size = new Size(65, 25);
            label1.TabIndex = 3;
            label1.Text = "panel1";
            // 
            // panel3
            // 
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(label3);
            panel3.Controls.Add(textBox3);
            panel3.Location = new Point(16, 180);
            panel3.Name = "panel3";
            panel3.Size = new Size(200, 80);
            panel3.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(16, 0);
            label3.Name = "label3";
            label3.Size = new Size(65, 25);
            label3.TabIndex = 1;
            label3.Text = "panel3";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(16, 28);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(150, 31);
            textBox3.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(label2);
            panel2.Controls.Add(textBox2);
            panel2.Location = new Point(16, 80);
            panel2.Name = "panel2";
            panel2.Size = new Size(200, 80);
            panel2.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(16, 0);
            label2.Name = "label2";
            label2.Size = new Size(65, 25);
            label2.TabIndex = 2;
            label2.Text = "panel2";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(16, 28);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(150, 31);
            textBox2.TabIndex = 1;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(16, 28);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(150, 31);
            textBox1.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(toolStripContainer1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            toolStripContainer1.ContentPanel.ResumeLayout(false);
            toolStripContainer1.ContentPanel.PerformLayout();
            toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer1.TopToolStripPanel.PerformLayout();
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonCommand1;
        private ToolStripContainer toolStripContainer1;
        private Panel panel1;
        private Panel panel2;
        private TextBox textBox2;
        private TextBox textBox1;
        private TextBox textBox4;
        private Panel panel3;
        private TextBox textBox3;
        private ToolStripButton toolStripButtonClose;
        private ToolStripButton toolStripButtonCommand2;
        private Button button1;
        private Button button2;
        private Label label1;
        private Label label2;
        private Label label3;
    }
}
