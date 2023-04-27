using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Filecast
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            lblFile.Content = "";
            lblTime.Content = "00:00:00  |-----------------|  00:00:00";

            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Tick += new EventHandler(Tick);
            tmr.Interval = new TimeSpan(10);
            tmr.Start();

            // Get files ending in .mp3, .wav or .wma
            List<string> files = new List<string>();
            string[] allFilesInDirectory = Directory.GetFiles(".", "*.*", SearchOption.TopDirectoryOnly);
            foreach (string f in allFilesInDirectory)
                if (f.EndsWith(".mp3") || f.EndsWith(".wav") || f.EndsWith("wma"))
                    files.Add(f.Replace(".\\", ""));

            if (files.Count > 0)
            {
                Au.audioFile = new AudioFileReader(files[0]);
                fileName = files[0];
                fileNameOverflow = fileName.Length - 38;
                Au.waveOut.Init(Au.audioFile);
                Au.waveOut.Play();
                lblTime.Content = Au.waveOut.GetPositionTimeSpan().Seconds;
            }
        }


        //   T E X T   U P D A T E S

        int stylusRotate = 0;
        char[] stylus = new char[] { '|', '/', '=', '\\' };
        int fileNameScroll = 0;
        int fileNameOverflow = 0;
        string fileName = "";
        void Tick(object? sender, EventArgs e)
        {
            if (Au.audioFile != null)
            {
                // Progress stylus rotation.
                stylusRotate = Au.audioFile.CurrentTime.Seconds % 4;

                // Progress file name scroll.
                if (fileNameOverflow > 0)
                {
                    int currentScroll = ((int)Au.audioFile.CurrentTime.TotalSeconds ) % fileNameOverflow;
                    lblFile.Content = fileName.Substring(currentScroll, 39);
                }
                else
                    lblFile.Content = fileName;

                // Current time.
                string timeline = Au.audioFile.CurrentTime.ToString(@"hh\:mm\:ss");

                // Timeline.
                timeline += "  [";
                float progressF = (float)Au.audioFile.Position / (float)Au.audioFile.Length;
                int progress = (int)(progressF * 16.999f);
                if (progress > 16) // No idea how this happens, but it does.
                    progress = 16;
                for (int n = 0; n < progress; ++n)
                    timeline += "-";
                timeline += stylus[stylusRotate];
                for (int n = progress; n < 16; ++n)
                    timeline += "-";
                timeline += "]  ";

                // End time.
                timeline += Au.audioFile.TotalTime.ToString(@"hh\:mm\:ss");

                // Apply.
                lblTime.Content = timeline;
            }
        }


        //   N A V I G A T I O  N

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (Au.audioFile != null)
            {
                // Slow skip.
                if (e.Key == Key.A)
                {
                    if (Au.audioFile.CurrentTime.TotalSeconds > 5)
                        Au.audioFile.CurrentTime -= new TimeSpan(0, 0, 5);
                    else
                        Au.audioFile.CurrentTime = new TimeSpan(0);
                }
                else if (e.Key == Key.D)
                {
                    Au.audioFile.CurrentTime += new TimeSpan(0, 0, 5);
                    if (Au.audioFile.Position > Au.audioFile.Length)
                        Au.audioFile.Position = Au.audioFile.Length;
                }

                // Fast skip.
                else if (e.Key == Key.Q)
                {
                    if (Au.audioFile.CurrentTime.TotalMinutes >= 1 &&
                        Au.audioFile.CurrentTime.Seconds >= 1)
                        Au.audioFile.CurrentTime -= new TimeSpan(0, 1, 0);
                    else
                        Au.audioFile.CurrentTime = new TimeSpan(0);
                }
                else if (e.Key == Key.E)
                {
                    Au.audioFile.CurrentTime += new TimeSpan(0, 1, 0);
                    if (Au.audioFile.Position > Au.audioFile.Length)
                        Au.audioFile.Position = Au.audioFile.Length;
                }

                // Play/Pause
                else if (e.Key == Key.Space)
                {
                    if (Au.waveOut.PlaybackState == PlaybackState.Playing)
                        Au.waveOut.Pause();
                    else if (Au.waveOut.PlaybackState == PlaybackState.Paused ||
                             Au.waveOut.PlaybackState == PlaybackState.Stopped)
                        Au.waveOut.Play();
                }
            }
        }


        //   W I N D O W   D R A G

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        //   C L O S E   B U T T O N

        bool closing = false;
        private void lblX_MouseEnter(object sender, MouseEventArgs e)
        {
            lblX.Opacity = .5d;
        }
        private void lblX_MouseLeave(object sender, MouseEventArgs e)
        {
            closing = false;
            lblX.Opacity = 1d;
        }
        private void lblX_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (closing)
                Close();
        }
        private void lblX_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            closing = true;
        }


        //   M I N I M I S E   B U T T O N

        static bool minimising = false;
        private void lbl__MouseEnter(object sender, MouseEventArgs e)
        {
            lbl_.Opacity = .5d;
        }
        private void lbl__MouseLeave(object sender, MouseEventArgs e)
        {
            minimising = false;
            lbl_.Opacity = 1d;
        }
        private void lbl__MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (minimising)
                WindowState = WindowState.Minimized;
        }
        private void lbl__MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            minimising = true;
        }


        //   E X I T


    }

    static class Au
    {
        public static WaveOutEvent waveOut = new WaveOutEvent();
        public static AudioFileReader? audioFile;
    }
}
