using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : MonoBehaviour, IPoolable
{
    private MonsterType type;
    private float hp;
    private float attackDistance;
    public void Init(EnemyInfo _info)
    {
        name = _info.name.ToString();
        type = _info.type;
        hp = _info.hp;
        attackDistance = _info.attackDistance;
    }

    public void OnActivate()
    {
        gameObject.SetActive(true);
    }

    public void OnDeactivate()
    {
        gameObject.SetActive(false);
    }
}
