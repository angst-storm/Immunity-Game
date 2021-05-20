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

    // public static List<Vector2> CreateExclusiveForm(Vector2 start, Vector2 end)
    // {
        // var v1 = new Vector2(1, 0);
        // var v2 = new Vector2(end.x - start.x, end.y - start.y); 
        // var lenV1 = 1;
        // var lenV2 = Math.Sqrt(v2.x * v2.x + v2.y * v2.y);
        // var startAngle = Math.Atan2(v2.x / lenV2, v2.y / lenV2);
        // // костыль немножка, не придумал как решить (не работающий костыль кста)
        // if (start.x > end.x && start.y < end.y || start.x < end.x && start.y > end.y)
        //     startAngle += Math.PI;
        //
        // var randomAngle = Math.PI * Random.Range(0,40) / 180;
        // var randomSign = Random.Range(-1, 1);
        // // деление на ноль
        // randomSign /= Math.Abs(randomSign);
        //
        // var cos = Math.Cos(Math.PI - 2 * randomAngle);
        // var sideLength = Math.Sqrt(lenV2 * lenV2 / (2 * (1 - cos)));
        // var point = new Vector2((int)(sideLength * Math.Cos(startAngle + randomSign * randomAngle) + start.x),
        //     (int)(sideLength * Math.Sin(startAngle + randomSign * randomAngle) + start.y));
        //     
        // var route = new List<Vector2>
        // {
        //     start,
        //     point,
        //     end
        // };
        //     
        // return route;
    //     return 
    // }

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