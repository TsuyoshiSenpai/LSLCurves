using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LSLCurves
{
    public class PropertyChangedHandler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
