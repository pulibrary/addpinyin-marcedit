using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace AddPinyin
{
    public class Connection
    {
        public static void Startup(Interfaces.IHost objEditor)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, argsAssembly) =>
            {
                if (argsAssembly.Name.StartsWith("System.Data.SQLite"))
                {
                    String dllName = argsAssembly.Name.Split(',').First<string>() + ".dll";
                    FileInfo theDLL = variables.meDir.GetFiles(dllName).FirstOrDefault<FileInfo>();
                    return Assembly.LoadFile(theDLL.FullName);
                }                
                return null;
            };
            if (objEditor != null)
            {
                variables.objEditor = objEditor;
            }
            else
            {
                MessageBox.Show("This plugin can only be run from within the MarcEditor");
                return;
            }
            DialogResult proceed = MessageBox.Show("Warning: This operation cannot be undone, and the file will be saved.  You may want to back up the original file.  Click 'OK' to proceed or 'Cancel' to abort."
                , "Message", MessageBoxButtons.OKCancel);

            if (proceed.Equals(DialogResult.OK))
            {
                //changes location where plugin looks for SQL Romanization table
                string profilepath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string mepath = profilepath + "\\marcedit75\\plugins75";
                if (!Directory.Exists(mepath))
                {
                    mepath = profilepath + "\\marcedit7\\plugins";
                }
                if(!Directory.Exists(mepath))
                {
                    mepath = profilepath + "\\marcedit\\plugins";
                }

                AppDomain.CurrentDomain.SetData("DataDirectory", mepath);
                frmMain objf = new frmMain();
            }
        }
    }
}
