using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LSLCurves
{
    class LSLWindowViewModel : PropertyChangedHandler, IDataStorage, IDataProvider
    {
        private LSLLibrary.StreamInfo[] allStreams;
        private ObservableCollection<ComboBoxItem> availableStreams;
        private ComboBoxItem selectedAvailableStream;

        #region PublicProperties
        public ObservableCollection<ComboBoxItem> AvailableStreams
        {
            get { return availableStreams; }
            set { availableStreams = value; OnPropertyChanged(); }
        }
        public ComboBoxItem SelectedAvailableStream
        {
            get { return selectedAvailableStream; }
            set { selectedAvailableStream = value; OnPropertyChanged(); }
        }
        #endregion

        public LSLWindowViewModel()
        {
            AvailableStreams = new ObservableCollection<ComboBoxItem>();
        }


        #region IDataProvider
        async Task IDataProvider.GetStream(LSLLibrary.StreamInfo[] allStreams, ObservableCollection<ComboBoxItem> availableStreams, ComboBoxItem selectedAvailableStream)
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

        void IDataProvider.ReadStream(bool isRunning, int channelsCount, LSLLibrary.StreamInlet inlet, int bufferLength, List<DataPoint[]> curves)
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
        #endregion
    }
}
