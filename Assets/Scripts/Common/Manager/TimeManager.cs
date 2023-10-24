using Cysharp.Threading.Tasks;
using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private float time;

    public float SetTime { get { return time; } set { time = value; } }

    public void ResetTime()
    {
        time = 0;
    }

    public async UniTaskVoid UpdateTime(CancellationToken _cancellationToken)
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            time += Time.deltaTime;

            await UniTask.Yield();
            Debug.Log($"½Ã°£ {time}");
        }
    }
}
