using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    //настройки игры
    public int winPointsCount;
    public int startProteinCount;
    public int maxThreatsCount = 5;
    public float minThreatsDistance = 2;
    public float timeToThreatSpawn = 1;

    //ссылки на другие объекты
    public GameObject lymphnode;
    public GameObject ActiveThreat { get; set; }
    public List<GameObject> Threats { get; } = new List<GameObject>();

    //префабы
    public GameObject threatPrefab;

    //поля, хранящие информацию о игре
    private readonly List<int> threatsWithAntiBodiesCodes = new List<int>();
    private SizeF fieldSize;
    private float millisecondsToSpawn;
    public int GamePoints { get; set; }
    public int ProteinPoints { get; set; }


    private void Start()
    {
        var fieldHorizontalRadius = Camera.main.orthographicSize;
        fieldSize = new SizeF((int) (fieldHorizontalRadius * 2), (int) (fieldHorizontalRadius * 2));
        millisecondsToSpawn = timeToThreatSpawn;
        GamePoints = 0;
        ProteinPoints = startProteinCount;
    }

    private void Update()
    {
        if (millisecondsToSpawn > 0)
            millisecondsToSpawn -= Time.deltaTime;
        if ((millisecondsToSpawn <= 0 || Threats.Count == 0) && Threats.Count < maxThreatsCount)
        {
            millisecondsToSpawn = timeToThreatSpawn;
            SpawnThreat();
        }
    }

    private void SpawnThreat()
    {
        Vector2 spawnPoint = default;
        if (TryGetSpawnPoint(ref spawnPoint, 100))
        {
            var newThreat = Instantiate(threatPrefab, spawnPoint, new Quaternion());
            Threats.Add(newThreat);
            newThreat.GetComponent<Threat>().Controller = gameObject.GetComponent<GameController>();
            newThreat.GetComponent<Threat>().ThreatInitialize(new ThreatData(), 5, 5);
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
        return Threats
            .Select(t => (Vector2) t.transform.position)
            .Concat(new[] {(Vector2) transform.position})
            .All(p => (spawnPoint - p).magnitude >= minThreatsDistance);
    }
}