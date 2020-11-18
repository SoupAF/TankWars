namespace View
{
    public partial class GameWindow
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
            this.GamePanel = new View.DrawingPanel();
            this.SuspendLayout();
            // 
            // GamePanel
            // 
            this.GamePanel.Location = new System.Drawing.Point(3, -1);
            this.GamePanel.MaximumSize = new System.Drawing.Size(900, 900);
            this.GamePanel.MinimumSize = new System.Drawing.Size(900, 900);
            this.GamePanel.Name = "GamePanel";
            this.GamePanel.Size = new System.Drawing.Size(900, 900);
            this.GamePanel.TabIndex = 0;
            // 
            // GameWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(902, 920);
            this.Controls.Add(this.GamePanel);
            this.Name = "GameWindow";
            this.Text = " Tank Wars!";
            this.ResumeLayout(false);

        }

        #endregion

        public DrawingPanel GamePanel;

       

    }
}