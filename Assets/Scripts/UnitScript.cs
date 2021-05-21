using System;
using System.Collections.Generic;
using UnityEngine;

public enum UnitSpecies
{
    Macrophage,
    DendriticCell,
    Neutrophil,
    NkCell,
    Killer
}

public class UnitScript : MonoBehaviour
{
    public static readonly Dictionary<UnitSpecies, UnitData> UnitsCharacteristics =
        new Dictionary<UnitSpecies, UnitData>
        {
            {UnitSpecies.Macrophage, new UnitData(0, 3, 7, 1)},
            {UnitSpecies.DendriticCell, new UnitData(0, 5, 5, 2)},
            {UnitSpecies.Neutrophil, new UnitData(3, 0, 7, 1)},
            {UnitSpecies.NkCell, new UnitData(5, 0, 5, 2)},
            {UnitSpecies.Killer, new UnitData(10, 0, 5, 1)}
        };

    public Sprite macrophageSprite;
    public Sprite dendriticCellSprite;
    public Sprite neutrophilSprite;
    public Sprite nKCellSprite;
    public Sprite tKiller;
    private Vector2 currentTarget;
    private bool initialized;
    private IEnumerator<Vector2> path;
    private GameObject targetThreat;
    private UnitData UnitData { get; set; }

    private void Update()
    {
        if (!initialized) return;

        if (((Vector2) transform.position - currentTarget).magnitude <= 1e-9)
        {
            if (path.MoveNext()) currentTarget = path.Current;
            else ThreatReached();
        }
        else
        {
            GetComponent<Rigidbody2D>()
                .MovePosition(Vector2.MoveTowards(transform.position, currentTarget, UnitData.Speed * Time.deltaTime));
        }
    }

    private void ThreatReached()
    {
        var threatScript = targetThreat.GetComponent<Threat>();
        threatScript.HealthPoints -= UnitData.LifeDamage;
        threatScript.AntiBodiesPoints -= UnitData.AntiBodyDamage;
        threatScript.AttackUnits.Remove(gameObject);
        Destroy(gameObject);
    }


    public void Initialize(UnitSpecies unit, GameObject threat)
    {
        GetComponent<SpriteRenderer>().sprite = unit switch
        {
            UnitSpecies.Macrophage => macrophageSprite,
            UnitSpecies.DendriticCell => dendriticCellSprite,
            UnitSpecies.Neutrophil => neutrophilSprite,
            UnitSpecies.NkCell => nKCellSprite,
            UnitSpecies.Killer => tKiller,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
        UnitData = UnitsCharacteristics[unit];
        targetThreat = threat;
        var threatScript = targetThreat.GetComponent<Threat>();
        threatScript.AttackUnits.Add(gameObject);
        path = threatScript.PathData.PathsPoints.GetEnumerator();
        if (path.MoveNext())
            currentTarget = path.Current;
        initialized = true;
    }
}