using System.Linq;
using UnityEngine;

public class Lymphnode : MonoBehaviour
{
    public GameObject unitPrefab;
    public GameObject controller;

    public void BuildPath(GameObject threat)
    {
        var thisPosition = transform.position;
        var pathData = new PathData(thisPosition, threat.transform.position);
        threat.GetComponent<Threat>().PathData = pathData;
        threat.GetComponent<LineRenderer>().SetPositions(pathData.PathsPoints.Select(v => (Vector3) v).ToArray());
    }

    public void SendUnit(int unitNumber)
    {
        var unit = (UnitSpecies) unitNumber;
        var controllerScript = controller.GetComponent<GameController>();
        if (controllerScript.ActiveThreat != null && controllerScript.ProteinPoints - UnitScript.UnitsCharacteristics[unit].Cost >= 0)
        {
            Instantiate(unitPrefab, transform.position, new Quaternion()).GetComponent<UnitScript>()
                .Initialize((UnitSpecies) unitNumber, controllerScript.ActiveThreat);
            controllerScript.ProteinPoints -= UnitScript.UnitsCharacteristics[unit].Cost;
        }
    }
}