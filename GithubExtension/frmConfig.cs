using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GithubExtension
{
    public partial class frmConfig : Form
    {
        public string GitExeLocation;

        public frmConfig()
        {
            InitializeComponent();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            txtGitLocation.Text = GitExeLocation;
            openFileDialog.Filter = "Git Binary|git.exe";
            openFileDialog.FileName = txtGitLocation.Text;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnGitLocation_Click(object sender, EventArgs e)
        {
            var mbResult = MessageBox.Show("Would you like Github Extension to search a few common locations first?", "Search Common Locations", MessageBoxButtons.YesNo);
            if (mbResult == System.Windows.Forms.DialogResult.Yes)
            {
                List<string> potentialGitLocations = new List<string>();

                var appDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                //potentialGitLocations.AddRange(Directory.GetFiles(appDataLocal, @"GitHub\\PortableGit*\\bin\\git.exe", SearchOption.AllDirectories));

                foreach (var potentialGitDirectory in Directory.GetDirectories(appDataLocal, @"GitHub\PortableGit*"))
                {
                    foreach (var potentialGitLocation in Directory.GetFiles(potentialGitDirectory, @"bin\git.exe"))
                    {
                        potentialGitLocations.Add(potentialGitLocation);
                    }
                }

                foreach (var potentialGitLocation in potentialGitLocations)
                {
                    if (testGit(potentialGitLocation))
                    {
                        MessageBox.Show("Git installation found!");
                        txtGitLocation.Text = potentialGitLocation;
                        GitExeLocation = potentialGitLocation;
                        return;
                    }
                }
                MessageBox.Show("A git installation was not automatically detected.");
            }

            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != string.Empty)
            {
                var chosenPath = openFileDialog.FileName;
                if (!testGit(chosenPath))
                {
                    MessageBox.Show("Incompatible git installation!");
                    return;
                }
                txtGitLocation.Text = openFileDialog.FileName;
                GitExeLocation = txtGitLocation.Text;
            }
        }

        private bool testGit(string gitPath){
            try{
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = gitPath;
                p.StartInfo.Arguments = "version";
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                return output.StartsWith("git version") ? true : false;
            }catch{
                return false;
            }
        }
    }
}
