using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int winPointsCount;
    public int proteinCount;
    public GameObject threatPrefab;
    public GameObject lymphnode;
    public float minThreatsDistance;
    public int maxThreatsCount;
    public float timeToThreatSpawn;
    private readonly List<GameObject> threats = new List<GameObject>();
    private readonly List<int> threatsWithAntiBodiesCodes = new List<int>();
    private SizeF fieldSize;
    private float millisecondsToSpawn;
    public int GamePoints { get; set; }
    public GameObject ActiveThreat { get; set; }

    private void Start()
    {
        var fieldHorizontalRadius = Camera.main.orthographicSize;
        fieldSize = new SizeF((int) (fieldHorizontalRadius * 1.8 * 2), (int) (fieldHorizontalRadius * 2));
        millisecondsToSpawn = timeToThreatSpawn;
    }

    private void Update()
    {
        if (millisecondsToSpawn > 0)
            millisecondsToSpawn -= Time.deltaTime;
        if ((millisecondsToSpawn <= 0 || threats.Count == 0) && threats.Count < maxThreatsCount)
        {
            millisecondsToSpawn = timeToThreatSpawn;
            SpawnThreat();
        }
    }

    private void SpawnThreat()
    {
        var spawnPoint = new Vector2();
        while (!SuitableSpawnPoint(spawnPoint))
            spawnPoint = new Vector2(Random.Range(-fieldSize.Width / 2 + 1, fieldSize.Width / 2 - 1),
                Random.Range(-fieldSize.Height / 2 + 1, fieldSize.Height / 2 - 1));

        GameObject newThreat;
        threats.Add(newThreat = Instantiate(threatPrefab, spawnPoint, new Quaternion()));
        //из контроллера идет обращение к лимфоузлу и вызывается метод создания пути до угрозы
        lymphnode.GetComponent<Lymphnode>().BuildPath(newThreat);
    }

    private bool SuitableSpawnPoint(Vector2 spawnPoint)
    {
        return threats
            .Select(t => (Vector2) t.transform.position)
            .Concat(new[] {(Vector2) transform.position})
            .All(p => (spawnPoint - p).magnitude >= minThreatsDistance);
    }
}