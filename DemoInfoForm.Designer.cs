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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.ColumnType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnField = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.textNewName = new System.Windows.Forms.TextBox();
            this.buttonRename = new System.Windows.Forms.Button();
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
            this.dataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnType,
            this.ColumnField,
            this.ColumnValue});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dataGridView.Location = new System.Drawing.Point(12, 46);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
            this.dataGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dataGridView.RowTemplate.Height = 18;
            this.dataGridView.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.Size = new System.Drawing.Size(659, 476);
            this.dataGridView.TabIndex = 4;
            // 
            // ColumnType
            // 
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ColumnType.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColumnType.HeaderText = "Type";
            this.ColumnType.Name = "ColumnType";
            this.ColumnType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnField
            // 
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ColumnField.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColumnField.HeaderText = "Field";
            this.ColumnField.Name = "ColumnField";
            this.ColumnField.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnField.Width = 130;
            // 
            // ColumnValue
            // 
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ColumnValue.DefaultCellStyle = dataGridViewCellStyle3;
            this.ColumnValue.HeaderText = "Value";
            this.ColumnValue.Name = "ColumnValue";
            this.ColumnValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ColumnValue.Width = 400;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "New demo name";
            // 
            // textNewName
            // 
            this.textNewName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textNewName.Location = new System.Drawing.Point(105, 12);
            this.textNewName.Name = "textNewName";
            this.textNewName.Size = new System.Drawing.Size(456, 20);
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
            // DemoInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(683, 553);
            this.Controls.Add(this.buttonRename);
            this.Controls.Add(this.textNewName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView);
            this.Name = "DemoInfoForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo Information";
            this.Load += new System.EventHandler(this.DemoInfoForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnField;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textNewName;
        private System.Windows.Forms.Button buttonRename;
    }
}