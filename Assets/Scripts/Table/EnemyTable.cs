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
    // TODO :: ���⼭ ���� �־ ���� ���� ��������Table �Ǵ� �ٸ������� �ҷ��� �� �ֵ��� �ٲ��� ���߿�..
    private int createCount = 20; // ���۽� ���� �� ������ ��..
    private int regenCount = 20; // �ֱ������� ���� �� ������ ��..
    #endregion
    public async UniTask<bool> Initialize()
    {
        enemyInfos = await TableLoader.getInstance.LoadTableJson<EnemyInfo[]>("EnemyInfo");
        
        return true;
    }

    public int GetCreateCount()
    {
        return createCount;
    }

    public int GetRegenCount()
    {
        return regenCount;
    }

    public EnemyInfo GetEnemyInfoByIndex(int _index)
    {
        if (_index >= enemyInfos.Length)
            _index = enemyInfos.Length - 1;
        return enemyInfos[_index];
    }
}
