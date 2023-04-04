﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using DemoCleaner3.DemoParser.huffman;
using DemoCleaner3.DemoParser.parser;
using DemoCleaner3.ExtClasses;
using DemoCleaner3.structures;

namespace DemoCleaner3
{
    public partial class Form1 : Form {
        public enum JobType { CLEAN, MOVE, RENAME }
        JobType job = JobType.CLEAN;

        FileHelper fileHelper;
        Properties.Settings prop;

        delegate void SetItem<T>(T num);

        //It is used if progress bar in taskbar cannot be used on this OS
        bool _useTaskBarProgress = true;

        bool _loadingSettings = false;

        private void setProgressPercent(int num) {
            if (_useTaskBarProgress) {
                try {
                    TaskbarProgress.SetValue(this.Handle, num, 100);
                } catch (Exception) {
                    _useTaskBarProgress = false;
                }
            }
            toolStripProgressBar1.Value = num;
        }

        private void setProgressFileNumber(int num) {
            var amount = fileHelper._countDemosAmount;
            if (num == 0 || num == amount) {
                toolStripStatusNumbers.Text = "";
            } else {
                toolStripStatusNumbers.Text = num + " / " + amount;
            }
        }

        private void setProgressFileName(string fileName) {
            toolStripStatusFileName.Text = fileName;
        }

        FolderBrowser2 folderBrowserDialog;

        public DirectoryInfo _currentDemoPath;
        public DirectoryInfo _currentMovePath;
        public DirectoryInfo _currentBadDemosPath;
        public DirectoryInfo _currentSlowDemosPath;

        Thread backgroundThread;

        string _brokenDemosDirName = ".broken";
        string _badDemosDirName = ".no_time";
        string _slowDemosDirName = ".slow_demos";
        string _moveDemosdirName = "!demos";

