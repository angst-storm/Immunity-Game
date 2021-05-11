using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PathData
{
    public PathData(Vector2 startPoint, Vector2 endPoint)
    {
        PathsPoints = CreateExclusiveForm(startPoint, endPoint);
    }

    public List<Vector2> PathsPoints { get; }

    private List<Vector2> CreateExclusiveForm(Vector2 start, Vector2 end)
    {
        var paths = new List<List<Vector2>>
        {
            new List<Vector2>
            {
                start, new Vector2(start.x, Math.Min(start.y, end.y) + Math.Abs(start.y - end.y) / 2), end
            },
            new List<Vector2>
            {
                start, new Vector2(end.x, Math.Min(start.y, end.y) + Math.Abs(start.y - end.y) / 2), end
            },
            new List<Vector2>
            {
                start, end
            }
        };
        return paths[Random.Range(0, 3)];
    }
}