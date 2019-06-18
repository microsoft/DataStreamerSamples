//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.System;

namespace LauncherExtension
{
    class Program
    {
        static void Main(string[] args)
        {
            LaunchElectron();
            Thread.Sleep(1000);
        }

        static private void LaunchElectron()
        {
            string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            int index = location.LastIndexOf("\\");
            string rootPath = $"{location.Substring(0, index)}\\";
            var electronPath = "\"" + rootPath + @"node_modules\electron\dist\electron.exe" + "\"";
            var appPath = "--inspect=5858 \"" + rootPath + @".\";
            try
            {
                Process newProcess = Process.Start(electronPath, appPath);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}