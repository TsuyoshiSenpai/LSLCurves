using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml.Linq;

namespace LSLCurves
{
    public class LSLWindowViewModel : PropertyChangedHandler, IDataStorage, IDataProvider
    {
        private ObservableCollection<ComboBoxItem> availableStreams;
        private int selectedStreamIndex;
        private ComboBoxItem selectedAvailableStream;
        private const int bufferLength = 2000;
        private List<Plot> plots;
        private bool startIsEnabled;
        private string pathToSelectedFolder;
        private bool saveIsEnabled;
        private List<DataPoint[]> curves;
        private bool isRunning;
        private readonly DispatcherTimer timer = new DispatcherTimer();

        #region PublicProperties
        public int channelsCount;
        public LSLLibrary.StreamInfo[] allStreams;
        public LSLLibrary.StreamInlet inlet;
        public LSLWindow Window { get; set; }
        public FolderBrowserDialog FolderBrowserDialog = new FolderBrowserDialog();
        public ObservableCollection<ComboBoxItem> AvailableStreams
        {
            get { return availableStreams; }
            set { availableStreams = value; OnPropertyChanged(); }
        }
        public bool SaveIsEnabled
        {
            get { return saveIsEnabled; }
            set { saveIsEnabled = value; OnPropertyChanged(); }
        }
        public int SelectedStreamIndex
        {
            get { return selectedStreamIndex; }
            set { selectedStreamIndex = value; OnPropertyChanged(); }
        }
        public string PathToSelectedFolder
        {
            get { return pathToSelectedFolder; }
            set { pathToSelectedFolder = value; OnPropertyChanged(); }
        }
        public bool StartIsEnabled
        {
            get { return startIsEnabled; }
            set { startIsEnabled = value; OnPropertyChanged(); }
        }
        public ComboBoxItem SelectedAvailableStream
        {
            get { return selectedAvailableStream; }
            set { selectedAvailableStream = value; OnPropertyChanged(); }
        }
        public List<Plot> Plots
        {
            get { return plots; }
            set { plots = value; OnPropertyChanged(); }
        }
        public List<DataPoint[]> Curves
        {
            get { return curves; }
            set { curves = value; OnPropertyChanged(); }
        }
        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; OnPropertyChanged(); }
        }

        #endregion

        #region Commands
        public RelayCommand UpdateCommand
        { get; set; }
        public RelayCommand StartCommand
        { get; set; }
        public RelayCommand StopCommand
        { get; set; }
        public RelayCommand SelectFolderCommand
        { get; set; }
        #endregion

        public LSLWindowViewModel()
        {
            AvailableStreams = new ObservableCollection<ComboBoxItem>();
            Plots = new List<Plot>();
            GetStream(allStreams, AvailableStreams, SelectedAvailableStream);
            UpdateCommand = new RelayCommand(o => UpdateInfo(Plots));
            StopCommand = new RelayCommand(o => StopReading(IsRunning, StartIsEnabled, timer));
            StartCommand = new RelayCommand(o => StartReading(timer, StartIsEnabled, IsRunning));
            SelectFolderCommand = new RelayCommand(o => SelectFolder());
        }

        #region Methods
        public void SelectFolder()
        {
            FolderBrowserDialog.ShowDialog();
            PathToSelectedFolder = FolderBrowserDialog.SelectedPath;
        }
        public void PrepareCurves()
        {
            Curves = new List<DataPoint[]>();
            for (var i = 0; i < channelsCount; i++)
            {
                Curves.Add(new DataPoint[bufferLength]);
                for (var j = 0; j < Curves[i].Length; j++)
                    Curves[i][j] = new DataPoint(j, 0);
            }
        }
        #endregion

        #region IDataProvider
        public async Task GetStream(LSLLibrary.StreamInfo[] allStreams, ObservableCollection<ComboBoxItem> availableStreams, ComboBoxItem selectedAvailableStream)
        {
            allStreams = await Task.Run(() => LSLLibrary.resolve_streams());
            var selectedStream = new ComboBoxItem { Content = "<--Select-->" };
            selectedAvailableStream = selectedStream;
            availableStreams.Add(selectedStream);
            foreach (var stream in allStreams)
            {
                availableStreams.Add(new ComboBoxItem { Content = stream.name() });
            }
        }

        public void ReadStream(bool isRunning, int channelsCount, LSLLibrary.StreamInlet inlet, int bufferLength, List<DataPoint[]> curves)
        {
            var index = 0;
            var lslBuffLen = 4096;

            var buffer = new float[lslBuffLen, channelsCount];
            var timestamps = new double[lslBuffLen];
            while (isRunning)
            {
                var num = inlet.pull_chunk(buffer, timestamps, 0.05);

                for (var s = 0; s < num; s++)
                {
                    for (var c = 0; c < channelsCount; c++)
                    {
                        curves[c][index % bufferLength] = new DataPoint((index % bufferLength), buffer[s, c]);
                    }
                    index++;
                    if ((index % bufferLength) == 0)
                        for (var c = 0; c < channelsCount; c++)
                        {
                            for (var i = 0; i < bufferLength; i++)
                                curves[c][i] = new DataPoint(i, 0);
                        }
                }
            }
        }

        public void UpdateInfo(List<Plot> plots)
        {
            foreach (var plot in plots)
            {
                plot.InvalidatePlot();
            }
        }
        public void StopReading(bool isRunning, bool startEnabled, DispatcherTimer timer)
        {
            startIsEnabled = true;
            timer.Stop();
            timer.IsEnabled = false;
            isRunning = false;
        }
        public async void StartReading(DispatcherTimer timer, bool startEnabled, bool isRunning)
        {
            startEnabled = false;
            isRunning = true;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            timer.IsEnabled = true;
            timer.Tick += (o, args) =>
            {
                UpdateInfo(Plots);
            };
            timer.Start();

            if (!Window.Prepare()) return;
            await Task.Run(() => ReadStream(isRunning, channelsCount, inlet, bufferLength, Curves));

        }
        #endregion
    }
}
