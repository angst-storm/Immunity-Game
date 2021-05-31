using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public int startProteinCount;
    public int timeToProteinIncrement = 1;
    public int timeToTemperatureDecrement = 1;
    public double temperatureIncrement = 0.5;
    public double temperatureDecrement = 0.1;
    public double gameOverTemperature = 40;
    public int startThreatDifficult = 1;
    public int startTimeToThreatSpawn = 1;
    public float minThreatsDistance = 2;
    public int maxThreatsCount = 5;
    public int pointsForDestruction = 15;
    public GameObject lymphnode;
    public Text proteinCountText;
    public Text temperatureText;
    public GameObject threatPrefab;
    private readonly Func<int, int> difficultyCurve = i => 2 * i;
    private readonly Func<int, int> spawnTimeCurve = i => i;
    private readonly List<GameObject> threats = new List<GameObject>();
    private double currentTemperature = 36.6;
    private SizeF fieldSize;
    private bool onPause;
    private int proteinIncrementCounter;
    private int temperatureDecrementCounter;
    private IEnumerator<int> threatDifficult;
    private int threatSpawnCounter;
    private IEnumerator<int> timeToThreatSpawn;
    public List<int> ThreatsWithAntiBodiesCodes { get; } = new List<int>();
    public GameObject ActiveThreat { get; private set; }
    public int ProteinPoints { get; set; }
    private int GamePoints { get; set; }

    private void Start()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        
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
        proteinCountText.text = "x" + ProteinPoints;
        temperatureText.text = $"{currentTemperature: 0.0}";

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (onPause)
            {
                onPause = false;
                Time.timeScale = 1;
            }
            else
            {
                onPause = true;
                Time.timeScale = 0;
            }
        }
    }

    private void TimerTick()
    {
        ThreatSpawnControl();
        ProteinIncrementControl();
        TemperatureControl();
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

    private void TemperatureControl()
    {
        if (currentTemperature >= gameOverTemperature) GameOver();
        if (currentTemperature <= 36.6)
        {
            currentTemperature = 36.6;
            temperatureDecrementCounter = 0;
            return;
        }

        temperatureDecrementCounter++;
        if (temperatureDecrementCounter >= timeToTemperatureDecrement)
        {
            temperatureDecrementCounter = 0;
            currentTemperature -= temperatureDecrement;
            currentTemperature = Math.Round(currentTemperature, 1);
        }
    }

    public void RaiseTheTemperature()
    {
        currentTemperature += temperatureIncrement;
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

    private void GameOver()
    {
        print("its all");
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