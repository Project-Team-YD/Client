using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

// TEST
public struct JobController : IJob
{
    private NativeArray<int> result;

    private int index;

    public int SetIndex { set { index = value; } }

    public NativeArray<int> SetNativeArray { set { result = value; } }

    public void Execute()
    {

    }
}

/// <summary>
/// MoveController 
/// curPos Current Object Position 
/// destPos Destination Position
/// </summary>
public struct JobMoveController : IJobParallelForTransform
{
    private NativeArray<Vector3> result;

    private Vector3 curPos;
    private Vector3 destPos;
    private float speed;
    private float time;

    public Vector3 SetCurrentPosition { set { curPos = value; } }
    public Vector3 SetDestinationPosition { set { destPos = value; } }
    public float SetSpeed { set { speed = value; } }
    public float SetTime { set { time = value; } }

    public NativeArray<Vector3> SetNativeArray { get { return result; } set { result = value; } }

    public void Execute(int index, TransformAccess transform)
    {
        if (curPos == Vector3.zero || curPos == destPos)
        {
            // TODO:: 임시 수정예정 
            result[index] = curPos;
            return;
        }

        Vector3 direction = (destPos - curPos).normalized;
        //var distance = Vector3.Distance(destPos, curPos);
        float curSpeed = speed * time;

        // TODO:: 임시 수정예정 
        //result[index] = curPos += (direction * curSpeed);
        transform.position = curPos += (direction * curSpeed);
    }
}

// TODO:: 예상위치 체크 추후 예정


/// <summary>
/// CheckCollition to AABB 
/// curAABB Current AABB 
/// otherAABB Other AABB
/// </summary>
public struct JobAABBController : IJob
{
    private NativeArray<bool> result;

    private AABB curAABB;
    private AABB otherAABB;

    public AABB SetCurrentAABB { set { curAABB = value; } }
    public AABB SetOtherAABB { set { otherAABB = value; } }

    public NativeArray<bool> SetNativeArray { set { result = value; } }

    public void Execute()
    {
        bool check = curAABB.CheckCollision(otherAABB);

        // TODO:: 임시 수정예정 
        result[0] = check;
    }
}

//using UnityEngine;
//using Unity.Collections;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Burst;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int enemyCount = 10;
    public float spawnRadius = 10f;

    private NativeArray<Vector3> positions;
    private Transform playerTransform;
    private JobHandle spawnerJobHandle;

    private void Start()
    {
        positions = new NativeArray<Vector3>(enemyCount, Allocator.Persistent);
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        SpawnEnemies();
    }

    private void Update()
    {
        if (spawnerJobHandle.IsCompleted)
        {
            MoveEnemies();
            spawnerJobHandle.Complete();
            SpawnEnemies();
        }
    }

    private void OnDestroy()
    {
        spawnerJobHandle.Complete();
        positions.Dispose();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = new Vector3(randomPoint.x, 0f, randomPoint.y);
            positions[i] = spawnPosition;
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    //[BurstCompile]
    private struct MoveJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<Vector3> positions;
        public TransformAccessArray transforms;
        public float speed;
        public float deltaTime;

        public void Execute(int index, TransformAccess transform)
        {
            Vector3 direction = positions[index] - transform.position;
            transform.position += direction.normalized * speed * deltaTime;
        }
    }

    private void MoveEnemies()
    {
        var transforms = new TransformAccessArray(enemyCount);
        var moveJob = new MoveJob
        {
            positions = positions,
            transforms = transforms,
            speed = 5f, // Adjust the speed as desired
            deltaTime = Time.deltaTime
        };
        //transforms.SetTransforms(enemyPrefab.transform);
        spawnerJobHandle = moveJob.Schedule(transforms);
        JobHandle.ScheduleBatchedJobs();
        transforms.Dispose();
    }
}