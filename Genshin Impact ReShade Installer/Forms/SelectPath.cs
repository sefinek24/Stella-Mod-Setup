using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Genshin_Impact_MP_Installer.Scripts;

namespace Genshin_Impact_MP_Installer.Forms
{
    public partial class SelectPath : Form
    {
        public SelectPath()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Log.Error(e, false);
            }
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;

            var t = new Thread(() =>
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.InitialDirectory = Installation.ProgramFiles64;
                    dialog.Filter = "Process (*.exe)|*.exe";
                    dialog.FilterIndex = 0;
                    dialog.RestoreDirectory = true;

                    if (dialog.ShowDialog() != DialogResult.OK) return;
                    var selectedFile = dialog.FileName;

                    if (!selectedFile.Contains("GenshinImpact.exe") && !selectedFile.Contains("YuanShen.exe"))
                    {
                        MessageBox.Show(
                            "Please select the game exe.\n\nGenshinImpact.exe for OS version.\nYuanShen.exe for CN version.",
                            Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    var directory = Path.GetDirectoryName(selectedFile);
                    if (!File.Exists($@"{directory}\UnityPlayer.dll"))
                    {
                        MessageBox.Show("That's not the right place.", Program.AppName, MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    filePath = dialog.FileName;
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            if (string.IsNullOrEmpty(filePath)) return;
            comboBox1.Items.Clear();
            comboBox1.Items.Add(filePath);
            comboBox1.SelectedIndex = 0;
        }

        private void SaveSettings_Click(object sender, EventArgs e)
        {
            var selectedFile = comboBox1.GetItemText(comboBox1.SelectedItem);
            if (!selectedFile.Contains("GenshinImpact.exe") && !selectedFile.Contains("YuanShen.exe"))
            {
                MessageBox.Show(
                    "I can't save your settings. Please select the game exe.\n\nGenshinImpact.exe for OS version.\nYuanShen.exe for CN version.",
                    Program.AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var directory = Path.GetDirectoryName(selectedFile);
            if (!File.Exists($@"{directory}\UnityPlayer.dll"))
            {
                MessageBox.Show("That's not the right place. UnityPlayer.dll file was not found.", Program.AppName,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            File.WriteAllText($@"{Program.AppData}\game-path.sfn", selectedFile);
            Close();
        }

        private void Help_Click(object sender, EventArgs e)
        {
            var help = new Help { Icon = Icon.ExtractAssociatedIcon("Data/Images/52x52.ico") };
            help.Show();
        }
    }
}