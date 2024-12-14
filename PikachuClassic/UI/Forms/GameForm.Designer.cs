namespace PikachuClassic.UI.Forms
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.Label scoreLbP1;
        private System.Windows.Forms.Label scoreLbP2;
        private System.Windows.Forms.Label timeLb;

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
            this.gridPanel = new System.Windows.Forms.Panel();
            this.scoreLbP1 = new System.Windows.Forms.Label();
            this.scoreLbP2 = new System.Windows.Forms.Label();
            this.timeLb = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // gridPanel
            // 
            this.gridPanel.Location = new System.Drawing.Point(12, 50);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(416, 416);
            this.gridPanel.TabIndex = 0;
            // 
            // scoreLbP1
            // 
            this.scoreLbP1.AutoSize = true;
            this.scoreLbP1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scoreLbP1.Location = new System.Drawing.Point(12, 20);
            this.scoreLbP1.Name = "scoreLbP1";
            this.scoreLbP1.Size = new System.Drawing.Size(60, 17);
            this.scoreLbP1.TabIndex = 1;
            this.scoreLbP1.Text = "P1: 0";
            // 
            // scoreLbP2
            // 
            this.scoreLbP2.AutoSize = true;
            this.scoreLbP2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scoreLbP2.Location = new System.Drawing.Point(150, 20);
            this.scoreLbP2.Name = "scoreLbP2";
            this.scoreLbP2.Size = new System.Drawing.Size(60, 17);
            this.scoreLbP2.TabIndex = 2;
            this.scoreLbP2.Text = "P2: 0";
            // 
            // timeLb
            // 
            this.timeLb.AutoSize = true;
            this.timeLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLb.Location = new System.Drawing.Point(300, 20);
            this.timeLb.Name = "timeLb";
            this.timeLb.Size = new System.Drawing.Size(75, 17);
            this.timeLb.TabIndex = 3;
            this.timeLb.Text = "Time: 0s";
            // 
            // GameForm
            // 
            this.ClientSize = new System.Drawing.Size(440, 480);
            this.Controls.Add(this.timeLb);
            this.Controls.Add(this.scoreLbP2);
            this.Controls.Add(this.scoreLbP1);
            this.Controls.Add(this.gridPanel);
            this.Name = "GameForm";
            this.Text = "PikachuClassic - Game";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

