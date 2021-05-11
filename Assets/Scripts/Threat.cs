using System;
using System.Collections.Generic;
using UnityEngine;

public class Threat : MonoBehaviour
{
    //ссылки на префабы и спрайты
    public GameObject typePrefab;
    public Transform healthBar;
    public Transform antibodyBar;
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
    private int allAntiBodiesPoints;
    private int allHealthPoints;
    private bool initialized;

    //информация о игре

    public GameController Controller { get; set; }
    public List<GameObject> AttackUnits { get; } = new List<GameObject>();

    private void Update()
    {
        if (initialized)
        {
            healthBar.localScale = new Vector3((float) HealthPoints / allHealthPoints, 1, 1);
            if (AntiBodiesPoints >= 0)
                antibodyBar.localScale = new Vector3((float) AntiBodiesPoints / allAntiBodiesPoints, 1, 1);
        }

        if (HealthPoints == 0)
        {
            foreach (var unit in AttackUnits)
                Destroy(unit);
            Controller.ThreatDeath(gameObject);
        }
    }

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

    public void ThreatInitialize(ThreatData data, int healthPoints, int antiBodies)
    {
        ThreatData = data;

        allHealthPoints = healthPoints;
        allAntiBodiesPoints = antiBodies;
        HealthPoints = healthPoints;
        AntiBodiesPoints = antiBodies;

        Instantiate(typePrefab, transform).GetComponent<SpriteRenderer>().sprite =
            data.Type switch
            {
                ThreatType.Wound => wound,
                ThreatType.Virus => virus,
                ThreatType.Cancer => cancer,
                _ => throw new ArgumentOutOfRangeException()
            };

        initialized = true;
    }
}