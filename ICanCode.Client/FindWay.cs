using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ICanCode.Api;
using ICanCode.Client.Models;

namespace ICanCode.Client
{

    public class FindWay
    {

        public class PathNode
        {
            public Point Position { get; set; }
            public int PathLengthFromStart { get; set; }
            public PathNode CameFrom { get; set; }
            public int HeuristicEstimatePathLength { get; set; }
            public int EstimateFullPathLength => PathLengthFromStart + HeuristicEstimatePathLength;
        }

        public static AimPath GetPath(Board board, Point pointFrom, Point pointTo, List<Point> stepBarriers = null, List<Point> jumpBarriers = null)
        {
            if (stepBarriers == null) stepBarriers = new List<Point>();
            if (jumpBarriers == null) jumpBarriers = new List<Point>();

            var closedSet = new Collection<PathNode>();
            var openSet = new Collection<PathNode>();

            var startNode = new PathNode
            {
                Position = board.GetMe(),
                CameFrom = null,
                PathLengthFromStart = 0,
                HeuristicEstimatePathLength = GetHeuristicPathLength(pointFrom, pointTo)
            };
            openSet.Add(startNode);
            while (openSet.Count > 0)
            {
                var currentNode = openSet.OrderBy(node =>
                  node.EstimateFullPathLength).First();
                if (currentNode.Position == pointTo)
                {
                    return new AimPath(GetPathForNode(currentNode));
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                foreach (var neighborNode in GetNeighbors(currentNode, pointTo, board, stepBarriers, jumpBarriers))
                {
                    if (closedSet.Count(node => node.Position == neighborNode.Position) > 0)
                    {
                        continue;
                    }

                    var openNode = openSet.OrderBy(x => GetHeuristicPathLength(currentNode.Position, x.Position)).Reverse().FirstOrDefault(node =>
                      node.Position == neighborNode.Position);
                    if (openNode == null)
                        openSet.Add(neighborNode);
                    else
                        if (openNode.PathLengthFromStart > neighborNode.PathLengthFromStart)
                    {
                        openNode.CameFrom = currentNode;
                        openNode.PathLengthFromStart = neighborNode.PathLengthFromStart;
                    }
                }
            }
            return null;
        }

        private static int GetHeuristicPathLength(Point from, Point to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        private static Collection<PathNode> GetNeighbors(PathNode pathNode,
            Point goal, Board board, List<Point> stepBarriers, List<Point> jumpBarriers)
        {
            var result = new Collection<PathNode>();

            var neighborPoints = new Point[8];
            neighborPoints[0] = new Point(pathNode.Position.X + 1, pathNode.Position.Y);
            neighborPoints[1] = new Point(pathNode.Position.X - 1, pathNode.Position.Y);
            neighborPoints[2] = new Point(pathNode.Position.X, pathNode.Position.Y + 1);
            neighborPoints[3] = new Point(pathNode.Position.X, pathNode.Position.Y - 1);
            neighborPoints[4] = new Point(pathNode.Position.X + 2, pathNode.Position.Y);
            neighborPoints[5] = new Point(pathNode.Position.X - 2, pathNode.Position.Y);
            neighborPoints[6] = new Point(pathNode.Position.X, pathNode.Position.Y + 2);
            neighborPoints[7] = new Point(pathNode.Position.X, pathNode.Position.Y - 2);
            int i = -1;
            foreach (var point in neighborPoints)
            {
                i++;
                if (point.X < 0 || point.X >= board.Size)
                    continue;
                if (point.Y < 0 || point.Y >= board.Size)
                    continue;
                if(i<4 && IsBarrierAt(board, point, stepBarriers))
                    continue;
                if (i >= 4 && IsBarrierAt(board, point, jumpBarriers))
                    continue;

                var neighborNode = new PathNode
                {
                    Position = point,
                    CameFrom = pathNode,
                    PathLengthFromStart = pathNode.PathLengthFromStart + i/4,
                    HeuristicEstimatePathLength = GetHeuristicPathLength(point, goal)
                };
                result.Add(neighborNode);
                
            }
            return result;
        }

        private static bool IsBarrierAt(Board board, Point pointTo, List<Point> barriers)
        {
            var isBarrier = board.IsWallAt(pointTo.X, pointTo.Y) || barriers.Contains(pointTo);
            return isBarrier;
        }

        private static List<Point> GetPathForNode(PathNode pathNode)
        {
            var result = new List<Point>();
            var currentNode = pathNode;
            while (currentNode != null)
            {
                result.Add(currentNode.Position);
                currentNode = currentNode.CameFrom;
            }
            result.Reverse();
            return result;
        }
    }
}