namespace GKCaptcha
{
    partial class DataBaseCollectorForm
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
            this.imagePanel = new System.Windows.Forms.Panel();
            this.saveButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.answerTextBox = new System.Windows.Forms.TextBox();
            this.hintImagePanel = new System.Windows.Forms.Panel();
            this.randLabel = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // imagePanel
            // 
            this.imagePanel.Location = new System.Drawing.Point(12, 12);
            this.imagePanel.Name = "imagePanel";
            this.imagePanel.Size = new System.Drawing.Size(260, 100);
            this.imagePanel.TabIndex = 0;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(12, 301);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 1;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(102, 301);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 2;
            this.nextButton.Text = "Next";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // answerTextBox
            // 
            this.answerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.answerTextBox.Location = new System.Drawing.Point(12, 251);
            this.answerTextBox.Name = "answerTextBox";
            this.answerTextBox.Size = new System.Drawing.Size(165, 44);
            this.answerTextBox.TabIndex = 3;
            this.answerTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.answerTextBox_KeyDown);
            // 
            // hintImagePanel
            // 
            this.hintImagePanel.Location = new System.Drawing.Point(12, 118);
            this.hintImagePanel.Name = "hintImagePanel";
            this.hintImagePanel.Size = new System.Drawing.Size(260, 100);
            this.hintImagePanel.TabIndex = 1;
            // 
            // randLabel
            // 
            this.randLabel.Location = new System.Drawing.Point(12, 225);
            this.randLabel.Name = "randLabel";
            this.randLabel.Size = new System.Drawing.Size(100, 20);
            this.randLabel.TabIndex = 4;
            // 
            // DataBaseCollectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 331);
            this.Controls.Add(this.randLabel);
            this.Controls.Add(this.hintImagePanel);
            this.Controls.Add(this.answerTextBox);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.imagePanel);
            this.Name = "DataBaseCollectorForm";
            this.Text = "DataBaseCollectorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel imagePanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.TextBox answerTextBox;
        private System.Windows.Forms.Panel hintImagePanel;
        private System.Windows.Forms.TextBox randLabel;
    }
}