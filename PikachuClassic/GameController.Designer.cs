namespace PikachuClassic
{
    partial class GameController
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameController));
            this.infoPanel = new System.Windows.Forms.Panel();
            this.timeLb = new System.Windows.Forms.Label();
            this.scoreLb = new System.Windows.Forms.Label();
            this.gamePanel = new System.Windows.Forms.Panel();
            this.gridPanel = new System.Windows.Forms.Panel();
            this.infoPanel.SuspendLayout();
            this.gamePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            this.infoPanel.Controls.Add(this.timeLb);
            this.infoPanel.Controls.Add(this.scoreLb);
            this.infoPanel.Location = new System.Drawing.Point(3, 3);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(665, 60);
            this.infoPanel.TabIndex = 0;
            // 
            // timeLb
            // 
            this.timeLb.BackColor = System.Drawing.SystemColors.ControlDark;
            this.timeLb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.timeLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLb.Location = new System.Drawing.Point(291, 10);
            this.timeLb.Name = "timeLb";
            this.timeLb.Size = new System.Drawing.Size(164, 38);
            this.timeLb.TabIndex = 1;
            this.timeLb.Text = "Time Left: ";
            this.timeLb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // scoreLb
            // 
            this.scoreLb.BackColor = System.Drawing.SystemColors.ControlDark;
            this.scoreLb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.scoreLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scoreLb.Location = new System.Drawing.Point(3, 10);
            this.scoreLb.Name = "scoreLb";
            this.scoreLb.Size = new System.Drawing.Size(139, 38);
            this.scoreLb.TabIndex = 0;
            this.scoreLb.Text = "Score: 0";
            this.scoreLb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // 
            // gamePanel
            // 
            this.gamePanel.BackColor = System.Drawing.Color.Transparent;
            this.gamePanel.Controls.Add(this.gridPanel);
            this.gamePanel.Controls.Add(this.infoPanel);
            this.gamePanel.Location = new System.Drawing.Point(27, 13);
            this.gamePanel.Name = "gamePanel";
            this.gamePanel.Size = new System.Drawing.Size(672, 506);
            this.gamePanel.TabIndex = 0;
            // 
            // gridPanel
            // 
            this.gridPanel.Location = new System.Drawing.Point(7, 70);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(661, 433);
            this.gridPanel.TabIndex = 1;
            // 
            // GameController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(727, 531);
            this.Controls.Add(this.gamePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "GameController";
            this.Text = "PikachuProMax";
            this.infoPanel.ResumeLayout(false);
            this.gamePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.Label timeLb;
        private System.Windows.Forms.Label scoreLb;
        private System.Windows.Forms.Panel gamePanel;
        private System.Windows.Forms.Panel gridPanel;
    }
}

