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
    public bool plotMode;
    public PlotController plotController;
    private readonly Func<int, int> difficultyCurve = i => 2 * i;
    private readonly Func<int, int> spawnTimeCurve = i => i;
    public readonly List<GameObject> Threats = new List<GameObject>();
    private double currentTemperature = 36.6;
    private SizeF fieldSize;
    private bool onPause;
    private IEnumerator<Action> plotActionsEnumerator;
    private int proteinIncrementCounter;
    private int temperatureDecrementCounter;
    private IEnumerator<int> threatDifficult;
    private int threatSpawnCounter;
    private IEnumerator<int> timeToThreatSpawn;
    public HashSet<int> ThreatsWithAntiBodiesCodes { get; } = new HashSet<int>();
    public GameObject ActiveThreat { get; private set; }
    public int ProteinPoints { get; set; }
    private int GamePoints { get; set; }
    public bool FirstThreatWin { get; set; } = false;

    private void Start()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (plotMode) plotActionsEnumerator = plotController.PlotActions().GetEnumerator();
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
        ProteinIncrementControl();
        TemperatureControl();

        if (!plotMode)
            ThreatSpawnControl();
        else
            PlotControl();
    }

    private void PlotControl()
    {
        if (plotActionsEnumerator.MoveNext()) plotActionsEnumerator.Current?.Invoke();
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
        Threats.Remove(threat);
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
        if (Threats.Count >= maxThreatsCount) return;

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
            SpawnThreat(spawnPoint, new ThreatData(), threatDifficult.Current);
            threatDifficult.MoveNext();
        }
    }

    public void SpawnThreat(Vector2 spawnPoint, ThreatData data, int difficult)
    {
        var newThreat = Instantiate(threatPrefab, spawnPoint, new Quaternion());
        Threats.Add(newThreat);
        newThreat.GetComponent<Threat>().Controller = gameObject.GetComponent<GameController>();
        newThreat.GetComponent<Threat>()
            .ThreatInitialize(data, difficult, threatDifficult.Current,
                ThreatsWithAntiBodiesCodes.Contains(data.Code));
        lymphnode.GetComponent<Lymphnode>().BuildPath(newThreat);
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
        return Threats
            .Select(t => (Vector2) t.transform.position)
            .Concat(new[] {(Vector2) transform.position})
            .All(p => (spawnPoint - p).magnitude >= minThreatsDistance);
    }

    #endregion
}