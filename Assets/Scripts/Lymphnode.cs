using System.Linq;
using UnityEngine;

public class Lymphnode : MonoBehaviour
{
    public GameObject unitPrefab;
    public GameController controller;

    public void BuildPath(GameObject threat)
    {
        var pathDates = controller.threats.Select(t => t.GetComponent<Threat>().PathData).ToList();
        var pathData = new PathData(threat.transform.position, pathDates);
        threat.GetComponent<Threat>().PathData = pathData;
        var lineRenderer = threat.GetComponent<LineRenderer>();
        lineRenderer.startColor = pathData.PathColor;
        lineRenderer.endColor = pathData.PathColor;
        lineRenderer.positionCount = pathData.PathPoints.Count;
        lineRenderer.SetPositions(pathData.PathPoints.Select(v => (Vector3) v).ToArray());
    }

    public void SendUnit(int unitNumber)
    {
        var unit = (UnitSpecies) unitNumber;
        if (controller.ActiveThreat == null) return;
        var threatScript = controller.ActiveThreat.GetComponent<Threat>();
        if (unit == UnitSpecies.Killer && !threatScript.WithAntiBodies) return;
        if (controller.ProteinPoints - UnitScript.UnitsCharacteristics[unit].Cost >= 0)
        {
            Instantiate(unitPrefab, transform.position, new Quaternion()).GetComponent<UnitScript>()
                .Initialize((UnitSpecies) unitNumber, controller.ActiveThreat);
            controller.ProteinPoints -= UnitScript.UnitsCharacteristics[unit].Cost;
        }
    }
}