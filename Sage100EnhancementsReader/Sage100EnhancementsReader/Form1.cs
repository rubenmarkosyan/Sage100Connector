using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sage100EnhancementsReader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {

            folderBrowserDialog1.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtPath.Text = folderBrowserDialog1.SelectedPath;
                Environment.SpecialFolder root = folderBrowserDialog1.RootFolder;
                try
                {
                    GetEnhancementData(txtPath.Text);
                }
                catch (System.ArgumentOutOfRangeException ex)
                {
                    MessageBox.Show("The system does not have any enhancements.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message );
                }

            }
        }
        [STAThread]
        void GetEnhancementData(string path)
        {

            using (DispatchObject pvx = new DispatchObject("ProvideX.Script"))
            {

                pvx.InvokeMethod("Init", path);

                using (DispatchObject oSS = new DispatchObject(pvx.InvokeMethod("NewObject", "SY_Session")))
                {

                    using (DispatchObject sy_enhancement_svc = new DispatchObject(pvx.InvokeMethod("NewObject", "SY_Enhancement_svc", oSS.GetObject())))
                    {
                        object[] getResultSetParams = new object[] { "EnhancementName$", "EnhancementCode$", "", "", "", "", "" };
                        sy_enhancement_svc.InvokeMethodByRef("nGetResultSets", getResultSetParams);

                        string[] names = ResultSetForColumn(getResultSetParams[2]);
                        string[] codes = ResultSetForColumn(getResultSetParams[3]);

                        getResultSetParams = new object[] { "ModuleCode$", "DeveloperCode$", "", "", "", "", "" };
                        sy_enhancement_svc.InvokeMethodByRef("nGetResultSets", getResultSetParams);
                        string[] devCodes = ResultSetForColumn(getResultSetParams[2]);
                        string[] modules = ResultSetForColumn(getResultSetParams[3]);

                        for (int i = 0; i < names.Length; i++)
                        {
                            dataGridView1.Rows.Add(modules[i], devCodes[i], codes[i], names[i]);
                        }

                    }

                }

        
            }
        }

        string[] ResultSetForColumn(object o)
        {
            string result = o.ToString();
            char separetor = result.Substring(result.Length - 1)[0];
            string[] results = result.ToString().Trim().Split(separetor);
            results = results.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return results;
        }
    }
}
