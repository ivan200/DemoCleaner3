namespace DemoCleaner3
{
    partial class DemoInfoForm
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
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnField = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.textNewName = new System.Windows.Forms.TextBox();
            this.buttonRename = new System.Windows.Forms.Button();
            this.buttonCreateRec = new System.Windows.Forms.Button();
            this.saveRecFileDialog = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnType,
            this.ColumnField,
            this.ColumnValue});
            this.dataGridView.Location = new System.Drawing.Point(12, 46);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.Height = 18;
            this.dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.Size = new System.Drawing.Size(659, 495);
            this.dataGridView.TabIndex = 4;
            this.dataGridView.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView_CellPainting);
            // 
            // ColumnType
            // 
            this.ColumnType.HeaderText = "Type";
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnField
            // 
            this.ColumnField.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ColumnField.HeaderText = "Field";
            this.ColumnField.Name = "ColumnField";
            this.ColumnField.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnField.Width = 35;
            // 
            // ColumnValue
            // 
            this.ColumnValue.HeaderText = "Value";
            this.ColumnValue.Name = "ColumnValue";
            this.ColumnValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnValue.Width = 400;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(120, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "New demo name";
            // 
            // textNewName
            // 
            this.textNewName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textNewName.Location = new System.Drawing.Point(213, 12);
            this.textNewName.Name = "textNewName";
            this.textNewName.Size = new System.Drawing.Size(348, 20);
            this.textNewName.TabIndex = 6;
            // 
            // buttonRename
            // 
            this.buttonRename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRename.Location = new System.Drawing.Point(567, 10);
            this.buttonRename.Name = "buttonRename";
            this.buttonRename.Size = new System.Drawing.Size(104, 23);
            this.buttonRename.TabIndex = 7;
            this.buttonRename.Text = "Rename";
            this.buttonRename.UseVisualStyleBackColor = true;
            this.buttonRename.Click += new System.EventHandler(this.buttonRename_Click);
            // 
            // buttonCreateRec
            // 
            this.buttonCreateRec.Location = new System.Drawing.Point(12, 10);
            this.buttonCreateRec.Name = "buttonCreateRec";
            this.buttonCreateRec.Size = new System.Drawing.Size(102, 23);
            this.buttonCreateRec.TabIndex = 8;
            this.buttonCreateRec.Text = "Generate .rec file";
            this.buttonCreateRec.UseVisualStyleBackColor = true;
            this.buttonCreateRec.Click += new System.EventHandler(this.buttonCreateRec_Click);
            // 
            // saveRecFileDialog
            // 
            this.saveRecFileDialog.DefaultExt = "rec";
            // 
            // DemoInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(683, 553);
            this.Controls.Add(this.buttonCreateRec);
            this.Controls.Add(this.buttonRename);
            this.Controls.Add(this.textNewName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView);
            this.KeyPreview = true;
            this.Name = "DemoInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo Information";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DemoInfoForm_FormClosed);
            this.Load += new System.EventHandler(this.DemoInfoForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DemoInfoForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.DemoInfoForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textNewName;
        private System.Windows.Forms.Button buttonRename;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnField;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnValue;
        private System.Windows.Forms.Button buttonCreateRec;
        private System.Windows.Forms.SaveFileDialog saveRecFileDialog;
    }
}