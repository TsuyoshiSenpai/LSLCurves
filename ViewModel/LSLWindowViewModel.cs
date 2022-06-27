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
    }
}
