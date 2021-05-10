using System.Linq;
using UnityEngine;

public class Lymphnode : MonoBehaviour
{
    public void BuildPath(GameObject threat)
    {
        var thisPosition = transform.position;
        var pathData = new PathData(thisPosition, threat.transform.position);
        threat.GetComponent<Threat>().PathData = pathData;
        threat.GetComponent<LineRenderer>().SetPositions(pathData.PathsPoints.Select(v => (Vector3) v).ToArray());
    }
}