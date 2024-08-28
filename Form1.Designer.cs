namespace CelticKnots
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
            pictureBox1 = new PictureBox();
            buttonDrawAll = new Button();
            buttonClipboard = new Button();
            buttonOverlayed = new Button();
            buttonTest = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(170, 50);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(1150, 745);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += PictureBox1_Paint;
            // 
            // buttonDrawAll
            // 
            buttonDrawAll.Location = new Point(16, 105);
            buttonDrawAll.Name = "buttonDrawAll";
            buttonDrawAll.Size = new Size(152, 34);
            buttonDrawAll.TabIndex = 1;
            buttonDrawAll.Text = "Draw all";
            buttonDrawAll.UseVisualStyleBackColor = true;
            buttonDrawAll.Click += ButtonDrawAll_Click;
            // 
            // buttonClipboard
            // 
            buttonClipboard.Location = new Point(12, 145);
            buttonClipboard.Name = "buttonClipboard";
            buttonClipboard.Size = new Size(152, 34);
            buttonClipboard.TabIndex = 2;
            buttonClipboard.Text = "All to Clipboard";
            buttonClipboard.UseVisualStyleBackColor = true;
            buttonClipboard.Click += Button2_Click;
            // 
            // buttonOverlayed
            // 
            buttonOverlayed.Location = new Point(16, 235);
            buttonOverlayed.Name = "buttonOverlayed";
            buttonOverlayed.Size = new Size(147, 32);
            buttonOverlayed.TabIndex = 3;
            buttonOverlayed.Text = "Overlayed";
            buttonOverlayed.UseVisualStyleBackColor = true;
            buttonOverlayed.Click += ButtonOverlayed_Click;
            // 
            // buttonTest
            // 
            buttonTest.Location = new Point(29, 307);
            buttonTest.Name = "buttonTest";
            buttonTest.Size = new Size(112, 34);
            buttonTest.TabIndex = 4;
            buttonTest.Text = "test";
            buttonTest.UseVisualStyleBackColor = true;
            buttonTest.Click += ButtonTest_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1332, 807);
            Controls.Add(buttonTest);
            Controls.Add(buttonOverlayed);
            Controls.Add(buttonClipboard);
            Controls.Add(buttonDrawAll);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Button buttonDrawAll;
        private Button buttonClipboard;
        private Button buttonOverlayed;
        private Button buttonTest;
    }
}
