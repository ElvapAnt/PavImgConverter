namespace PavImgConverter
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
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            loadMenuItem = new ToolStripMenuItem();
            pavMenuItem = new ToolStripMenuItem();
            tableLayoutPanel1 = new TableLayoutPanel();
            imageBox = new PictureBox();
            convertAndSaveBtn = new Button();
            ditheringBtn = new Button();
            edgeBtn = new Button();
            gammaBtn = new Button();
            restoreImgBtn = new Button();
            redTxtBox = new TextBox();
            greenTxtBox = new TextBox();
            blueTxtBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)imageBox).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1381, 24);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadMenuItem, pavMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadMenuItem
            // 
            loadMenuItem.Name = "loadMenuItem";
            loadMenuItem.Size = new Size(163, 22);
            loadMenuItem.Text = "Load New Image";
            loadMenuItem.Click += loadMenuItem_Click;
            // 
            // pavMenuItem
            // 
            pavMenuItem.Name = "pavMenuItem";
            pavMenuItem.Size = new Size(163, 22);
            pavMenuItem.Text = "Load .pav Image";
            pavMenuItem.Click += pavMenuItem_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
            tableLayoutPanel1.Location = new Point(347, 163);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(200, 0);
            tableLayoutPanel1.TabIndex = 4;
            // 
            // imageBox
            // 
            imageBox.Dock = DockStyle.Right;
            imageBox.Location = new Point(248, 24);
            imageBox.Name = "imageBox";
            imageBox.Size = new Size(1133, 716);
            imageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            imageBox.TabIndex = 0;
            imageBox.TabStop = false;
            // 
            // convertAndSaveBtn
            // 
            convertAndSaveBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            convertAndSaveBtn.Location = new Point(12, 27);
            convertAndSaveBtn.Name = "convertAndSaveBtn";
            convertAndSaveBtn.Size = new Size(180, 60);
            convertAndSaveBtn.TabIndex = 1;
            convertAndSaveBtn.Text = "Convert and Save";
            convertAndSaveBtn.UseVisualStyleBackColor = true;
            convertAndSaveBtn.Click += convertAndSaveBtn_Click;
            // 
            // ditheringBtn
            // 
            ditheringBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ditheringBtn.Location = new Point(12, 93);
            ditheringBtn.Name = "ditheringBtn";
            ditheringBtn.Size = new Size(180, 60);
            ditheringBtn.TabIndex = 2;
            ditheringBtn.Text = "Stucki dithering";
            ditheringBtn.UseVisualStyleBackColor = true;
            ditheringBtn.Click += ditheringBtn_Click;
            // 
            // edgeBtn
            // 
            edgeBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            edgeBtn.Location = new Point(12, 159);
            edgeBtn.Name = "edgeBtn";
            edgeBtn.Size = new Size(180, 62);
            edgeBtn.TabIndex = 3;
            edgeBtn.Text = "Edge detect homogenity";
            edgeBtn.UseVisualStyleBackColor = true;
            edgeBtn.Click += edgeBtn_Click;
            // 
            // gammaBtn
            // 
            gammaBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            gammaBtn.Location = new Point(12, 227);
            gammaBtn.Name = "gammaBtn";
            gammaBtn.Size = new Size(180, 61);
            gammaBtn.TabIndex = 4;
            gammaBtn.Text = "Gamma";
            gammaBtn.UseVisualStyleBackColor = true;
            gammaBtn.Click += gammaBtn_Click;
            // 
            // restoreImgBtn
            // 
            restoreImgBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            restoreImgBtn.Location = new Point(12, 689);
            restoreImgBtn.Name = "restoreImgBtn";
            restoreImgBtn.Size = new Size(180, 39);
            restoreImgBtn.TabIndex = 5;
            restoreImgBtn.Text = "Restore image";
            restoreImgBtn.UseVisualStyleBackColor = true;
            restoreImgBtn.Click += restoreImgBtn_Click;
            // 
            // redTxtBox
            // 
            redTxtBox.Location = new Point(92, 294);
            redTxtBox.Name = "redTxtBox";
            redTxtBox.Size = new Size(100, 23);
            redTxtBox.TabIndex = 6;
            // 
            // greenTxtBox
            // 
            greenTxtBox.Location = new Point(92, 323);
            greenTxtBox.Name = "greenTxtBox";
            greenTxtBox.Size = new Size(100, 23);
            greenTxtBox.TabIndex = 7;
            // 
            // blueTxtBox
            // 
            blueTxtBox.Location = new Point(92, 352);
            blueTxtBox.Name = "blueTxtBox";
            blueTxtBox.Size = new Size(100, 23);
            blueTxtBox.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.Red;
            label1.Location = new Point(59, 297);
            label1.Name = "label1";
            label1.Size = new Size(27, 15);
            label1.TabIndex = 9;
            label1.Text = "Red";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.Lime;
            label2.Location = new Point(48, 326);
            label2.Name = "label2";
            label2.Size = new Size(38, 15);
            label2.TabIndex = 10;
            label2.Text = "Green";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.Blue;
            label3.Location = new Point(56, 355);
            label3.Name = "label3";
            label3.Size = new Size(30, 15);
            label3.TabIndex = 11;
            label3.Text = "Blue";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1381, 740);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(blueTxtBox);
            Controls.Add(greenTxtBox);
            Controls.Add(redTxtBox);
            Controls.Add(restoreImgBtn);
            Controls.Add(gammaBtn);
            Controls.Add(edgeBtn);
            Controls.Add(ditheringBtn);
            Controls.Add(convertAndSaveBtn);
            Controls.Add(imageBox);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Pav Image Converter";
            WindowState = FormWindowState.Maximized;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)imageBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loadMenuItem;
        private TableLayoutPanel tableLayoutPanel1;
        private PictureBox imageBox;
        private ToolStripMenuItem pavMenuItem;
        private Button convertAndSaveBtn;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button ditheringBtn;
        private Button edgeBtn;
        private Button gammaBtn;
        private Button restoreImgBtn;
        private TextBox redTxtBox;
        private TextBox greenTxtBox;
        private TextBox blueTxtBox;
        private Label label1;
        private Label label2;
        private Label label3;
    }
}
