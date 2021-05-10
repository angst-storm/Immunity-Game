using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathData
{
    public PathData(Vector2 startPoint, Vector2 endPoint)
    {
        PathsPoints = CreateExclusiveForm(startPoint, endPoint).ToList();
    }

    public List<Vector2> PathsPoints { get; }

    private IEnumerable<Vector2> CreateExclusiveForm(Vector2 startPoint, Vector2 endPoint)
    {
        return new[] {startPoint, endPoint};
    }
}