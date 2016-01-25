namespace layoutApp
{
    partial class albumControlBar
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
            this.label1 = new System.Windows.Forms.Label();
            this.previousPageBtn = new System.Windows.Forms.Button();
            this.nextPageBtn = new System.Windows.Forms.Button();
            this.spreadDescriptionTB = new System.Windows.Forms.TextBox();
            this.newSinglePageBtn = new System.Windows.Forms.Button();
            this.newSpreadBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Page/Spread Description";
            // 
            // previousPageBtn
            // 
            this.previousPageBtn.Location = new System.Drawing.Point(3, 3);
            this.previousPageBtn.Name = "previousPageBtn";
            this.previousPageBtn.Size = new System.Drawing.Size(75, 23);
            this.previousPageBtn.TabIndex = 1;
            this.previousPageBtn.Text = "Previous";
            this.previousPageBtn.UseVisualStyleBackColor = true;
            // 
            // nextPageBtn
            // 
            this.nextPageBtn.Location = new System.Drawing.Point(476, 3);
            this.nextPageBtn.Name = "nextPageBtn";
            this.nextPageBtn.Size = new System.Drawing.Size(75, 23);
            this.nextPageBtn.TabIndex = 2;
            this.nextPageBtn.Text = "Next";
            this.nextPageBtn.UseVisualStyleBackColor = true;
            // 
            // spreadDescriptionTB
            // 
            this.spreadDescriptionTB.Location = new System.Drawing.Point(213, 5);
            this.spreadDescriptionTB.Name = "spreadDescriptionTB";
            this.spreadDescriptionTB.Size = new System.Drawing.Size(246, 20);
            this.spreadDescriptionTB.TabIndex = 3;
            // 
            // newSinglePageBtn
            // 
            this.newSinglePageBtn.Location = new System.Drawing.Point(561, 2);
            this.newSinglePageBtn.Name = "newSinglePageBtn";
            this.newSinglePageBtn.Size = new System.Drawing.Size(86, 23);
            this.newSinglePageBtn.TabIndex = 4;
            this.newSinglePageBtn.Text = "Add End Page";
            this.newSinglePageBtn.UseVisualStyleBackColor = true;
            this.newSinglePageBtn.Click += new System.EventHandler(this.newSinglePageBtn_Click);
            // 
            // newSpreadBtn
            // 
            this.newSpreadBtn.Location = new System.Drawing.Point(652, 2);
            this.newSpreadBtn.Name = "newSpreadBtn";
            this.newSpreadBtn.Size = new System.Drawing.Size(75, 23);
            this.newSpreadBtn.TabIndex = 5;
            this.newSpreadBtn.Text = "Add Spread";
            this.newSpreadBtn.UseVisualStyleBackColor = true;
            this.newSpreadBtn.Click += new System.EventHandler(this.newSpreadBtn_Click);
            // 
            // albumControlBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.newSpreadBtn);
            this.Controls.Add(this.newSinglePageBtn);
            this.Controls.Add(this.spreadDescriptionTB);
            this.Controls.Add(this.nextPageBtn);
            this.Controls.Add(this.previousPageBtn);
            this.Controls.Add(this.label1);
            this.Name = "albumControlBar";
            this.Size = new System.Drawing.Size(734, 30);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.albumControlBar_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button previousPageBtn;
        private System.Windows.Forms.Button nextPageBtn;
        private System.Windows.Forms.TextBox spreadDescriptionTB;
        private System.Windows.Forms.Button newSinglePageBtn;
        private System.Windows.Forms.Button newSpreadBtn;
    }
}
