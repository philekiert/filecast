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
                string output = Au.audioFile.FileName + "\n" + Au.audioFile.CurrentTime.ToString(@"hh\:mm\:ss");
                output += "\n---"; // --- indicates start of played file list.
                foreach (var playedFile in Au.playedTracks)
                    output += "\n" + playedFile.Key;

                File.WriteAllText("Filecast.txt", output);
            }
        }
    }
}
