using System.Linq;
using UnityEngine;

public class Lymphnode : MonoBehaviour
{
    public GameObject pathPrefab;
    public int healthPoints;

    public void BuildPath(GameObject threat)
    {
        var thisPosition = transform.position;
        //создается новый объект класса PathData, он в этой реализации хранит только точку начала и конца пути,
        //но может и хранить какой-нибуд сложный путь
        var pathData = new PathData(thisPosition, threat.transform.position);
        //создается unity объект из префаба, в его компонент LineRenderer передается массив точек по которым и строится линия
        Instantiate(pathPrefab, thisPosition, new Quaternion(), threat.transform)
            .GetComponent<LineRenderer>()
            .SetPositions(pathData.PathsPoints.Select(v => (Vector3) v).ToArray());
        // пока что, по сути, скрипт path пустой и даже не сохраняет в себе pathData, но для реализации на данном этапе это необязательно
        // дальше понятно нада буит реализовать
    }
}