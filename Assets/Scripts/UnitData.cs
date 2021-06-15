public class UnitData
{
    public UnitData(int lifeDamage, int antiBodyDamage, int speed, int cost)
    {
        LifeDamage = lifeDamage;
        AntiBodyDamage = antiBodyDamage;
        Speed = speed;
        Cost = cost;
    }

    public int LifeDamage { get; set; }
    public int AntiBodyDamage { get; set;}
    public int Speed { get; set;}
    public int Cost { get; set;}
}