using System;
using System.Collections.Generic;

namespace Maze.Utils
{
    public static class Utils
    {
        static Dictionary<string, CellType> StringToCellTypeMapper = new Dictionary<string, CellType>
        {
            {"X", CellType.Wall },
            {"S", CellType.Start },
            {"F", CellType.Finish },
        };

        static Dictionary<CellType, string> CellTypeToStringMapper = new Dictionary<CellType, string>
        {
            {CellType.Wall, "X" },
            {CellType.Start, "S" },
            {CellType.Finish, "F" },
            {CellType.Empty, "" },
        };

        public static Tuple<bool, int> ParseInt(string value)
        {
            if (int.TryParse(value, out int result))
            {
                result--;
                if (result >= 0)
                    return new Tuple<bool, int>(true, result);
            }

            return new Tuple<bool, int>(false, int.MaxValue);
        }

        public static string GetCellString(CellType cellType)
        {
            if (CellTypeToStringMapper.ContainsKey(cellType))
                return CellTypeToStringMapper[cellType];

            return "";
        }

        public static CellType GetCellType(string cellType)
        {
            if (StringToCellTypeMapper.ContainsKey(cellType))
                return StringToCellTypeMapper[cellType];

            return CellType.Empty;
        }
    }
}
