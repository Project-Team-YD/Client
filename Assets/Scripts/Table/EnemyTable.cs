using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Generics;
using HSMLibrary.Tables;
using Cysharp.Threading.Tasks;


public class EnemyTable : Singleton<EnemyTable>, ITable
{
    private EnemyInfo[] enemyInfos = null;
    #region MonsterCount
    // TODO :: 여기서 직접 넣어서 하지 말고 스테이지Table 또는 다른곳에서 불러올 수 있도록 바꾸자 나중에..
    private int createCount = 10; // 시작시 리젠 될 몬스터의 수..
    private int regenCount = 10; // 주기적으로 리젠 될 몬스터의 수..
    #endregion
    public async UniTask<bool> Initialize()
    {
        enemyInfos = await TableLoader.getInstance.LoadTableJson<EnemyInfo[]>("EnemyInfo");
        
        return true;
    }
    /// <summary>
    /// 생성 숫자 반환 함수.
    /// </summary>
    /// <returns>createCount</returns>
    public int GetCreateCount()
    {
        return createCount;
    }
    /// <summary>
    /// 리젠 숫자 반환 함수.
    /// </summary>
    /// <returns>regenCount</returns>
    public int GetRegenCount()
    {
        return regenCount;
    }
    /// <summary>
    /// 몬스터 객체 반환 함수.
    /// </summary>
    /// <param name="_index">index</param>
    /// <returns>enemyInfos[_index]</returns>
    public EnemyInfo GetEnemyInfoByIndex(int _index)
    {
        if (_index >= enemyInfos.Length)
            _index = enemyInfos.Length - 1;
        return enemyInfos[_index];
    }
}
