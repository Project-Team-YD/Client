using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Generics;
using HSMLibrary.Tables;
using Cysharp.Threading.Tasks;

public class StageTable : Singleton<StageTable>, ITable
{
    private StageInfo[] stageInfos = null;
    private const int END_STAGE_WAVE = 2;

    public async UniTask<bool> Initialize()
    {
        stageInfos = await TableLoader.getInstance.LoadTableJson<StageInfo[]>("StageInfo");

        return true;
    }
    /// <summary>
    /// EndWave 반환 함수.
    /// </summary>
    /// <returns>END_STAGE_WAVE</returns>
    public int GetEndWave()
    {
        return END_STAGE_WAVE;
    }
    /// <summary>
    /// CurrentWave에 대한 StageInfo반환.
    /// </summary>
    /// <param name="_index">CurrentWave</param>
    /// <returns>stageInfos[_index]</returns>
    public StageInfo GetStageInfoByIndex(int _index)
    {
        if (_index >= END_STAGE_WAVE)
            _index = END_STAGE_WAVE;
        return stageInfos[_index];
    }
}
