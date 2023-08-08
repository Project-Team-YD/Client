using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Generics;
using HSMLibrary.Tables;
using Cysharp.Threading.Tasks;


public class EnemyTable : Singleton<EnemyTable>, ITable
{
    private EnemyInfo[] enemyInfos = null;

    public async UniTask<bool> Initialize()
    {
        enemyInfos = await TableLoader.getInstance.LoadTableJson<EnemyInfo[]>("EnemyInfo");
        
        return true;
    }

    public int GetDataCount()
    {
        return enemyInfos.Length;
    }

    public EnemyInfo GetEnemyInfoByIndex(int _index)
    {
        if (_index >= enemyInfos.Length)
            _index = enemyInfos.Length - 1;
        return enemyInfos[_index];
    }
}
