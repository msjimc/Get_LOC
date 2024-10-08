﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Reflection;
using static System.Windows.Forms.LinkLabel;

namespace Get_LOC
{
    public partial class Form1 : Form
    {
        WebClient client;
        string url = "https://www.ncbi.nlm.nih.gov/gene/?term=";
        string urlHumanSymbol1 = "https://www.ncbi.nlm.nih.gov/gene/?term=";
        string urlHumanSymbol2 = "&report=docsum&format=text";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtLOCList.Text = "SOX9\r\nLOC751814\r\nLOC610636\r\nLOC607095\r\nLOC484356\r\nLOC480441\r\nLOC479761\r\nLOC477539\r\nLOC403585\r\nLOC106557476\r\nLOC102155886\r\nLOC102155410\r\nLOC102153034\r\nLOC102152706\r\nLOC102152620\r\nLOC102151424\r\nLOC100856786\r\nLOC100855634\r\nLOC100855600\r\nLOC100686502\r\nLOC100686073\r\nLOC100684432\r\nLOC100683370\r\nLOC100049001\r\n";

            client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            string[] entries = txtLOCList.Lines;
            txtAnswers.Clear();
            string lastTerm = "";
            string answer = "";

            string nothing = "-";
            if (chkHuman.Checked==true)
            { nothing = "-\t-"; }

            foreach (string entry in entries)
            {
                string term = entry.Trim().ToLower();

                if (string.IsNullOrEmpty(term) == false && term != lastTerm)
                {
                    if (term == "-")
                    { sb.Append("Blank\t" + nothing + "\r\n"); }
                    else
                    {
                        answer = searchBrief(term);
                        if (chkHuman.Checked == true && answer != "-\t-")
                        { answer = getHumanSymbol(answer); }
                        else if (chkHuman.Checked == true && answer == "-\t-")
                        { answer = "\t-\t-\t-"; }
                        sb.Append(entry + "\t" + answer + "\r\n");
                    }                                                       
                }
                else if (lastTerm == term)
                { sb.Append(entry + "\t" + answer + "\r\n"); }
                txtAnswers.Clear();
                txtAnswers.Text = sb.ToString();
                lastTerm = term;
                Application.DoEvents();
            }
        }
  
        private string getHumanSymbol(string shortAnswer)
        {
            string extendedAnswer = shortAnswer +"\t-";
            string[] items = shortAnswer.Split('\t');
            string name = items[1];
            int index = name.IndexOf("-like");
            if (index > -1)
            { name = name.Substring(0, index).Trim(); }

            index = name.IndexOf("[");
            if (index > -1)
            { name = name.Substring(0, index).Trim(); }

            if (name.ToLower().StartsWith(items[0].ToLower()) == true)
            { name = name.Substring(items[0].Length + 1).Trim(); }

            if (name[0] == '-')
            { name = name.Substring(1).Trim(); }

            name = name.Replace(" ", "+");

            string contents = getText(urlHumanSymbol1 + name + urlHumanSymbol2);
            if (contents == "Error") { return extendedAnswer; }

            if (contents.Contains("<pre>") == true)
            {
                int coordins = contents.IndexOf("<pre>") + 5;
                contents = contents.Substring(coordins).Trim();
            }

            if (contents == "</pre>")
            { return extendedAnswer; }

            string response = "-";
            string[] lines = contents.Split('\n');

            for (int lineCount = 0;lineCount < lines.Length;lineCount++)
            { 
                if (lines[lineCount].Contains("(human)]") == true)
                {
                    int dotIndex = lines[lineCount - 1].IndexOf(".") + 2;
                    response = lines[lineCount -1].Substring(dotIndex);
                    break;
                }
            }
            
            extendedAnswer = shortAnswer + "\t" +  response;
            return extendedAnswer;
        }

        private string searchBrief(string term)
        {
            string answer = "Error";

            char[] endOfLineChar = { '\r', '\n' };

            string thisURL = url + term + "&report=docsum&format=text";

            string contents = getText(thisURL);

            if (contents == "Error") { return answer; }

            if (contents.Contains("<pre>") == true)
            {
                int coordins = contents.IndexOf("<pre>") + 5;
                contents=contents.Substring(coordins).Trim();
            }

            string[] lines = contents.Split('\n');
            if (lines.Length > 1)
            {
                answer = lines[1];
                answer = lines[0].Substring(3) + "\t" + answer.Replace("Official Symbol: ", "").Replace(" and Name: ", " - ");
            }
            else { answer = "-\t-"; }
            return answer;
        }

        private string CleanTags(string text)
        {
            string answer = text;

            int length = text.Length + 1;
            while (length != answer.Length)
            {
                length = answer.Length;
                int first = answer.IndexOf("<");
                if (first > -1)
                {
                    int second = answer.IndexOf(">");
                    if (second > first)
                    {
                        string answerPart = answer.Substring(0, first) + answer.Substring(second + 1);
                        answer = answerPart.Trim();
                        if (answer.StartsWith("\n") == true)
                        { answer = answer.Substring(1); }
                    }
                    else
                    {
                        answer = answer.Substring(second+1);
                    }
                }
                
            }
            return answer;
        }

        private string getText(string urlToUse)
        {
            try
            {
                Stream data = client.OpenRead(urlToUse);
                StreamReader sr = new StreamReader(data);
                string fileText = sr.ReadToEnd();

                data.Close();
                sr.Close();

                return fileText;
            }
            catch { return "Error"; }
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            Form2 f2 =new Form2();
            if (f2.ShowDialog() == DialogResult.OK)
            {
                getData(f2.FileName, f2.ItemIndex, f2.Delimitor);
            }
        }

        private void getData(string name, int index, char delimitor)
        {

            System.IO.StreamReader sf = null;

            try
            {
                sf = new System.IO.StreamReader(name);
                string line = "";
                string[] items = null;

                List<string> sybmols = new List<string>();
                while (sf.Peek() > 0 )
                {
                    line = sf.ReadLine();
                    items = line.Split(delimitor);
                    if (items.Length > index)
                    { sybmols.Add(items[index].Replace("\"","")); }
                    else { sybmols.Add("-"); }
                }

                txtLOCList.Lines = sybmols.ToArray();
                
            }
            catch (Exception ex) { MessageBox.Show("Could not read file: " + ex.Message, "Error"); }
            finally
            { if (sf != null) { sf.Close(); } }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = FileString.SaveAs("Enter the file name you want to save the results too", "Text file (*.txt)|*.txt");
            if (name.Equals("Cancel") == true)
            { return; }

            System.IO.StreamWriter fw = null;

            try
            {
                fw = new System.IO.StreamWriter(name);
                string[] lines = txtAnswers.Lines;

                foreach (string line in lines)
                {
                    fw.Write(line + "\n");
                }
            }
            catch (Exception ex) { MessageBox.Show("Could not read file: " + ex.Message, "Error"); }
            finally
            { if (fw != null) { fw.Close(); } }
        }

        private void txtLOCList_TextChanged(object sender, EventArgs e)
        {
            string original = txtLOCList.Text;
            bool changed = false;
            int n = original.IndexOf("\n");
            int r = original.IndexOf("\r");
            if (n == -1 && r > -1)
            {
                original = original.Replace("\r", "\r\n");
                changed = true;
            }
            else if (n > -1 && r == -1)
            {
                original = original.Replace("\n", "\r\n");
                changed = true;
            }

            if (changed == true) { txtLOCList.Text = original; }
        }
    }
}
