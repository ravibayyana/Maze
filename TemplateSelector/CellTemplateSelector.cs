using Maze.Utils;
using Maze.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Maze.TemplateSelector
{
    public class CellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Empty { get; set; }
        public DataTemplate Wall { get; set; }
        public DataTemplate Start { get; set; }
        public DataTemplate Finish { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            var cellViewModel = item as CellViewModel;
            if (cellViewModel == null)
                return null;

            switch (cellViewModel.CellType)
            {
                case CellType.Start:
                    return Start;
                case CellType.Finish:
                    return Finish;
                case CellType.Wall:
                    return Wall;
                default:
                    return Empty;
            }
        }
    }
}

