public enum MonsterType
{
    Close = 0, // 근거리
    Long, // 원거리
    Boss // 레이드 보스
}
public struct EnemyInfo
{
    public string name;
    public MonsterType type;
    public float hp;
    public float moveSpeed;
    public float attackDistance;
}
