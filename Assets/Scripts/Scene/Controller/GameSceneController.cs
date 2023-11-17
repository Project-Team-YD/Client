using Cysharp.Threading.Tasks;
using HSMLibrary.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneController : BaseSceneController
{
    private const float MELEE_WEAPON_ENHANCE_POWER = 0.5f;
    private const float RANGED_WEAPON_ENHANCE_POWER = 0.2f;

    #region Enemy
    [SerializeField] private Transform monsterPoolRoot;
    private ObjectPool<IPoolable> monsterPool = null;
    private List<EnemyObject> monsterList = new List<EnemyObject>();
    private int createCount;
    private int regenCount;
    private List<Vector2> summonPosition = new List<Vector2>();
    private readonly int MAX_WAVE_MONSTER = 20;
    private int deathCount = 0;
    private int monsterCount = 0;
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
    private readonly float BULLET_SPEED = 5f;
    #endregion

    #region UniTask
    private CancellationTokenSource monsterCreateCancel = new CancellationTokenSource();
    private CancellationTokenSource monsterRegenCancel;
    private CancellationTokenSource monsterMoveCancel;
    private CancellationTokenSource monsterCheckCollisionCancel;
    private CancellationTokenSource getTargetEnemyCancel = new CancellationTokenSource();
    private CancellationTokenSource timeManagerCancel;
    #endregion

    #region InGameUI
    [SerializeField] Image playerHpBar = null;
    [SerializeField] Button gameStopButton = null;
    [SerializeField] TextMeshProUGUI timeText = null;
    [SerializeField] TextMeshProUGUI hpText = null;
    [SerializeField] TextMeshProUGUI goldText = null;
    [SerializeField] TextMeshProUGUI waveText = null;
    [SerializeField] GameObject bossHp = null;
    [SerializeField] Image bossHpBar = null;

    private StringBuilder sb = new StringBuilder();
    private StringBuilder hpStringBuilder = new StringBuilder();
    private StringBuilder goldStringBuilder = new StringBuilder();
    private StringBuilder waveStringBuilder = new StringBuilder();
    private float playerMaxHp;
    private float currentPlayerHp;
    private float currentGold;
    #endregion

    [SerializeField] private Transform damageTextRoot = null;
    [SerializeField] private Transform PlayerHUDTransform = null;
    private ObjectPool<IPoolable> damageTextPool = null;
    private readonly int DAMAGE_TEXT_COUNT = 30;

    private TimeManager timeManager = null;
    private int gameWave;
    private UIManager uIManager = null;
    private PlayerManager playerManager = null;
    private WeaponSlot[] weapons = null;
    public List<EnemyObject> GetEnemyList { get { return monsterList; } }

    private bool isPlaying;
    private bool isTouch;
    private float topPanel;

    private void Awake()
    {
        PoolManager.getInstance.RegisterObjectPool<EnemyObject>(new ObjectPool<IPoolable>());
        PoolManager.getInstance.RegisterObjectPool<Bullet>(new ObjectPool<IPoolable>());
        uIManager = UIManager.getInstance;
        timeManager = TimeManager.getInstance;
        playerManager = PlayerManager.getInstance;
        timeManager.ResetTime();
        timeManager.UpdateTime(timeManagerCancel = new CancellationTokenSource()).Forget();
        gameStopButton.onClick.AddListener(OnClickGameStopButton);
        bossHp.SetActive(false);
        sb.Clear();
        hpStringBuilder.Clear();
        goldStringBuilder.Clear();
        waveStringBuilder.Clear();
        currentGold = 0;
        gameWave = 1;
    }

    private void Start()
    {
        createCount = EnemyTable.getInstance.GetCreateCount();
        regenCount = EnemyTable.getInstance.GetRegenCount();

        monsterPool = PoolManager.getInstance.GetObjectPool<EnemyObject>();
        monsterPool.Initialize("Prefabs/EnemyObject", MAX_WAVE_MONSTER, monsterPoolRoot);

        bulletPool = PoolManager.getInstance.GetObjectPool<Bullet>();
        bulletPool.Initialize("Prefabs/Bullet", BULLET_COUNT, BulletPoolRoot);

        damageTextPool = PoolManager.getInstance.GetObjectPool<DamageText>();
        damageTextPool.Initialize("Prefabs/DamageText", DAMAGE_TEXT_COUNT, damageTextRoot);

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
            playerMaxHp = localPlayerController.GetMaxHP();
            currentPlayerHp = playerMaxHp;
        }
        SetHpText(playerMaxHp);
        SetGoldText(currentGold);
        SetWaveText(gameWave);
        // 화면 크기에서의 퍼센트
        topPanel = Screen.height * 0.75f;
        var bossHpHeight = bossHp.GetComponent<RectTransform>().rect.height;
        // 상단 ui 위치부터의 범위
        topPanel = bossHp.transform.position.y - (bossHpHeight * 2);

        Debug.Log($"topPanel 범위 : {topPanel}");

        isTouch = false;

        StartGameWave();

        playerManager.SetPlayerWeaponController.StartAttack();
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

        if (deathCount >= MAX_WAVE_MONSTER)
        {
            timeManagerCancel.Cancel();
            monsterMoveCancel.Cancel();
            monsterCheckCollisionCancel.Cancel();

            EndGameWave();

            // 초기화 필요
            deathCount = 0;
        }
        if (monsterCount >= MAX_WAVE_MONSTER)
        {
            monsterRegenCancel.Cancel();
        }
        // 다른 팝업 띄워져있을때 joypad안뜨도록 해야함
        // 조이패드 상단 간섭 금지
        // 조이패드 임시
        if (isPlaying)
        {
            OnClickJoypad();
        }

        SetPlayTime();
    }
    /// <summary>
    /// InGameUI HP 텍스트 셋팅.
    /// </summary>
    /// <param name="_hp">hp 수치</param>
    private void SetHpText(float _hp)
    {
        if (_hp <= 0)
            _hp = 0;
        hpStringBuilder.Clear();
        hpStringBuilder.Append(_hp);
        hpText.text = $"{hpStringBuilder}";
        playerHpBar.fillAmount = _hp / playerMaxHp;
    }
    /// <summary>
    /// InGameUI Gold 텍스트 셋팅.
    /// </summary>
    /// <param name="_gold">gold 수치</param>
    private void SetGoldText(float _gold)
    {
        goldStringBuilder.Clear();
        goldStringBuilder.Append(_gold);
        goldText.text = $"{goldStringBuilder}";
    }
    /// <summary>
    /// InGameUI Wave 텍스트 셋팅.
    /// </summary>
    /// <param name="_wave">wave 수치</param>
    private void SetWaveText(int _wave)
    {
        waveStringBuilder.Clear();
        waveStringBuilder.Append(_wave);
        waveText.text = $"{waveStringBuilder}";
    }
    /// <summary>
    /// 게임 일시정지 버튼.
    /// </summary>
    private async void OnClickGameStopButton()
    {
        SetPlaying(false);
        timeManager.PauseTime();
        var popup = await uIManager.Show<PausePopupController>("PausePopup");
        popup.SetCallback(SetPlaying);
    }
    /// <summary>
    /// Wave 시작할때 호출.
    /// </summary>
    private async void StartGameWave()
    {
        await CreateMonster();
        RegenMonster(monsterRegenCancel = new CancellationTokenSource()).Forget();
        StartMoveMonster();
        StartCheckMonster();
        weapons = playerManager.SetPlayerWeaponController.GetWeapons;

        SetPlaying(true);
        isTouch = false;
    }

    /// <summary>
    /// 상점 팝업 닫을때 실행할 것
    /// 시간은 이어서 할지 or 0부터 시작할지 고민
    /// update에 필요한 변수 초기화
    /// StartGameWave를 실행하려하는데 다음 웨이브확인할 방법이?
    /// StartGameWave안에 createmonster가 있는데 어떻게 할지 확인 필요
    /// 3,2,1 이라는 큰 텍스트를 보여줘서 다음 웨이브라는것을 표현할지
    /// startnextwave 실행하면 다음 웨이브 시작임
    /// </summary>
    private void StartNextWave()
    {
        StartGameWave();

        playerManager.SetPlayerWeaponController.StartAttack();

        timeManager.UpdateTime(timeManagerCancel = new CancellationTokenSource()).Forget();

        timeManager.PlayTime();
        // 몬스터 숫자 생각 필요
        monsterCount = 0;
        deathCount = 0;
        gameWave++;
        currentPlayerHp = playerMaxHp;
        SetWaveText(gameWave);
        SetHpText(currentPlayerHp);
    }
    /// <summary>
    /// Wave 끝났을때 호출.
    /// </summary>
    private async void EndGameWave()
    {
        SetPlaying(false);
        timeManager.PauseTime();
        var popup = await uIManager.Show<InGameShopPanelController>("InGameShopPanel");
        popup.SetData(StartNextWave);

        joypadController.OnJoypadUp();
        isTouch = false;

        playerManager.SetPlayerWeaponController.StopAttack();
    }

    private void SetPlaying(bool _value)
    {
        isPlaying = _value;
    }
    /// <summary>
    /// 인게임 시간 체크 및 Text적용 함수.
    /// </summary>
    private void SetPlayTime()
    {
        sb.Append(string.Format("{0}:{1:N3}", (int)timeManager.SetTime / 60, timeManager.SetTime % 60));
        timeText.text = sb.ToString();
        sb.Clear();
    }
    /// <summary>
    /// 몬스터 생성 함수(중복 포지션 생성이 안되도록 난수 중복 제거 로직 사용)
    /// </summary>
    /// <returns></returns>
    private async UniTask CreateMonster()
    {
        await UniTask.Delay(1500, cancellationToken: monsterCreateCancel.Token);

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
            monsterCount++;
        }
        summonPosition.Clear();
    }

    /// <summary>
    /// 몬스터 리젠 함수..리젠 시간이 될때마다 호출.
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid RegenMonster(CancellationTokenSource _cancellationToken)
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            await UniTask.Delay(5000); // 몬스터 리젠 주기..5초
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
                monsterCount++;
            }
            summonPosition.Clear();
            await UniTask.Delay(100, cancellationToken: monsterRegenCancel.Token);
        }
    }

    private async void StartMoveMonster()
    {
        await MoveMonster(monsterMoveCancel = new CancellationTokenSource());
    }

    /// <summary>
    /// 몬스터 상태에 따른 추격 및 공격 함수.
    /// </summary>
    /// <param name="_cancellationToken">UniTask Cancel Token</param>
    /// <returns></returns>
    private async UniTask MoveMonster(CancellationTokenSource _cancellationToken)
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            for (int i = 0; i < monsterList.Count; i++)
            {
                if (monsterList[i].GetState() == MonsterState.Chase)
                {
                    monsterList[i].OnMoveTarget(playerTransform);
                    MonsterType type = monsterList[i].GetMonsterType();
                    if (type == MonsterType.Long || type == MonsterType.Boss)
                    {
                        EnemyObject monster = monsterList[i];
                        if (PossibleAttackPlayerMonsterBullet(monster, monster.GetAttackRange()))
                        {
                            FireMonsterBullet(monster).Forget();
                        }
                    }
                }
            }

            await UniTask.Yield();
        }
    }
    /// <summary>
    /// 몬스터 생성 포지션 랜덤으로 뽑기(중복난수X)
    /// </summary>
    /// <param name="_x">x좌표</param>
    /// <param name="_y">y좌표</param>
    /// <returns>Vector2(랜덤X,랜덤Y)</returns>
    private Vector2 RandomSummonPosition(float _x, float _y)
    {
        float randomX = Random.Range(-(_x / 2) + 1f, (_x / 2) - 1f);
        float randomY = Random.Range(-(_y / 2) + 1f, (_y / 2) - 1f);
        Vector2 position = new Vector2(randomX, randomY);

        return position;
    }

    #region MapCreate
    /// <summary>
    /// 맵 정보 초기화
    /// </summary>
    /// <param name="_info">맵 정보 객체</param>
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
    /// <summary>
    /// 맵안에 장애물 정보 초기화
    /// </summary>
    /// <param name="_info">장애물 정보 객체</param>
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
        if (TopTouch() && !isTouch)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            joypadController.OnJoypadDown(Input.mousePosition);
            isTouch = true;
        }

        if (Input.GetMouseButton(0))
        {
            joypadController.OnJoypad(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            joypadController.OnJoypadUp();
            isTouch = false;
        }
    }

    private async void StartCheckMonster()
    {
        await CheckCollisionMonster(monsterCheckCollisionCancel = new CancellationTokenSource());
    }

    /// <summary>
    /// 플레이어 AABB 충돌 체크
    /// </summary>
    /// <param name="_cancellationToken">UniTask CancelToken</param>
    /// <returns></returns>
    private async UniTask CheckCollisionMonster(CancellationTokenSource _cancellationToken)
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            for (int i = 0; i < monsterList.Count; i++)
            {
                var isCollision = monsterList[i].OnCheckCollision(localPlayerController.GetPlayerAABB);

                if (isCollision)
                {
                    Debug.Log($"충돌 / {i}번");
                    SetDamageText(monsterList[i].GetAttackPower(), PlayerHUDTransform.position, Color.red).Forget();
                    currentPlayerHp -= monsterList[i].GetAttackPower();
                    SetHpText(currentPlayerHp);
                    await UniTask.Delay(1000); // 공격 받은 후 무적시간 1초                    
                }
            }

            await UniTask.Yield();
        }
    }
    /// <summary>
    /// 플레이어 공격 범위 체크 후 공격 가능한 몬스터 객체 반환.
    /// </summary>
    /// <param name="_range">플레이어 공격 범위</param>
    /// <returns></returns>
    public EnemyObject GetTargetEnemy(float _range)
    {
        if (playerTransform == null)
            return null;
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
    /// <summary>
    /// 원거리 몬스터 공격 범위 가능한지 체크.
    /// </summary>
    /// <param name="_enemy">원거리 몬스터 객체</param>
    /// <param name="_range">공격 범위</param>
    /// <returns></returns>
    public bool PossibleAttackPlayerMonsterBullet(EnemyObject _enemy, float _range)
    {
        if (playerTransform != null)
        {
            if ((playerTransform.position - _enemy.transform.position).sqrMagnitude <= _range * _range)
            {
                return true;
            }
        }
        return false;
    }

    //TODO :: 몬스터 충돌 처리 후 DeActive + bulletpool로 발사체 Enqueue 처리해줘야함..수리검은 또한 DoTween kill해줘야함.
    public async UniTaskVoid FireBullet(EnemyObject _enemy, WeaponSlot _weapon)
    {
        var obj = (Bullet)bulletPool.GetObject();
        var type = _weapon.GetWeaponType();
        if (type == WeaponType.ninjastar && type != WeaponType.gun)
        {
            TransitionManager.getInstance.Play(TransitionManager.TransitionType.Rotate, BULLET_ROTATE_SPEED, new Vector3(0, 0, 360f), obj.gameObject);
        }
        obj.transform.position = _weapon.transform.position;
        var direction = _enemy.transform.position - obj.transform.position;
        obj.SetBulletSprite(type, _enemy.transform);
        obj.OnActivate();

        bool isMove = true;

        while (isMove)
        {
            if (obj == null)
                isMove = false;

            obj.transform.position += (direction.normalized * BULLET_SPEED) * Time.deltaTime;

            // 원거리 무기의 경우 여기서 AABB 적용
            // 충돌체크
            var isCheck = CheckMonsterAttack(_weapon, obj);
            if (isCheck)
            {
                bulletPool.EnqueueObject(obj);

                if (type == WeaponType.ninjastar && type != WeaponType.gun)
                    TransitionManager.getInstance.KillSequence(TransitionManager.TransitionType.Rotate);

                isMove = false;
            }

            // 플레이어와 거리체크 소멸
            float distance = Vector3.Distance(playerTransform.position, obj.transform.position);
            if (distance >= 25)
            {
                bulletPool.EnqueueObject(obj);

                if (type == WeaponType.ninjastar && type != WeaponType.gun)
                    TransitionManager.getInstance.KillSequence(TransitionManager.TransitionType.Rotate);

                isMove = false;
            }

            await UniTask.Yield();

            if (obj == null)
                isMove = false;
        }
    }
    /// <summary>
    /// 원거리 몬스터 거리 계산 후 공격.
    /// </summary>
    /// <param name="_enemy">원거리 공격을 하는 몬스터 객체</param>
    /// <returns></returns>
    public async UniTaskVoid FireMonsterBullet(EnemyObject _enemy)
    {
        var obj = (Bullet)bulletPool.GetObject();
        obj.transform.position = _enemy.transform.position;
        var direction = playerTransform.position - obj.transform.position;
        obj.SetMonsterBulletSprite(playerTransform);
        obj.OnActivate();
        _enemy.SetState(MonsterState.Attack);
        bool isMove = true;

        while (isMove)
        {
            if (obj == null)
                isMove = false;

            obj.transform.position += (direction.normalized * BULLET_SPEED) * Time.deltaTime;

            // 충돌체크
            var isCheck = obj.OnCheckCollision(localPlayerController.GetPlayerAABB);
            if (isCheck)
            {
                bulletPool.EnqueueObject(obj);
                isMove = false;
                SetDamageText(_enemy.GetAttackPower(), PlayerHUDTransform.position, Color.red).Forget();
                currentPlayerHp -= _enemy.GetAttackPower();
                SetHpText(currentPlayerHp);
            }

            float distance = Vector3.Distance(_enemy.transform.position, obj.transform.position);
            if (distance >= 6)
            {
                bulletPool.EnqueueObject(obj);
                isMove = false;
            }

            await UniTask.Yield();

            if (obj == null)
                isMove = false;
        }
        await UniTask.Delay(1500);
        _enemy.SetState(MonsterState.Chase);
    }
    /// <summary>
    /// 원거리 무기 AABB
    /// </summary>
    /// <param name="_bullet"></param>
    /// <returns></returns>
    private bool CheckMonsterAttack(WeaponSlot _weapon, Bullet _bullet)
    {
        for (int i = 0; i < monsterList.Count; i++)
        {
            // 라인그리기
            var aabb = _bullet.GetBulletAABB;
            var leftTop = new Vector3(aabb.GetLeft, aabb.GetTop, 0);
            var rightTop = new Vector3(aabb.GetRight, aabb.GetTop, 0);
            var leftBottom = new Vector3(aabb.GetLeft, aabb.GetBottom, 0);
            var rightBottom = new Vector3(aabb.GetRight, aabb.GetBottom, 0);

            Debug.DrawLine(leftTop, rightTop, Color.black);
            Debug.DrawLine(rightTop, rightBottom, Color.black);
            Debug.DrawLine(rightBottom, leftBottom, Color.black);
            Debug.DrawLine(leftBottom, leftTop, Color.black);

            var isCollision = monsterList[i].OnCheckCollision(_bullet.GetBulletAABB);
            if (isCollision)
            {
                AttackMonster(_weapon, i);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 근접무기 용 충돌 체크
    /// </summary>
    /// <param name="_aabb"></param>
    /// <returns></returns>
    public async UniTask<bool> CheckMonsterAttack(WeaponSlot _weapon)
    {
        await UniTask.Yield();

        for (int i = 0; i < monsterList.Count; i++)
        {
            var isCollision = monsterList[i].OnCheckCollision(_weapon.GetWeaponAABB);
            if (isCollision)
            {
                AttackMonster(_weapon, i);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 충돌된 몬스터
    /// hp 적용필요
    /// </summary>
    /// <param name="_index"></param>
    private async UniTaskVoid AttackMonster(WeaponSlot _weapon, int _index)
    {
        if (weapons == null)
            return;

        var monster = monsterList[_index];
        // 몬스터 충격 이펙트
        monster.SetState(MonsterState.Hit);
        monster.SetAttack(playerTransform);

        // TODO :: weapons는 무기슬릇 배열로 어느 무기로 때렸는지 알아내어야 해당 무기슬릇의 데미지를 가져와 몬스터 hp를 계산후 밑의 로직을 타도록 수정해야함..
        var weapon = _weapon.GetWeaponInfo();

        // 임시 강화 데미지 적용
        float ENHANCE_POWER;
        if (_weapon.GetWeaponType() == WeaponType.gun || _weapon.GetWeaponType() == WeaponType.ninjastar)
        {
            ENHANCE_POWER = RANGED_WEAPON_ENHANCE_POWER;
        }
        else
        {
            ENHANCE_POWER = MELEE_WEAPON_ENHANCE_POWER;
        }

        var damage = weapon.attackPower + ((weapon.enhance * ENHANCE_POWER) * weapon.attackPower);

        monster.SetDamage(damage);
        // hp가 0이하면 죽임
        if (monster.IsDie())
        {
            monster.SetState(MonsterState.Die);
            monsterPool.EnqueueObject(monsterList[_index]);
            monsterList.RemoveAt(_index);
            deathCount++;
            currentGold += 100;
            SetGoldText(currentGold);
        }
        SetDamageText(damage, monster.GetHUDTransform().position, Color.black).Forget();
    }
    /// <summary>
    /// 데미지 HUD 텍스트 띄우는 함수.
    /// </summary>
    /// <param name="_attackPower">데미지 수치</param>
    /// <param name="_position">HUD Position</param>
    /// <param name="_damageColor">데미지 Text Color</param>
    /// <returns></returns>
    private async UniTaskVoid SetDamageText(float _attackPower, Vector3 _position, Color _damageColor)
    {
        var text = (DamageText)damageTextPool.GetObject();
        var transform = Camera.main.WorldToScreenPoint(_position);
        text.SetDamage(_attackPower, transform, _damageColor);
        await UniTask.Delay(1500);
        text.ResetText();
        damageTextPool.EnqueueObject(text);
    }

    private bool TopTouch()
    {
        if (topPanel < Input.mousePosition.y)
        {
            return true;
        }

        return false;
    }
}