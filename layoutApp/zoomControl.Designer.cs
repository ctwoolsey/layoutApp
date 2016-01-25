namespace layoutApp
{
    partial class zoomControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.zoomBar = new System.Windows.Forms.TrackBar();
            this.zoomTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fitToScreenButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.zoomBar)).BeginInit();
            this.SuspendLayout();
            // 
            // zoomBar
            // 
            this.zoomBar.Location = new System.Drawing.Point(3, 0);
            this.zoomBar.Margin = new System.Windows.Forms.Padding(0);
            this.zoomBar.Maximum = 400;
            this.zoomBar.Name = "zoomBar";
            this.zoomBar.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.zoomBar.Size = new System.Drawing.Size(45, 222);
            this.zoomBar.TabIndex = 0;
            this.zoomBar.TickFrequency = 0;
            this.zoomBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.zoomBar.Value = 100;
            this.zoomBar.Scroll += new System.EventHandler(this.zoomBar_Scroll);
            // 
            // zoomTB
            // 
            this.zoomTB.Location = new System.Drawing.Point(5, 262);
            this.zoomTB.Name = "zoomTB";
            this.zoomTB.Size = new System.Drawing.Size(36, 20);
            this.zoomTB.TabIndex = 1;
            this.zoomTB.TextChanged += new System.EventHandler(this.zoomTB_TextChanged);
            this.zoomTB.Leave += new System.EventHandler(this.zoomTB_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 246);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Scale";
            // 
            // fitToScreenButton
            // 
            this.fitToScreenButton.Location = new System.Drawing.Point(3, 305);
            this.fitToScreenButton.Name = "fitToScreenButton";
            this.fitToScreenButton.Size = new System.Drawing.Size(42, 28);
            this.fitToScreenButton.TabIndex = 3;
            this.fitToScreenButton.Text = "Fit";
            this.fitToScreenButton.UseVisualStyleBackColor = true;
            this.fitToScreenButton.Click += new System.EventHandler(this.fitToScreenButton_Click);
            // 
            // zoomControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fitToScreenButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.zoomTB);
            this.Controls.Add(this.zoomBar);
            this.Name = "zoomControl";
            this.Size = new System.Drawing.Size(48, 341);
            ((System.ComponentModel.ISupportInitialize)(this.zoomBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar zoomBar;
        private System.Windows.Forms.TextBox zoomTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button fitToScreenButton;
    }
}
