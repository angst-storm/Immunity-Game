using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private readonly List<GameObject> threats = new List<GameObject>();
    private readonly List<int> threatsWithAntiBodiesCodes = new List<int>();
    public int gamePoints = 0;
    public int proteinCount  = 0;
}