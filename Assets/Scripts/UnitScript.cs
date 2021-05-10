using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitScript : MonoBehaviour
{
    public static Dictionary<UnitSpecies, UnitData> UnitsCharacteristics = new Dictionary<UnitSpecies, UnitData>
    {
        {UnitSpecies.Macrophage, new UnitData(UnitSpecies.Macrophage, 1, 1, 5, 1)},
        {UnitSpecies.DendriticCell, new UnitData(UnitSpecies.DendriticCell, 1, 1, 5, 1)},
        {UnitSpecies.Neutrophil, new UnitData(UnitSpecies.Neutrophil, 1, 1, 5, 1)},
        {UnitSpecies.NKCell, new UnitData(UnitSpecies.NKCell, 1, 1, 5, 1)},
        {UnitSpecies.TKiller, new UnitData(UnitSpecies.TKiller, 1, 1, 5, 1)}
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
    public UnitData UnitData { get; private set; }

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
        targetThreat.GetComponent<Threat>().HealthPoints -= UnitData.LifeDamage;
        targetThreat.GetComponent<Threat>().AntiBodiesPoints -= UnitData.AntiBodyDamage;
        Destroy(gameObject);
    }


    public void Initialize(UnitSpecies unit, GameObject threat)
    {
        GetComponent<SpriteRenderer>().sprite = unit switch
        {
            UnitSpecies.Macrophage => macrophageSprite,
            UnitSpecies.DendriticCell => dendriticCellSprite,
            UnitSpecies.Neutrophil => neutrophilSprite,
            UnitSpecies.NKCell => nKCellSprite,
            UnitSpecies.TKiller => tKiller,
            _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
        };
        UnitData = UnitsCharacteristics[unit];
        targetThreat = threat;
        path = threat.GetComponent<Threat>().PathData.PathsPoints.GetEnumerator();
        if (path.MoveNext())
            currentTarget = path.Current;
        initialized = true;
    }
}