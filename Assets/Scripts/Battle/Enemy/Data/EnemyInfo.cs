public enum MonsterType
{
    Close = 0, // 근거리
    Long, // 원거리
    Boss // 보스
}

public enum MonsterState
{
    Chase = 0, // 추격
    Attack, // 공격
    Hit, // 피격
    Die // 죽음
}

public enum BossMonsterAttackPattern
{
    BulletFire = 0,
    BodyAttack,
    Max
}

public struct EnemyInfo
{
    public string name;
    public MonsterType type;
    public float hp;
    public float moveSpeed;
    public float attackDistance;
    public float attackPower;
}
