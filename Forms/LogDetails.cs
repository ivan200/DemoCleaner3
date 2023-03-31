using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DemoCleaner3 {
    public partial class LogDetails : Form {
        public Form1 formLink = null;

        Properties.Settings prop;
        bool _loadingSettings = false;
        List<LDetails> detailsList;
        private class LDetails {
            public string name;
            public bool value;
            public Action<bool> setValue;
            public LDetails(string name, bool value, Action<bool> setValue) {
                this.name = name;
                this.value = value;
                this.setValue = setValue;
            }
        }

        public LogDetails() {
            InitializeComponent();

            prop = Properties.Settings.Default;

            detailsList = new List<LDetails>();
            detailsList.Add(new LDetails("Create directory", prop.logCreateDir, x => { prop.logCreateDir = x; }));
            detailsList.Add(new LDetails("Delete directory", prop.logDelDir, x => { prop.logDelDir = x; }));
            detailsList.Add(new LDetails("Delete file", prop.logDelFile, x => { prop.logDelFile = x; }));
            detailsList.Add(new LDetails("Move file", prop.logMoveFile, x => { prop.logMoveFile = x; }));
            detailsList.Add(new LDetails("Rename file", prop.logRenameFile, x => { prop.logRenameFile = x; }));
            detailsList.Add(new LDetails("Change file creation date", prop.logChangeCreationDate, x => { prop.logChangeCreationDate = x; }));

            prop = Properties.Settings.Default;
        }

       private void LogDetails_Load(object sender, EventArgs e) {
            _loadingSettings = true;
            checkedListBox1.Items.Clear();
            foreach (LDetails detail in detailsList) {
                checkedListBox1.Items.Add(detail.name, detail.value);
            }
            _loadingSettings = false;

        }

        private void LogDetails_FormClosed(object sender, FormClosedEventArgs e) {
            prop.Save();
            formLink?.BringToFront();
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e) {
            if (!_loadingSettings) {
                detailsList[e.Index].setValue(e.NewValue == CheckState.Checked);
            }
        }
    }
}
