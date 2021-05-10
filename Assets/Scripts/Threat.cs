using System;
using UnityEngine;

public class Threat : MonoBehaviour
{
    public GameObject barPrefab;
    public Sprite cancer;
    public GameObject typeSprite;
    public Sprite virus;
    public Sprite wound;
    public GameController Controller { get; set; }
    public ThreatData threatData { get; set; }
    public int HealthPoints { get; set; }
    public int AntiBodiesPoints { get; set; }

    private void OnMouseDown()
    {
        print(gameObject + " я тут");
        Controller.ThreatDeath(gameObject);
        Destroy(gameObject);
    }

    public void ThreatInitialize(ThreatData data, int allHealthPoints, int allAntibodiesPoints)
    {
        threatData = data;
        HealthPoints = allHealthPoints;
        AntiBodiesPoints = allAntibodiesPoints;
        Instantiate(typeSprite, transform).GetComponent<SpriteRenderer>().sprite =
            data.Type switch
            {
                ThreatType.Wound => wound,
                ThreatType.Virus => virus,
                ThreatType.Cancer => cancer,
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}