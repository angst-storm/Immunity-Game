﻿public class UnitData
{
    public UnitData(int lifeDamage, int antiBodyDamage, int speed, int cost)
    {
        LifeDamage = lifeDamage;
        AntiBodyDamage = antiBodyDamage;
        Speed = speed;
        Cost = cost;
    }

    public int LifeDamage { get; }
    public int AntiBodyDamage { get; }
    public int Speed { get; }
    public int Cost { get; }
}