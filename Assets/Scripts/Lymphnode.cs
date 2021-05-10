using System.Linq;
using UnityEngine;

public class Lymphnode : MonoBehaviour
{
    public GameObject pathPrefab;
    public int healthPoints;

    public void BuildPath(GameObject threat)
    {
        var thisPosition = transform.position;
        var pathData = new PathData(thisPosition, threat.transform.position);
        Instantiate(pathPrefab, thisPosition, new Quaternion(), threat.transform)
            .GetComponent<LineRenderer>()
            .SetPositions(pathData.PathsPoints.Select(v => (Vector3) v).ToArray());
    }
}