using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace AddPinyin
{
    class variables
    {
        public static Interfaces.IHost objEditor = null;
        public static Type MarcEngine = null;
        public static object MEObject = null;
        public static DirectoryInfo meDir = new FileInfo(new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).AbsolutePath.Replace("%20", " ")).Directory;
    }
}
