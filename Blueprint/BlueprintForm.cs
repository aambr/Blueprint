using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace Blueprint
{
    public partial class BlueprintForm : Form
    {
        protected readonly Regex re = new Regex(@"^(/?)([\w\.]+/)*([\w\.]+)?(?::([\w]+))?$");

        public BlueprintForm()
        {
            InitializeComponent();
        }

        private void basePathBtn_Click(object sender, EventArgs e)
        {
            if (basePathDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                basePathLbl.Text = basePathDialog.SelectedPath;
                generateBtn.Enabled = true;
            }
        }

        private void quitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void generateBtn_Click(object sender, EventArgs e)
        {
            string basePath = this.basePathDialog.SelectedPath + "/",
                   path = basePath;

            foreach (string line in structureTxt.Lines)
            {
                GroupCollection groups = re.Match(line).Groups;

                if (groups[1].Value == "/")
                {
                    path = basePath;
                    foreach (Capture capt in groups[2].Captures)
                        path += capt;
                }
                else if (groups[2].Captures.Count > 0)
                {
                    foreach (Capture capt in groups[2].Captures)
                        path += capt;
                }

                if (groups[3].ToString() != String.Empty) // file
                {
                    try
                    {
                        StreamWriter writer = File.CreateText(path + groups[3]);
                        if (groups[4].ToString() != String.Empty)
                        {
                            string blueprintName = groups[4].ToString();

                            try
                            {
                                StreamReader blueprint = new StreamReader(Application.StartupPath + "/blueprints/" + blueprintName + ".txt");
                                writer.Write(blueprint.ReadToEnd());
                                blueprint.Close();
                            }
                            catch (FileNotFoundException)
                            {
                                MessageBox.Show("Blueprint " + blueprintName + " not found", "Blueprint");
                            }
                        }
                        writer.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else // directory
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

            MessageBox.Show("Blueprint created!", "Blueprint");
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter writer = new StreamWriter(saveFileDialog.FileName);
                foreach (string line in structureTxt.Lines)
                    writer.WriteLine(line);
                writer.Close();
            }
        }

        private void openMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamReader reader = new StreamReader(openFileDialog.FileName);
                structureTxt.Text = reader.ReadToEnd();
            }
        }
    }
}
