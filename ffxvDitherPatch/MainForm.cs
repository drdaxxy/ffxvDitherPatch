using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ffxvDitherPatch
{
    public partial class MainForm : Form
    {
        Craf _archive;
        Patcher _patcher;

        public MainForm()
        {
            InitializeComponent();
        }

        private void UpdateProgressBar(int val)
        {
            progressBar.Value = val;
        }

        private async void processButton_Click(object sender, EventArgs e)
        {
            processButton.Enabled = false;

            progressBar.Value = 0;
            await _patcher.PatchAsync(new Progress<int>(UpdateProgressBar), Patcher.PatchMode.NarrowDithering);

            progressBar.Value = 0;
            await _archive.SaveAsync("patchedShaders.earc", new Progress<int>(UpdateProgressBar));

            //await _patcher.DumpDiscardPsAsync(new Progress<int>(UpdateProgressBar));

            processButton.Text = "Done";
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            _archive = Craf.Open("datas/shader/shadergen/autoexternal.earc");
            progressBar.Maximum = _archive.Count();
            await _archive.LoadAsync(new Progress<int>(UpdateProgressBar));
            _archive.CloseReader();
            _patcher = new Patcher(_archive);
            processButton.Enabled = true;
        }
    }
}
