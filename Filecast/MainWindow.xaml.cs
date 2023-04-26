using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Filecast
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            WaveOutEvent outputDevice = new WaveOutEvent();
            AudioFileReader audioFile;


            // Get files ending in .mp3, .wav or .wma
            List<string> files = new List<string>();
            string[] allFilesInDirectory = Directory.GetFiles(".", "*.*", SearchOption.TopDirectoryOnly);
            foreach (string f in allFilesInDirectory)
                if (f.EndsWith(".mp3") || f.EndsWith(".wav") || f.EndsWith("wma"))
                    files.Add(f.Replace(".\\", ""));

            if (files.Count > 0)
            {
                audioFile = new AudioFileReader(files[0]);
                outputDevice.Init(audioFile);
                outputDevice.Play(); 
            }
        }
    }
}
