using System;
using System.Collections.Generic;
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
    public int maxThreatsCount = 5;
    public int pointsForDestruction = 15;
    public GameObject lymphnode;
    public Text proteinCountText;
    public Text temperatureText;
    public GameObject threatPrefab;
    public bool plotMode;
    public PlotController plotController;
    public UIManagerScript uiManager;
    public AudioSource threatWin;
    public AudioSource threatDefeat;
    public double difficultyK = 1;
    public double difficultyA = 1;
    public double difficultyM = 1;
    public double spawnTimeK = 1;
    public double spawnTimeA = 1;
    public double spawnTimeM = 1;
    public readonly List<GameObject> threats = new List<GameObject>();
    private double currentTemperature = 36.6;
    private Func<int, int> difficultyCurve;
    private bool onPause;
    private IEnumerator<Action> plotActionsEnumerator;
    private int proteinIncrementCounter;
    private Func<int, int> spawnTimeCurve = i => (int) (i > 50 ? 1 : -0.08 * i + 5);
    private int temperatureDecrementCounter;
    private IEnumerator<int> threatDifficult;
    private int threatSpawnCounter;
    private IEnumerator<int> timeToThreatSpawn;
    public HashSet<int> ThreatsWithAntiBodiesCodes { get; } = new HashSet<int>();
    public GameObject ActiveThreat { get; private set; }
    public int ProteinPoints { get; set; }
    private int GamePoints { get; set; }
    public bool FirstThreatWin { get; set; }

    private void Start()
    {
        plotMode = UIManagerScript.learn;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        if (plotMode) plotActionsEnumerator = plotController.PlotActions().GetEnumerator();
        difficultyCurve = x =>
        {
            var result = (int) (difficultyK * Math.Pow(x, difficultyA) + difficultyM);
            return result < 1 ? 1 : result;
        };
        threatDifficult = GetNextCurveValue(startThreatDifficult, difficultyCurve);
        threatDifficult.MoveNext();
        spawnTimeCurve = x =>
        {
            var result = (int) (spawnTimeK * Math.Pow(x, spawnTimeA) + spawnTimeM);
            return result < 1 ? 1 : result;
        };
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
        plotController.ShowMessage($"???? ??????????????????! ?????? ????????: {GamePoints}", 0, 0, Color.red,
            () => uiManager.ChangeScene(0));
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
        var acceptablePoints =
            PathData.Paths.Keys
                .Except(threats
                    .Select(t => t.GetComponent<Threat>().PathData.PathPoints
                        .Last()))
                .ToList();
        if (acceptablePoints.Any())
        {
            SpawnThreat(acceptablePoints[Random.Range(0, acceptablePoints.Count)], new ThreatData(),
                threatDifficult.Current);
            threatDifficult.MoveNext();
        }
    }

    public void SpawnThreat(Vector2 spawnPoint, ThreatData data, int difficult)
    {
        var newThreat = Instantiate(threatPrefab, spawnPoint, new Quaternion());
        newThreat.GetComponent<Threat>().Controller = gameObject.GetComponent<GameController>();
        newThreat.GetComponent<Threat>()
            .ThreatInitialize(data, difficult, threatDifficult.Current,
                ThreatsWithAntiBodiesCodes.Contains(data.Code));
        lymphnode.GetComponent<Lymphnode>().BuildPath(newThreat);
        threats.Add(newThreat);
    }

    #endregion
}