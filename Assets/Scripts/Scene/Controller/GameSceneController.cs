using Cysharp.Threading.Tasks;
using HSMLibrary.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameSceneController : BaseSceneController
{
    #region Enemy
    [SerializeField] private Transform monsterPoolRoot;
    private ObjectPool<IPoolable> monsterPool = null;
    private List<EnemyObject> monsterList = new List<EnemyObject>();
    private int createCount;
    private int regenCount;
    private List<Vector2> summonPosition = new List<Vector2>();
    private readonly int MAX_WAVE_MONSTER = 100;
    #endregion

    #region Map
    private GameObject prefab = null;
    private GameObject obstructionPrefab = null;
    private SpriteRenderer backgroundSpriteRenderer = null;
    private SpriteRenderer boundarySpriteRenderer = null;
    private SpriteRenderer spriteRenderer = null;
    private SpriteRenderer obstructionspriteRenderer = null;
    private GameObject mapObj = null;

    private float width;
    private float height;
    #endregion

    #region Bullet
    [SerializeField] private Transform BulletPoolRoot;
    private ObjectPool<IPoolable> bulletPool = null;
    private readonly int BULLET_COUNT = 30;
    private readonly float BULLET_ROTATE_SPEED = 0.15f;
    #endregion

    #region UniTask
    private CancellationToken monsterCreateCancel = new CancellationToken();
    private CancellationToken monsterRegenCancel = new CancellationToken();
    private CancellationToken monsterMoveCancel = new CancellationToken();
    private CancellationToken getTargetEnemyCancel = new CancellationToken();
    #endregion

    private void Awake()
    {
        PoolManager.getInstance.RegisterObjectPool<EnemyObject>(new ObjectPool<IPoolable>());
        PoolManager.getInstance.RegisterObjectPool<Bullet>(new ObjectPool<IPoolable>());
    }

    private async void Start()
    {
        createCount = EnemyTable.getInstance.GetCreateCount();
        regenCount =  EnemyTable.getInstance.GetRegenCount();

        monsterPool = PoolManager.getInstance.GetObjectPool<EnemyObject>();
        monsterPool.Initialize("Prefabs/EnemyObject", MAX_WAVE_MONSTER, monsterPoolRoot);

        bulletPool = PoolManager.getInstance.GetObjectPool<Bullet>();
        bulletPool.Initialize("Prefabs/Bullet", BULLET_COUNT, BulletPoolRoot);

        prefab = Resources.Load("Prefabs/Map/MapObject", typeof(GameObject)) as GameObject;
        obstructionPrefab = Resources.Load("Prefabs/Map/Obstruction", typeof(GameObject)) as GameObject;
        mapObj = Instantiate(prefab);
        backgroundSpriteRenderer = mapObj.transform.GetChild(0).GetComponent<SpriteRenderer>();
        spriteRenderer = mapObj.transform.GetChild(1).GetComponent<SpriteRenderer>();
        boundarySpriteRenderer = mapObj.transform.GetChild(2).GetComponent<SpriteRenderer>();
        var map = MapTable.getInstance.GetMapInfoByIndex(chapterIndex);
        MapInfoInit(map);

        if (playerTransform != null)
        {
            localPlayerController = new PlayerController(playerTransform);
            joypadController = new UIJoyPad(joypadBackgroundRectTransform, joypadStickRectTransform);

            joypadController.SetTarget(localPlayerController);

            cameraController = new CameraController(Camera.main);

            cameraController.SetMapSize(map.MapWidth, map.MapHeight);

            localPlayerController.SetMapSize = spriteRenderer.size;//new Vector2(28.5f, 28.5f);//spriteRenderer.size;            
        }

        await CreateMonster();
        RegenMonster().Forget();

        StartMoveMonster();

        StartCheckMonster();
    }

    private void LateUpdate()
    {
        cameraController.UpdateCamera(localPlayerController.GetPlayerTransform);
    }

    private void Update()
    {
        //if (Input.GetKeyUp(KeyCode.Escape))
        //{

        //}

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    var obj = (EnemyObject)pool.GetObject();
        //    obj.OnActivate();
        //    obj.Init(EnemyTable.getInstance.GetEnemyInfoByIndex(test.Count));
        //    test.Enqueue(obj);
        //}

        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    monsterPool.EnqueueObject(monsterList[0]);
        //}

        // 조이패드 임시
        OnClickJoypad();
    }
    /// <summary>
    /// 몬스터 생성 함수(중복 포지션 생성이 안되도록 난수 중복 제거 로직 사용)
    /// </summary>
    /// <returns></returns>
    private async UniTask CreateMonster()
    {
        await UniTask.Delay(1500, cancellationToken: monsterCreateCancel);

        for (int i = 0; i < createCount; i++)
        {
            var obj = (EnemyObject)monsterPool.GetObject();
            var monsterPosition = RandomSummonPosition(width - (obj.transform.localScale.x / 2), height - (obj.transform.localScale.y / 2));
            if (summonPosition.Contains(monsterPosition))
            {
                monsterPosition = RandomSummonPosition(width - (obj.transform.localScale.x / 2), height - (obj.transform.localScale.y / 2));
            }
            else
            {
                summonPosition.Add(monsterPosition);
            }
            obj.transform.localPosition = monsterPosition;
            obj.OnActivate();
            obj.Init(EnemyTable.getInstance.GetEnemyInfoByIndex(0)); // 일단 근거리 한종류..추후 몬스터 추가 될수록 Random함수를 이용해 난수로 몬스터 종류별 랜덤 생성되게..
            monsterList.Add(obj);
        }
        summonPosition.Clear();
    }
    /// <summary>
    /// 몬스터 리젠 함수..리젠 시간이 될때마다 호출.
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid RegenMonster()
    {
        await UniTask.Delay(5000, cancellationToken: monsterRegenCancel);

        for (int i = 0; i < regenCount; i++)
        {
            var obj = (EnemyObject)monsterPool.GetObject();
            var monsterPosition = RandomSummonPosition(width - (obj.transform.localScale.x / 2), height - (obj.transform.localScale.y / 2));
            if (summonPosition.Contains(monsterPosition))
            {
                monsterPosition = RandomSummonPosition(width - (obj.transform.localScale.x / 2), height - (obj.transform.localScale.y / 2));
            }
            else
            {
                summonPosition.Add(monsterPosition);
            }
            obj.transform.localPosition = monsterPosition;
            obj.OnActivate();
            obj.Init(EnemyTable.getInstance.GetEnemyInfoByIndex(1)); // 일단 근거리 한종류..추후 몬스터 추가 될수록 Random함수를 이용해 난수로 몬스터 종류별 랜덤 생성되게..
            monsterList.Add(obj);
        }
        summonPosition.Clear();
    }

    private async void StartMoveMonster()
    {
        await MoveMonster();
    }

    // 몬스터 실시간 플레이어 위치로 이동..추후 수정해야함.
    private async UniTask MoveMonster()
    {
        while (true)
        {
            for (int i = 0; i < monsterList.Count; i++)
            {
                if (monsterList[i].GetState() == MonsterState.Chase)
                    // playerTarget 생성때 넣어주는 방법 생각해보기
                    monsterList[i].OnMoveTarget(playerTransform);
            }

            await UniTask.Yield();
        }
    }

    private Vector2 RandomSummonPosition(float _x, float _y)
    {
        float randomX = Random.Range(-(_x / 2) + 1f, (_x / 2) - 1f);
        float randomY = Random.Range(-(_y / 2) + 1f, (_y / 2) - 1f);
        Vector2 position = new Vector2(randomX, randomY);

        return position;
    }

    #region MapCreate
    public void MapInfoInit(MapInfo _info)
    {
        backgroundSpriteRenderer.color = _info.MapBackgroundColor;
        width = _info.MapWidth;
        height = _info.MapHeight;
        spriteRenderer.transform.localPosition = Vector3.zero;
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        spriteRenderer.sprite = GetMapSprite(_info.MapImage);
        spriteRenderer.size = new Vector2(width, height);
        boundarySpriteRenderer.sprite = GetMapSprite(_info.BoundaryImage);
        boundarySpriteRenderer.transform.localScale = new Vector2(width / 10, height / 10);
        boundarySpriteRenderer.color = _info.BoundaryColor;
        if (_info.ObstructionInfos != null)
        {
            int count = _info.ObstructionInfos.Length;
            for (int i = 0; i < count; i++)
            {
                GameObject obstruction = Instantiate(obstructionPrefab, mapObj.transform);
                obstructionspriteRenderer = obstruction.GetComponent<SpriteRenderer>();
                obstructionspriteRenderer.sortingOrder = -9;
                ObstructionInit(_info.ObstructionInfos[i]);
            }
        }
        spriteRenderer.sortingOrder = -10;
        boundarySpriteRenderer.sortingOrder = -8;
    }
    public void ObstructionInit(ObstructionInfo _info)
    {
        float positionX = _info.obstructionPositionX;
        float positionY = _info.obstructionPositionY;
        Vector2 position = Vector2.zero;

        float sizeX = _info.obstructionWidth;
        float sizeY = _info.obstructionHeight;
        obstructionspriteRenderer.drawMode = SpriteDrawMode.Tiled;
        #region Map 영역 벗어나는지 체크 후 포지션 조정..
        if (positionX >= ((width / 2) - (sizeX / 2)))
        {
            positionX = (width / 2) - (sizeX / 2);
        }
        if (positionY >= ((height / 2) - (sizeY / 2)))
        {
            positionY = (height / 2) - (sizeY / 2);
        }
        if (positionX <= (-width / 2) + (sizeX / 2))
        {
            positionX = (-width / 2) + (sizeX / 2);
        }
        if (positionY <= (-height / 2) + (sizeY / 2))
        {
            positionY = (-height / 2) + (sizeY / 2);
        }
        #endregion
        position.x = positionX;
        position.y = positionY;
        obstructionspriteRenderer.transform.localPosition = position;
        obstructionspriteRenderer.size = new Vector2(sizeX, sizeY);
        obstructionspriteRenderer.sprite = GetMapSprite(_info.obstructionImageName);
        obstructionspriteRenderer.sortingOrder = -9;
    }
    #endregion

    private void OnClickJoypad()
    {
        if (Input.GetMouseButtonDown(0))
        {
            joypadController.OnJoypadDown(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            joypadController.OnJoypad(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            joypadController.OnJoypadUp();
        }
    }


    private async void StartCheckMonster()
    {
        await CheckCollisionMonster();
    }

    // 몬스터 실시간 플레이어 위치로 이동..추후 수정해야함.
    private async UniTask CheckCollisionMonster()
    {
        while (true)
        {
            for (int i = 0; i < monsterList.Count; i++)
            {
                var isCollision = monsterList[i].OnCheckCollision(localPlayerController.GetPlayerAABB);

                if (isCollision)
                {
                    Debug.Log($"충돌 / {i}번");
                }
            }

            await UniTask.Yield();
        }
    }

    public EnemyObject GetTargetEnemy(float _range)
    {
        int count = monsterList.Count;
        for (int i = 0; i < count; i++)
        {
            if ((monsterList[i].transform.position - playerTransform.position).sqrMagnitude <= _range * _range)
            {
                return monsterList[i];
            }
        }
        return null;
    }
    //TODO :: 몬스터 충돌 처리 후 DeActive + bulletpool로 발사체 Enqueue 처리해줘야함..수리검은 또한 DoTween kill해줘야함.
    public async void FireBullet(EnemyObject _enemy, WeaponType _type, Transform _transform)
    {
        var obj = (Bullet)bulletPool.GetObject();
        if (_type == WeaponType.ninjastar && _type != WeaponType.gun)
        {
            TransitionManager.getInstance.Play(TransitionManager.TransitionType.Rotate, BULLET_ROTATE_SPEED, new Vector3(0, 0, 360f), null, obj.gameObject);
        }
        obj.transform.position = _transform.position;
        var direction = _enemy.transform.position - obj.transform.position;
        obj.SetBulletSprite(_type, _enemy.transform);
        obj.OnActivate();
        while (true)
        {
            obj.transform.position += (direction.normalized * 5f) * Time.deltaTime;
            await UniTask.Yield();
        }
    }
}
