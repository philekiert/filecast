using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Filecast
{
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Au.audioFile != null)
            {
                string fileName = Au.audioFile.FileName + "\n" + Au.audioFile.CurrentTime.ToString(@"hh\:mm\:ss");
                File.WriteAllText("Filecast.txt", fileName);
            }
        }
    }
}
