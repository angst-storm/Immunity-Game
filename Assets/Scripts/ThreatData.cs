using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ThreatType
{
    Wound,
    Virus,
    Cancer
}

public class ThreatData
{
    public static readonly Dictionary<int, (string, ThreatType)> ThreatsDictionary =
        new Dictionary<int, (string, ThreatType)>
        {
            {0, ("Повреждение кожи", ThreatType.Wound)},
            {1, ("Посттравматическая раневая инфекция (T79.3)", ThreatType.Wound)},
            {2, ("Вирусный гепатит (B15 – B19)", ThreatType.Virus)},
            {3, ("Грипп (J09 – J11)", ThreatType.Virus)},
            {4, ("Инфекции, вызванные вирусом герпеса (B00)", ThreatType.Virus)},
            {5, ("Корь (B05)", ThreatType.Virus)},
            {6, ("Злокачественные новообразования (C00 – C97)", ThreatType.Cancer)}
        };

    public ThreatData()
    {
        var data = ThreatsDictionary.Skip(Random.Range(0, ThreatsDictionary.Count - 1)).First();
        Code = data.Key;
        CodeName = data.Value.Item1;
        Type = data.Value.Item2;
    }

    public ThreatData(int code, string codeName, ThreatType type)
    {
        Code = code;
        CodeName = codeName;
        Type = type;
        if (!ThreatsDictionary.ContainsKey(Code)) ThreatsDictionary.Add(code, (codeName, type));
    }

    public int Code { get; }
    public string CodeName { get; }
    public ThreatType Type { get; }
}