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
                    if (term.StartsWith("loc") == true)
                    {
                        answer = searchThisLOCTerm(term);
                        if (answer != "Error")
                        { sb.Append(entry + "\t" + answer + "\r\n"); }
                        else
                        { sb.Append(entry + "\t" + answer + "\r\n"); }
                    }
                    else
                    {
                        answer = searchThis(term);
                        if (answer != "Error")
                        { sb.Append(entry + "\t" + answer + "\r\n"); }
                        else
                        { sb.Append(entry + "\t" + answer + "\r\n"); }
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

        private string searchThisLOCTerm(string term) 
        {
            string answer = "Error";

            char[] endOfLineChar = { '\r', '\n', '<' };
            
            string thisURL = url + term;
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            string contents = getText(thisURL);

            int index = contents.ToLower().IndexOf(">" + term.ToLower());
            if (index > -1)
            {
                int endOfLine = contents.IndexOfAny(endOfLineChar, index + 10);
                if (endOfLine > -1)
                { 
                    int startTextClipFrom = index + 1 + term.Length;
                    answer = contents.Substring(startTextClipFrom, endOfLine - startTextClipFrom);
                    if (answer.Contains("- Gene") == true)
                    {
                        int newEnd = answer.IndexOf("- Gene");
                        if (newEnd > -1)
                        { answer =  answer.Substring(0, newEnd); }
                    }
                    else if (string.IsNullOrEmpty(answer) == true)
                    {
                        index = contents.ToLower().IndexOf("<title>") + 7;
                        endOfLine = contents.IndexOf("</title>", index);
                        answer = contents.Substring(index, endOfLine-index);
                        if (answer.Contains("- Gene") == true)
                        {
                            int newEnd = answer.IndexOf("- Gene");
                            if (newEnd > -1)
                            { answer = answer.Substring(0, newEnd); }
                        }
                    }
                }
            }

            return answer.Trim();

        }

        private string searchThis(string term)
        {
            string answer = "Error";

            char[] endOfLineChar = { '\r', '\n' };

            string thisURL = url + term;
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            string contents = getText(thisURL);

            if (contents == "Error") { return answer; }
            //contents = CleanTags(contents);
            
            int startPoint = contents.IndexOf("https://www.ncbi.nlm.nih.gov/gene/") + 1;
            if (startPoint == 0)
            { startPoint = contents.IndexOf("<h3>See <a href=\"/gene/") + 1; }
            int index = contents.ToLower().IndexOf(">" + term.ToLower(), startPoint);
            if (index > -1)
            {
                int endOfLine = contents.IndexOfAny(endOfLineChar, index + term.Length + 10);
                if (endOfLine > -1)
                {
                    int textClipStar = index + 3 + term.Length;
                    answer = contents.Substring(textClipStar, endOfLine - textClipStar); 
                    if (answer.Contains("<") == true)
                    { answer=CleanTags(answer); }
                }
            }            

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

    }
}
