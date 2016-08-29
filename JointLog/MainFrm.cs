using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace JointLogFile
{
    public partial class MainFrm : Form
    {
        string template;
        string ext;
        string path;
        public MainFrm()
        {
            InitializeComponent();
        }

        private static int compare_filename(string x, string y)
        {
            string xfilename = Path.GetFileNameWithoutExtension(x);
            string yfilename = Path.GetFileNameWithoutExtension(y);
            int xx = Int32.Parse(xfilename.Split(new char[] { '@' })[1]);
            int yy = Int32.Parse(yfilename.Split(new char[] { '@' })[1]);
            return (xx - yy);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if(dlg.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = dlg.FileName;
                template = Path.GetFileName(txtFileName.Text).Split(new char[] { '@' })[0];
                ext = Path.GetExtension(txtFileName.Text);
                path = Path.GetDirectoryName(txtFileName.Text);
                string[] files = Directory.GetFiles(path, template + "@*" + ext);
                Array.Sort<string>(files, compare_filename);
                foreach(string filename in files)
                {
                    lstFileNames.Items.Add(filename);
                }
            }
        }

        private string get_header(string filename)
        {
            StreamReader reader = new StreamReader(filename);
            string line = reader.ReadLine();
            string[] items = line.Split(new char[] { '\t' });
            string result = "";
            foreach(string item in items)
            {
                string name = item.Split(new char[] { ':' })[0];
                result += name + "#1," + name + ",";
            }
            return result;
        }

        private void btnJoint_Click(object sender, EventArgs e)
        {
            btnJoint.Enabled = false;
            StreamWriter writer = new StreamWriter(path + "\\" + template + ext);
            string header = get_header(lstFileNames.Items[0].ToString());
            writer.WriteLine(header);
            for(int i = 0; i < lstFileNames.Items.Count; i++)
            {
                string filename = (string)lstFileNames.Items[i];
                StreamReader reader = new StreamReader(filename);
                while (!reader.EndOfStream)
                    writer.WriteLine(reader.ReadLine().Replace('\t',',').Replace(':',','));
                reader.Close();
            }
            writer.Close();
            btnJoint.Enabled = true;
        }
    }
}
