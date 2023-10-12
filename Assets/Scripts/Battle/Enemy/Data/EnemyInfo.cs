public enum MonsterType
{
    Close = 0, // �ٰŸ�
    Long, // ���Ÿ�
    Boss // ����
}

public enum MonsterState
{
    Chase = 0, // �߰�
    Attack, // ����
    Hit, // �ǰ�
    Die // ����
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
