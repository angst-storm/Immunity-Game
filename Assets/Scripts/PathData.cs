using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathData
{
    public PathData(Vector2 startPoint, Vector2 endPoint)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        PathsPoints = CreateExclusiveForm(startPoint, endPoint).ToList();
    }

    public Vector2 StartPoint { get; }
    public Vector2 EndPoint { get; }
    public List<Vector2> PathsPoints { get; }

    //вот этот метод надо расширить чтоб он создавал путь поинтереснее
    private IEnumerable<Vector2> CreateExclusiveForm(Vector2 startPoint, Vector2 endPoint)
    {
        return new[] {startPoint, endPoint};
    }
}