using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathData
{
    private static readonly HashSet<Color> Colors = new HashSet<Color>
    {
        new Color(0.36f, 0.70f, 0.38f, 1),
        new Color(0.80f, 0.30f, 0.41f, 1),
        new Color(0.97f, 0.89f, 0.46f, 1),
        new Color(0.07f, 0.68f, 0.76f, 1),
        new Color(0.98f, 0.57f, 0.32f, 1)
    };

    public static readonly Dictionary<Vector2, List<Vector2>> Paths = new Dictionary<Vector2, List<Vector2>>
    {
        {
            new Vector2(1.5f, -3),
            new List<Vector2> {new Vector2(0, 0), new Vector2(1.5f, -3)}
        },
        {
            new Vector2(3, 1.5f),
            new List<Vector2> {new Vector2(0, 0), new Vector2(3, 1.5f)}
        },
        {
            new Vector2(1.5f, 3),
            new List<Vector2> {new Vector2(0, 0.5f), new Vector2(1.5f, 1.25f), new Vector2(1.5f, 3)}
        },
        {
            new Vector2(-4.5f, 2.25f),
            new List<Vector2> {new Vector2(0, 0), new Vector2(-2.25f, 0.45f), new Vector2(-4.5f, 2.25f)}
        },
        {
            new Vector2(-4.5f, -1.5f),
            new List<Vector2> {new Vector2(0, 0), new Vector2(-0.75f, -0.75f), new Vector2(-4.5f, -1.5f)}
        },
        {
            new Vector2(-1.5f, 3),
            new List<Vector2> {new Vector2(0, 0.5f), new Vector2(-1, 1.25f), new Vector2(-1.5f, 3)}
        },
        {
            new Vector2(3, -1.5f),
            new List<Vector2> {new Vector2(0, 0), new Vector2(1.25f, -1), new Vector2(3, -1.5f)}
        },
        {
            new Vector2(-1.5f, -3),
            new List<Vector2> {new Vector2(0, -0.5f), new Vector2(-1.5f, -1.25f), new Vector2(-1.5f, -3)}
        },
    };

    public PathData(Vector2 endPoint, List<PathData> createdPaths)
    {
        var acceptableColors = Colors.Except(createdPaths.Select(p => p.PathColor)).ToList();
        PathColor = !acceptableColors.Any()
            ? new Color(0.35f, 0.7f, 0.38f, 1)
            : acceptableColors[Random.Range(0, acceptableColors.Count)];
        PathPoints = Paths[endPoint];
    }

    public List<Vector2> PathPoints { get; }
    public Color PathColor { get; }
}