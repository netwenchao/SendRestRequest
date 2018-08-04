using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLib;
using FileUpload.Common;
using Newtonsoft.Json;

namespace FileUpload
{
    public partial class MainMDI : Form
    {
        private int childFormNumber = 0;
        private string configFile = "config.txt";

        public MainMDI()
        {
            InitializeComponent();
            tsVersion.Text = "V1.3";
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            var childForm = new FormTestRest
            {
                MdiParent = this,
                Text = "窗口 " + childFormNumber++
            };
            childForm.Show();
        }


        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void MainMDI_Load(object sender, EventArgs e)
        {
            LoadWorkSpace(configFile);
            if (this.MdiChildren.Length== 0)
            {
                var childForm = new FormTestRest
                {
                    MdiParent = this,
                    Text = "窗口 " + childFormNumber++
                };
                childForm.Show();
            }
        }

        private void MainMDI_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                SaveToConfigFile(configFile);
            }
            catch (Exception exception)
            {
                if (MessageBox.Show(string.Format("保存配置文件失败:{0}.是否继续退出?", exception.Message), "提示",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
        }

        private void menuItemNewTestTool_Click(object sender, EventArgs e)
        {
            ShowNewForm(sender, e);
        }

        private void SaveToConfigFile(string fileName)
        {
            var settings = new List<FormDto>();
            foreach (var form in this.MdiChildren)
            {
                var currentForm = form as FormTestRest;
                if (currentForm != null)
                {
                    settings.Add(currentForm.GetFormData());
                }
            }
            var configedJson = JsonConvert.SerializeObject(settings);
            FileUtils.SaveFile(fileName, configedJson);
        }

        private void menuSaveworkspaceAs_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    SaveToConfigFile(saveFileDialog.FileName);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("保存配置文件失败:" + exception.Message);
                }
            }
        }

        private void menuSaveWorkspace_Click(object sender, EventArgs e)
        {
            try
            {
                SaveToConfigFile(configFile);
            }
            catch (Exception exception)
            {
                MessageBox.Show("保存配置文件失败:" + exception.Message);
            }
        }

        private void menuLoadWorkspace_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadWorkSpace(openFileDialog.FileName,MessageBox.Show("是否关闭当前已打开的窗口？","提示",MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2)==DialogResult.Yes);
            }
        }

        private void LoadWorkSpace(string fileName, bool closeCurrentForms = false)
        {
            var configJson = FileUtils.GetFileContent(fileName);
            if (!string.IsNullOrWhiteSpace(configJson))
            {
                try
                {
                    var configDtos = JsonConvert.DeserializeObject<List<FormDto>>(configJson);
                    if (configDtos == null) return;
                    if (closeCurrentForms)
                    {
                        this.CloseAllToolStripMenuItem_Click(null,null);
                    }

                    foreach (var config in configDtos)
                    {
                        var form = new FormTestRest();
                        form.ApplyFormData(config);
                        form.MdiParent = this;
                        form.Show();
                    }
                    LayoutMdi(MdiLayout.TileVertical);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(string.Format("加载配置文件出错：{0}", exception.Message));
                }
            }
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuSignatureTool_Click(object sender, EventArgs e)
        {
            var fm=new PercentEncodingTool();
            fm.MdiParent = this;
            fm.Show();
        }
    }
}
