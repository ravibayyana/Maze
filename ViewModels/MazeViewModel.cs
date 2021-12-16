using Maze.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Maze.ViewModels
{
    public class MazeViewModel : PropertyChangedBase
    {
        private string? filePath;
        private bool isFilePathValid;
        private List<List<CellViewModel>> maze;
        private int totalWalls;
        private string row;
        private string column;
        private bool manual = true;
        private bool automatic;
        private CellViewModel selectedCell;
        private bool isFinished;

        public MazeViewModel()
        {
            MazeRun = new ObservableCollection<CellViewModel>();
        }

        public List<List<CellViewModel>> Maze
        {
            get => maze;
            set
            {
                maze = value;
                OnPropertyChanged(nameof(Maze));
            }
        }

        public string? FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged(nameof(FilePath));
                IsFilePathValid = !string.IsNullOrWhiteSpace(value);
            }
        }

        public void UpdatePath(Direction direction)
        {
            if (SelectedCell == null || IsFinished)
                return;

            var row = 0;
            var column = 0;
            switch (direction)
            {
                case Direction.Left:
                    column = -1;
                    break;
                case Direction.Right:
                    column = 1;
                    break;
                case Direction.Up:
                    row = -1;
                    break;
                case Direction.Down:
                    row = 1;
                    break;
            }

            row += SelectedCell.Row;
            column += SelectedCell.Column;

            if (row >= 0 && row < Maze.Count && column >= 0 && column < Maze[row].Count)
            {
                var cellVm = Maze[row][column];
                if (cellVm.CellType != CellType.Wall)
                {
                    MazeRun.ToList().ForEach(c => c.IsSelected = false);
                    MazeRun.Add(cellVm);
                    SelectedCell = cellVm;
                    OnPropertyChanged(nameof(MazePath));
                    if (cellVm.CellType == CellType.Finish)
                        IsFinished = true;

                    return;
                }
            }

            SelectedCell = SelectedCell;
        }

        public string MazePath => string.Join(" => ", MazeRun.Select(x => x.Path));

        public bool IsFilePathValid
        {
            get => isFilePathValid;
            set
            {
                isFilePathValid = value;
                OnPropertyChanged(nameof(IsFilePathValid));
            }
        }

        public void LoadMazeData()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "csv file (*.csv)|*.csv";
            openFileDialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "InputFiles");

            if (openFileDialog.ShowDialog() == false)
                return;

            FilePath = openFileDialog.FileName;
            CreateMaze();
        }

        public int TotalCells { get; set; }

        public int TotalWalls
        {
            get => totalWalls;
            set
            {
                totalWalls = value;
                OnPropertyChanged(nameof(TotalWalls));
                OnPropertyChanged(nameof(Walls));
                OnPropertyChanged(nameof(ValidCells));
            }
        }

        public string Walls => $"Walls: {TotalWalls}";
        public string ValidCells => $"Empty: {TotalCells - TotalWalls}";

        public string Row
        {
            get => row;
            set
            {
                row = value;
                OnPropertyChanged(nameof(CellInfo));
            }
        }

        public string CellInfo => GetCellInfo();

        private string GetCellInfo()
        {
            if (string.IsNullOrWhiteSpace(Row) || string.IsNullOrWhiteSpace(Column))
                return string.Empty;

            var rowResult = Utils.Utils.ParseInt(Row);
            var columnResult = Utils.Utils.ParseInt(Column);
            if (rowResult.Item1 && columnResult.Item1)
            {
                var row = rowResult.Item2;
                var column = columnResult.Item2;

                if (row < Maze.Count && column < Maze[row].Count)
                {
                    var cell = Maze[row][column];
                    return cell.CellType.ToString();
                }
            }

            return "Invalid Entry";
        }

        public string Column
        {
            get => column;
            set
            {
                column = value;
                OnPropertyChanged(nameof(CellInfo));
            }
        }

        public CellViewModel StartCell { get; private set; }
        public CellViewModel FinishCell { get; private set; }

        public CellViewModel SelectedCell
        {
            get => selectedCell;
            set
            {
                selectedCell = value;
                OnPropertyChanged(nameof(SelectedCell));
                if (value == null)
                    return;
                selectedCell.IsSelected = true;
            }
        }

        public bool Manual
        {
            get => manual;
            set
            {
                manual = value;
                OnPropertyChanged(nameof(Manual));
            }
        }

        public bool Automatic
        {
            get => automatic;
            set
            {
                automatic = value;
                OnPropertyChanged(nameof(Automatic));
            }
        }

        public ObservableCollection<CellViewModel> MazeRun { get; set; }

        public bool IsFinished
        {
            get => isFinished;
            set
            {
                isFinished = value;
                OnPropertyChanged(nameof(IsFinished));
            }
        }

        public void SolveMaze()
        {
            IsFinished = false;
            MazeRun.ToList().ForEach(x => x.IsSelected = false);
            MazeRun.Clear();
            OnPropertyChanged(nameof(MazePath));

            if (Manual)
            {
                SolveManually();
                return;
            }

            AutoSolveMaze();
        }

        private void SolveManually()
        {
            SelectedCell = StartCell;
            MazeRun.Add(StartCell);            
            OnPropertyChanged(nameof(MazePath));
        }

        private void AutoSolveMaze()
        {
            var output = new List<List<CellViewModel>>();
            SolveMaze(StartCell, FinishCell, new List<CellViewModel>(), new List<CellViewModel>(), output);
            MazeRun.Clear();
            if(output.Count > 0)
            {
                output.ElementAt(0).ForEach(MazeRun.Add);
            }
            this.IsFinished = true;
        }

        List<int> xMoves = new List<int> { 0, 0, 1, -1 };
        List<int> yMoves = new List<int> { -1, 1, 0, 0 };

        private void SolveMaze(CellViewModel start, CellViewModel end, 
            List<CellViewModel> visited, 
            List<CellViewModel> tempOutput,  
            List<List<CellViewModel>> output)
        {
            if (visited.Any(v => v.Row == start.Row && v.Column == start.Column))
                return;

            visited.Add(start);
            if (tempOutput.Count == 0)
                tempOutput.Add(start);
            
            for (int i = 0; i < xMoves.Count; i++)
            {
                var x = xMoves[i] + start.Row;
                var y = yMoves[i] + start.Column;

                if (x >= 0 && x < Maze.Count && y >= 0 && y < Maze[x].Count)
                {
                    var cell = Maze.ElementAt(x).ElementAt(y);
                    if (cell.CellType == CellType.Wall)
                        continue;

                    if (visited.Any(v => v.Row == cell.Row && v.Column == cell.Column))
                        continue;

                        tempOutput.Add(cell);
                    if (cell.CellType == CellType.Finish)
                    {
                        output.Add(tempOutput);
                        return;
                    }

                    SolveMaze(cell, end, new List<CellViewModel>(visited), new List<CellViewModel>(tempOutput), output);
                }
            }
        }

        private void CreateMaze()
        {
            var rows = File.ReadAllLines(FilePath);
            int rowCount = rows.Length;

            var tempMaze = new List<List<CellViewModel>>();
            var cellCount = 0;
            var wallCount = 0;

            for (int i = 0; i < rowCount; i++)
            {
                var cells = new List<CellViewModel>();
                var columns = rows[i].Split(',');
                var columnsCount = columns.Length;

                for (int j = 0; j < columnsCount; j++)
                {
                    var cell = new CellViewModel(i, j, columns[j], this);
                    cells.Add(cell);
                    cellCount++;

                    if (cell.CellType == CellType.Wall)
                        wallCount++;
                    if (cell.CellType == CellType.Start)
                        StartCell = cell;
                    if (cell.CellType == CellType.Finish)
                        FinishCell = cell;
                }

                tempMaze.Add(cells);
            }

            this.Maze = new List<List<CellViewModel>>(tempMaze);
            this.TotalCells = cellCount;
            this.TotalWalls = wallCount;
        }
    }
}
