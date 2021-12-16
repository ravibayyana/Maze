using Maze.ViewModels;
using System.Windows;

namespace Maze
{
    /// <summary>
    /// Interaction logic for MazeWindow.xaml
    /// </summary>
    public partial class MazeWindow : Window
    {
        public MazeViewModel MazeViewModel { get; set; }
        public MazeWindow()
        {
            InitializeComponent();

            this.MazeViewModel = new MazeViewModel();

            this.DataContext = this.MazeViewModel;
        }
    }
}
