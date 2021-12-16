using Maze.Utils;

namespace Maze.ViewModels
{
    public class CellViewModel : PropertyChangedBase
    {
        private bool isSelected;
        private Direction direction;
        private MazeViewModel parent;

        public CellViewModel(int row, int column, string cellType, MazeViewModel parent)
        {
            Row = row;
            Column = column;
            this.parent = parent;
            CellType = Utils.Utils.GetCellType(cellType);
        }

        public string Path => this.GetPath();
        
        private string GetPath()
        {
            if (CellType == CellType.Start)
                return $"[{Row + 1}:{Column + 1}]:START";
            if (CellType == CellType.Finish)
                return $"[{Row + 1}:{Column + 1}]:FINISH";
            
            return $"[{Row + 1}:{Column + 1}]";
        }
        public Direction Direction
        {
            get => direction;
            set
            {
                direction = value;
                OnPropertyChanged(nameof(Direction));
                parent.UpdatePath(value);
            }
        }

        public int Row { get; }
        public int Column { get; }        
        public CellType CellType { get; }

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public override string ToString()
        {
            return $"{Utils.Utils.GetCellString(CellType)}";
        }
    }
}
