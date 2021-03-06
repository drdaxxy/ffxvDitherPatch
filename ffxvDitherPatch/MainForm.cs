﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Craf;

namespace ffxvDitherPatch
{
    public partial class MainForm : Form
    {
        private const string archivePath = "datas/shader/shadergen/autoexternal.earc";
        private const string tempArchivePath = "patchedShadersTemp.earc";
        private const string backupArchivePath = "datas/shader/shadergen/autoexternal.preDitherPatch.earc";
        private const string dummyFileName = "dummy/ffxvDitherPatch";
        private const string dummyVfsPath = "data://dummy/ffxvDitherPatch";
        private readonly byte[] dummyFileContent = { 0x01, 0x00 };

        private const string versionNumber = "1.0";

        CrafArchive _archive;
        Patcher _patcher;

        private List<RadioButton> radioSet;

        public MainForm()
        {
            InitializeComponent();

            wideGroupBox.Click += (sender, ev) => { wideRadio.PerformClick(); };
            mediumGroupBox.Click += (sender, ev) => { mediumRadio.PerformClick(); };
            narrowGroupBox.Click += (sender, ev) => { narrowRadio.PerformClick(); };
            offGroupBox.Click += (sender, ev) => { offRadio.PerformClick(); };
            widePic.Click += (sender, ev) => { wideRadio.PerformClick(); };
            mediumPic.Click += (sender, ev) => { mediumRadio.PerformClick(); };
            narrowPic.Click += (sender, ev) => { narrowRadio.PerformClick(); };
            offPic.Click += (sender, ev) => { offRadio.PerformClick(); };

            radioSet = new List<RadioButton> { wideRadio, mediumRadio, narrowRadio, offRadio };

            helpTextBox.Rtf = Assets.HelpDesc;
        }

        private void radioCheck(object sender, EventArgs e)
        {
            if (!((RadioButton)sender).Checked) return;

            foreach (var other in radioSet)
            {
                if (other == sender) continue;
                other.Checked = false;
            }
        }

        private void UpdateProgressBar(int val)
        {
            progressBar.Value = val;
        }

        private async void processButton_Click(object sender, EventArgs e)
        {
            processButton.Enabled = false;
            foreach (var btn in radioSet)
            {
                btn.Enabled = false;
            }

            _archive.Append(dummyFileName, dummyVfsPath, false, dummyFileContent);

            progressBar.Value = 0;
            progressBar.Maximum = _patcher.CandidateCount();
            statusLabel.Text = "Processing shaders";
            if (offRadio.Checked)
            {
                await _patcher.PatchAsync(new Progress<int>(UpdateProgressBar), Patcher.PatchMode.DisableDithering, 0.0f);
            }
            else if (wideRadio.Checked)
            {
                await _patcher.PatchAsync(new Progress<int>(UpdateProgressBar), Patcher.PatchMode.NarrowDithering, 32.0f);
            }
            else if (narrowRadio.Checked)
            {
                await _patcher.PatchAsync(new Progress<int>(UpdateProgressBar), Patcher.PatchMode.NarrowDithering, 56.0f);
            }
            else
            {
                await _patcher.PatchAsync(new Progress<int>(UpdateProgressBar), Patcher.PatchMode.NarrowDithering, 40.0f);
            }

            progressBar.Value = 0;
            progressBar.Maximum = _archive.Count();
            statusLabel.Text = "Writing archive";
            try
            {
                await _archive.SaveAsync(tempArchivePath, new Progress<int>(UpdateProgressBar));

                // Use case:
                // - User applies patch, producing patched archive and backup archive
                // - Game update overwrites patched archive
                // - Backup archive is now present despite regular archive having no patch marker
                if (File.Exists(backupArchivePath)) File.Delete(backupArchivePath);

                File.Move(archivePath, backupArchivePath);
                File.Move(tempArchivePath, archivePath);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Could not write shader archive. "
                              + "Please ensure nothing is accessing the following paths (close the game if you're running it):\n\n"
                              + archivePath + "\n"
                              + tempArchivePath + "\n"
                              + backupArchivePath + "\n\n"
                              + "If you're still having problems, try running as Administrator and/or disabling your antivirus.\n\n"
                              + "Details:\n"
                              + ex.Message);
                Application.Exit();
            }

            //await _patcher.DumpDiscardPsAsync(new Progress<int>(UpdateProgressBar));

            statusLabel.Text = "Done";
        }

        private void InitialOpen()
        {
            try
            {
                _archive = CrafArchive.Open(archivePath);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Shader archive \"" + archivePath + "\" is not readable. "
                              + "Please ensure you are running this program in the game installation folder. "
                              + "If this program is correctly installed and you're still seeing this error, try running as Administrator and/or disabling your antivirus.\n\n"
                              + "Details:\n"
                              + ex.Message);
                Application.Exit();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            versionLabel.Text = string.Format("{0} (built {1})",
                    versionNumber,
                    Assembly.GetExecutingAssembly().GetLinkerTime()
                        .ToString("MMM dd, yyyy (hh:mm tt)", CultureInfo.InvariantCulture));

            this.BeginInvoke((MethodInvoker) async delegate {
                InitialOpen();

                if (_archive.IndexOf(dummyVfsPath) != -1)
                {
                    _archive.CloseReader();
                    if (File.Exists(backupArchivePath))
                    {
                        var dr = MessageBox.Show("Your shaders were already patched. "
                                               + "Want to restore the originals? You'll be able to re-patch with different settings afterwards.",
                            "ffxvDitherPatch",
                            MessageBoxButtons.YesNo);
                        if (dr != DialogResult.Yes)
                        {
                            Application.Exit();
                        }
                        // not redundant - Application.Exit() does not stop control flow here
                        else
                        {
                            File.Delete(archivePath);
                            File.Move(backupArchivePath, archivePath);
                            InitialOpen();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Your shaders were already patched and no backup was found. "
                                      + "Please restore the original \"" + archivePath + "\" (e.g. with Steam's Verify Integrity feature).");
                        Application.Exit();
                    }
                }

                progressBar.Maximum = _archive.Count();
                statusLabel.Text = "Loading archive into memory";
                await _archive.LoadAsync(new Progress<int>(UpdateProgressBar));
                _archive.CloseReader();

                _patcher = new Patcher(_archive);
                processButton.Enabled = true;
                statusLabel.Text = "Waiting for input";
            });
        }
    }
}
