using System.ComponentModel;

namespace MultiYoutubePlex.Helpers
{
    public class ProgressBarWrapper: INotifyPropertyChanged
    {
        public ProgressBarWrapper()
        {
            _currentProgress = 0;
        }
        private double _currentProgress;

        public double CurrentProgress
        {
            get => _currentProgress;
            set {
                _currentProgress = value;
                OnPropertyChanged("Soba");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
