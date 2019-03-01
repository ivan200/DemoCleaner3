namespace DemoCleaner2
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBoxBadDemos = new System.Windows.Forms.GroupBox();
            this.buttonBadDemosBrowse = new System.Windows.Forms.Button();
            this.radioButtonMoveBad = new System.Windows.Forms.RadioButton();
            this.textBoxBadDemos = new System.Windows.Forms.TextBox();
            this.radioButtonSkipBad = new System.Windows.Forms.RadioButton();
            this.radioButtonDeleteBad = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonBrowseDemos = new System.Windows.Forms.Button();
            this.textBoxDemosFolder = new System.Windows.Forms.TextBox();
            this.checkBoxUseSubfolders = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDownCountOfBest = new System.Windows.Forms.NumericUpDown();
            this.checkBoxProcessMdf = new System.Windows.Forms.CheckBox();
            this.radioBestTimeOfEachPlayer = new System.Windows.Forms.RadioButton();
            this.radioBestTimesOnMap = new System.Windows.Forms.RadioButton();
            this.buttonClean = new System.Windows.Forms.Button();
            this.numericUpDownMaxFiles = new System.Windows.Forms.NumericUpDown();
            this.buttonMove = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBoxSlowDemos = new System.Windows.Forms.GroupBox();
            this.buttonSlowDemosBrowse = new System.Windows.Forms.Button();
            this.radioButtonMoveSlow = new System.Windows.Forms.RadioButton();
            this.radioButtonSkipSlow = new System.Windows.Forms.RadioButton();
            this.radioButtonDeleteSlow = new System.Windows.Forms.RadioButton();
            this.textBoxSlowDemos = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.checkBoxMoveOnlyYour = new System.Windows.Forms.CheckBox();
            this.checkBoxSplitFolders = new System.Windows.Forms.CheckBox();
            this.groupBoxSplit = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDownMaxFolders = new System.Windows.Forms.NumericUpDown();
            this.groupBoxName = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxYourName = new System.Windows.Forms.TextBox();
            this.groupBoxMoveDemos = new System.Windows.Forms.GroupBox();
            this.buttonBrowseWhereMove = new System.Windows.Forms.Button();
            this.textBoxMoveDemosFolder = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.buttonSingleFileInfo = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.checkBoxDeleteIdentical = new System.Windows.Forms.CheckBox();
            this.checkBoxDeleteEmptyDirs = new System.Windows.Forms.CheckBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonRename = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBoxBadDemos.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountOfBest)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxFiles)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBoxSlowDemos.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBoxSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxFolders)).BeginInit();
            this.groupBoxName.SuspendLayout();
            this.groupBoxMoveDemos.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxBadDemos
            // 
            this.groupBoxBadDemos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxBadDemos.Controls.Add(this.buttonBadDemosBrowse);
            this.groupBoxBadDemos.Controls.Add(this.radioButtonMoveBad);
            this.groupBoxBadDemos.Controls.Add(this.textBoxBadDemos);
            this.groupBoxBadDemos.Controls.Add(this.radioButtonSkipBad);
            this.groupBoxBadDemos.Controls.Add(this.radioButtonDeleteBad);
            this.groupBoxBadDemos.Location = new System.Drawing.Point(6, 6);
            this.groupBoxBadDemos.Name = "groupBoxBadDemos";
            this.groupBoxBadDemos.Size = new System.Drawing.Size(325, 66);
            this.groupBoxBadDemos.TabIndex = 9;
            this.groupBoxBadDemos.TabStop = false;
            this.groupBoxBadDemos.Text = "Incorrectly named demos";
            // 
            // buttonBadDemosBrowse
            // 
            this.buttonBadDemosBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBadDemosBrowse.Location = new System.Drawing.Point(257, 37);
            this.buttonBadDemosBrowse.Name = "buttonBadDemosBrowse";
            this.buttonBadDemosBrowse.Size = new System.Drawing.Size(57, 23);
            this.buttonBadDemosBrowse.TabIndex = 8;
            this.buttonBadDemosBrowse.Text = "Browse";
            this.buttonBadDemosBrowse.UseVisualStyleBackColor = true;
            this.buttonBadDemosBrowse.Click += new System.EventHandler(this.buttonBadDemosBrowse_Click);
            // 
            // radioButtonMoveBad
            // 
            this.radioButtonMoveBad.AutoSize = true;
            this.radioButtonMoveBad.Checked = true;
            this.radioButtonMoveBad.Location = new System.Drawing.Point(156, 19);
            this.radioButtonMoveBad.Name = "radioButtonMoveBad";
            this.radioButtonMoveBad.Size = new System.Drawing.Size(67, 17);
            this.radioButtonMoveBad.TabIndex = 7;
            this.radioButtonMoveBad.TabStop = true;
            this.radioButtonMoveBad.Text = "Move to:";
            this.radioButtonMoveBad.UseVisualStyleBackColor = true;
            this.radioButtonMoveBad.CheckedChanged += new System.EventHandler(this.radioButtonMoveBad_CheckedChanged);
            // 
            // textBoxBadDemos
            // 
            this.textBoxBadDemos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBadDemos.Location = new System.Drawing.Point(156, 39);
            this.textBoxBadDemos.Name = "textBoxBadDemos";
            this.textBoxBadDemos.Size = new System.Drawing.Size(95, 20);
            this.textBoxBadDemos.TabIndex = 5;
            this.textBoxBadDemos.TextChanged += new System.EventHandler(this.textBoxBadDemos_TextChanged);
            // 
            // radioButtonSkipBad
            // 
            this.radioButtonSkipBad.AutoSize = true;
            this.radioButtonSkipBad.Location = new System.Drawing.Point(6, 42);
            this.radioButtonSkipBad.Name = "radioButtonSkipBad";
            this.radioButtonSkipBad.Size = new System.Drawing.Size(46, 17);
            this.radioButtonSkipBad.TabIndex = 6;
            this.radioButtonSkipBad.Text = "Skip";
            this.radioButtonSkipBad.UseVisualStyleBackColor = true;
            // 
            // radioButtonDeleteBad
            // 
            this.radioButtonDeleteBad.AutoSize = true;
            this.radioButtonDeleteBad.Location = new System.Drawing.Point(6, 19);
            this.radioButtonDeleteBad.Name = "radioButtonDeleteBad";
            this.radioButtonDeleteBad.Size = new System.Drawing.Size(56, 17);
            this.radioButtonDeleteBad.TabIndex = 5;
            this.radioButtonDeleteBad.Text = "Delete";
            this.radioButtonDeleteBad.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.buttonBrowseDemos);
            this.groupBox1.Controls.Add(this.textBoxDemosFolder);
            this.groupBox1.Controls.Add(this.checkBoxUseSubfolders);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(343, 50);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Demos is here:";
            // 
            // buttonBrowseDemos
            // 
            this.buttonBrowseDemos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseDemos.Location = new System.Drawing.Point(257, 17);
            this.buttonBrowseDemos.Name = "buttonBrowseDemos";
            this.buttonBrowseDemos.Size = new System.Drawing.Size(78, 23);
            this.buttonBrowseDemos.TabIndex = 7;
            this.buttonBrowseDemos.Text = "Browse";
            this.buttonBrowseDemos.UseVisualStyleBackColor = true;
            this.buttonBrowseDemos.Click += new System.EventHandler(this.buttonBrowseDemos_Click);
            // 
            // textBoxDemosFolder
            // 
            this.textBoxDemosFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDemosFolder.Location = new System.Drawing.Point(87, 19);
            this.textBoxDemosFolder.Name = "textBoxDemosFolder";
            this.textBoxDemosFolder.Size = new System.Drawing.Size(164, 20);
            this.textBoxDemosFolder.TabIndex = 5;
            this.textBoxDemosFolder.TextChanged += new System.EventHandler(this.textBoxDemosFolder_TextChanged);
            // 
            // checkBoxUseSubfolders
            // 
            this.checkBoxUseSubfolders.AutoSize = true;
            this.checkBoxUseSubfolders.Location = new System.Drawing.Point(10, 21);
            this.checkBoxUseSubfolders.Name = "checkBoxUseSubfolders";
            this.checkBoxUseSubfolders.Size = new System.Drawing.Size(76, 17);
            this.checkBoxUseSubfolders.TabIndex = 8;
            this.checkBoxUseSubfolders.Text = "Subfolders";
            this.checkBoxUseSubfolders.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.numericUpDownCountOfBest);
            this.groupBox4.Controls.Add(this.checkBoxProcessMdf);
            this.groupBox4.Controls.Add(this.radioBestTimeOfEachPlayer);
            this.groupBox4.Controls.Add(this.radioBestTimesOnMap);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(326, 66);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Clean option";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(153, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Count of best demos";
            // 
            // numericUpDownCountOfBest
            // 
            this.numericUpDownCountOfBest.Location = new System.Drawing.Point(273, 42);
            this.numericUpDownCountOfBest.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownCountOfBest.Name = "numericUpDownCountOfBest";
            this.numericUpDownCountOfBest.Size = new System.Drawing.Size(42, 20);
            this.numericUpDownCountOfBest.TabIndex = 24;
            this.numericUpDownCountOfBest.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // checkBoxProcessMdf
            // 
            this.checkBoxProcessMdf.AutoSize = true;
            this.checkBoxProcessMdf.Location = new System.Drawing.Point(156, 19);
            this.checkBoxProcessMdf.Name = "checkBoxProcessMdf";
            this.checkBoxProcessMdf.Size = new System.Drawing.Size(110, 17);
            this.checkBoxProcessMdf.TabIndex = 9;
            this.checkBoxProcessMdf.Text = "Process mdf as df";
            this.checkBoxProcessMdf.UseVisualStyleBackColor = true;
            // 
            // radioBestTimeOfEachPlayer
            // 
            this.radioBestTimeOfEachPlayer.AutoSize = true;
            this.radioBestTimeOfEachPlayer.Checked = true;
            this.radioBestTimeOfEachPlayer.Location = new System.Drawing.Point(6, 19);
            this.radioBestTimeOfEachPlayer.Name = "radioBestTimeOfEachPlayer";
            this.radioBestTimeOfEachPlayer.Size = new System.Drawing.Size(138, 17);
            this.radioBestTimeOfEachPlayer.TabIndex = 3;
            this.radioBestTimeOfEachPlayer.TabStop = true;
            this.radioBestTimeOfEachPlayer.Text = "Best time of each player";
            this.radioBestTimeOfEachPlayer.UseVisualStyleBackColor = true;
            // 
            // radioBestTimesOnMap
            // 
            this.radioBestTimesOnMap.AutoSize = true;
            this.radioBestTimesOnMap.Location = new System.Drawing.Point(6, 42);
            this.radioBestTimesOnMap.Name = "radioBestTimesOnMap";
            this.radioBestTimesOnMap.Size = new System.Drawing.Size(111, 17);
            this.radioBestTimesOnMap.TabIndex = 2;
            this.radioBestTimesOnMap.Text = "Best times on map";
            this.radioBestTimesOnMap.UseVisualStyleBackColor = true;
            // 
            // buttonClean
            // 
            this.buttonClean.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonClean.Location = new System.Drawing.Point(130, 149);
            this.buttonClean.Name = "buttonClean";
            this.buttonClean.Size = new System.Drawing.Size(79, 25);
            this.buttonClean.TabIndex = 16;
            this.buttonClean.Text = "Clean";
            this.buttonClean.UseVisualStyleBackColor = true;
            this.buttonClean.Click += new System.EventHandler(this.buttonClean_Click);
            // 
            // numericUpDownMaxFiles
            // 
            this.numericUpDownMaxFiles.Location = new System.Drawing.Point(119, 21);
            this.numericUpDownMaxFiles.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownMaxFiles.Minimum = new decimal(new int[] {
            63,
            0,
            0,
            0});
            this.numericUpDownMaxFiles.Name = "numericUpDownMaxFiles";
            this.numericUpDownMaxFiles.Size = new System.Drawing.Size(42, 20);
            this.numericUpDownMaxFiles.TabIndex = 8;
            this.numericUpDownMaxFiles.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // buttonMove
            // 
            this.buttonMove.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonMove.BackColor = System.Drawing.Color.Transparent;
            this.buttonMove.ForeColor = System.Drawing.Color.Black;
            this.buttonMove.Location = new System.Drawing.Point(130, 149);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(79, 25);
            this.buttonMove.TabIndex = 18;
            this.buttonMove.Text = "Move";
            this.buttonMove.UseVisualStyleBackColor = false;
            this.buttonMove.Click += new System.EventHandler(this.buttonMove_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 68);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(346, 209);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBoxSlowDemos);
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.buttonClean);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(338, 183);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Demo cleaner";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBoxSlowDemos
            // 
            this.groupBoxSlowDemos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSlowDemos.Controls.Add(this.buttonSlowDemosBrowse);
            this.groupBoxSlowDemos.Controls.Add(this.radioButtonMoveSlow);
            this.groupBoxSlowDemos.Controls.Add(this.radioButtonSkipSlow);
            this.groupBoxSlowDemos.Controls.Add(this.radioButtonDeleteSlow);
            this.groupBoxSlowDemos.Controls.Add(this.textBoxSlowDemos);
            this.groupBoxSlowDemos.Location = new System.Drawing.Point(6, 78);
            this.groupBoxSlowDemos.Name = "groupBoxSlowDemos";
            this.groupBoxSlowDemos.Size = new System.Drawing.Size(326, 66);
            this.groupBoxSlowDemos.TabIndex = 17;
            this.groupBoxSlowDemos.TabStop = false;
            this.groupBoxSlowDemos.Text = "Slow demos";
            // 
            // buttonSlowDemosBrowse
            // 
            this.buttonSlowDemosBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSlowDemosBrowse.Location = new System.Drawing.Point(258, 37);
            this.buttonSlowDemosBrowse.Name = "buttonSlowDemosBrowse";
            this.buttonSlowDemosBrowse.Size = new System.Drawing.Size(57, 23);
            this.buttonSlowDemosBrowse.TabIndex = 9;
            this.buttonSlowDemosBrowse.Text = "Browse";
            this.buttonSlowDemosBrowse.UseVisualStyleBackColor = true;
            this.buttonSlowDemosBrowse.Click += new System.EventHandler(this.buttonSlowDemosBrowse_Click);
            // 
            // radioButtonMoveSlow
            // 
            this.radioButtonMoveSlow.AutoSize = true;
            this.radioButtonMoveSlow.Location = new System.Drawing.Point(156, 19);
            this.radioButtonMoveSlow.Name = "radioButtonMoveSlow";
            this.radioButtonMoveSlow.Size = new System.Drawing.Size(67, 17);
            this.radioButtonMoveSlow.TabIndex = 7;
            this.radioButtonMoveSlow.Text = "Move to:";
            this.radioButtonMoveSlow.UseVisualStyleBackColor = true;
            this.radioButtonMoveSlow.CheckedChanged += new System.EventHandler(this.radioButtonMoveSlow_CheckedChanged);
            // 
            // radioButtonSkipSlow
            // 
            this.radioButtonSkipSlow.AutoSize = true;
            this.radioButtonSkipSlow.Location = new System.Drawing.Point(6, 42);
            this.radioButtonSkipSlow.Name = "radioButtonSkipSlow";
            this.radioButtonSkipSlow.Size = new System.Drawing.Size(46, 17);
            this.radioButtonSkipSlow.TabIndex = 6;
            this.radioButtonSkipSlow.Text = "Skip";
            this.radioButtonSkipSlow.UseVisualStyleBackColor = true;
            // 
            // radioButtonDeleteSlow
            // 
            this.radioButtonDeleteSlow.AutoSize = true;
            this.radioButtonDeleteSlow.Checked = true;
            this.radioButtonDeleteSlow.Location = new System.Drawing.Point(6, 19);
            this.radioButtonDeleteSlow.Name = "radioButtonDeleteSlow";
            this.radioButtonDeleteSlow.Size = new System.Drawing.Size(56, 17);
            this.radioButtonDeleteSlow.TabIndex = 5;
            this.radioButtonDeleteSlow.TabStop = true;
            this.radioButtonDeleteSlow.Text = "Delete";
            this.radioButtonDeleteSlow.UseVisualStyleBackColor = true;
            // 
            // textBoxSlowDemos
            // 
            this.textBoxSlowDemos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSlowDemos.Location = new System.Drawing.Point(156, 39);
            this.textBoxSlowDemos.Name = "textBoxSlowDemos";
            this.textBoxSlowDemos.Size = new System.Drawing.Size(96, 20);
            this.textBoxSlowDemos.TabIndex = 5;
            this.textBoxSlowDemos.TextChanged += new System.EventHandler(this.textBoxSlowDemos_TextChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.AllowDrop = true;
            this.tabPage3.Controls.Add(this.checkBoxMoveOnlyYour);
            this.tabPage3.Controls.Add(this.checkBoxSplitFolders);
            this.tabPage3.Controls.Add(this.groupBoxSplit);
            this.tabPage3.Controls.Add(this.groupBoxName);
            this.tabPage3.Controls.Add(this.groupBoxMoveDemos);
            this.tabPage3.Controls.Add(this.buttonMove);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(338, 183);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Demo mover";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // checkBoxMoveOnlyYour
            // 
            this.checkBoxMoveOnlyYour.AutoSize = true;
            this.checkBoxMoveOnlyYour.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBoxMoveOnlyYour.Location = new System.Drawing.Point(189, 61);
            this.checkBoxMoveOnlyYour.Name = "checkBoxMoveOnlyYour";
            this.checkBoxMoveOnlyYour.Size = new System.Drawing.Size(132, 17);
            this.checkBoxMoveOnlyYour.TabIndex = 29;
            this.checkBoxMoveOnlyYour.Text = "Move only your demos";
            this.checkBoxMoveOnlyYour.UseVisualStyleBackColor = false;
            this.checkBoxMoveOnlyYour.CheckedChanged += new System.EventHandler(this.checkBoxMoveOnlyYoyr_CheckedChanged);
            // 
            // checkBoxSplitFolders
            // 
            this.checkBoxSplitFolders.AutoSize = true;
            this.checkBoxSplitFolders.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.checkBoxSplitFolders.Checked = true;
            this.checkBoxSplitFolders.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSplitFolders.Location = new System.Drawing.Point(12, 61);
            this.checkBoxSplitFolders.Name = "checkBoxSplitFolders";
            this.checkBoxSplitFolders.Size = new System.Drawing.Size(120, 17);
            this.checkBoxSplitFolders.TabIndex = 29;
            this.checkBoxSplitFolders.Text = "Split by small folders";
            this.checkBoxSplitFolders.UseVisualStyleBackColor = false;
            this.checkBoxSplitFolders.CheckedChanged += new System.EventHandler(this.checkBoxSplitFolders_CheckedChanged);
            // 
            // groupBoxSplit
            // 
            this.groupBoxSplit.Controls.Add(this.label2);
            this.groupBoxSplit.Controls.Add(this.numericUpDownMaxFiles);
            this.groupBoxSplit.Controls.Add(this.label1);
            this.groupBoxSplit.Controls.Add(this.numericUpDownMaxFolders);
            this.groupBoxSplit.Location = new System.Drawing.Point(6, 62);
            this.groupBoxSplit.Name = "groupBoxSplit";
            this.groupBoxSplit.Size = new System.Drawing.Size(171, 77);
            this.groupBoxSplit.TabIndex = 30;
            this.groupBoxSplit.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Max files In folder";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Max folders In folder";
            // 
            // numericUpDownMaxFolders
            // 
            this.numericUpDownMaxFolders.Location = new System.Drawing.Point(119, 47);
            this.numericUpDownMaxFolders.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownMaxFolders.Minimum = new decimal(new int[] {
            40,
            0,
            0,
            0});
            this.numericUpDownMaxFolders.Name = "numericUpDownMaxFolders";
            this.numericUpDownMaxFolders.Size = new System.Drawing.Size(42, 20);
            this.numericUpDownMaxFolders.TabIndex = 8;
            this.numericUpDownMaxFolders.Value = new decimal(new int[] {
            63,
            0,
            0,
            0});
            // 
            // groupBoxName
            // 
            this.groupBoxName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxName.Controls.Add(this.label6);
            this.groupBoxName.Controls.Add(this.textBoxYourName);
            this.groupBoxName.Location = new System.Drawing.Point(183, 63);
            this.groupBoxName.Name = "groupBoxName";
            this.groupBoxName.Size = new System.Drawing.Size(148, 53);
            this.groupBoxName.TabIndex = 27;
            this.groupBoxName.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 26;
            this.label6.Text = "Your name";
            // 
            // textBoxYourName
            // 
            this.textBoxYourName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxYourName.Location = new System.Drawing.Point(67, 24);
            this.textBoxYourName.Name = "textBoxYourName";
            this.textBoxYourName.Size = new System.Drawing.Size(74, 20);
            this.textBoxYourName.TabIndex = 5;
            // 
            // groupBoxMoveDemos
            // 
            this.groupBoxMoveDemos.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMoveDemos.Controls.Add(this.buttonBrowseWhereMove);
            this.groupBoxMoveDemos.Controls.Add(this.textBoxMoveDemosFolder);
            this.groupBoxMoveDemos.Location = new System.Drawing.Point(6, 6);
            this.groupBoxMoveDemos.Name = "groupBoxMoveDemos";
            this.groupBoxMoveDemos.Size = new System.Drawing.Size(325, 50);
            this.groupBoxMoveDemos.TabIndex = 26;
            this.groupBoxMoveDemos.TabStop = false;
            this.groupBoxMoveDemos.Text = "Move demos to:";
            // 
            // buttonBrowseWhereMove
            // 
            this.buttonBrowseWhereMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonBrowseWhereMove.Location = new System.Drawing.Point(247, 17);
            this.buttonBrowseWhereMove.Name = "buttonBrowseWhereMove";
            this.buttonBrowseWhereMove.Size = new System.Drawing.Size(72, 23);
            this.buttonBrowseWhereMove.TabIndex = 7;
            this.buttonBrowseWhereMove.Text = "Browse";
            this.buttonBrowseWhereMove.UseVisualStyleBackColor = true;
            this.buttonBrowseWhereMove.Click += new System.EventHandler(this.buttonBrowseWhereMove_Click);
            // 
            // textBoxMoveDemosFolder
            // 
            this.textBoxMoveDemosFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMoveDemosFolder.Location = new System.Drawing.Point(6, 19);
            this.textBoxMoveDemosFolder.Name = "textBoxMoveDemosFolder";
            this.textBoxMoveDemosFolder.Size = new System.Drawing.Size(235, 20);
            this.textBoxMoveDemosFolder.TabIndex = 5;
            this.textBoxMoveDemosFolder.TextChanged += new System.EventHandler(this.textBoxMoveDemosFolder_TextChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.checkBox1);
            this.tabPage4.Controls.Add(this.checkBox3);
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Controls.Add(this.checkBox2);
            this.tabPage4.Controls.Add(this.buttonRename);
            this.tabPage4.Controls.Add(this.buttonSingleFileInfo);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(338, 183);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Demo renamer";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // buttonSingleFileInfo
            // 
            this.buttonSingleFileInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSingleFileInfo.Location = new System.Drawing.Point(223, 16);
            this.buttonSingleFileInfo.Name = "buttonSingleFileInfo";
            this.buttonSingleFileInfo.Size = new System.Drawing.Size(96, 37);
            this.buttonSingleFileInfo.TabIndex = 8;
            this.buttonSingleFileInfo.Text = "View single demo information";
            this.buttonSingleFileInfo.UseVisualStyleBackColor = true;
            this.buttonSingleFileInfo.Click += new System.EventHandler(this.buttonSingleFileInfo_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.linkLabel1);
            this.tabPage2.Controls.Add(this.checkBoxDeleteIdentical);
            this.tabPage2.Controls.Add(this.groupBoxBadDemos);
            this.tabPage2.Controls.Add(this.checkBoxDeleteEmptyDirs);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(338, 183);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Additional options";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(307, 154);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(13, 13);
            this.linkLabel1.TabIndex = 24;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "?";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // checkBoxDeleteIdentical
            // 
            this.checkBoxDeleteIdentical.AutoSize = true;
            this.checkBoxDeleteIdentical.Checked = true;
            this.checkBoxDeleteIdentical.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDeleteIdentical.Location = new System.Drawing.Point(6, 101);
            this.checkBoxDeleteIdentical.Name = "checkBoxDeleteIdentical";
            this.checkBoxDeleteIdentical.Size = new System.Drawing.Size(162, 17);
            this.checkBoxDeleteIdentical.TabIndex = 23;
            this.checkBoxDeleteIdentical.Text = "Delete identically named files";
            this.checkBoxDeleteIdentical.UseVisualStyleBackColor = true;
            // 
            // checkBoxDeleteEmptyDirs
            // 
            this.checkBoxDeleteEmptyDirs.AutoSize = true;
            this.checkBoxDeleteEmptyDirs.Checked = true;
            this.checkBoxDeleteEmptyDirs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDeleteEmptyDirs.Location = new System.Drawing.Point(6, 78);
            this.checkBoxDeleteEmptyDirs.Name = "checkBoxDeleteEmptyDirs";
            this.checkBoxDeleteEmptyDirs.Size = new System.Drawing.Size(139, 17);
            this.checkBoxDeleteEmptyDirs.TabIndex = 22;
            this.checkBoxDeleteEmptyDirs.Text = "Delete empty directories";
            this.checkBoxDeleteEmptyDirs.UseVisualStyleBackColor = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 278);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(369, 22);
            this.statusStrip1.TabIndex = 21;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "Demo files|*.dm_68;*.dm_67;*.dm_66";
            // 
            // buttonRename
            // 
            this.buttonRename.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.buttonRename.BackColor = System.Drawing.Color.Transparent;
            this.buttonRename.ForeColor = System.Drawing.Color.Black;
            this.buttonRename.Location = new System.Drawing.Point(128, 152);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(79, 25);
            this.buttonRename.TabIndex = 19;
            this.buttonRename.Text = "Rename";
            this.buttonRename.UseVisualStyleBackColor = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 74);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rename options";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(12, 106);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(132, 17);
            this.checkBox2.TabIndex = 1;
            this.checkBox2.Text = "Enable rules validation";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 20);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(78, 17);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Rename all";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(6, 43);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(143, 17);
            this.radioButton2.TabIndex = 3;
            this.radioButton2.Text = "Rename only bad named";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Location = new System.Drawing.Point(12, 129);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(170, 17);
            this.checkBox3.TabIndex = 21;
            this.checkBox3.Text = "Set correct demo creation time";
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(200, 106);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(131, 17);
            this.checkBox1.TabIndex = 22;
            this.checkBox1.Text = "Show rename preview";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 300);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(385, 327);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DemoCleaner2";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBoxBadDemos.ResumeLayout(false);
            this.groupBoxBadDemos.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownCountOfBest)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxFiles)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBoxSlowDemos.ResumeLayout(false);
            this.groupBoxSlowDemos.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.groupBoxSplit.ResumeLayout(false);
            this.groupBoxSplit.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxFolders)).EndInit();
            this.groupBoxName.ResumeLayout(false);
            this.groupBoxName.PerformLayout();
            this.groupBoxMoveDemos.ResumeLayout(false);
            this.groupBoxMoveDemos.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBoxBadDemos;
        private System.Windows.Forms.Button buttonBadDemosBrowse;
        private System.Windows.Forms.TextBox textBoxBadDemos;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonBrowseDemos;
        private System.Windows.Forms.TextBox textBoxDemosFolder;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton radioBestTimeOfEachPlayer;
        private System.Windows.Forms.RadioButton radioBestTimesOnMap;
        private System.Windows.Forms.Button buttonClean;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxFiles;
        private System.Windows.Forms.Button buttonMove;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxFolders;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBoxDeleteEmptyDirs;
        private System.Windows.Forms.CheckBox checkBoxProcessMdf;
        private System.Windows.Forms.GroupBox groupBoxMoveDemos;
        private System.Windows.Forms.Button buttonBrowseWhereMove;
        private System.Windows.Forms.TextBox textBoxMoveDemosFolder;
        private System.Windows.Forms.GroupBox groupBoxName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxYourName;
        private System.Windows.Forms.CheckBox checkBoxMoveOnlyYour;
        private System.Windows.Forms.GroupBox groupBoxSplit;
        private System.Windows.Forms.CheckBox checkBoxSplitFolders;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.CheckBox checkBoxDeleteIdentical;
        private System.Windows.Forms.GroupBox groupBoxSlowDemos;
        private System.Windows.Forms.Button buttonSlowDemosBrowse;
        private System.Windows.Forms.RadioButton radioButtonMoveSlow;
        private System.Windows.Forms.RadioButton radioButtonSkipSlow;
        private System.Windows.Forms.RadioButton radioButtonDeleteSlow;
        private System.Windows.Forms.TextBox textBoxSlowDemos;
        private System.Windows.Forms.RadioButton radioButtonMoveBad;
        private System.Windows.Forms.RadioButton radioButtonSkipBad;
        private System.Windows.Forms.RadioButton radioButtonDeleteBad;
        private System.Windows.Forms.CheckBox checkBoxUseSubfolders;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDownCountOfBest;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button buttonSingleFileInfo;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonRename;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox3;
    }
}
