using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OtomatSearching.Engine;

namespace OtomatSearching
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void textBoxX2_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop) == true)
                {
                    e.Effect = DragDropEffects.Move;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxX2_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var files = e.Data.GetData(DataFormats.FileDrop, false) as string[];
                for (int i = 0; i < files.Length; i++)
                {
                    this.txtFileInputBox.Text = files[i];
                    var text = File.ReadAllText(files[i]);
                    this.richTextBox1.Text = text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOsearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtPatternBox.Text) == true)
                {
                    throw new Exception("Pattern cannot be empty!");
                }
                else if (string.IsNullOrEmpty(this.txtFileInputBox.Text) == true)
                {
                    throw new Exception("Drop a text file into textbox file input");
                }
                else if (File.Exists(this.txtFileInputBox.Text.Trim()) == false)
                {
                    throw new Exception("File input does not exists!");
                }
                // Reset back-color
                var sstart = this.richTextBox1.SelectionStart;
                this.richTextBox1.Text = this.richTextBox1.Text;
                this.richTextBox1.SelectionStart = sstart;
                // Parameters
                var osearch = new OtomatSearch();
                var patterns = this.txtPatternBox.Text.Trim()
                    .Split(new char[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < patterns.Length; i++)
                {
                    var next = this.btnEnableSearchNext.Checked == true ?
                        (this.richTextBox1.SelectionStart >= this.richTextBox1.TextLength - osearch.Next
                        ? 0 : this.richTextBox1.SelectionStart)
                        : osearch.Next;
                    var result = osearch.Osearch(this.richTextBox1.Text, patterns[i].Trim(), next, this.btnCheckPatternIsApproximate.Checked);
                    if (result < 0)
                    {
                        this.richTextBox1.Text = this.richTextBox1.Text;
                        MessageBox.Show("Pattern not found!", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }
                    // Tô màu
                    this.richTextBox1.Select(result, osearch.Length);
                    this.richTextBox1.SelectionBackColor = Color.Indigo;
                    this.richTextBox1.SelectionColor = Color.White;
                    this.richTextBox1.SelectionStart += osearch.Length;
                }

                this.richTextBox1.ScrollToCaret();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void chkPatternApproximate_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.btnCheckPatternIsApproximate.Checked = this.rbtnPatternIsApproximate.Checked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCheckPatternIsApproximate_Click(object sender, EventArgs e)
        {
            try
            {
                this.rbtnPatternIsAccurate.Checked
                    = !(this.rbtnPatternIsApproximate.Checked
                    = this.btnCheckPatternIsApproximate.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearRichtextBox_Click(object sender, EventArgs e)
        {
            try
            {
                this.richTextBox1.Text = this.richTextBox1.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            try
            {
                var fopen = new OpenFileDialog();
                fopen.Filter = "Text File|*.txt|All Files|*.*";
                if (DialogResult.OK == fopen.ShowDialog())
                {
                    this.txtFileInputBox.Text = fopen.FileName;
                    var text = File.ReadAllText(fopen.FileName);
                    this.richTextBox1.Text = text;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
