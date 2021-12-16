using System.ComponentModel;

namespace Maze.ViewModels
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
