using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace jav_organizer_gui
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                PathTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            PathTextBox.Text = "";
            LogTextBox.Text = "";
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            string path = PathTextBox.Text;
            if (Directory.Exists(path))
            {
                ToggleComponents(false);
                Process process = new Process();
                process.StartInfo.FileName = Application.StartupPath + "\\jav-organizer.exe";
                process.StartInfo.Arguments = "-d " + path;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
                process.Exited += OnProcessExit;
                process.EnableRaisingEvents = true;
                try
                {
                    process.Start();
                }
                catch 
                {
                    MessageBox.Show("Cannot run jav-organizer executable", "Error");
                    ToggleComponents(true);
                    return;
                }
                process.BeginOutputReadLine();
                
            }
            else
            {
                MessageBox.Show("\"" + path + "\" is not a valid directory.","Error");
            }
        }

        delegate void BoolArgReturnVoidDelegate(bool value);
        private void ToggleComponents(bool enabled)
        {
            if (PathTextBox.InvokeRequired)
            {
                BoolArgReturnVoidDelegate d = new BoolArgReturnVoidDelegate(ToggleComponents);
                Invoke(d, new object[] { enabled });
            }
            else
            {
                PathTextBox.Enabled = enabled;
                BrowseButton.Enabled = enabled;
                ClearButton.Enabled = enabled;
                StartButton.Enabled = enabled;
            }
        }
        
        private void OnProcessExit(object sender, EventArgs e)
        {
            ToggleComponents(true);
        }

        private void OutputHandler(object sender, DataReceivedEventArgs outLine)
        {
            if (outLine.Data != null)
            {
                byte[] bytes = Encoding.Default.GetBytes(outLine.Data);
                string decoded = Encoding.UTF8.GetString(bytes);
                LogOutput(decoded);
            }
        }

        delegate void StringArgReturningVoidDelegate(string text);
        private void LogOutput(string text)
        {
            if (LogTextBox.InvokeRequired)
            {
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(LogOutput);
                Invoke(d, new object[] { text });
            }
            else
            {
                LogTextBox.AppendText(text + "\r\n");
            }
        }
        
    }
}
