namespace PikachuClassic
{
    partial class GameForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.infoPanel = new System.Windows.Forms.Panel();
            this.scoreLbP2 = new System.Windows.Forms.Label();
            this.timeLb = new System.Windows.Forms.Label();
            this.scoreLbP1 = new System.Windows.Forms.Label();
            this.gamePanel = new System.Windows.Forms.Panel();
            this.gridPanel = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.infoPanel.SuspendLayout();
            this.gamePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = System.Drawing.Color.White;
            this.infoPanel.Controls.Add(this.button4);
            this.infoPanel.Controls.Add(this.scoreLbP2);
            this.infoPanel.Controls.Add(this.timeLb);
            this.infoPanel.Controls.Add(this.scoreLbP1);
            this.infoPanel.Location = new System.Drawing.Point(3, 3);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(902, 60);
            this.infoPanel.TabIndex = 0;
            // 
            // scoreLbP2
            // 
            this.scoreLbP2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.scoreLbP2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.scoreLbP2.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scoreLbP2.Location = new System.Drawing.Point(708, 10);
            this.scoreLbP2.Name = "scoreLbP2";
            this.scoreLbP2.Size = new System.Drawing.Size(191, 38);
            this.scoreLbP2.TabIndex = 2;
            this.scoreLbP2.Text = "Score P2: 0";
            this.scoreLbP2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timeLb
            // 
            this.timeLb.BackColor = System.Drawing.SystemColors.ControlDark;
            this.timeLb.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.timeLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLb.Location = new System.Drawing.Point(371, 10);
            this.timeLb.Name = "timeLb";
            this.timeLb.Size = new System.Drawing.Size(164, 38);
            this.timeLb.TabIndex = 1;
            this.timeLb.Text = "Time Left: ";
            this.timeLb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.timeLb.Click += new System.EventHandler(this.timeLb_Click);
            // 
            // scoreLbP1
            // 
            this.scoreLbP1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.scoreLbP1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.scoreLbP1.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scoreLbP1.Location = new System.Drawing.Point(3, 10);
            this.scoreLbP1.Name = "scoreLbP1";
            this.scoreLbP1.Size = new System.Drawing.Size(191, 38);
            this.scoreLbP1.TabIndex = 0;
            this.scoreLbP1.Text = "Score P1: 0";
            this.scoreLbP1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gamePanel
            // 
            this.gamePanel.BackColor = System.Drawing.Color.Transparent;
            this.gamePanel.Controls.Add(this.gridPanel);
            this.gamePanel.Controls.Add(this.infoPanel);
            this.gamePanel.Location = new System.Drawing.Point(12, 13);
            this.gamePanel.Name = "gamePanel";
            this.gamePanel.Size = new System.Drawing.Size(908, 506);
            this.gamePanel.TabIndex = 0;
            // 
            // gridPanel
            // 
            this.gridPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.gridPanel.Location = new System.Drawing.Point(7, 70);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(898, 433);
            this.gridPanel.TabIndex = 1;
            this.gridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gridPanel_Paint);
            // 
            // button4
            // 
            this.button4.BackgroundImage = global::PikachuClassic.Properties.Resources.tutorial1;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button4.Location = new System.Drawing.Point(590, 0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(57, 57);
            this.button4.TabIndex = 4;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // GameForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(932, 553);
            this.Controls.Add(this.gamePanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PikachuProMax";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GameController_FormClosed);
            this.Load += new System.EventHandler(this.GameController_Load);
            this.infoPanel.ResumeLayout(false);
            this.gamePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.Label timeLb;
        private System.Windows.Forms.Label scoreLbP1;
        private System.Windows.Forms.Panel gamePanel;
        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.Label scoreLbP2;
        private System.Windows.Forms.Button button4;
    }
}

