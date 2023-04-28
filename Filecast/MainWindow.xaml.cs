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

            Blackout();

            DispatcherTimer tmr = new DispatcherTimer();
            tmr.Tick += new EventHandler(Tick);
            tmr.Interval = new TimeSpan(10);
            tmr.Start();

            // Pick up where we left off, if possible.
            List<string> files = GetFileList();
            int fileIndex = 0;
            TimeSpan filePosition = new TimeSpan(0);
            if (files.Count > 0)
            {
                if (File.Exists("Filecast.txt"))
                {
                    string[] lines = File.ReadAllLines("Filecast.txt");
                    if (lines.Length >= 1)
                    {
                        for (int n = 0; n < files.Count; ++n)
                        {
                            if (files[n] == lines[0])
                            {
                                fileIndex = n;
                                break;
                            }
                        }
                        if (lines.Length >= 2) // Read time only if stated.
                        {
                            filePosition = TimeSpan.Parse(lines[1]);
                        }
                    }
                }
                PlayFile(files[fileIndex], filePosition);
            }
        }

        List<string> GetFileList()
        {
            List<string> files = new List<string>();
            string[] allFilesInDirectory = Directory.GetFiles(".", "*.*", SearchOption.TopDirectoryOnly);
            foreach (string f in allFilesInDirectory)
                if (f.EndsWith(".mp3") || f.EndsWith(".wav") || f.EndsWith("wma"))
                    files.Add(f.Replace(".\\", ""));
            return files;
        }
        void PlayFile(string file, TimeSpan time)
        {
            try
            {
                Au.audioFile = new AudioFileReader(file);
                Au.audioFile.CurrentTime = time;
                fileName = file;
                fileNameOverflow = fileName.Length - 38;
                Au.waveOut.Stop();
                Au.waveOut.Init(Au.audioFile);
                Au.waveOut.Play();
                ranIntoUnsupportedFile = false;
            }
            catch
            {
                fileName = file;
                ranIntoUnsupportedFile = true;
                Au.waveOut.Stop();
            }
        }
        void PlayFile(string file, long startPos)
        {
            try
            {
                Au.audioFile = new AudioFileReader(file);
                Au.audioFile.Position = startPos;
                fileName = file;
                fileNameOverflow = fileName.Length - 38;
                Au.waveOut.Stop();
                Au.waveOut.Init(Au.audioFile);
                Au.waveOut.Play();
                ranIntoUnsupportedFile = false;
            }
            catch
            {
                fileName = file;
                ranIntoUnsupportedFile = true;
                Au.waveOut.Stop();
            }
        }


        //   T E X T   U P D A T E S

        int stylusRotate = 0;
        char[] stylus = new char[] { '|', '/', '=', '\\' };
        int fileNameScroll = 0;
        int fileNameOverflow = 0;
        string fileName = "";
        bool ranIntoUnsupportedFile = false;
        void Tick(object? sender, EventArgs e)
        {
            if (Au.audioFile != null)
            {
                if (fileName.Length > 0 && !ranIntoUnsupportedFile)
                {
                    // Progress stylus rotation.
                    stylusRotate = Au.audioFile.CurrentTime.Seconds % 4;

                    // Progress file name scroll.
                    if (fileNameOverflow > 0)
                    {
                        int currentScroll = ((int)Au.audioFile.CurrentTime.TotalSeconds) % fileNameOverflow;
                        string clip = fileName.Substring(currentScroll, 39);

                        // Player only plays files with three-letter extensions, if this changes this will need a re-write.
                        if (fileNameOverflow - currentScroll <= 4)
                        {
                            runFileName.Text = clip.Remove(clip.LastIndexOf('.'));
                            runFileExt.Text = clip.Substring(clip.LastIndexOf('.'));
                        }
                        else
                        {
                            runFileName.Text = clip;
                            runFileExt.Text = "";
                        }

                    }
                    else
                    {
                        runFileName.Text = fileName.Remove(fileName.LastIndexOf('.'));
                        runFileExt.Text = fileName.Substring(fileName.LastIndexOf('.'));
                    }

                    // Current time.
                    runStartTime.Text = Au.audioFile.CurrentTime.ToString(@"hh\:mm\:ss");

                    // Timeline.
                    runStart.Text = " [";
                    float progressF = (float)Au.audioFile.Position / (float)Au.audioFile.Length;
                    int progress = (int)(progressF * 16.999f);
                    string bar = "";
                    if (progress > 16) // No idea how this happens, but it does.
                        progress = 16;
                    for (int n = 0; n < progress; ++n)
                        bar += "-";
                    runBar1.Text = bar;
                    runStylus.Text = Au.waveOut.PlaybackState == PlaybackState.Playing ? ">" : "\"";
                    bar = "";
                    for (int n = progress; n < 16; ++n)
                        bar += "-";
                    runBar2.Text = bar;

                    runEnd.Text = "] ";

                    // End time.
                    runEndTime.Text = Au.audioFile.TotalTime.ToString(@"hh\:mm\:ss");
                }
                else
                    Blackout();

                // Move to the next track if we're at the end.
                if (Au.audioFile.Position >= Au.audioFile.Length)
                    NextTrack(true);
            }
            // If audioFile was never initialised, keep checking for tracks and Blackout() if it's ever back to null.
            else
            {
                Blackout();
                if (GetFileList().Count > 0)
                    NextTrack(true);
            }
        }
        void Blackout()
        {
            if (ranIntoUnsupportedFile)
            {
                if (fileName.Length > 39)
                    runFileName.Text = fileName.Remove(39);
                else
                    runFileName.Text = fileName;
                runStartTime.Text = "corrupt or unsupported.";
            }
            else
            {
                runFileName.Text = "No file.";
                runStartTime.Text = "";
            }
            runFileExt.Text = "";
            runStart.Text = "";
            runBar1.Text = "";
            runStylus.Text = "";
            runBar2.Text = "";
            runEnd.Text = "";
            runEndTime.Text = "";
        }


        //   N A V I G A T I O  N

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (Au.audioFile != null)
            {
                if (e.Key == Key.A && fileName.Length > 0)
                {
                    // Fast skip backward.
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        if (Au.audioFile.CurrentTime.TotalMinutes >= 1 &&
                            Au.audioFile.CurrentTime.Seconds >= 1)
                            Au.audioFile.CurrentTime -= new TimeSpan(0, 1, 0);
                        else
                            Au.audioFile.CurrentTime = new TimeSpan(0);
                    }
                    // Slow skip backward.
                    else
                    {
                        if (Au.audioFile.CurrentTime.TotalSeconds > 5)
                            Au.audioFile.CurrentTime -= new TimeSpan(0, 0, 5);
                        else
                            Au.audioFile.CurrentTime = new TimeSpan(0);
                    }
                }
                else if (e.Key == Key.D && fileName.Length > 0)
                {
                    // Fast skip forward.
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        Au.audioFile.CurrentTime += new TimeSpan(0, 1, 0);
                        if (Au.audioFile.Position > Au.audioFile.Length)
                            Au.audioFile.Position = Au.audioFile.Length;
                    }
                    // Slow skip backward.
                    else
                    {
                        Au.audioFile.CurrentTime += new TimeSpan(0, 0, 5);
                        if (Au.audioFile.Position > Au.audioFile.Length)
                            Au.audioFile.Position = Au.audioFile.Length;
                    }
                }

                // Play/Pause
                else if (e.Key == Key.Space && fileName.Length > 0)
                {
                    if (Au.waveOut.PlaybackState == PlaybackState.Playing)
                        Au.waveOut.Pause();
                    else if (Au.waveOut.PlaybackState == PlaybackState.Paused ||
                             Au.waveOut.PlaybackState == PlaybackState.Stopped)
                        Au.waveOut.Play();
                }

                // Skip track.
                else if (e.Key == Key.Q || e.Key == Key.E)
                {
                    List<string> files = GetFileList();

                    if (files.Count > 0)
                    {
                        // Backward.
                        if (e.Key == Key.Q)
                            NextTrack(false, files);

                        // Forward.
                        else
                        {
                            NextTrack(true, files);
                        }
                    }
                    else
                    {
                        Au.waveOut.Stop();
                        fileName = "";
                    }
                }
            }
        }
        void NextTrack(bool forward)
        {
            NextTrack(forward, GetFileList());
        }
        void NextTrack(bool forward, List<string> files)
        {
            if (files.Count > 0)
            {
                int currentIndex = 0;
                for (int n = 0; n < files.Count; ++n)
                    if (files[n] == fileName)
                    {
                        currentIndex = n;
                        break;
                    }

                // Backward.
                if (forward)
                {
                    ++currentIndex;
                    if (currentIndex == files.Count)
                        currentIndex = 0;
                }
                // Forward.
                else
                {
                    --currentIndex;
                    if (currentIndex == -1)
                        currentIndex = files.Count - 1;
                }

                PlayFile(files[currentIndex], 0);
            }
            else
            {
                Au.waveOut.Stop();
                fileName = "";
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
    }

    static class Au
    {
        public static WaveOutEvent waveOut = new WaveOutEvent();
        public static AudioFileReader? audioFile;
    }
}
