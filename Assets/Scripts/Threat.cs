using System;
using UnityEngine;

public class Threat : MonoBehaviour
{
    //ссылки на префабы и спрайты
    public GameObject typePrefab;
    public GameObject barPrefab;
    public Sprite wound;
    public Sprite virus;
    public Sprite cancer;
    public Sprite defaultSprite;
    public Sprite activeSprite;

    //информация о угрозе
    public ThreatData ThreatData { get; set; }
    public PathData PathData { get; set; }
    public int HealthPoints { get; set; }
    public int AntiBodiesPoints { get; set; }

    //информация о игре
    public GameController Controller { get; set; }


    private void OnMouseDown()
    {
        ActivateThreat();
    }

    private void ActivateThreat()
    {
        Controller.ActivateThreat(gameObject);
        GetComponent<SpriteRenderer>().sprite = activeSprite;
    }

    public void DeactivateThreat()
    {
        GetComponent<SpriteRenderer>().sprite = defaultSprite;
    }

    public void ThreatInitialize(ThreatData data, int allHealthPoints, int allAntibodiesPoints)
    {
        ThreatData = data;
        HealthPoints = allHealthPoints;
        AntiBodiesPoints = allAntibodiesPoints;
        Instantiate(typePrefab, transform).GetComponent<SpriteRenderer>().sprite =
            data.Type switch
            {
                ThreatType.Wound => wound,
                ThreatType.Virus => virus,
                ThreatType.Cancer => cancer,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}