using Cysharp.Threading.Tasks;
using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private float time;

    public float GetTime { get { return time; } }

    public void ResetTime()
    {
        time = 0;
    }
    /// <summary>
    /// timeScale = 0
    /// </summary>
    public void PauseTime()
    {
        Time.timeScale = 0f;
    }
    /// <summary>
    /// timeScale = 1
    /// </summary>
    public void PlayTime()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 랭킹을 위한 play time update
    /// </summary>
    /// <param name="_cancellationToken"></param> unitask cancel을 위한 token
    /// <returns></returns>
    public async UniTaskVoid UpdateTime(CancellationTokenSource _cancellationToken)
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            time += Time.deltaTime;

            await UniTask.Yield();
        }
    }
}
