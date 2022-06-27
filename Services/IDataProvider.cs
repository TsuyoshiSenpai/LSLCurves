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
        async Task GetStream(LSLLibrary.StreamInfo[] allStreams, ObservableCollection<ComboBoxItem> availableStreams, ComboBoxItem selectedAvailableStream)
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
    }
}
