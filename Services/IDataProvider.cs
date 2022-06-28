using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace LSLCurves
{
    /// <summary>
    /// Дает возможность выводить информацию в виде графиков в приложении
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Получение названия потока.
        /// </summary>
        /// <param name="allStreams">Список всех потоков.</param>
        /// <param name="availableStreams">Доступные для выбора потоки.</param>
        /// <param name="selectedAvailableStream">Выбранный поток.</param>
        /// <returns></returns>
        Task GetStream(LSLLibrary.StreamInfo[] allStreams, ObservableCollection<ComboBoxItem> availableStreams, ComboBoxItem selectedAvailableStream);
        /// <summary>
        /// Чтение данных кривых.
        /// </summary>
        /// <param name="isRunning">Булевый параметр определяющий запуск программы.</param>
        /// <param name="channelsCount">Количество графиков.</param>
        /// <param name="inlet"></param>
        /// <param name="bufferLength">Размер буфера.</param>
        /// <param name="curves">Массив с точками.</param>
        void ReadStream(bool isRunning, int channelsCount, LSLLibrary.StreamInlet inlet, int bufferLength, List<DataPoint[]> curves);
        void UpdateInfo(List<Plot> plots);
        void StopReading(bool isRunning, bool startEnabled, DispatcherTimer timer);
    }
}
