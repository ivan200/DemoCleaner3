using DemoCleaner2.DemoParser.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DemoCleaner2
{
    public partial class DemoInfoForm : Form
    {
        public RawInfo info = null;

        public DemoInfoForm()
        {
            InitializeComponent();
        }

        private void DemoInfoForm_Load(object sender, EventArgs e)
        {
            loadFriendlyConfig(dataGridView);
        }


        void loadFriendlyConfig(DataGridView grid)
        {
            if (info == null)
            {
                return;
            }

            var frInfo = info.getFriendlyInfo();

            grid.Rows.Clear();
            foreach (var cType in frInfo)
            {
                grid.Rows.Add();
                grid.Rows[grid.RowCount - 1].Cells[0].Value = cType.Key;

                foreach (var cKey in cType.Value) {
                    grid.Rows.Add();

                    grid.Rows[grid.RowCount - 1].Cells[1].Value = cKey.Key;
                    grid.Rows[grid.RowCount - 1].Cells[2].Value = cKey.Value;
                }
            }
        }
    }
}
