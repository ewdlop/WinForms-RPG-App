namespace WinFormsApp1
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
            this.components = new System.ComponentModel.Container();
            this.gameTextBox = new System.Windows.Forms.RichTextBox();
            this.commandTextBox = new System.Windows.Forms.TextBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.newGameButton = new System.Windows.Forms.Button();
            this.loadGameButton = new System.Windows.Forms.Button();
            this.saveGameButton = new System.Windows.Forms.Button();
            this.inventoryButton = new System.Windows.Forms.Button();
            this.statsButton = new System.Windows.Forms.Button();
            this.skillsButton = new System.Windows.Forms.Button();
            this.helpButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.gamePanel = new System.Windows.Forms.Panel();
            this.statusStrip.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.gamePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameTextBox
            // 
            this.gameTextBox.BackColor = System.Drawing.Color.Black;
            this.gameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameTextBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.gameTextBox.ForeColor = System.Drawing.Color.White;
            this.gameTextBox.Location = new System.Drawing.Point(0, 0);
            this.gameTextBox.Name = "gameTextBox";
            this.gameTextBox.ReadOnly = true;
            this.gameTextBox.Size = new System.Drawing.Size(784, 350);
            this.gameTextBox.TabIndex = 0;
            this.gameTextBox.Text = "";
            // 
            // commandTextBox
            // 
            this.commandTextBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.commandTextBox.Font = new System.Drawing.Font("Consolas", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.commandTextBox.Location = new System.Drawing.Point(0, 350);
            this.commandTextBox.Name = "commandTextBox";
            this.commandTextBox.PlaceholderText = "Enter command here...";
            this.commandTextBox.Size = new System.Drawing.Size(784, 23);
            this.commandTextBox.TabIndex = 1;
            this.commandTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.commandTextBox_KeyDown);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(118, 15);
            this.statusLabel.Text = "Welcome to the game!";
            // 
            // newGameButton
            // 
            this.newGameButton.Location = new System.Drawing.Point(10, 10);
            this.newGameButton.Name = "newGameButton";
            this.newGameButton.Size = new System.Drawing.Size(75, 30);
            this.newGameButton.TabIndex = 2;
            this.newGameButton.Text = "New Game";
            this.newGameButton.UseVisualStyleBackColor = true;
            this.newGameButton.Click += new System.EventHandler(this.newGameButton_Click);
            // 
            // loadGameButton
            // 
            this.loadGameButton.Location = new System.Drawing.Point(95, 10);
            this.loadGameButton.Name = "loadGameButton";
            this.loadGameButton.Size = new System.Drawing.Size(75, 30);
            this.loadGameButton.TabIndex = 3;
            this.loadGameButton.Text = "Load Game";
            this.loadGameButton.UseVisualStyleBackColor = true;
            this.loadGameButton.Click += new System.EventHandler(this.loadGameButton_Click);
            // 
            // saveGameButton
            // 
            this.saveGameButton.Location = new System.Drawing.Point(180, 10);
            this.saveGameButton.Name = "saveGameButton";
            this.saveGameButton.Size = new System.Drawing.Size(75, 30);
            this.saveGameButton.TabIndex = 4;
            this.saveGameButton.Text = "Save Game";
            this.saveGameButton.UseVisualStyleBackColor = true;
            this.saveGameButton.Click += new System.EventHandler(this.saveGameButton_Click);
            // 
            // inventoryButton
            // 
            this.inventoryButton.Location = new System.Drawing.Point(265, 10);
            this.inventoryButton.Name = "inventoryButton";
            this.inventoryButton.Size = new System.Drawing.Size(75, 30);
            this.inventoryButton.TabIndex = 5;
            this.inventoryButton.Text = "Inventory";
            this.inventoryButton.UseVisualStyleBackColor = true;
            this.inventoryButton.Click += new System.EventHandler(this.inventoryButton_Click);
            // 
            // statsButton
            // 
            this.statsButton.Location = new System.Drawing.Point(350, 10);
            this.statsButton.Name = "statsButton";
            this.statsButton.Size = new System.Drawing.Size(75, 30);
            this.statsButton.TabIndex = 6;
            this.statsButton.Text = "Stats";
            this.statsButton.UseVisualStyleBackColor = true;
            this.statsButton.Click += new System.EventHandler(this.statsButton_Click);
            // 
            // skillsButton
            // 
            this.skillsButton.Location = new System.Drawing.Point(435, 10);
            this.skillsButton.Name = "skillsButton";
            this.skillsButton.Size = new System.Drawing.Size(75, 30);
            this.skillsButton.TabIndex = 7;
            this.skillsButton.Text = "Skills";
            this.skillsButton.UseVisualStyleBackColor = true;
            this.skillsButton.Click += new System.EventHandler(this.skillsButton_Click);
            // 
            // helpButton
            // 
            this.helpButton.Location = new System.Drawing.Point(520, 10);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(75, 30);
            this.helpButton.TabIndex = 8;
            this.helpButton.Text = "Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 428);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(800, 22);
            this.statusStrip.TabIndex = 9;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel.Text = "Welcome to the game!";
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.newGameButton);
            this.buttonPanel.Controls.Add(this.loadGameButton);
            this.buttonPanel.Controls.Add(this.saveGameButton);
            this.buttonPanel.Controls.Add(this.inventoryButton);
            this.buttonPanel.Controls.Add(this.statsButton);
            this.buttonPanel.Controls.Add(this.skillsButton);
            this.buttonPanel.Controls.Add(this.helpButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonPanel.Location = new System.Drawing.Point(0, 0);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(800, 55);
            this.buttonPanel.TabIndex = 10;
            // 
            // gamePanel
            // 
            this.gamePanel.Controls.Add(this.gameTextBox);
            this.gamePanel.Controls.Add(this.commandTextBox);
            this.gamePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gamePanel.Location = new System.Drawing.Point(0, 55);
            this.gamePanel.Name = "gamePanel";
            this.gamePanel.Size = new System.Drawing.Size(784, 373);
            this.gamePanel.TabIndex = 11;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.gamePanel);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.statusStrip);
            this.Name = "Form1";
            this.Text = "Realm of Aethermoor - RPG Adventure";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.gamePanel.ResumeLayout(false);
            this.gamePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.RichTextBox gameTextBox;
        private System.Windows.Forms.TextBox commandTextBox;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Button newGameButton;
        private System.Windows.Forms.Button loadGameButton;
        private System.Windows.Forms.Button saveGameButton;
        private System.Windows.Forms.Button inventoryButton;
        private System.Windows.Forms.Button statsButton;
        private System.Windows.Forms.Button skillsButton;
        private System.Windows.Forms.Button helpButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Panel gamePanel;
    }
}
