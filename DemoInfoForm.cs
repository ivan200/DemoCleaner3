using DemoCleaner3.DemoParser.parser;
using DemoCleaner3.ExtClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DemoCleaner3
{
    public partial class DemoInfoForm : Form
    {
        public FileInfo demoFile = null;
        public Form1 formLink = null;

        Demo demo = null;

        FileHelper fileHelper;
        Properties.Settings prop;
        SolidBrush brush;

        public DemoInfoForm()
        {
            InitializeComponent();
        }

        private void DemoInfoForm_Load(object sender, EventArgs e) {
            brush = new SolidBrush(Color.Black);

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Icon = (Icon)resources.GetObject("$this.Icon");

            try {
                demo = Demo.GetDemoFromFileRaw(demoFile);

                prop = Properties.Settings.Default;
                demo.useValidation = prop.renameValidation;

                loadFriendlyConfig(dataGridView);
                textNewName.Text = demo.demoNewName;

                fileHelper = new FileHelper();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

                if (cType.Key == RawInfo.keyConsole && cType.Value.Count > 500) { //reduce huge lag with console duplicates spamming
                    var newDict = new Dictionary<string, string>();
                    foreach (var cKey in cType.Value) {
                        if (!newDict.ContainsKey(cKey.Value)) {
                            newDict.Add(cKey.Value, cKey.Key);  //just put k/v in new dict by value
                            grid.Rows.Add();
                            grid.Rows[grid.RowCount - 1].Cells[1].Value = cKey.Key;
                            grid.Rows[grid.RowCount - 1].Cells[2].Value = cKey.Value;
                        }
                    }
                } else {
                    foreach (var cKey in cType.Value) {
                        grid.Rows.Add();

                        grid.Rows[grid.RowCount - 1].Cells[1].Value = cKey.Key;
                        grid.Rows[grid.RowCount - 1].Cells[2].Value = cKey.Value;
                    }
                }
            }
        }

        private void buttonRename_Click(object sender, EventArgs e)
        {
            try {
                string newPath = fileHelper.renameFile(demo.file, textNewName.Text, prop.deleteIdentical);
                if (prop.renameFixCreationTime) {
                    fileHelper.fixCreationTime(demo.file, demo.recordTime);
                }
                MessageBox.Show("File was Renamed", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DemoInfoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            brush.Dispose();
            formLink?.BringToFront();
        }

        
        bool needToExit = false;
        private void DemoInfoForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape && !dataGridView.IsCurrentCellInEditMode) {
                needToExit = true;
            }
        }
        private void DemoInfoForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape && needToExit) {
                this.Close();
            } else {
                needToExit = false;
            }
        }

        private void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e) {
            if (e.Value != null && e.Value.ToString().Contains("^") && !e.Value.ToString().StartsWith("n\\")) {
                string content = e.Value.ToString().Replace("\n", "");
                var line = Regex.Split(content, "\\^.");
                var colorMatches = Regex.Matches(content, "\\^.");

                var colorTexts = new List<string>();
                colorTexts.Add("^0");
                foreach (Match match in colorMatches) {
                    colorTexts.Add(match.Value);
                }

                var colors = colorTexts.ConvertAll(x => {
                    var lower = x.ToLowerInvariant();
                    string hex;
                    colorsKeys.TryGetValue(lower, out hex);
                    if (hex == null || hex.Length == 0) {
                        hex = "#000000";
                    }
                    return ColorTranslator.FromHtml(hex);
                });

                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                SizeF[] size = new SizeF[line.Length];
                for (int i = 0; i < line.Length; ++i) {
                    size[i] = e.Graphics.MeasureString(line[i], e.CellStyle.Font);
                }
                RectangleF rec = new RectangleF(e.CellBounds.Location, new Size(0, 0));
                var offset = 0;
                for (int i = 0; i < line.Length; ++i) {
                    brush.Color = colors[i];
                    rec = new RectangleF(new PointF(rec.Location.X + rec.Width - offset, rec.Location.Y), new SizeF(size[i].Width, e.CellBounds.Height));
                    e.Graphics.DrawString(line[i], e.CellStyle.Font, brush, rec, sf);
                    if (offset == 0 && line[i].Length > 0) {
                        offset = 3;
                    }
                }
                e.Handled = true;
            }
        }

        private Dictionary<string, string> _colorsKeys = new Dictionary<string, string>();
        private Dictionary<string, string> colorsKeys {
            get {
                if (_colorsKeys.Count == 0) initDict();
                return _colorsKeys;
            }
        }

        void initDict() {
            //key for colors in q3 / hex value for table / original hex value in q3
            _colorsKeys.Add("^1", "#FE0000");    //"#FE0000");
            _colorsKeys.Add("^2", "#00de00");    //"#00FF00");
            _colorsKeys.Add("^3", "#e5c000");    //"#FEFE00");
            _colorsKeys.Add("^4", "#0000FE");    //"#0000FE");
            _colorsKeys.Add("^5", "#00b9ff");    //"#00FEFE");
            _colorsKeys.Add("^6", "#FF00FF");    //"#FF00FF");
            _colorsKeys.Add("^7", "#b7b7b7");    //"#FFFFFF");
            _colorsKeys.Add("^8", "#FE7E00");    //"#FE7E00");
            _colorsKeys.Add("^9", "#7E7E7E");    //"#7E7E7E");
            _colorsKeys.Add("^0", "#000000");    //"#000000");

            _colorsKeys.Add("^a", "#FE0000");    //"#FE0000");
            _colorsKeys.Add("^b", "#FE4300");    //"#FE4300");
            _colorsKeys.Add("^c", "#FE7E00");    //"#FE7E00");
            _colorsKeys.Add("^d", "#FEB900");    //"#FEB900");
            _colorsKeys.Add("^e", "#FEFE00");    //"#FEFE00");
            _colorsKeys.Add("^f", "#B9FE00");    //"#B9FE00");
            _colorsKeys.Add("^g", "#7EFE00");    //"#7EFE00");
            _colorsKeys.Add("^h", "#43FE00");    //"#43FE00");
            _colorsKeys.Add("^i", "#00FE00");    //"#00FE00");
            _colorsKeys.Add("^j", "#00FE43");    //"#00FE43");
            _colorsKeys.Add("^k", "#00FE7E");    //"#00FE7E");
            _colorsKeys.Add("^l", "#00FEB9");    //"#00FEB9");
            _colorsKeys.Add("^m", "#00FEFE");    //"#00FEFE");
            _colorsKeys.Add("^n", "#00B9FE");    //"#00B9FE");
            _colorsKeys.Add("^o", "#007EFE");    //"#007EFE");
            _colorsKeys.Add("^p", "#0043FE");    //"#0043FE");
            _colorsKeys.Add("^q", "#0000FE");    //"#0000FE");
            _colorsKeys.Add("^r", "#4300FE");    //"#4300FE");
            _colorsKeys.Add("^s", "#7E00FE");    //"#7E00FE");
            _colorsKeys.Add("^t", "#B900FE");    //"#B900FE");
            _colorsKeys.Add("^u", "#FE00FE");    //"#FE00FE");
            _colorsKeys.Add("^v", "#FE00B9");    //"#FE00B9");
            _colorsKeys.Add("^w", "#FE007E");    //"#FE007E");
            _colorsKeys.Add("^x", "#FE0043");    //"#FE0043");
            _colorsKeys.Add("^y", "#b7b7b7");    //"#FEFEFE");
            _colorsKeys.Add("^z", "#9898FE");    //"#9898FE");
        
        }

        private void buttonCreateRec_Click(object sender, EventArgs e) {
            var bytes = getRecBytes();
            if (bytes == null) {
                MessageBox.Show("Can not create rec file for current demo", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var name = demo.mapName + "_" + demo.modphysic.Replace('.', '_') + ".rec";

            saveRecFileDialog.FileName = name;
            saveRecFileDialog.Filter = "rec files (*.rec)|*.rec|All files (*.*)|*.*";
            saveRecFileDialog.FilterIndex = 0;
            saveRecFileDialog.RestoreDirectory = true;

            Stream myStream;

            if (saveRecFileDialog.ShowDialog(this) == DialogResult.OK) {
                if ((myStream = saveRecFileDialog.OpenFile()) != null) {
                    myStream.Write(bytes, 0, bytes.Length);
                    myStream.Close();
                }
            }
        }

        /// <summary>
        /// Generating rec bytes by demo file
        /// thx frog (aka H@des) for code
        /// </summary>
        private byte[] getRecBytes() {
            var info = demo.rawInfo;
            if (info == null) {
                return null;
            }
            var MAX_CHECKPOINTS = 32;
            var cps = info.cpData;
            if (cps.Count == 0) {
                return null;
            }

            if (cps.Count > MAX_CHECKPOINTS) {
                cps = cps.GetRange(0, MAX_CHECKPOINTS);
            }

            //checkpoint data -> 128 bytes
            int cp_sum = 0;
            var cp_data = new List<byte>();

            foreach (var cp in cps) {
                cp_sum += (int)cp;  //total time at current checkpoint
                var cp_byte = toBytes(cp_sum, 4, true);
                cp_data.AddRange(cp_byte);
            }
            var filledData = toBytes(0, MAX_CHECKPOINTS * 4 - cp_data.Count, true);
            cp_data.AddRange(filledData);

            //last four bytes -> keeps track of how many checkpoints there are (excluding final) -> num of cps - 1
            var cpDataFooter = toBytes(cps.Count - 1, 4, true);
            cp_data.AddRange(cpDataFooter);

            //byte header -> 4 bytes. [version number][checksum byte][0][0]
            byte versionNum = 3;  //.rec structure version number (3)

            //checksum = sum all bytes in cp data -> 1st byte of result
            var cpDataSum = 0;
            foreach (var b in cp_data) {
                cpDataSum += b;
            }
            var checksum = toBytes(cpDataSum, 0, true)[0];
            var header = new byte[4] { versionNum, checksum, 0, 0 };

            //add header
            var rec_data = new List<byte>();
            rec_data.AddRange(header);
            rec_data.AddRange(cp_data);

            return rec_data.ToArray();
        }

        private byte[] toBytes(int value, int size, bool isLittle) {
            byte[] intBytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                Array.Reverse(intBytes);
            }
            if (size <= 0) {
                if (isLittle) Array.Reverse(intBytes);
                return intBytes;
            }
            var sizedBytes = new byte[size];
            for (int i = 0; i < size; i++) {
                if (i < intBytes.Length) {
                    sizedBytes[i] = intBytes[i];
                } else {
                    sizedBytes[i] = 0;
                }
            }
            if (isLittle) Array.Reverse(sizedBytes);
            return sizedBytes;
        }

    }
}
