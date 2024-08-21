using System;
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
            foreach (string entry in entries)
            {
                string term = entry.Trim().ToLower();

                if (string.IsNullOrEmpty(term) == false && term != lastTerm)
                {
                    if (term == "-")
                    { sb.Append("Blank\t" + answer + "\r\n"); }
                    else
                    {
                        answer = searchBrief(term);
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
            answer = lines[1];
            answer = lines[0].Substring(3) + "\t" + answer.Replace("Official Symbol: ", "").Replace(" and Name: ", " - ");
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

    }
}
