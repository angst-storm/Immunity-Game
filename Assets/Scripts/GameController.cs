using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public int startProteinCount;
    public int maxThreatsCount = 5;
    public float minThreatsDistance = 2;
    public GameObject lymphnode;
    public Text proteinCountText;
    public GameObject threatPrefab;
    public int timeToProteinIncrement = 1;
    public int startTimeToThreatSpawn = 1;
    public int startThreatDifficult = 1;
    public int pointsForDestruction = 15;
    private readonly Func<int, int> difficultyCurve = i => 2 * i;
    private readonly Func<int, int> spawnTimeCurve = i => i;
    private readonly List<GameObject> threats = new List<GameObject>();
    private SizeF fieldSize;
    private int proteinIncrementCounter;
    private IEnumerator<int> threatDifficult;
    private int threatSpawnCounter;
    private IEnumerator<int> timeToThreatSpawn;
    public List<int> ThreatsWithAntiBodiesCodes { get; } = new List<int>();
    public GameObject ActiveThreat { get; private set; }
    public int ProteinPoints { get; set; }
    private int GamePoints { get; set; }

    private void Start()
    {
        {
            if (Camera.main == null) throw new NullReferenceException();
            var fieldHorizontalRadius = Camera.main.orthographicSize;
            fieldSize = new SizeF((int) (fieldHorizontalRadius * 2), (int) (fieldHorizontalRadius * 2));
        }

        threatDifficult = GetNextCurveValue(startThreatDifficult, difficultyCurve);
        threatDifficult.MoveNext();
        timeToThreatSpawn = GetNextCurveValue(startTimeToThreatSpawn, spawnTimeCurve);
        timeToThreatSpawn.MoveNext();

        threatSpawnCounter = startTimeToThreatSpawn;
        ProteinPoints = startProteinCount;
        InvokeRepeating(nameof(TimerTick), 0, 1);
    }

    private void Update()
    {
        proteinCountText.text = ProteinPoints.ToString();
    }

    private void TimerTick()
    {
        ThreatSpawnControl();
        ProteinIncrementControl();
    }

    private void ProteinIncrementControl()
    {
        proteinIncrementCounter++;
        if (proteinIncrementCounter >= timeToProteinIncrement)
        {
            proteinIncrementCounter = 0;
            ProteinPoints++;
        }
    }

    public void ThreatDeath(GameObject threat)
    {
        GamePoints += pointsForDestruction;
        threats.Remove(threat);
        if (ActiveThreat == threat)
            ActiveThreat = null;
        Destroy(threat);
    }

    public void ActivateThreat(GameObject threat)
    {
        if (ActiveThreat != null)
            ActiveThreat.GetComponent<Threat>().DeactivateThreat();
        ActiveThreat = threat;
    }

    private IEnumerator<int> GetNextCurveValue(int start, Func<int, int> curve)
    {
        var value = start;
        while (true)
        {
            yield return curve(value);
            value++;
            if (value == int.MaxValue) yield break;
        }
    }

    #region SpawnThreat()

    private void ThreatSpawnControl()
    {
        if (threats.Count >= maxThreatsCount) return;

        threatSpawnCounter++;
        if (threatSpawnCounter >= timeToThreatSpawn.Current)
        {
            threatSpawnCounter = 0;
            SpawnThreat();
            timeToThreatSpawn.MoveNext();
        }
    }

    private void SpawnThreat()
    {
        Vector2 spawnPoint = default;
        if (TryGetSpawnPoint(ref spawnPoint, 100))
        {
            var newThreat = Instantiate(threatPrefab, spawnPoint, new Quaternion());
            threats.Add(newThreat);
            newThreat.GetComponent<Threat>().Controller = gameObject.GetComponent<GameController>();
            var newData = new ThreatData();
            newThreat.GetComponent<Threat>()
                .ThreatInitialize(newData, threatDifficult.Current, threatDifficult.Current,
                    ThreatsWithAntiBodiesCodes.Contains(newData.Code));
            threatDifficult.MoveNext();
            lymphnode.GetComponent<Lymphnode>().BuildPath(newThreat);
        }
    }

    private bool TryGetSpawnPoint(ref Vector2 spawnPoint, int spawnTryCount)
    {
        for (var i = 0; i < spawnTryCount; i++)
        {
            var tryPoint = spawnPoint = new Vector2(Random.Range(-fieldSize.Width / 2 + 1, fieldSize.Width / 2 - 1),
                Random.Range(-fieldSize.Height / 2 + 1, fieldSize.Height / 2 - 1));
            if (!IsSuitableSpawnPoint(tryPoint)) continue;
            spawnPoint = tryPoint;
            return true;
        }

        return false;
    }

    private bool IsSuitableSpawnPoint(Vector2 spawnPoint)
    {
        return threats
            .Select(t => (Vector2) t.transform.position)
            .Concat(new[] {(Vector2) transform.position})
            .All(p => (spawnPoint - p).magnitude >= minThreatsDistance);
    }

    #endregion
}