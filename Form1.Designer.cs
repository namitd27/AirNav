namespace AirNav
{
    partial class AirNavForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AirNavForm));
            this.display = new Emgu.CV.UI.ImageBox();
            this.settingsBtn = new System.Windows.Forms.Button();
            this.startBtn = new System.Windows.Forms.Button();
            this.presentationBtn = new System.Windows.Forms.Button();
            this.AirNav = new System.Windows.Forms.NotifyIcon(this.components);
            this.SystemTrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.restoreAirNavToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.togglePresentationModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitApplicationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.invertScroll = new System.Windows.Forms.CheckBox();
            this.disableTipsCheckbox = new System.Windows.Forms.CheckBox();
            this.coordinates = new System.Windows.Forms.CheckBox();
            this.displayDefault = new System.Windows.Forms.CheckBox();
            this.sensitivitySlider = new System.Windows.Forms.TrackBar();
            this.leaveSettingsBtn = new System.Windows.Forms.Button();
            this.quitAirNavBtn = new System.Windows.Forms.Button();
            this.coordinatesTextBox = new System.Windows.Forms.Label();
            this.settingsTextBox = new System.Windows.Forms.Label();
            this.sensitivityTextBox = new System.Windows.Forms.Label();
            this.gloveMode = new System.Windows.Forms.CheckBox();
            this.gloveColor = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.display)).BeginInit();
            this.SystemTrayMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sensitivitySlider)).BeginInit();
            this.SuspendLayout();
            // 
            // display
            // 
            this.display.Location = new System.Drawing.Point(0, 0);
            this.display.Name = "display";
            this.display.Size = new System.Drawing.Size(320, 248);
            this.display.TabIndex = 2;
            this.display.TabStop = false;
            this.display.DoubleClick += new System.EventHandler(this.display_DoubleClick);
            // 
            // settingsBtn
            // 
            this.settingsBtn.Image = ((System.Drawing.Image)(resources.GetObject("settingsBtn.Image")));
            this.settingsBtn.Location = new System.Drawing.Point(12, 254);
            this.settingsBtn.Name = "settingsBtn";
            this.settingsBtn.Size = new System.Drawing.Size(43, 39);
            this.settingsBtn.TabIndex = 3;
            this.settingsBtn.UseVisualStyleBackColor = true;
            this.settingsBtn.Click += new System.EventHandler(this.settingsBtn_Click);
            // 
            // startBtn
            // 
            this.startBtn.Location = new System.Drawing.Point(206, 254);
            this.startBtn.Name = "startBtn";
            this.startBtn.Size = new System.Drawing.Size(102, 39);
            this.startBtn.TabIndex = 4;
            this.startBtn.Text = "Navigate!";
            this.startBtn.UseVisualStyleBackColor = true;
            this.startBtn.Click += new System.EventHandler(this.startBtn_Click);
            // 
            // presentationBtn
            // 
            this.presentationBtn.Enabled = false;
            this.presentationBtn.Location = new System.Drawing.Point(61, 254);
            this.presentationBtn.Name = "presentationBtn";
            this.presentationBtn.Size = new System.Drawing.Size(139, 39);
            this.presentationBtn.TabIndex = 5;
            this.presentationBtn.Text = "Presentation Mode";
            this.presentationBtn.UseVisualStyleBackColor = true;
            this.presentationBtn.Click += new System.EventHandler(this.presentationBtn_Click);
            // 
            // AirNav
            // 
            this.AirNav.BalloonTipText = "Welcome to AirNav";
            this.AirNav.BalloonTipTitle = "Hello!";
            this.AirNav.ContextMenuStrip = this.SystemTrayMenu;
            this.AirNav.Icon = ((System.Drawing.Icon)(resources.GetObject("AirNav.Icon")));
            this.AirNav.Text = "AirNav (Running in Background)";
            this.AirNav.Visible = true;
            this.AirNav.Click += new System.EventHandler(this.AirNav_Click);
            // 
            // SystemTrayMenu
            // 
            this.SystemTrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.restoreAirNavToolStripMenuItem,
            this.togglePresentationModeToolStripMenuItem,
            this.quitApplicationToolStripMenuItem});
            this.SystemTrayMenu.Name = "SystemTrayMenu";
            this.SystemTrayMenu.Size = new System.Drawing.Size(215, 70);
            // 
            // restoreAirNavToolStripMenuItem
            // 
            this.restoreAirNavToolStripMenuItem.Name = "restoreAirNavToolStripMenuItem";
            this.restoreAirNavToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.restoreAirNavToolStripMenuItem.Text = "Restore AirNav";
            this.restoreAirNavToolStripMenuItem.Click += new System.EventHandler(this.restoreAirNavToolStripMenuItem_Click);
            // 
            // togglePresentationModeToolStripMenuItem
            // 
            this.togglePresentationModeToolStripMenuItem.Name = "togglePresentationModeToolStripMenuItem";
            this.togglePresentationModeToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.togglePresentationModeToolStripMenuItem.Text = "Toggle Presentation Mode";
            this.togglePresentationModeToolStripMenuItem.Click += new System.EventHandler(this.presentationBtn_Click);
            // 
            // quitApplicationToolStripMenuItem
            // 
            this.quitApplicationToolStripMenuItem.Name = "quitApplicationToolStripMenuItem";
            this.quitApplicationToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.quitApplicationToolStripMenuItem.Text = "Quit Application";
            this.quitApplicationToolStripMenuItem.Click += new System.EventHandler(this.quitApplicationToolStripMenuItem_Click);
            // 
            // invertScroll
            // 
            this.invertScroll.AutoSize = true;
            this.invertScroll.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.invertScroll.Location = new System.Drawing.Point(12, 76);
            this.invertScroll.Name = "invertScroll";
            this.invertScroll.Size = new System.Drawing.Size(115, 23);
            this.invertScroll.TabIndex = 7;
            this.invertScroll.Text = "Invert Scrolling";
            this.invertScroll.UseVisualStyleBackColor = true;
            this.invertScroll.Visible = false;
            // 
            // disableTipsCheckbox
            // 
            this.disableTipsCheckbox.AutoSize = true;
            this.disableTipsCheckbox.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.disableTipsCheckbox.Location = new System.Drawing.Point(157, 76);
            this.disableTipsCheckbox.Name = "disableTipsCheckbox";
            this.disableTipsCheckbox.Size = new System.Drawing.Size(151, 23);
            this.disableTipsCheckbox.TabIndex = 7;
            this.disableTipsCheckbox.Text = "Disable Random Tips";
            this.disableTipsCheckbox.UseVisualStyleBackColor = true;
            this.disableTipsCheckbox.Visible = false;
            // 
            // coordinates
            // 
            this.coordinates.AutoSize = true;
            this.coordinates.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.coordinates.Location = new System.Drawing.Point(12, 111);
            this.coordinates.Name = "coordinates";
            this.coordinates.Size = new System.Drawing.Size(143, 23);
            this.coordinates.TabIndex = 7;
            this.coordinates.Text = "Show Co-Ordinates";
            this.coordinates.UseVisualStyleBackColor = true;
            this.coordinates.Visible = false;
            // 
            // displayDefault
            // 
            this.displayDefault.AutoSize = true;
            this.displayDefault.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.displayDefault.Location = new System.Drawing.Point(157, 111);
            this.displayDefault.Name = "displayDefault";
            this.displayDefault.Size = new System.Drawing.Size(161, 23);
            this.displayDefault.TabIndex = 7;
            this.displayDefault.Text = "Default to Grey Display";
            this.displayDefault.UseVisualStyleBackColor = true;
            this.displayDefault.Visible = false;
            // 
            // sensitivitySlider
            // 
            this.sensitivitySlider.Location = new System.Drawing.Point(83, 201);
            this.sensitivitySlider.Maximum = 60;
            this.sensitivitySlider.Name = "sensitivitySlider";
            this.sensitivitySlider.Size = new System.Drawing.Size(151, 45);
            this.sensitivitySlider.TabIndex = 9;
            this.sensitivitySlider.TickFrequency = 10;
            this.sensitivitySlider.Value = 30;
            this.sensitivitySlider.Visible = false;
            // 
            // leaveSettingsBtn
            // 
            this.leaveSettingsBtn.Location = new System.Drawing.Point(12, 254);
            this.leaveSettingsBtn.Name = "leaveSettingsBtn";
            this.leaveSettingsBtn.Size = new System.Drawing.Size(143, 39);
            this.leaveSettingsBtn.TabIndex = 11;
            this.leaveSettingsBtn.Text = "Leave Settings";
            this.leaveSettingsBtn.UseVisualStyleBackColor = true;
            this.leaveSettingsBtn.Visible = false;
            this.leaveSettingsBtn.Click += new System.EventHandler(this.leaveSettingsBtn_Click);
            // 
            // quitAirNavBtn
            // 
            this.quitAirNavBtn.Location = new System.Drawing.Point(161, 254);
            this.quitAirNavBtn.Name = "quitAirNavBtn";
            this.quitAirNavBtn.Size = new System.Drawing.Size(147, 39);
            this.quitAirNavBtn.TabIndex = 12;
            this.quitAirNavBtn.Text = "Quit AirNav";
            this.quitAirNavBtn.UseVisualStyleBackColor = true;
            this.quitAirNavBtn.Visible = false;
            this.quitAirNavBtn.Click += new System.EventHandler(this.quitApplicationToolStripMenuItem_Click);
            // 
            // coordinatesTextBox
            // 
            this.coordinatesTextBox.Location = new System.Drawing.Point(0, 0);
            this.coordinatesTextBox.Name = "coordinatesTextBox";
            this.coordinatesTextBox.Size = new System.Drawing.Size(85, 20);
            this.coordinatesTextBox.TabIndex = 13;
            this.coordinatesTextBox.Visible = false;
            // 
            // settingsTextBox
            // 
            this.settingsTextBox.Font = new System.Drawing.Font("Segoe UI Light", 20F);
            this.settingsTextBox.Location = new System.Drawing.Point(12, 21);
            this.settingsTextBox.Name = "settingsTextBox";
            this.settingsTextBox.Size = new System.Drawing.Size(188, 43);
            this.settingsTextBox.TabIndex = 14;
            this.settingsTextBox.Text = "Settings";
            this.settingsTextBox.Visible = false;
            // 
            // sensitivityTextBox
            // 
            this.sensitivityTextBox.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sensitivityTextBox.Location = new System.Drawing.Point(83, 173);
            this.sensitivityTextBox.Name = "sensitivityTextBox";
            this.sensitivityTextBox.Size = new System.Drawing.Size(151, 25);
            this.sensitivityTextBox.TabIndex = 15;
            this.sensitivityTextBox.Text = "Sensitivity:";
            this.sensitivityTextBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.sensitivityTextBox.Visible = false;
            // 
            // gloveMode
            // 
            this.gloveMode.AutoSize = true;
            this.gloveMode.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gloveMode.Location = new System.Drawing.Point(12, 144);
            this.gloveMode.Name = "gloveMode";
            this.gloveMode.Size = new System.Drawing.Size(138, 23);
            this.gloveMode.TabIndex = 16;
            this.gloveMode.Text = "Glove Mode (Beta)";
            this.gloveMode.UseVisualStyleBackColor = true;
            this.gloveMode.Visible = false;
            // 
            // gloveColor
            // 
            this.gloveColor.Enabled = false;
            this.gloveColor.FormattingEnabled = true;
            this.gloveColor.Items.AddRange(new object[] {
            "Black",
            "Blue",
            "Green",
            "Grey",
            "Red",
            "Yellow"});
            this.gloveColor.Location = new System.Drawing.Point(161, 146);
            this.gloveColor.Name = "gloveColor";
            this.gloveColor.Size = new System.Drawing.Size(147, 21);
            this.gloveColor.TabIndex = 17;
            this.gloveColor.Visible = false;
            // 
            // AirNavForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(320, 305);
            this.Controls.Add(this.gloveColor);
            this.Controls.Add(this.gloveMode);
            this.Controls.Add(this.sensitivityTextBox);
            this.Controls.Add(this.settingsTextBox);
            this.Controls.Add(this.coordinatesTextBox);
            this.Controls.Add(this.sensitivitySlider);
            this.Controls.Add(this.displayDefault);
            this.Controls.Add(this.disableTipsCheckbox);
            this.Controls.Add(this.coordinates);
            this.Controls.Add(this.invertScroll);
            this.Controls.Add(this.presentationBtn);
            this.Controls.Add(this.startBtn);
            this.Controls.Add(this.settingsBtn);
            this.Controls.Add(this.display);
            this.Controls.Add(this.leaveSettingsBtn);
            this.Controls.Add(this.quitAirNavBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "AirNavForm";
            this.Text = "AirNav";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AirNavForm_FormClosing);
            this.Load += new System.EventHandler(this.AirNav_Load);
            ((System.ComponentModel.ISupportInitialize)(this.display)).EndInit();
            this.SystemTrayMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sensitivitySlider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Emgu.CV.UI.ImageBox display;
        private System.Windows.Forms.Button settingsBtn;
        private System.Windows.Forms.Button startBtn;
        private System.Windows.Forms.Button presentationBtn;
        private System.Windows.Forms.NotifyIcon AirNav;
        private System.Windows.Forms.ContextMenuStrip SystemTrayMenu;
        private System.Windows.Forms.ToolStripMenuItem restoreAirNavToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitApplicationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem togglePresentationModeToolStripMenuItem;
        private System.Windows.Forms.CheckBox invertScroll;
        private System.Windows.Forms.CheckBox disableTipsCheckbox;
        private System.Windows.Forms.CheckBox coordinates;
        private System.Windows.Forms.CheckBox displayDefault;
        private System.Windows.Forms.TrackBar sensitivitySlider;
        private System.Windows.Forms.Button leaveSettingsBtn;
        private System.Windows.Forms.Button quitAirNavBtn;
        private System.Windows.Forms.Label coordinatesTextBox;
        private System.Windows.Forms.Label settingsTextBox;
        private System.Windows.Forms.Label sensitivityTextBox;
        private System.Windows.Forms.CheckBox gloveMode;
        private System.Windows.Forms.ComboBox gloveColor;
    }
}

