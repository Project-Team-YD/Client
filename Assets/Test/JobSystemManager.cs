using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class JobSystemManager : Singleton<JobSystemManager>
{

    //NativeArray
    public void Emptys()
    {
        JobMoveController jobMoveController = new JobMoveController();
        jobMoveController.SetNativeArray = new NativeArray<Vector3>(1, Allocator.TempJob);

        //JobHandle handle = jobMoveController.Schedule();
    }
}


/// <summary>
/// //////////////////////////////
/// </summary>

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemyCount = 10;
    public float spawnRadius = 10f;
    public float moveSpeed = 5f;

    private NativeArray<Vector3> positions;
    private TransformAccessArray enemyTransforms;
    private JobHandle moveJobHandle;

    private void Start()
    {
        positions = new NativeArray<Vector3>(enemyCount, Allocator.Persistent);
        enemyTransforms = new TransformAccessArray(enemyCount);

        SpawnEnemies();
        ScheduleMoveJob();
    }

    private void Update()
    {
        CompleteMoveJob();
        MoveEnemies();
    }

    private void OnDestroy()
    {
        CompleteMoveJob();
        positions.Dispose();
        enemyTransforms.Dispose();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPoint.x, 0f, randomPoint.y);
            positions[i] = spawnPosition;
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemyTransforms.Add(enemyInstance.transform);
        }
    }

    private struct MoveJob : IJobParallelForTransform
    {
        public NativeArray<Vector3> positions;
        public float moveSpeed;
        public float deltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 position = positions[index];
            position.y = 0f; // Ensure enemies stay at the same height (optional)

            //Vector3 direction = math.normalize(position - transform.position);
            //Vector3 newPosition = transform.position + direction * moveSpeed * deltaTime;

            //transform.position = newPosition;
        }
    }

    private void MoveEnemies()
    {
        moveJobHandle = ScheduleMoveJob();
    }

    private JobHandle ScheduleMoveJob()
    {
        // for¹®
        var moveJob = new MoveJob
        {
            positions = positions,
            moveSpeed = moveSpeed,
            deltaTime = Time.deltaTime
        };

        return moveJob.Schedule(enemyTransforms);
    }

    private void CompleteMoveJob()
    {
        moveJobHandle.Complete();
    }
}