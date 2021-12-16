using Maze.Utils;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Maze.Behavior
{
    public class ContentControlBehavior : Behavior<ContentControl>
    {
        public bool IsFinished
        {
            get { return (bool)GetValue(IsFinishedProperty); }
            set { SetValue(IsFinishedProperty, value); }
        }
        
        public static readonly DependencyProperty IsFinishedProperty =
            DependencyProperty.Register("IsFinished", typeof(bool), typeof(ContentControlBehavior), new PropertyMetadata(false));


        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(ContentControlBehavior),
                new PropertyMetadata(false, OnIsSelectedChanged));

        private static void OnIsSelectedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var selected = (bool)e.NewValue;
            if (!selected)
                return;

            var behavior = o as ContentControlBehavior;
            if (behavior == null)
                return;

            Keyboard.ClearFocus();
            Keyboard.Focus(behavior.AssociatedObject);
        }

        public Direction Direction
        {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }
        
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Direction), 
                typeof(ContentControlBehavior), 
                new PropertyMetadata(Direction.None));

        protected override void OnAttached()
        {
            base.OnAttached();
            if (this.AssociatedObject == null)
                return;

            this.AssociatedObject.PreviewKeyDown += OnKeyDown;
            this.AssociatedObject.Unloaded += OnUnLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewKeyUp -= OnKeyDown;
        }

        private void OnUnLoaded(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.PreviewKeyUp -= OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (IsFinished)
                return;

            switch(e.Key)
            {
                case Key.Left:
                    Direction = Direction.Left;
                    return;
                case Key.Right:
                    Direction = Direction.Right;
                    return;
                case Key.Up:
                    Direction = Direction.Up;
                    return;
                case Key.Down:
                    Direction = Direction.Down;
                    return;
                default:
                    return;
            }
        }
    }
}

