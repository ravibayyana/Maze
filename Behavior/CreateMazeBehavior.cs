using Maze.ViewModels;
using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Maze.Behavior
{
    public class CreateMazeBehavior : Behavior<Grid>
    {
        public List<List<CellViewModel>> Maze
        {
            get { return (List<List<CellViewModel>>)GetValue(MazeProperty); }
            set { SetValue(MazeProperty, value); }
        }

        public static readonly DependencyProperty MazeProperty =
            DependencyProperty.Register("Maze", typeof(List<List<CellViewModel>>), 
                typeof(CreateMazeBehavior), new PropertyMetadata(null, OnMazeChanged));

        private static void OnMazeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var behavior = o as CreateMazeBehavior;
            if (behavior == null)
                return;

            if (behavior.Maze == null)
                return;

            if (behavior.Maze.Count == 0)
                return;

            if (behavior.AssociatedObject is not Grid grid)
                return;

            var rows = behavior.Maze.Count;
            var columns = behavior.Maze.Select(x => x.Count).Max();

            Enumerable.Range(0, rows).ToList().ForEach(x => grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(20, GridUnitType.Pixel) }));
            Enumerable.Range(0, columns).ToList().ForEach(x => grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20, GridUnitType.Pixel) }));

            foreach (var row in behavior.Maze)
            {
                foreach(var cell in row)
                {
                    var contentControl = grid.FindResource("Cell") as ContentControl;
                    contentControl.Content = cell;
                    contentControl.DataContext = cell;

                    Grid.SetColumn(contentControl, cell.Column);
                    Grid.SetRow(contentControl, cell.Row);

                    grid.Children.Add(contentControl);
                }
            }
        }        
    }
}

