using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Get_LOC
{
    public partial class Form2 : Form
    {
        private string fileName = "";
        private List<string> lines = null;
        private Char delimitor;
        private int index = -1;

        public Form2()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            string name = FileString.OpenAs("Select the file containing the gene SYMBOL terms", "Delimitored text file*.txt;*.csv;*.xls|*.txt;*.csv;*.xls");
            if (System.IO.File.Exists(name)==false) 
            { return; }

            System.IO.StreamReader sf = null;

            try 
            { 
                sf =new System.IO.StreamReader(name);
                lines = new List<string>();

                while (sf.Peek() > 0 && lines.Count < 10)
                {
                    lines.Add(sf.ReadLine());
                }
                DisplayData();
                nudIndex.Enabled = false;
                txtChar.Enabled = true;
                fileName = name;
            }
            catch (Exception ex) { MessageBox.Show("Could not read file: " + ex.Message, "Error"); }
            finally
            { if (sf != null) { sf.Close(); } }

        }

        private void DisplayData()
        {
            txtDisplay.Clear();

            if (lines == null || lines.Count == 0)
            { return; }

            if (delimitor != new char())
            {
                index = (int)nudIndex.Value - 1;
                foreach (string line in lines)
                {
                    string[] items = line.Split(delimitor);
                    if (items.Length > index)
                    { txtDisplay.Text += items[index].Replace("\"","") + "\r\n"; }
                    else { txtDisplay.Text += "-\r\n"; }
                }
            }
            else
            {
                txtDisplay.Lines = lines.ToArray();
            }
        }

        private int getMaximumIndex(char delimitor)
        {
            int max = -1;
            foreach (string line in lines)
            {
                if (line.Split(delimitor).Length > max)
                { max = line.Split(delimitor).Length; }
            }
            return max;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void txtChar_TextChanged(object sender, EventArgs e)
        {
           txtChar.Enabled = true;
            if (txtChar.Text.Length == 1)
            { delimitor = txtChar.Text[0]; }
            else if (txtChar.Text.Equals("\\t") == true)
            { delimitor = '\t'; }
            else { delimitor = new char(); }

            int max = getMaximumIndex(delimitor);
            if (max > 0)
            {
                nudIndex.Maximum = max;
                nudIndex.Value = 1;
                nudIndex.Enabled = true;
            }
            else 
            {
                nudIndex.Value = 1;
                nudIndex.Enabled = false; 
            }

            DisplayData();
        }

        private void nudIndex_ValueChanged(object sender, EventArgs e)
        {
            DisplayData();
        }

        public int ItemIndex { get { return index; } }

        public string FileName { get { return fileName; } }

        public char Delimitor { get { return delimitor; } }

    }
}
