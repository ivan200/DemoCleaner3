using DemoCleaner2.DemoParser.parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DemoCleaner2
{
    public partial class DemoInfoForm : Form
    {
        public RawInfo info = null;

        Demo demo = null;

        public DemoInfoForm()
        {
            InitializeComponent();
        }

        private void DemoInfoForm_Load(object sender, EventArgs e)
        {
            loadFriendlyConfig(dataGridView);

            demo = Demo.GetDemoFromRawInfo(info);
            textNewName.Text = demo.demoNewName;
        }

        void loadFriendlyConfig(DataGridView grid)
        {
            if (info == null) {
                return;
            }

            var frInfo = info.getFriendlyInfo();

            textNewName.Text = new FileInfo(info.demoPath).Name;

            grid.Rows.Clear();
            foreach (var cType in frInfo) {
                grid.Rows.Add();
                grid.Rows[grid.RowCount - 1].Cells[0].Value = cType.Key;

                foreach (var cKey in cType.Value) {
                    grid.Rows.Add();

                    grid.Rows[grid.RowCount - 1].Cells[1].Value = cKey.Key;
                    grid.Rows[grid.RowCount - 1].Cells[2].Value = cKey.Value;
                }
            }
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            string newPath = Path.Combine(demo.file.Directory.FullName, textNewName.Text);
            File.Move(demo.file.FullName, newPath);

            if (demo.recordTime != null) {
                File.SetCreationTime(newPath, demo.recordTime);
            }

            MessageBox.Show("File was Renamed", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
