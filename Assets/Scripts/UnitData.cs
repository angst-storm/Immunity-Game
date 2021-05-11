public enum UnitSpecies
{
    Macrophage,
    DendriticCell,
    Neutrophil,
    NKCell,
    TKiller
}

public class UnitData
{
    public UnitData(UnitSpecies species, int lifeDamage, int antiBodyDamage, int speed, int cost)
    {
        Species = species;
        LifeDamage = lifeDamage;
        AntiBodyDamage = antiBodyDamage;
        Speed = speed;
        Cost = cost;
    }

    public UnitSpecies Species { get; }
    public int LifeDamage { get; set; }
    public int AntiBodyDamage { get; set; }
    public int Speed { get; set; }
    public int Cost { get; set; }
}