        public Form1() {
            if (Environment.OSVersion.Version.Major < 6) {
                _useTaskBarProgress = false;
            }

            fileHelper = new FileHelper((fileNumber) => {
                this.Invoke(new SetItem<int>(setProgressFileNumber), fileNumber);
            }, (percent) => {
                if (toolStripProgressBar1.Value != percent) {
                    this.Invoke(new SetItem<int>(setProgressPercent), percent);
                }
            });

            InitializeComponent();

            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        void Form1_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 1) {
                MessageBox.Show("Please, drop only one file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else {
                FileInfo file = new FileInfo(files[0]);
                if (file.Extension.ToLowerInvariant().StartsWith(".dm_")) {
                    showDemoInfoFormForFile(files[0]);
                } else {
                    MessageBox.Show("Please, drop \"Quake 3\" demo file, not other.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            loadSettings();
            InitHelp();
            this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Environment.GetCommandLineArgs()[0]);

            if (Properties.Settings.Default.requestForAssociate == false) {
                var isAssociated = FileAssociations.isAsociated();
                if (!isAssociated) {
                    ShowAssociateDialog();
                }
            }
        }

        private void ShowAssociateDialog() {
            var title = "File association?";
            var message = "You can add a .dm_68 file association, and view information about demo files by simply opening them. Do you want to add a file association?";
            if (isRuLanguage()) {
                title = "Ассоциирование демофайлов";
                message = "Вы можете добавить ассоциации файлов .dm_68, и просматривать информацию о демо файлах просто открывая их. Хотите проассоциировать файлы?";
            }
            var result = MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Yes) {
                Properties.Settings.Default.requestForAssociate = true;
                try {
                    FileAssociations.EnsureAssociationsSet();
                    MessageBox.Show(".dm_68 was assocated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            } else if (result == DialogResult.No) {
                Properties.Settings.Default.requestForAssociate = true;
            }
        }

        private void InitHelp() {
            if (isRuLanguage()) {
                toolTip1.SetToolTip(checkBoxUseSubfolders, "Искать демки в подкаталогах тоже?");

                //clean
                toolTip1.SetToolTip(radioBestTimeOfEachPlayer, "Сохранять лучшие таймы каждого из игроков на карте");
                toolTip1.SetToolTip(radioBestTimesOnMap, "Сохранять лучшие таймы на карте");
                toolTip1.SetToolTip(checkBoxProcessMdf, "Если будут и онлайн и оффлайн демки на одной карте у одного игрока," +
                    "\nдолжны ли мы сохранить только лучший тайм из них?");
                toolTip1.SetToolTip(numericUpDownCountOfBest, "Количество лучших демок для сохранения");
                toolTip1.SetToolTip(labelCountOfBest, "Количество лучших демок для сохранения");
                toolTip1.SetToolTip(radioButtonDeleteSlow, "Медленные демки - УДАЛИТЬ");
                toolTip1.SetToolTip(radioButtonSkipSlow, "Медленные демки - не обрабатывать." +
                    "\nЭтот пункт нужен, если вы хотите только обработать демки с неправильными именами," +
                    "\nили поудалять пустые папки (см. дополнительные опции)");
                toolTip1.SetToolTip(radioButtonMoveSlow, "Медленные демки - переместить в папку:");

                //move
                toolTip1.SetToolTip(textBoxMoveDemosFolder, "Расположение демок после перемещения:");
                toolTip1.SetToolTip(checkBoxSplitFolders, "Разложить все демки по подкаталогам");
                toolTip1.SetToolTip(labelMaxFiles, "Максимальное количество демок в одном каталоге");
                toolTip1.SetToolTip(numericUpDownMaxFiles, "Максимальное количество демок в одном каталоге");
                toolTip1.SetToolTip(labelMaxFolders, "Максимальное количество папок в одном каталоге");
                toolTip1.SetToolTip(numericUpDownMaxFolders, "Максимальное количество папок в одном каталоге");
                toolTip1.SetToolTip(checkBoxMoveOnlyYour, "Перемещать только твои демки");
                toolTip1.SetToolTip(labelYourName, "Твой ник");
                toolTip1.SetToolTip(textBoxYourName, "Твой ник");
                toolTip1.SetToolTip(checkBoxMoveToMap, "Переместить демки в папку: (буква карты)/(название карты)/(демка), " +
                    "\nи параллельно почистить то что там сейчас есть," +
                    "\nчтобы в папке оставались только лучшие таймы");

                //rename
                toolTip1.SetToolTip(radioRenameBad,
                    "Будут обрабатываться только те демо файлы," +
                    "\nв которых название не соответствует паттерну по умолчанию" +
                    "\nmap[gametype.physic]time(player.country)");
                toolTip1.SetToolTip(radioRenameAll,
                    "Попробуем обработать все файлы" +
                    "\n(Например чтобы проверить на корректность конфига в демках," +
                    "\nили поменять даты создания)");
                toolTip1.SetToolTip(checkBoxRulesValidation,
                    "Если у демки неправильно установлены параметры в консоли," +
                    "\nто в имя демки добавится информация (вроде \"{ sv_cheats = 1}\")");
                toolTip1.SetToolTip(checkBoxFixCreationTime,
                    "Если в демо файле есть информация о дате финиширования карты," +
                    "\nто поменять дату создания файла на дату прохождения.");
                toolTip1.SetToolTip(buttonSingleFileInfo,
                    "Просмотреть детальную информацию об одной демке." +
                    "\n(Помимо этой кнопки, можно также просто перенести файл на окошко программы)");
                toolTip1.SetToolTip(checkBoxBrokenDemos,
                    "Создать папку со сломанными демками." +
                    "\n(Которые вообще невозможно воспроизвести в дефраге)");

                //additional
                toolTip1.SetToolTip(radioButtonDeleteBad, "Некорректно именованые демки - УДАЛИТЬ");
                toolTip1.SetToolTip(radioButtonSkipBad, "Некорректно именованые демки - пропустить");
                toolTip1.SetToolTip(radioButtonMoveBad, "Некорректно именованые демки - переместить в каталог:");
                toolTip1.SetToolTip(checkBoxDeleteEmptyDirs, "Удалить пустые папки если такие останутся после обработки.");
                toolTip1.SetToolTip(checkBoxDeleteIdentical,
                    "Удалять демку, если при перемещении демка с таким именем" +
                    "\nуже существует в папке назначения");
                toolTip1.SetToolTip(checkBoxMakeLog,
                    "Логировать все операции с файлами в лог файл" +
                    "\n(Файл будет создан в папке программы).");

                //Info
                toolTip1.SetToolTip(linkLabelInfoCleaner,
                    "В данном разделе можно произвести очистку папки с демками." +
                    "\nЕсли у вас их большое количество, и они вам мешают, то можно оставить только самые лучшие из них," +
                    "\nа остальные удалить или переместить в определённую папку.");
                toolTip1.SetToolTip(linkLabelInfoMover,
                    "В данном разделе можно произвести распределение всех демок по подкаталогам." +
                    "\nЭто требуется потому что Quake 3 не умеет отображать большок количество файлов" +
                    "\nв каталоге с демками, обрезая их отображение. Плюс при их большом количестве" +
                    "\nухудшается поиск определённого файла." +
                    "\nКаталоги будут именоваться на начальную букву карты (например a\\ark3...)," +
                    "\nа при превышении количества демок в таком каталоге," +
                    "\nбудут создаваться дополнительные каталоги (например a\\ar\\ark3...)");
                toolTip1.SetToolTip(linkLabelInfoRenamer,
                    "В данном разделе можно посмотреть информацию о отдельных демо файлах" +
                    "\nи попробовать пачкой переименовать все кривоименованые демки," +
                    "\nа также проверить демки на соответствие правилам." +
                    "\nУ вас же наверняка есть каталоги с пачками демок вроде demo0001," +
                    "\nтак вот этот раздел поможет получить для них всех нормальные названия.");
                toolTip1.SetToolTip(linkLabelInfoAdditional, "Это дополнительный раздел, который влияет на каждую из вкладок.");
            } else {
                toolTip1.SetToolTip(checkBoxUseSubfolders, "Search for demos in subdirectories too?");

                //clean
                toolTip1.SetToolTip(radioBestTimeOfEachPlayer, "Save the best times of each player on the map");
                toolTip1.SetToolTip(radioBestTimesOnMap, "Save the best times on the map");
                toolTip1.SetToolTip(checkBoxProcessMdf,
                    "If there are both online and offline demos on the same map for one player," +
                    "\nshould we keep only the best time of them?");
                toolTip1.SetToolTip(numericUpDownCountOfBest, "Number of best demos to save");
                toolTip1.SetToolTip(labelCountOfBest, "A number of the best demos to save");
                toolTip1.SetToolTip(radioButtonDeleteSlow, "Slow demos - DELETE");
                toolTip1.SetToolTip(radioButtonSkipSlow,
                    "Slow demos - do not process." +
                    "\nThis option is needed if you only want to process demos with incorrect names," +
                    "\nor delete empty folders (see additional options)");
                toolTip1.SetToolTip(radioButtonMoveSlow, "Slow demos - move to folder:");

                //move
                toolTip1.SetToolTip(textBoxMoveDemosFolder, "The location of the demos after they move:");
                toolTip1.SetToolTip(checkBoxSplitFolders, "Split all the demos by the subdirectories");
                toolTip1.SetToolTip(labelMaxFiles, "Maximum number of demos in one directory");
                toolTip1.SetToolTip(numericUpDownMaxFiles, "Maximum number of demos in one directory");
                toolTip1.SetToolTip(labelMaxFolders, "Maximum number of folders in one directory");
                toolTip1.SetToolTip(numericUpDownMaxFolders, "Maximum number of folders in one directory");
                toolTip1.SetToolTip(checkBoxMoveOnlyYour, "Move only your demos");
                toolTip1.SetToolTip(labelYourName, "Your nickname");
                toolTip1.SetToolTip(textBoxYourName, "Your nickname");
                toolTip1.SetToolTip(checkBoxMoveToMap, "Move demos to: (mapname letter)/(map name)/(demo name)" +
                    "\nand at the same time clean up what is there now, " +
                    "\nso that only the best demos stay in the folder");

                //rename
                toolTip1.SetToolTip(radioRenameBad,
                    "Only demo files will be processed," +
                    "\nin which the name does not match the default pattern" +
                    "\nmap[gametype.physic]time(player.country)");
                toolTip1.SetToolTip(radioRenameAll,
                    "Let's try to process all the files" +
                    "\n(perhaps to check for correctness of the set rules or change the dates of creation)");
                toolTip1.SetToolTip(checkBoxRulesValidation,
                    "If the demo has the wrong parameters, information will be added" +
                    "\nto the name of the demo (e.g. \"{sv_cheats = 1}\")");
                toolTip1.SetToolTip(checkBoxFixCreationTime,
                    "If the demo file contains information about completion date," +
                    "\nchange the file creation date to the completion date.");
                toolTip1.SetToolTip(buttonSingleFileInfo,
                    "View detailed information about one demo." +
                    "\n(in addition to this button, you can also" +
                    "\nsimply drop demo file to this program window)");
                toolTip1.SetToolTip(checkBoxBrokenDemos,
                    "Create a folder with broken demos." +
                    "\n(Which cannot be played in defrag at all)");

                //additional
                toolTip1.SetToolTip(radioButtonDeleteBad, "Incorrectly named demos - DELETE");
                toolTip1.SetToolTip(radioButtonSkipBad, "Incorrectly named demos - skip");
                toolTip1.SetToolTip(radioButtonMoveBad, "Incorrectly named demos - move to the directory:");
                toolTip1.SetToolTip(checkBoxDeleteEmptyDirs, "Delete empty folders if they remain after processing.");
                toolTip1.SetToolTip(checkBoxDeleteIdentical,
                    "Delete a demo if a demo with the same name already exists" +
                    "\nin the destination folder when you move it");
                toolTip1.SetToolTip(checkBoxMakeLog,
                    "Log all operations with files to the log file" +
                    "\n(the file will be created in the program folder).");

                //Info
                toolTip1.SetToolTip(linkLabelInfoCleaner,
                    "In this tab, you can clean up the folder with demos." +
                    "\nIf you have a large number of them, and they're causing you issues," +
                    "\nyou can leave only the best of them, and remove the rest or move to a specific folder.");
                toolTip1.SetToolTip(linkLabelInfoMover,
                    "In this tab it is possible to make the splitting of all the demos in the subdirectories." +
                    "\nThis is required because Quake 3 does not know how to display a large number of files" +
                    "\nin the directory with demos, cutting their display." +
                    "\nPlus, if there are a lot of them, the search for a particular file gets worse." +
                    "\nCatalogs will be named on the initial letter of the demo (e.g. a\\ark3...)" +
                    "\nand if the number of demos in such a directory exceeds the number of demos," +
                    "\nadditional directories will be created (e.g. a\\ar\\ark3...)");
                toolTip1.SetToolTip(linkLabelInfoRenamer,
                    "In this tab you can view information about individual demo files" +
                    "\nand try to rename all incorrectly named demos in a batch," +
                    "\nas well as check demos for compliance with the rules." +
                    "\nYou probably have directories with stacks of demo files like \"demo0001\"," +
                    "\nso this section will help you to get normal names for them all.");
                toolTip1.SetToolTip(linkLabelInfoAdditional, "This is an additional tab that affects each of the tabs.");
            }
        }

        private String getExMessage(Exception ex) {
#if DEBUG
            return ex.Message + "\n" + ex.StackTrace;
#else
            return ex.Message;
#endif
        }

        //Loading form settings
        private void loadSettings() {
            _loadingSettings = true;
            try {
                prop = Properties.Settings.Default;

                //main
                var dir = prop.demosFolder;
                if (string.IsNullOrEmpty(dir)) {
                    dir = Application.ExecutablePath.Substring(0,
                        Application.ExecutablePath.Length
                        - Path.GetFileName(Application.ExecutablePath).Length);
                }

                _currentDemoPath = new DirectoryInfo(dir);

                textBoxDemosFolder.Text = dir;
                textBoxDemosFolder.SelectionStart = textBoxBadDemos.Text.Length;
                textBoxDemosFolder.ScrollToCaret();
                textBoxDemosFolder.Refresh();

                tabControl1.SelectedIndex = prop.tabSelectedIndex;

                //clean
                setRadioFromInt(prop.cleanOption, radioBestTimeOfEachPlayer, radioBestTimesOnMap);
                setRadioFromInt(prop.slowDemosOption, radioButtonDeleteSlow, radioButtonSkipSlow, radioButtonMoveSlow);
                textBoxSlowDemos.Enabled = radioButtonMoveSlow.Checked;
                buttonSlowDemosBrowse.Enabled = radioButtonMoveSlow.Checked;
                checkBoxProcessMdf.Checked = prop.processMdf;
                groupBoxSplit.Enabled = checkBoxSplitFolders.Checked;
                numericUpDownCountOfBest.Value = (decimal)prop.countOfBestDemos;

                //move
                numericUpDownMaxFiles.Value = prop.maxFiles;
                numericUpDownMaxFolders.Value = prop.maxFolders;
                checkBoxMoveOnlyYour.Checked = prop.moveOnlyYourTimes;
                groupBoxName.Enabled = checkBoxMoveOnlyYour.Checked;
                textBoxYourName.Text = prop.yourName;
                checkBoxSplitFolders.Checked = prop.splitBySmallFolders;
                checkBoxUseSubfolders.Checked = prop.useSubFolders;
                checkBoxMoveToMap.Checked = prop.moveToMapname;

                if (!string.IsNullOrEmpty(prop.moveDemoFolder)) {
                    textBoxMoveDemosFolder.Text = prop.moveDemoFolder;
                }
                if (!string.IsNullOrEmpty(prop.slowDemoFolder)) {
                    textBoxSlowDemos.Text = prop.slowDemoFolder;
                }
                if (!string.IsNullOrEmpty(prop.badDemoFolder)) {
                    textBoxBadDemos.Text = prop.badDemoFolder;
                }

                //rename
                setRadioFromInt(prop.renameOption, radioRenameBad, radioRenameAll);
                checkBoxFixCreationTime.Checked = prop.renameFixCreationTime;
                checkBoxRulesValidation.Checked = prop.renameValidation;
                openFileDialog1.InitialDirectory = dir;
                checkBoxBrokenDemos.Checked = prop.makeBrokenFolder;

                //additional
                checkBoxDeleteEmptyDirs.Checked = prop.deleteEmptyDirs;
                setRadioFromInt(prop.badDemosOption,
                    radioButtonDeleteBad,
                    radioButtonSkipBad,
                    radioButtonMoveBad);

                textBoxBadDemos.Enabled = radioButtonMoveBad.Checked;
                buttonBadDemosBrowse.Enabled = radioButtonMoveBad.Checked;
                checkBoxDeleteIdentical.Checked = prop.deleteIdentical;
                checkBoxMakeLog.Checked = prop.makeLogFile;
            } catch (Exception) {

            }

            _loadingSettings = false;
        }

        //turn on one radio button from the transmitted list
        private void setRadioFromInt(int check, params RadioButton[] radio) {
            for (int i = 0; i < radio.Length; i++) {
                radio[i].Checked = i == check;
            }
        }

        //Get the int value from the Boolean array
        private int getIntFromParameters(params RadioButton[] t) {
            return t.TakeWhile(x => !x.Checked).Count();
        }

        //Save the settings
        private void SaveSettings() {
            //main
            prop.demosFolder = _currentDemoPath?.FullName ?? "";
            prop.tabSelectedIndex = tabControl1.SelectedIndex;

            //clean
            prop.cleanOption = getIntFromParameters(radioBestTimeOfEachPlayer, radioBestTimesOnMap);
            prop.slowDemosOption = getIntFromParameters(radioButtonDeleteSlow, radioButtonSkipSlow, radioButtonMoveSlow);
            prop.useSubFolders = checkBoxUseSubfolders.Checked;
            prop.processMdf = checkBoxProcessMdf.Checked;
            prop.countOfBestDemos = (int)numericUpDownCountOfBest.Value;

            //move
            prop.splitBySmallFolders = checkBoxSplitFolders.Checked;
            prop.maxFiles = decimal.ToInt32(numericUpDownMaxFiles.Value);
            prop.maxFolders = decimal.ToInt32(numericUpDownMaxFolders.Value);
            prop.moveOnlyYourTimes = checkBoxMoveOnlyYour.Checked;
            prop.yourName = textBoxYourName.Text;
            prop.moveDemoFolder = _currentMovePath?.FullName ?? "";
            prop.badDemoFolder = _currentBadDemosPath?.FullName ?? "";
            prop.slowDemoFolder = _currentSlowDemosPath?.FullName ?? "";
            prop.moveToMapname = checkBoxMoveToMap.Checked;

            //rename
            prop.renameOption = getIntFromParameters(radioRenameBad, radioRenameAll);
            prop.renameFixCreationTime = checkBoxFixCreationTime.Checked;
            prop.renameValidation = checkBoxRulesValidation.Checked;
            prop.makeBrokenFolder = checkBoxBrokenDemos.Checked;

            //additional
            prop.badDemosOption = getIntFromParameters(radioButtonDeleteBad, radioButtonSkipBad, radioButtonMoveBad);
            prop.deleteEmptyDirs = checkBoxDeleteEmptyDirs.Checked;
            prop.deleteIdentical = checkBoxDeleteIdentical.Checked;
            prop.makeLogFile = checkBoxMakeLog.Checked;
            prop.Save();
        }

        //Getting the correct directory from the text
        private DirectoryInfo checkGetFolder(TextBox textBox, string previousText) {
            DirectoryInfo folder = null;
            if (textBox.Text.Length > 0) {
                try {
                    folder = new DirectoryInfo(textBox.Text);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (!string.IsNullOrEmpty(previousText)) {
                        textBox.Text = previousText;
                    }
                }
            }
            return folder;
        }

        private void checkBoxSplitFolders_CheckedChanged(object sender, EventArgs e) {
            var ch = (sender as CheckBox).Checked;
            groupBoxSplit.Enabled = ch;
        }

        private void checkBoxMoveOnlyYoyr_CheckedChanged(object sender, EventArgs e) {
            var ch = (sender as CheckBox).Checked;
            groupBoxName.Enabled = ch;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
            SaveSettings();

            if (backgroundThread != null && backgroundThread.IsAlive) {
                backgroundThread.Abort();
            }
        }


        //Show the dialog box to select the destination folder and return the path
        private void ShowFolderBrowserDialog(string Title, ref DirectoryInfo path, Action action) {
            if (!path.Exists)
                path = path.Parent;

            if (folderBrowserDialog == null)
                folderBrowserDialog = new FolderBrowser2();
            folderBrowserDialog.Title = Title;
            folderBrowserDialog.DirectoryPath = path.FullName;

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK && folderBrowserDialog.DirectoryPath.Length > 0) {
                path = new DirectoryInfo(folderBrowserDialog.DirectoryPath);
                action.Invoke();
            }
        }

        //If you change the main folder with demos, change all other paths
        private void textBoxDemosFolder_TextChanged(object sender, EventArgs e) {
            var folder = checkGetFolder(sender as TextBox, prop.demosFolder);
            if (folder != null) {
                _currentDemoPath = folder;

                textBoxBadDemos.Text = Path.Combine(_currentDemoPath.FullName, _badDemosDirName);
                textBoxBadDemos.SelectionStart = textBoxBadDemos.Text.Length;
                textBoxBadDemos.ScrollToCaret();
                textBoxBadDemos.Refresh();

                textBoxSlowDemos.Text = Path.Combine(_currentDemoPath.FullName, _slowDemosDirName);
                textBoxSlowDemos.SelectionStart = textBoxSlowDemos.Text.Length;
                textBoxSlowDemos.ScrollToCaret();
                textBoxSlowDemos.Refresh();

                if (!checkBoxMoveToMap.Checked || textBoxMoveDemosFolder.Text.Length == 0) {
                    textBoxMoveDemosFolder.Text = Path.Combine(_currentDemoPath.FullName, _moveDemosdirName);
                    textBoxMoveDemosFolder.SelectionStart = textBoxMoveDemosFolder.Text.Length;
                    textBoxMoveDemosFolder.ScrollToCaret();
                    textBoxMoveDemosFolder.Refresh();
                }
            }
        }

        //save the path of slow demos
        private void textBoxSlowDemos_TextChanged(object sender, EventArgs e) {
            var folder = checkGetFolder(sender as TextBox, prop.slowDemoFolder);
            if (folder != null) {
                _currentSlowDemosPath = folder;
            }
        }

        //save the path of demos to move dir
        private void textBoxMoveDemosFolder_TextChanged(object sender, EventArgs e) {
            var folder = checkGetFolder(sender as TextBox, prop.moveDemoFolder);
            if (folder != null) {
                _currentMovePath = folder;
            }
        }

        //save the path of bad demos
        private void textBoxBadDemos_TextChanged(object sender, EventArgs e) {
            var folder = checkGetFolder(sender as TextBox, prop.badDemoFolder);
            if (folder != null) {
                _currentBadDemosPath = folder;
            }
        }

        //Selecting dialog of main folder with demos
        private void buttonBrowseDemos_Click(object sender, EventArgs e) {
            ShowFolderBrowserDialog("Choose demos directory", ref _currentDemoPath, () => {
                textBoxDemosFolder.Text = _currentDemoPath.FullName;
            });
        }

        private void buttonBrowseWhereMove_Click(object sender, EventArgs e) {
            ShowFolderBrowserDialog("Choose demos directory", ref _currentMovePath, () => {
                textBoxMoveDemosFolder.Text = _currentMovePath.FullName;
            });
        }

        private void buttonBadDemosBrowse_Click(object sender, EventArgs e) {
            ShowFolderBrowserDialog("Choose bad demos directory", ref _currentBadDemosPath, () => {
                textBoxBadDemos.Text = _currentBadDemosPath.FullName;
            });
        }

        private void buttonSlowDemosBrowse_Click(object sender, EventArgs e) {
            ShowFolderBrowserDialog("Choose slow demos directory", ref _currentSlowDemosPath, () => {
                textBoxSlowDemos.Text = _currentSlowDemosPath.FullName;
            });
        }

        //turn off access to the elements in the demo processing (and then turn on)
        private void setButtonCallBack(bool enabled) {
            tabControl1.Enabled = enabled;
            groupBox1.Enabled = enabled;
            toolStripProgressBar1.Visible = !enabled;
            if (enabled)
                toolStripStatusLabel1.Text = "Done!";
        }

        private void runBackgroundThread() {
            DirectoryInfo demosFolder = null;
            DirectoryInfo dirdemos = null;

            try {
                demosFolder = new DirectoryInfo(textBoxDemosFolder.Text);
            } catch (Exception) { }

            if (demosFolder == null || !demosFolder.Exists) {
                MessageBox.Show("Directory does not exist\n\n" + textBoxDemosFolder.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveSettings();
            setButtonCallBack(false);
            switch (job) {
                case JobType.CLEAN: toolStripStatusLabel1.Text = "Cleaning..."; break;
                case JobType.MOVE: toolStripStatusLabel1.Text = "Moving..."; break;
                case JobType.RENAME: toolStripStatusLabel1.Text = "Renaming..."; break;
            }

            if (job == JobType.MOVE) {
                dirdemos = new DirectoryInfo(textBoxMoveDemosFolder.Text);
            }

            if (string.IsNullOrEmpty(textBoxBadDemos.Text)) {
                textBoxBadDemos.Text = Path.Combine(_currentDemoPath.FullName, _badDemosDirName);
            }
            if (string.IsNullOrEmpty(textBoxSlowDemos.Text)) {
                textBoxSlowDemos.Text = Path.Combine(_currentDemoPath.FullName, _slowDemosDirName);
            }
            if (string.IsNullOrEmpty(textBoxMoveDemosFolder.Text)) {
                textBoxMoveDemosFolder.Text = Path.Combine(_currentDemoPath.FullName, _moveDemosdirName);
            }


            //We start a thread in which we will process everything
            backgroundThread = new Thread(delegate () {
                try {
                    if (prop.makeLogFile) {
                        fileHelper.StartLogger(job);
                    }

                    switch (job) {
                        case JobType.CLEAN: clean(_currentDemoPath); break;
                        case JobType.MOVE: moveDemos(demosFolder, dirdemos); break;
                        case JobType.RENAME: runRename(_currentDemoPath); break;
                    }

                    if (checkBoxDeleteEmptyDirs.Checked) {
                        fileHelper.deleteEmpty(demosFolder);
                    }

                    if (prop.makeLogFile) {
                        fileHelper.stopLogger();
                    }

                    this.Invoke(new SetItem<int>(setProgressPercent), 0);
                    this.Invoke(new SetItem<bool>(setButtonCallBack), true);
                    this.Invoke(new SetItem<JobType>(showEndMessage), job);
                    this.Invoke(new SetItem<string>(setProgressFileName), "");
                } catch (Exception ex) {
                    if (prop.makeLogFile) {
                        fileHelper.stopLogger();
                    }
                    if (!this.IsDisposed) {
                        this.Invoke(new SetItem<int>(setProgressPercent), 0);
                        this.Invoke(new SetItem<bool>(setButtonCallBack), true);
                        this.Invoke(new SetItem<string>(setProgressFileName), "");
                        MessageBox.Show(getExMessage(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            });
            backgroundThread.Start();
        }

        //Handling button cleaning demos
        private void buttonClean_Click(object sender, EventArgs e) {
            job = JobType.CLEAN;
            runBackgroundThread();
        }

        //formatting messagebox text
        private string getMessageText(decimal counter, string text) {
            if (counter > 0) {
                var s = counter > 1 ? "s" : string.Empty;
                text = string.Format(text, counter, s);
                return text;
            }
            return string.Empty;
        }

        //output message about the end of the work
        private void showEndMessage(JobType jobType) {
            string text = "";
            switch (jobType) {
                case JobType.CLEAN: text = "Cleaning"; break;
                case JobType.MOVE: text = "Moving"; break;
                case JobType.RENAME: text = "Renaming"; break;
            }
            text += " demos finished\n";

            text += getMessageText(fileHelper._countRenameFiles, "\n{0} file{1} were renamed");
            text += getMessageText(fileHelper._countMoveFiles, "\n{0} file{1} were moved");
            text += getMessageText(fileHelper._countDeleteFiles, "\n{0} file{1} were deleted");
            text += getMessageText(fileHelper._countCreateDir, "\n{0} folder{1} were created");
            text += getMessageText(fileHelper._countDeleteDir, "\n{0} folder{1} were deleted");

            if (_useTaskBarProgress) {
                try {
                    TaskbarProgress.SetState(this.Handle, TaskbarProgress.TaskbarStates.NoProgress);
                } catch (Exception) {
                    _useTaskBarProgress = false;
                }
            }

            MessageBox.Show(this, text, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Converting a number into a number based on the number of characters
        string numberToString(int number, int length) {
            var str = number.ToString();
            while (str.Length < length) {
                str = "0" + str;
            }
            return str;
        }

        //Process bad demos
        private void operateBadDemos(IEnumerable<Demo> badDemos) {
            if (radioButtonSkipBad.Checked) {
                return;
            }

            var count = badDemos.Count();

            if (count > 0) {
                if (radioButtonDeleteBad.Checked) {
                    //Or delete everything
                    foreach (var item in badDemos) {
                        fileHelper.deleteCheckRules(item.file);
                    }
                }
                if (radioButtonMoveBad.Checked) {
                    //or move to subdirectories (0-10)
                    if (!_currentBadDemosPath.Exists)
                        _currentBadDemosPath.Create();

                    var max = (int)numericUpDownMaxFiles.Value;
                    var maxDirlength = (count / max).ToString().Length;

                    if (count > max) {
                        var ordered = badDemos.OrderBy(x => x.file.Name, StringComparer.OrdinalIgnoreCase).ToList();
                        for (int i = 0; i < count; i++) {
                            var fn = Path.Combine(_currentBadDemosPath.FullName, numberToString((i / max) + 1, maxDirlength));
                            fileHelper.moveFile(ordered[i].file, new DirectoryInfo(fn), checkBoxDeleteIdentical.Checked);
                        }
                    } else {
                        foreach (var item in badDemos) {
                            fileHelper.moveFile(item.file, _currentBadDemosPath, checkBoxDeleteIdentical.Checked);
                        }
                    }
                }
            }
        }

        private void operateBrokenDemos(IEnumerable<Demo> brokenDemos) {
            if (!checkBoxBrokenDemos.Checked || brokenDemos.Count() == 0) {
                return;
            }
            var brokenDemosPath = new DirectoryInfo(Path.Combine(_currentDemoPath.FullName, _brokenDemosDirName));
            if (!brokenDemosPath.Exists) {
                brokenDemosPath.Create();
            }

            foreach (var item in brokenDemos) {
                fileHelper.moveFile(item.file, brokenDemosPath, checkBoxDeleteIdentical.Checked);
            }
        }
        private KeyValuePair<Exception, string>? operateSlowDemos(IEnumerable<Demo> slowDemos) {
            KeyValuePair<Exception, string>? rex = null;
            foreach (var demo in slowDemos) {
                try {
                    operateSlowDemo(demo);
                } catch (Exception ex) {
                    rex = new KeyValuePair<Exception, string>(ex, demo.file.FullName);
                }
            }
            return rex;
        }
        private void operateSlowDemo(Demo demo) {
            if (radioButtonDeleteSlow.Checked) {
                fileHelper.deleteCheckRules(demo.file);
            } else {
                if (radioButtonMoveSlow.Checked) {
                    fileHelper.moveFile(demo.file, _currentSlowDemosPath, checkBoxDeleteIdentical.Checked);
                }
            }
        }

        private KeyValuePair<List<Demo>, List<Demo>> splitByFastSlow(IEnumerable<Demo> demos, int countToSave) {
            List<Demo> fastDemos = new List<Demo>();
            List<Demo> slowDemos = new List<Demo>();

            //if we have two demos with same time by same player, then we select by longest country name
            var groupedTimes = demos.GroupBy(x => x.time).OrderBy(x => x.Key);
            var fast = groupedTimes.Take(countToSave);
            var slow = groupedTimes.Skip(countToSave).SelectMany(x => x);
            slowDemos.AddRange(slow);

            foreach (var item in fast) {
                if (item.Count() <= 1) {
                    fastDemos.Add(item.First());
                } else {
                    var sameTimeDemos = item.OrderBy(x => x.userId)
                        .ThenBy(x => !x.isSpectator)
                        .ThenByDescending(x => x.country.Length);
                    fastDemos.Add(sameTimeDemos.First());
                    slowDemos.AddRange(sameTimeDemos.Skip(1));
                }
            }
            return new KeyValuePair<List<Demo>, List<Demo>>(fastDemos, slowDemos);
        }

        private KeyValuePair<List<Demo>, List<Demo>> splitByFastSlowWithValidity(IEnumerable<Demo> group, int countToSave) {
            List<Demo> slowDemos = new List<Demo>();
            List<Demo> fastDemos = new List<Demo>();

            var fastDemosGroups = new Dictionary<string, List<Demo>>();

            var groupedByValidity = group.GroupBy(x => x.validity);
            if (groupedByValidity.Count() > 1) {
                foreach (var g in groupedByValidity) {
                    var fastSlow = splitByFastSlow(g, countToSave);
                    fastDemosGroups.Add(g.Key, fastSlow.Key);
                    slowDemos.AddRange(fastSlow.Value);
                }
                if (fastDemosGroups.ContainsKey("")) {
                    long slowestTime = long.MaxValue;
                    var fastValid = fastDemosGroups[""];

                    slowestTime = (long)fastValid.LastOrDefault().time.TotalMilliseconds;

                    fastDemos.AddRange(fastValid);

                    var fastInvalid = fastDemosGroups.Where(x => x.Key != "")
                        .SelectMany(x => x.Value)
                        .Where(x => x.time.TotalMilliseconds < slowestTime);
                    fastDemos.AddRange(fastInvalid);

                    var sametimeInvalid = fastDemosGroups.Where(x => x.Key != "")
                        .SelectMany(x => x.Value)
                        .Where(x => x.time.TotalMilliseconds >= slowestTime);
                    slowDemos.AddRange(sametimeInvalid);

                } else {
                    fastDemos = fastDemosGroups.SelectMany(x => x.Value).ToList();
                }
            } else {
                var fastSlow = splitByFastSlow(group, countToSave);
                fastDemos = fastSlow.Key;
                slowDemos = fastSlow.Value;
            }
            return new KeyValuePair<List<Demo>, List<Demo>>(fastDemos, slowDemos);
        }

        //Clean the demos!
        private void clean(DirectoryInfo filedemos) {
            var files = filedemos.GetFiles("*.dm_??", checkBoxUseSubfolders.Checked ?
                SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            fileHelper.resetValues(files.Length);

            var demos = files.Select(x => Demo.GetDemoFromFile(x));

            var badDemos = demos.Where(x => x.hasError == true);
            operateBadDemos(badDemos);

            if (radioButtonSkipSlow.Checked) {
                return;
            }
            _currentSlowDemosPath.Refresh();

            var goodDemos = demos.Where(x => x.hasError == false);

            IEnumerable<IGrouping<string, Demo>> groups = null;

            if (radioBestTimeOfEachPlayer.Checked) {
                groups = goodDemos.GroupBy(x => (x.mapName
                    + Demo.mdfToDf(x.modphysic, checkBoxProcessMdf.Checked)
                    + x.playerName).ToLower());
            }
            if (radioBestTimesOnMap.Checked) {
                groups = goodDemos.GroupBy(x => (x.mapName
                + Demo.mdfToDf(x.modphysic, checkBoxProcessMdf.Checked)).ToLower());
            }

            KeyValuePair<Exception, string>? rex = null;

            foreach (var group in groups) {
                int countToSave = (int)numericUpDownCountOfBest.Value;

                var fastSlow = splitByFastSlowWithValidity(group, countToSave);
                var fastDemos = fastSlow.Key;
                //since we dont touch fast demos, then just increase progress
                fileHelper.increaseProgressCount(fastDemos.Count);

                var slowDemos = fastSlow.Value;
                rex = operateSlowDemos(slowDemos);
            }
            if (rex.HasValue) {
                throw new Exception(rex.Value.Key.Message + "\nFile:\n" + rex.Value.Value);
            }
        }

        //Turning on and off the input fields, when checking a radio button
        private void radioButtonMoveSlow_CheckedChanged(object sender, EventArgs e) {
            textBoxSlowDemos.Enabled = radioButtonMoveSlow.Checked;
            buttonSlowDemosBrowse.Enabled = radioButtonMoveSlow.Checked;
        }

        //Turning on and off the input fields, when checking a radio button
        private void radioButtonMoveBad_CheckedChanged(object sender, EventArgs e) {
            textBoxBadDemos.Enabled = radioButtonMoveBad.Checked;
            buttonBadDemosBrowse.Enabled = radioButtonMoveBad.Checked;
        }

        //Handling the move button demos
        private void buttonMove_Click(object sender, EventArgs e) {
            job = JobType.MOVE;
            runBackgroundThread();
        }

        //Group files
        private IEnumerable<IGrouping<string, Demo>> GroupFiles(IEnumerable<Demo> files, int indexInside) {
            var groups = files.GroupBy(x => x.file.Name.Substring(0, indexInside + 1).ToLower()).ToList();

            for (int i = 0; i < groups.Count; i++) {
                var group = groups.ElementAt(i);
                if (group.Count() > numericUpDownMaxFiles.Value) {
                    //Рекурсивный вызов этого же метода группировки
                    var subGroupFiles = GroupFiles(group, indexInside + 1);

                    groups.RemoveAt(i);
                    groups.InsertRange(i, subGroupFiles);
                    i--;
                }
            }

            return groups;
        }

        //here comes a new demo from the user on the map, and an array of demos with the same time on the same map and physics
        //returns a boolean flag -
        //true - demo needs to be deleted
        //false - existing demos needs to be deleted, and then rescan the folder
        private bool checkSameTimeDemos(Demo demo, List<Demo> sameTimeDemos, bool checkName = false) {
            List<Demo> sameSizeDemos = sameTimeDemos.Where(x => x.file.Length == demo.file.Length).ToList();

            var demoList = new List<Demo>();
            demoList.AddRange(sameTimeDemos);
            demoList.Add(demo);

            if (sameSizeDemos.Count > 0) {
                //We got demos with same size and same time. I will not check hash, since time and size are enough

                //- demos have the same name - just delete the existing one
                if (sameSizeDemos.Where(x => x.file.Name == demo.file.Name).Count() > 0) {
                    return true;
                }
                var existDemo = sameSizeDemos.First();

                //1) if one demo does not have a country or the country has a tastrigger, and the second has a normal one, then the country is taken
                var countries = demoList
                    .Where(x => x.country != null && x.country.Length > 0 && !Demo.tasTriggers.Contains(x.country));
                if (countries.Count() > 0) {
                    existDemo.country = countries.First().country;
                }

                //2) if at least one has validation, then we take it
                var validities = demoList.Where(x => x.validDict.Count > 0);
                if (validities.Count() > 0) {
                    var isTas = validities.FirstOrDefault(x => x.isTas);
                    if (isTas != null) {
                        existDemo.validDict = isTas.validDict;
                    } else {
                        existDemo.validDict = validities.First().validDict;
                    }
                }

                //3) if at least one has a userId, then we take it
                var userIds = demoList.Where(x => x.userId >= 0).ToList();
                if (userIds.Count > 0) {
                    existDemo.userId = userIds.First().userId;
                }

                //if sameTimeDemos can contain demos with different username
                //for example "UnnamedPlayer" everywhere in first demo and "FM" in filename in second demo
                if (checkName) {
                    var demoname = DemoNames.chooseName(demoList.Select(x => x.playerName).ToArray());
                    existDemo.playerName = demoname;
                }

                //if 2 demos are same and probably demos have not time written in console, 
                //find oldest demo creation time and keep it
                var existOldestTime = DateTime.Now;
                foreach (var tmpDemo in demoList) {
                    if (tmpDemo.file.CreationTime < existOldestTime) existOldestTime = tmpDemo.file.CreationTime;
                    if (tmpDemo.file.LastWriteTime < existOldestTime) existOldestTime = tmpDemo.file.LastWriteTime;
                }
                if (existDemo.file.CreationTime != existOldestTime || existDemo.file.LastAccessTime != existOldestTime) {
                    fileHelper.fixCreationTime(existDemo.file, existOldestTime);
                }

                //rename demo if we change name
                if (existDemo.file.Name != existDemo.demoNewName) {
                    fileHelper._CountDemosAmount += 1;
                    //rename the existing
                    fileHelper.renameFile(existDemo.file, existDemo.demoNewName, prop.deleteIdentical);
                }
                return true;
            }

            List<Demo> diffSizeDemos = sameTimeDemos.Where(x => x.file.Length != demo.file.Length).ToList();
            if (diffSizeDemos.Count > 0) {
                //If demos with one time of the same user, but with different sizes, then:

                //- this is a demo recorded by spectator vs a demo without spectator - take the one where there is no inscription [spect]
                var check = checkDemoDifference(demo, diffSizeDemos, x => x.isSpectator);
                if (check.HasValue) return check.Value;

                //- this is a server demo vs a client demo - take the one where there is no inscription [123]
                check = checkDemoDifference(demo, diffSizeDemos, x => x.userId >= 0);
                if (check.HasValue) return check.Value;

                //- new tas demo without a tag, but there is already a tas demo. Here tas has priority since tas trigger can be added by hands
                check = checkDemoDifference(demo, diffSizeDemos, x => x.isTas == false);
                if (check.HasValue) return check.Value;

                //one demo is not finished, but other is ok
                check = checkDemoDifference(demo, diffSizeDemos, x => x.isNotFinished == true);
                if (check.HasValue) return check.Value;

                //one demo is valid, but other is not
                check = checkDemoDifference(demo, diffSizeDemos, x => x.validDict.Count >= 1);
                if (check.HasValue) return check.Value;

                //- it's just two different times of the same user - we take the smaller one by filesize
                if (diffSizeDemos.Where(x => x.file.Length < demo.file.Length).Count() > 0) {
                    return true;
                } else {
                    return false;
                }
            }
            return false;
        }

        //check demo and list of demo for some parameter, and if it is different, return flag which one
        //true - demo accept parameter, but other demos not
        //false - demo not accept parameter, but one of other demos are
        //null - parameter in all demos is same
        private bool? checkDemoDifference(Demo demo, List<Demo> otherDemos, Func<Demo, bool> function) {
            if (function.Invoke(demo) == true && otherDemos.Where(x => function(x) == false).Count() > 0) {
                return true;
            }
            if (function.Invoke(demo) == false && otherDemos.Where(x => function(x) == true).Count() > 0) {
                return false;
            }
            return null;
        }

        private List<Demo> getDemosForDir(DirectoryInfo mapDir) {
            return mapDir.GetFiles().Select(x => Demo.GetDemoFromFile(x)).Where(x => x.hasError == false).ToList();
        }
        private List<Demo> getPlayerRecsForDir(Demo demo, List<Demo> mapDirDemos) {
            return mapDirDemos.Where(x => {
                var xList = new List<string> { x.mapName.ToLowerInvariant(), x.modphysic.ToLowerInvariant(), x.playerName.ToLowerInvariant() };
                var demoList = new List<string> { demo.mapName.ToLowerInvariant(), demo.modphysic.ToLowerInvariant(), demo.playerName.ToLowerInvariant() };
                return xList.SequenceEqual(demoList);
            }).ToList();
        }

        private List<Demo> getSameRecsForDir(Demo demo, List<Demo> mapDirDemos) {
            return mapDirDemos.Where(x => {
                var xList = new List<string> { x.mapName.ToLowerInvariant(), x.modphysic.ToLowerInvariant(), x.time.TotalMilliseconds.ToString(), x.file.Length.ToString() };
                var demoList = new List<string> { demo.mapName.ToLowerInvariant(), demo.modphysic.ToLowerInvariant(), demo.time.TotalMilliseconds.ToString(), demo.file.Length.ToString() };
                return xList.SequenceEqual(demoList);
            }).ToList();
        }

        //Move new demos to static map folder
        private void moveToMap(IEnumerable<Demo> goodFiles, DirectoryInfo dirdemos) {
            _currentSlowDemosPath.Refresh();
            var groups = goodFiles.GroupBy(x => DemoFolder.GetFolderForMapname(x.mapName)).ToList();
            foreach (var mapDemos in groups) {
                var mapDir = new DirectoryInfo(Path.Combine(dirdemos.FullName, mapDemos.Key));
                if (!mapDir.Exists) {
                    mapDir.Create();
                    fileHelper._countCreateDir++;
                }
                var mapDirDemos = getDemosForDir(mapDir);

                foreach (var demo in mapDemos.Select(x => x)) {
                    var newPath = Path.Combine(mapDir.FullName, demo.file.Name);

                    var sameFileSizeAndTimeDemos = getSameRecsForDir(demo, mapDirDemos);
                    if (sameFileSizeAndTimeDemos.Count > 0) {
                        //if one of the files has username in filename - "UnnamedPlayer" with all the parameters of UnnamedPlayer,
                        //and then the same demo came, but with indicated name, we will rename the existing one and delete the new one
                        checkSameTimeDemos(demo, sameFileSizeAndTimeDemos, true);
                        operateSlowDemo(demo);
                        continue;
                    }

                    var playerRecs = getPlayerRecsForDir(demo, mapDirDemos);

                    List<Demo> sameTimeDemos = playerRecs.Where(x => x.time == demo.time).ToList();
                    if (sameTimeDemos.Count > 0) {
                        var isDemoNeedToDelete = checkSameTimeDemos(demo, sameTimeDemos);
                        if (isDemoNeedToDelete) {
                            fileHelper._CountDemosAmount += 1;
                            operateSlowDemo(demo);
                            continue;
                        } else {
                            fileHelper._CountDemosAmount += sameTimeDemos.Count;
                            operateSlowDemos(sameTimeDemos);

                            mapDir.Refresh();
                            mapDirDemos = getDemosForDir(mapDir);
                            playerRecs = getPlayerRecsForDir(demo, mapDirDemos);
                        }
                    }

                    fileHelper._CountDemosAmount += playerRecs.Count;
                    if (playerRecs.Count == 0) {
                        //just move file
                        fileHelper.moveFile(demo.file, mapDir, true);
                        mapDirDemos.Add(demo);
                    } else {
                        playerRecs.Add(demo);

                        var fastSlow = splitByFastSlowWithValidity(playerRecs, 1);
                        var rex = operateSlowDemos(fastSlow.Value);

                        var fastDemos = fastSlow.Key;
                        var demoIsFast = fastDemos.FirstOrDefault(x => x.file.FullName == demo.file.FullName);
                        if (demoIsFast != null) {
                            fileHelper.moveFile(demo.file, mapDir, true);
                            mapDirDemos.Add(demo);
                            fileHelper.increaseProgressCount(fastDemos.Count - 1);
                        } else {
                            fileHelper.increaseProgressCount(fastDemos.Count);
                        }
                    }
                }
            }
        }


        //Move demos
        private void moveDemos(DirectoryInfo filedemos, DirectoryInfo dirdemos) {
            //from all files we select only demos
            var files = filedemos.GetFiles("*.dm_??", checkBoxUseSubfolders.Checked ?
                SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            fileHelper.resetValues(files.Length);

            var demos = files.Select(x => Demo.GetDemoFromFile(x));

            //We select bad files
            var badDemos = demos.Where(x => x.hasError == true);
            operateBadDemos(badDemos);

            var goodFiles = demos.Where(x => x.hasError == false);

            //look for files with player's name if the name is entered
            if (checkBoxMoveOnlyYour.Checked && textBoxYourName.Text.Length > 0) {
                goodFiles = goodFiles.Where(x => x.playerName.ToLower().Contains(textBoxYourName.Text.ToLower()));
            }

            //Create the main folder
            if (goodFiles.Count() > 0) {
                if (!dirdemos.Exists) {
                    dirdemos.Create();
                    fileHelper._countCreateDir++;
                }
            }

            Exception exception = null;
            string filepath = "";

            if (checkBoxMoveToMap.Checked) {
                moveToMap(goodFiles, dirdemos);
            } else {

                var indexInside = 0;
                //group all files, key = folder name
                var groupedFiles = GroupFiles(goodFiles, indexInside);

                var onlyDirNames = groupedFiles.Select(x => new DemoFolder(x.Key));

                indexInside = 0;

                //We group all the folder names and in the same place we get the full paths to them.
                var groupedFolders = groupFolders(onlyDirNames, indexInside);

                var ListFolders = Ext.MakeListFromGroups(groupedFolders);

                //We pass through all the files and move them to directories.
                for (int i = 0; i < groupedFiles.Count(); i++) {
                    var group = groupedFiles.ElementAt(i);

                    for (int j = 0; j < group.Count(); j++) {
                        var demo = group.ElementAt(j);

                        var folderName = ListFolders.First(x => x.Value._folderName == group.Key).Value.fullFolderName;

                        var newPath = Path.Combine(dirdemos.FullName, folderName);

                        if (!checkBoxSplitFolders.Checked) {
                            newPath = dirdemos.FullName;
                        }
                        try {
                            fileHelper.moveFile(demo.file, new DirectoryInfo(newPath), checkBoxDeleteIdentical.Checked);
                        } catch (Exception ex) {
                            exception = ex;
                            filepath = demo.file.FullName;
                        }
                    }
                }
            }
            if (exception != null) {
                throw new Exception(exception.Message + "\nFile:\n" + filepath);
            }
        }


        //folder grouping
        private IEnumerable<IGrouping<string, DemoFolder>> groupFolders(IEnumerable<DemoFolder> folders, int indexInside) {
            var groups = folders.GroupBy(x => DemoFolder.GetKeyFromIndex(x._folderName, indexInside)).ToList();

            for (int i = 0; i < groups.Count; i++) {
                var group = groups.ElementAt(i);
                if (group.Count() > numericUpDownMaxFolders.Value) {
                    //Recursive call of the same grouping method
                    var subGroupFolders = groupFolders(group, indexInside + 1);

                    groups.RemoveAt(i);
                    groups.InsertRange(i, subGroupFolders);
                    i--;
                } else {
                    foreach (var item in group) {
                        if (item.fullFolderName == null) {
                            item.fullFolderName = DemoFolder.GetFullNameFromIndex(item._folderName, indexInside);
                        }
                    }
                }
            }
            return groups;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            MessageBox.Show("Made by Enter"
                + "\nusing MS Visual Studio 2017"
                + "\nmail: 79067180651@ya.ru"
                + "\nskype: Ivan.1010"
                + "\ndiscord: Enter#4725",
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonSingleFileInfo_Click(object sender, EventArgs e) {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK && openFileDialog1.FileName.Length > 0) {
                showDemoInfoFormForFile(openFileDialog1.FileName);
            }
        }

        private void showDemoInfoFormForFile(string fileName) {
            SaveSettings();

            DemoInfoForm demoInfoForm = new DemoInfoForm();
            demoInfoForm.demoFile = new FileInfo(fileName);
            demoInfoForm.formLink = this;
            demoInfoForm.Show();
        }

        //Handling the button rename demo
        private void buttonRename_Click(object sender, EventArgs e) {
            job = JobType.RENAME;
            runBackgroundThread();
        }


        //Fix demos!
        private void runRename(DirectoryInfo filedemos) {
            var files = filedemos.GetFiles("*.dm_??", checkBoxUseSubfolders.Checked ?
                SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            if (radioRenameBad.Checked) {
                files = files.Where(x => Demo.GetDemoFromFile(x).hasError == true).ToArray();
            }

            fileHelper.resetValues(files.Length);

            Demo demo;

            Exception exception = null;
            string filepath = "";

            List<Demo> badDemos = new List<Demo>();

            foreach (var file in files) {
                try {
                    this.Invoke(new SetItem<string>(setProgressFileName), file.Name);
                    demo = Demo.GetDemoFromFileRaw(file);
                    demo.useValidation = checkBoxRulesValidation.Checked;

                    string newPath = fileHelper.renameFile(file, demo.demoNewName, checkBoxDeleteIdentical.Checked);

                    if (File.Exists(newPath)) {
                        demo.file = new FileInfo(newPath);
                        if (checkBoxFixCreationTime.Checked) {
                            fileHelper.fixCreationTime(demo.file, demo.recordTime);
                        }
                    }
                    if (!demo.hasCorrectName) {
                        fileHelper._CountDemosAmount++;
                        operateBadOrBrockenDemo(demo);
                    }
                } catch (Exception ex) {
                    exception = ex;
                    filepath = file.FullName;
                }
            }
            if (exception != null) {
                throw new Exception(exception.Message + "\nFile:\n" + filepath);
            }
        }

        private void operateBadOrBrockenDemo(Demo demo) {
            var demoArray = new Demo[] { demo };
            if (checkBoxBrokenDemos.Checked) {
                if (demo.isBroken) {
                    operateBrokenDemos(demoArray);
                } else {
                    operateBadDemos(demoArray);
                }
            } else {
                operateBadDemos(demoArray);
            }
        }

        /* void RenameThread(DirectoryInfo filedemos) {
             //We start a thread in which we will process everything
             backgroundThread = new Thread(delegate () {
                 var files = filedemos.GetFiles("*.dm_??", checkBoxUseSubfolders.Checked ?
                     SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                 if (radioRenameBad.Checked) {
                     files = files.Where(x => Demo.GetDemoFromFile(x).hasError == true).ToArray();
                 }
                 fileHelper.resetValues(files.Length);

                 var badFiles = new Stack<Demo>();

                 int threadCount = 10;
                 int fileCountForThread = files.Length / threadCount;

                 var splittedFiles = Ext.Split(files, fileCountForThread);

                 int toProcess = splittedFiles.Count();
                 using (ManualResetEvent resetEvent = new ManualResetEvent(false)) {
                     foreach (var split in splittedFiles) {
                         ThreadPool.QueueUserWorkItem(new WaitCallback(x => {
                             renameInsideThread(x);

                             // Safely decrement the counter
                             if (Interlocked.Decrement(ref toProcess) == 0)
                                 resetEvent.Set();
                         }), new KeyValuePair<IEnumerable<FileInfo>, Stack<Demo>>(split, badFiles));
                     }
                     resetEvent.WaitOne();
                 }
                 fileHelper.resetValues(badFiles.Count, false);
                 operateBadDemos(badFiles);

                 if (checkBoxDeleteEmptyDirs.Checked) {
                     fileHelper.deleteEmpty(_currentDemoPath);
                 }
                 this.Invoke(new SetItem<int>(setProgressPercent), 0);
                 this.Invoke(new SetItem<bool>(setButtonCallBack), true);
                 this.Invoke(new SetItem<JobType>(showEndMessage), job);
             });
             backgroundThread.Start();
         }

         void renameInsideThread(object callback) {
             var filesObject = (KeyValuePair<IEnumerable<FileInfo>, Stack<Demo>>)callback;

             foreach (var file in filesObject.Key) {
                 this.Invoke(new SetItem<string>(setProgressFileName), file.Name);
                 var demo = Demo.GetDemoFromFileRaw(file);
                 demo.useValidation = checkBoxRulesValidation.Checked;

                 string newPath = fileHelper.renameFile(file, demo.demoNewName, checkBoxDeleteIdentical.Checked);

                 if (File.Exists(newPath)) {
                     demo.file = new FileInfo(newPath);
                     if (checkBoxFixCreationTime.Checked) {
                         fileHelper.fixCreationTime(demo.file, demo.recordTime);
                     }
                 }
                 if (!demo.hasCorrectName) {
                     filesObject.Value.Push(demo);
                 }
             }
         }*/

        private void checkBoxMakeLog_CheckedChanged(object sender, EventArgs e) {
            if (!_loadingSettings && checkBoxMakeLog.Checked) {
                showLogDetails();
            }
        }

        private void showLogDetails() {
            LogDetails logDetails = new LogDetails();
            logDetails.formLink = this;
            logDetails.ShowDialog();
        }

        private void checkBoxMoveToMap_CheckedChanged(object sender, EventArgs e) {
            groupBoxSplit.Enabled = !checkBoxMoveToMap.Checked;
            checkBoxSplitFolders.Enabled = !checkBoxMoveToMap.Checked;
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e) {
            ShowAssociateDialog();
        }

        private bool isRuLanguage() {
            var lang = System.Globalization.CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
            return lang == "ru" || lang == "uk" || lang == "kk";
        } 
    }
}
