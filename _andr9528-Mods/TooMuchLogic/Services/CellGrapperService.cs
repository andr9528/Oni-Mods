using System;
using System.Collections.Generic;
using System.Linq;

namespace TooMuchLogic.Services
{
    public class CellGrapperService
    {
        private bool IsCellGood(int source, Element sourceElement)
        {
            if (!Grid.IsValidCell(source))
                return false;
            if (Grid.IsCellOpenToSpace(source))
                return false;
            if (Grid.Mass[source] <= 0.0)
                return false;
            if (!(Grid.IsGas(source) || Grid.IsLiquid(source)))
                return false;
            if (Grid.Element[source] != sourceElement)
                return false;
            return true;
        }

        public HashSet<int> GrabCellsIterative(
            int source, Element sourceElement, Func<bool> earlySetFunc, Func<int, int, bool> earlyBreakFunc)
        {
            //CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(source);
            //SgtLogger.log($"Cell {source} is inside Cavity with {cavityForCell.numCells} total cells");

            HashSet<int> visited = new();
            HashSet<int> output = new();
            Queue<int> queue = new();
            queue.Enqueue(source);

            while (queue.Any())
            {
                int cell = queue.Dequeue();
                if (earlyBreakFunc(source, cell) && earlySetFunc())
                    break;

                if (visited.Contains(cell))
                    continue;
                visited.Add(cell);

                if (!IsCellGood(cell, sourceElement))
                    continue;

                output.Add(cell);
                queue.Enqueue(Grid.CellAbove(cell));
                queue.Enqueue(Grid.CellBelow(cell));
                queue.Enqueue(Grid.CellLeft(cell));
                queue.Enqueue(Grid.CellRight(cell));
            }

            return output;
        }

        public HashSet<int> GrabCellsRadius(int source, Element sourceElement, int radius)
        {
            HashSet<int> output = new();
            var queue = GrabRawCellsRadius(source, radius);

            while (queue.Any())
            {
                int cell = queue.Dequeue();

                if (!IsCellGood(cell, sourceElement))
                    continue;

                output.Add(cell);
            }

            return output;
        }

        private Queue<int> GrabRawCellsRadius(int source, int radius)
        {
            Queue<int> queue = new();
            int columnCell = GetTopLeftCell(source, radius);

            int squareLength = 2 * radius;

            for (var i = 0; i <= squareLength; i++)
            {
                queue.Enqueue(columnCell);
                int rowCell = Grid.CellRight(columnCell);

                for (var j = 0; j < squareLength; j++)
                {
                    queue.Enqueue(rowCell);
                    rowCell = Grid.CellRight(rowCell);
                }

                columnCell = Grid.CellBelow(columnCell);
            }

            return queue;
        }

        private int GetTopLeftCell(int source, int radius)
        {
            int output = source;

            for (var i = 0; i < radius; i++)
                output = Grid.CellUpLeft(output);

            return output;
        }
    }
}