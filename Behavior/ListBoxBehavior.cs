using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace Maze.Behavior
{
    public class ListBoxBehavior : Behavior<ListBox>
    {
        public string UpdateMazePath
        {
            get { return (string)GetValue(UpdateMazePathProperty); }
            set { SetValue(UpdateMazePathProperty, value); }
        }
        
        public static readonly DependencyProperty UpdateMazePathProperty =
            DependencyProperty.Register("UpdateMazePath", typeof(string), typeof(ListBoxBehavior), new PropertyMetadata(null, OnMazePathUpdated));

        private static void OnMazePathUpdated(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (ListBoxBehavior)o;
            var listBox = behavior.AssociatedObject;
            listBox.Items.MoveCurrentToLast();
            listBox.ScrollIntoView(listBox.Items.CurrentItem);
        }
    }
}

