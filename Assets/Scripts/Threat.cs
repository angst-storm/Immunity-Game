using System;
using System.Collections.Generic;
using UnityEngine;

public class Threat : MonoBehaviour
{
    public GameObject typePrefab;
    public Transform healthBar;
    public Transform antibodyBar;
    public Sprite wound;
    public Sprite virus;
    public Sprite cancer;
    public Sprite defaultSprite;
    public Sprite activeSprite;

    private int allAntiBodiesPoints;
    private int allHealthPoints;

    private int antiBodiesPoints;
    private int healthPoints;
    private bool initialized;

    public bool WithAntiBodies { get; private set; }
    private ThreatData ThreatData { get; set; }
    public PathData PathData { get; set; }

    public int HealthPoints
    {
        get => healthPoints;
        set
        {
            if (value < 0) value = 0;
            healthPoints = value;
        }
    }

    public int AntiBodiesPoints
    {
        get => antiBodiesPoints;
        set
        {
            if (value < 0) value = 0;
            antiBodiesPoints = value;
        }
    }

    public GameController Controller { get; set; }
    public List<GameObject> AttackUnits { get; } = new List<GameObject>();

    private void Update()
    {
        if (initialized)
        {
            healthBar.localScale = new Vector3((float) HealthPoints / allHealthPoints, 1, 1);
            antibodyBar.localScale = new Vector3((float) AntiBodiesPoints / allAntiBodiesPoints, 1, 1);
        }

        if (!WithAntiBodies && AntiBodiesPoints == 0)
        {
            WithAntiBodies = true;
            Controller.ThreatsWithAntiBodiesCodes.Add(ThreatData.Code);
        }

        if (HealthPoints == 0)
        {
            foreach (var unit in AttackUnits)
                Destroy(unit);
            Controller.FirstThreatWin = true;
            Controller.threatWin.PlayOneShot(Controller.threatWin.clip);
            Controller.ThreatDeath(gameObject);
        }
    }

    public void TimerFine()
    {
        Controller.RaiseTheTemperature();
        foreach (var unit in AttackUnits)
            Destroy(unit);
        Controller.threatDefeat.PlayOneShot(Controller.threatDefeat.clip);
        Controller.ThreatDeath(gameObject);
    }

    private void OnMouseDown()
    {
        ActivateThreat();
    }

    public void ActivateThreat()
    {
        Controller.ActivateThreat(gameObject);
        GetComponent<SpriteRenderer>().sprite = activeSprite;
    }

    public void DeactivateThreat()
    {
        GetComponent<SpriteRenderer>().sprite = defaultSprite;
    }

    public void ThreatInitialize(ThreatData data, int startHealthPoints, int startAntiBodiesPoints, bool withAntiBodies)
    {
        ThreatData = data;

        WithAntiBodies = withAntiBodies;

        allHealthPoints = startHealthPoints;
        allAntiBodiesPoints = startAntiBodiesPoints;
        HealthPoints = startHealthPoints;
        AntiBodiesPoints = !WithAntiBodies ? startAntiBodiesPoints : 0;

        Instantiate(typePrefab, transform).GetComponent<SpriteRenderer>().sprite =
            ThreatData.Type switch
            {
                ThreatType.Wound => wound,
                ThreatType.Virus => virus,
                ThreatType.Cancer => cancer,
                _ => throw new ArgumentOutOfRangeException()
            };

        initialized = true;
    }
}