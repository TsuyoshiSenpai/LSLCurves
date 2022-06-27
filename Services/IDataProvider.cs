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
    /// <summary>
    /// Дает возможность выводить информацию в виде графиков в приложении
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Получение названия потока
        /// </summary>
        /// <param name="allStreams">Список всех потоков</param>
        /// <param name="availableStreams">Доступные для выбора потоки</param>
        /// <param name="selectedAvailableStream">Выбранный поток</param>
        /// <returns></returns>
        Task GetStream(LSLLibrary.StreamInfo[] allStreams, ObservableCollection<ComboBoxItem> availableStreams, ComboBoxItem selectedAvailableStream);
        void ReadStream(bool isRunning, int channelsCount, LSLLibrary.StreamInlet inlet, int bufferLength, List<DataPoint[]> curves);
    }
}
