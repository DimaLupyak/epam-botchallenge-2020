using System.Collections.Generic;
using ICanCode.Api;

namespace ICanCode.Client.Models
{
    public class AimPath
    {
        public List<Point> Path { get; }
        public int Length { get; }

        public AimPath(List<Point> path)
        {
            Path = path;
            Length = path.Count - 1;
        }
    }
}