using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{


    public enum SpawnState { Vertical, Horizontal, Cross, All }

    // Struct to easily add different enemies in the Inspector
    [System.Serializable]
    public struct EnemyType
    {
        public string enemyName;
        public GameObject prefab;
        [Range(0, 100)] public float spawnChance; // Percentage chance (e.g., 70 for 70%)
    }

    [Header("Enemy Types & Selection")]
    [SerializeField] private EnemyType[] enemyPool;
    [SerializeField] private Transform playerTransform;

    [Header("Current Spawner State")]
    [SerializeField] private SpawnState currentState = SpawnState.Vertical;
    [SerializeField] private int enemiesPerWave = 3;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float minimumSpawnInterval = 0.8f; // Prevents the game from being impossible
    [SerializeField] private float speedUpFactor = 0.3f; // Decreases spawnInterval by this much every 30s
    [SerializeField] private float stateChangeInterval = 30f;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] topPoints;
    [SerializeField] private Transform[] bottomPoints;
    [SerializeField] private Transform[] leftPoints;
    [SerializeField] private Transform[] rightPoints;

    private bool useTopNext = true;
    private bool useLeftNext = true;

    void Start()
    {
        StartCoroutine(SpawnLoopRoutine());
        StartCoroutine(CycleSpawnStatesRoutine());
    }

    private IEnumerator SpawnLoopRoutine()
    {
        while (true)
        {
            ProcessSpawnWave();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator CycleSpawnStatesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(stateChangeInterval);

            // 1. Advance the wave pattern state
            int nextStateIndex = ((int)currentState + 1) % System.Enum.GetValues(typeof(SpawnState)).Length;
            currentState = (SpawnState)nextStateIndex;

            // 2. Increase difficulty by speeding up the spawn interval
            if (spawnInterval > minimumSpawnInterval)
            {
                spawnInterval = Mathf.Max(minimumSpawnInterval, spawnInterval - speedUpFactor);
            }

            Debug.Log($"<color=yellow>Wave Manager</color>: State is now <b>{currentState}</b>. Next wave in <b>{spawnInterval:F1}s</b>!");
        }
    }

    void ProcessSpawnWave()
    {
        if (enemyPool == null || enemyPool.Length == 0 ) return;

        List<Transform> pickedPoints = new List<Transform>();

        switch (currentState)
        {
            case SpawnState.Vertical:
                Transform[] verticalGroup = useTopNext ? topPoints : bottomPoints;
                pickedPoints = GetRandomPointsFromGroup(verticalGroup, enemiesPerWave);
                useTopNext = !useTopNext;
                break;

            case SpawnState.Horizontal:
                Transform[] horizontalGroup = useLeftNext ? leftPoints : rightPoints;
                pickedPoints = GetRandomPointsFromGroup(horizontalGroup, enemiesPerWave);
                useLeftNext = !useLeftNext;
                break;

            case SpawnState.Cross:
                List<Transform> crossPoints = new List<Transform>();
                crossPoints.AddRange(topPoints);
                crossPoints.AddRange(bottomPoints);
                crossPoints.AddRange(leftPoints);
                crossPoints.AddRange(rightPoints);
                pickedPoints = GetRandomPointsFromGroup(crossPoints.ToArray(), enemiesPerWave);
                break;

            case SpawnState.All:
                pickedPoints.AddRange(GetRandomPointsFromGroup(topPoints, 1));
                pickedPoints.AddRange(GetRandomPointsFromGroup(bottomPoints, 1));
                pickedPoints.AddRange(GetRandomPointsFromGroup(leftPoints, 1));
                pickedPoints.AddRange(GetRandomPointsFromGroup(rightPoints, 1));
                break;
        }

        foreach (Transform point in pickedPoints)
        {
            SpawnSingleEnemy(point);
        }
    }

    List<Transform> GetRandomPointsFromGroup(Transform[] group, int count)
    {
        List<Transform> shuffled = new List<Transform>(group);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            Transform temp = shuffled[i];
            shuffled[i] = shuffled[rnd];
            shuffled[rnd] = temp;
        }

        int maxToPick = Mathf.Min(count, shuffled.Count);
        return shuffled.GetRange(0, maxToPick);
    }

    void SpawnSingleEnemy(Transform point)
    {
        if (point == null) return;

        // Pick a random enemy based on probability weights
        GameObject enemyPrefab = GetRandomEnemyPrefab();
        if (enemyPrefab == null) return;

        GameObject spawnedEnemy = Instantiate(enemyPrefab, point.position, Quaternion.identity);

        //EnemyMovement movement = spawnedEnemy.GetComponent<EnemyMovement>();
        //if (movement != null)
        //{
        //    movement.SetTarget(playerTransform);
        //}
    }

    // Picks a prefab from the array using the assigned percentage chances
    GameObject GetRandomEnemyPrefab()
    {
        float totalWeight = 0;
        foreach (var enemy in enemyPool)
        {
            totalWeight += enemy.spawnChance;
        }

        float randomPick = Random.Range(0, totalWeight);
        float currentWeightSum = 0;

        foreach (var enemy in enemyPool)
        {
            currentWeightSum += enemy.spawnChance;
            if (randomPick <= currentWeightSum)
            {
                return enemy.prefab;
            }
        }

        return enemyPool[0].prefab; // Fallback
    }
                                    //    public enum SpawnType { Horizontal, Vertical, Cross};
                                    //    public SpawnType type;
                                    //    // three places each
                                    //    [SerializeField] private Transform[] topSpawnPoints;
                                    //    [SerializeField] private Transform[] bottomSpawnPoints;
                                    //    [SerializeField] private Transform[] leftSpawnPoints;
                                    //    [SerializeField] private Transform[] rightSpawnPoints;

        //    public float spawnInterval = 2f;
        //    public bool isSpawningFromTop = true; // top is true, bottom is false // vertical
        //    public bool isSpawningFromRight = true; // right is true, left is bottom // horizontal

        //    [Range(1, 3)]
        //    [SerializeField] private int enemiesToSpawn = 2; // Exact number to spawn per wave (Max 3)
        //    void Start()
        //    {

        //    }

        //    // Update is called once per frame
        //    void Update()
        //    {

        //    }

        //    void CreateWave()
        //    {
        //        switch(type)
        //        {
        //            case SpawnType.Vertical: 


        //                break;
        //            case SpawnType.Horizontal: 

        //                break;
        //            case SpawnType.Cross:

        //                break;
        //        }
        //    }
    }
