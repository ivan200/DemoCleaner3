using DemoCleaner3.DemoParser.huffman;
using DemoCleaner3.DemoParser.parser;
using DemoCleaner3.ExtClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DemoCleaner3
{
    public partial class DemoInfoForm : Form
    {
        public FileInfo demoFile = null;

        Demo demo = null;

        FileHelper fileHelper;
        Properties.Settings prop;

        public DemoInfoForm()
        {
            InitializeComponent();
        }

        private void DemoInfoForm_Load(object sender, EventArgs e)
        {
            demo = Demo.GetDemoFromFileRaw(demoFile);
            prop = Properties.Settings.Default;
            demo.useValidation = prop.renameValidation;

            loadFriendlyConfig(dataGridView);
            textNewName.Text = demo.demoNewName;

            fileHelper = new FileHelper();
        }

        void loadFriendlyConfig(DataGridView grid)
        {
            if (demo == null) {
                return;
            }
            var info = demo.rawInfo;
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
            try {
                string newPath = fileHelper.renameFile(demo.file, demo.demoNewName, prop.deleteIdentical);
                if (prop.renameFixCreationTime) {
                    fileHelper.fixCreationTime(demo.file, demo.recordTime);
                }
                MessageBox.Show("File was Renamed", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
