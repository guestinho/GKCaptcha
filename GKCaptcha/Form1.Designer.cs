namespace GKCaptcha
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
            this.components = new System.ComponentModel.Container();
            this.teachButton = new System.Windows.Forms.Button();
            this.errorLabel = new System.Windows.Forms.Label();
            this.addSamplesButton = new System.Windows.Forms.Button();
            this.startServerButton = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.teachStopButton = new System.Windows.Forms.Button();
            this.teachProgressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // teachButton
            // 
            this.teachButton.Location = new System.Drawing.Point(863, 182);
            this.teachButton.Name = "teachButton";
            this.teachButton.Size = new System.Drawing.Size(75, 23);
            this.teachButton.TabIndex = 0;
            this.teachButton.Text = "Teach";
            this.teachButton.UseVisualStyleBackColor = true;
            this.teachButton.Click += new System.EventHandler(this.teachButton_Click);
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Location = new System.Drawing.Point(944, 192);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(31, 13);
            this.errorLabel.TabIndex = 1;
            this.errorLabel.Text = "error:";
            // 
            // addSamplesButton
            // 
            this.addSamplesButton.Location = new System.Drawing.Point(863, 42);
            this.addSamplesButton.Name = "addSamplesButton";
            this.addSamplesButton.Size = new System.Drawing.Size(75, 23);
            this.addSamplesButton.TabIndex = 2;
            this.addSamplesButton.Text = "Add";
            this.addSamplesButton.UseVisualStyleBackColor = true;
            this.addSamplesButton.Click += new System.EventHandler(this.addSamplesButton_Click);
            // 
            // startServerButton
            // 
            this.startServerButton.Location = new System.Drawing.Point(863, 71);
            this.startServerButton.Name = "startServerButton";
            this.startServerButton.Size = new System.Drawing.Size(75, 23);
            this.startServerButton.TabIndex = 3;
            this.startServerButton.Text = "Start Server";
            this.startServerButton.UseVisualStyleBackColor = true;
            this.startServerButton.Click += new System.EventHandler(this.startServerButton_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // teachStopButton
            // 
            this.teachStopButton.Location = new System.Drawing.Point(863, 211);
            this.teachStopButton.Name = "teachStopButton";
            this.teachStopButton.Size = new System.Drawing.Size(75, 23);
            this.teachStopButton.TabIndex = 4;
            this.teachStopButton.Text = "Stop";
            this.teachStopButton.UseVisualStyleBackColor = true;
            this.teachStopButton.Click += new System.EventHandler(this.teachStopButton_Click);
            // 
            // teachProgressBar
            // 
            this.teachProgressBar.Location = new System.Drawing.Point(944, 211);
            this.teachProgressBar.Name = "teachProgressBar";
            this.teachProgressBar.Size = new System.Drawing.Size(205, 23);
            this.teachProgressBar.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1171, 681);
            this.Controls.Add(this.teachProgressBar);
            this.Controls.Add(this.teachStopButton);
            this.Controls.Add(this.startServerButton);
            this.Controls.Add(this.addSamplesButton);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.teachButton);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button teachButton;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.Button addSamplesButton;
        private System.Windows.Forms.Button startServerButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button teachStopButton;
        private System.Windows.Forms.ProgressBar teachProgressBar;
    }
}

