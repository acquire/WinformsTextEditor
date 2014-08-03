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

namespace TextEditor
{
    public partial class Form1 : Form
    {
        string _currentFile = "Untitled";
        string CurrentFile
        {
            get { return _currentFile; }
            set
            {
                _currentFile = value;
                this.Text = Path.GetFileName(_currentFile);
            }
        }

        bool TextChanged = false;

        public Form1(string[] args = null)
        {
            InitializeComponent();
            TextChanged = false;
            if (args != null)
            {
                if (args.Length > 0)
                {
                    try
                    {
                        CurrentFile = args[0];
                        using (StreamReader sr = new StreamReader(CurrentFile))
                        {
                            textBox1.Text = sr.ReadToEnd();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                    }
                }

            }
            TextChanged = false;

            //custom keys
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form_KeyDown);
            this.textBox1.DragDrop += new DragEventHandler(textBox1_DragDrop);
            this.textBox1.DragOver += new DragEventHandler(textBox1_DragOver);
            
        }

        void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                SaveButton_Click(sender, e);
            }
            if (e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                SendKeys.Send("    ");
            }
        }

        //open file button
        private void OpenButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                ReadFile(openFileDialog1.FileName);
            }
        }

        //save file button
        private void SaveButton_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (CurrentFile == "Untitled")
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    CurrentFile = saveFileDialog1.FileName + saveFileDialog1.DefaultExt;
                    using (StreamWriter sw = new StreamWriter(CurrentFile))
                    {
                        sw.Write(textBox1.Text);
                        TextChanged = false;
                    }
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(CurrentFile))
                {
                    sw.Write(textBox1.Text);
                    TextChanged = false;
                }
            }


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TextChanged == true)
            {
                var result = MessageBox.Show("Do you want to save changes to " + _currentFile + "?", "Text Editor", MessageBoxButtons.YesNoCancel);
                if (result.Equals(DialogResult.Yes))
                {
                    SaveButton_Click(sender, e);
                }
                if (result.Equals(DialogResult.Cancel))
                {
                    e.Cancel = true;
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextChanged = true;
        }

        private void NewDocumentButton_Click(object sender, EventArgs e)
        {
            var form = new Form1();
            form.Show();
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
                string[] File = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                string f = File[0];
                ReadFile(f);
                    
        }

        void textBox1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        public void WriteFile()
        {

        }
        public void ReadFile(string pathToFile)
        {
            CurrentFile = pathToFile + saveFileDialog1.DefaultExt;  //change title

            try
            {
                using (StreamReader sr = new StreamReader(pathToFile))
                {
                    textBox1.Text = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }

            TextChanged = false;

        }

    }
}
