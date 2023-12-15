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
    private EnemyObject bossMonster = null;
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
    private CancellationTokenSource damageTextCancel;
    private CancellationTokenSource regenHpCancel;
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
    #endregion

    [SerializeField] private Transform damageTextRoot = null;
    [SerializeField] private Transform PlayerHUDTransform = null;
    private ObjectPool<IPoolable> damageTextPool = null;
    private readonly int DAMAGE_TEXT_COUNT = 30;

    private TimeManager timeManager = null;
    private int gameWave;
    private int endWave;
    private UIManager uIManager = null;
    private PlayerManager playerManager = null;
    private WeaponSlot[] weapons = null;
    public List<EnemyObject> GetEnemyList { get { return monsterList; } }

    private StageInfo stage;
    private bool isPlaying;
    private bool isTouch;
    private float topPanel;

    private void Awake()
    {
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
        playerManager.CurrentGold = 0;
        gameWave = 0;
        endWave = StageTable.getInstance.GetEndWave();
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
        SetWaveText(gameWave);
        // 화면 크기에서의 퍼센트
        topPanel = Screen.height * 0.75f;
        var bossHpHeight = bossHp.GetComponent<RectTransform>().rect.height;
        // 상단 ui 위치부터의 범위
        topPanel = bossHp.transform.position.y - (bossHpHeight * 2);

        Debug.Log($"topPanel 범위 : {topPanel}");

        isTouch = false;

        StartGameWave();

        playerManager.PlayerWeaponController.StartAttack();
    }

    private void LateUpdate()
    {
        cameraController.UpdateCamera(localPlayerController.GetPlayerTransform);
    }

    private void Update()
    {
        // 다른 팝업 띄워져있을때 joypad안뜨도록 해야함
        // 조이패드 상단 간섭 금지
        // 조이패드 임시
        if (isPlaying)
        {
            OnClickJoypad();
            CheckGameOver();
        }
        if (monsterCount >= MAX_WAVE_MONSTER)
        {
            monsterRegenCancel.Cancel();
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
        hpText.text = string.Format("{0:N1}", hpStringBuilder);
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
        waveStringBuilder.Append(_wave + 1);
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
        SetGoldText(playerManager.CurrentGold);

        damageTextCancel = new CancellationTokenSource();

        monsterCount = 0;
        deathCount = 0;

        SetPlaying(true);

#if CHEAT_BOSS
        gameWave = endWave;
        CheckGameWaveEnd(true);
#endif
        stage = StageTable.getInstance.GetStageInfoByIndex(gameWave);
#if !CHEAT_BOSS
        await CreateMonster();
        RegenMonster(monsterRegenCancel = new CancellationTokenSource()).Forget();
#endif
        RegenHp(regenHpCancel = new CancellationTokenSource()).Forget();

        StartMoveMonster();
        StartCheckMonster();
        weapons = playerManager.PlayerWeaponController.GetWeapons;
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
        gameWave++;
        StartGameWave();

        playerManager.PlayerWeaponController.StartAttack();
        timeManager.UpdateTime(timeManagerCancel = new CancellationTokenSource()).Forget();

        timeManager.PlayTime();
        currentPlayerHp = playerMaxHp + (playerMaxHp * playerManager.GetPlayerMaxHP);
        SetWaveText(gameWave);
        SetHpText(currentPlayerHp);
    }
    /// <summary>
    /// Wave 끝났을때 호출.
    /// </summary>
    private async void EndGameWave()
    {
        damageTextCancel.Cancel();

        playerManager.PlayerWeaponController.StopAttack();

        SetPlaying(false);

        EndWaveActiveBulletObjectEnqueue();
        EndWaveActiveDamageTextObjectEnqueue();

        // 조건 게임이 끝났는지        
        if (gameWave >= endWave)
        {
            // show 하기 전에 서버에 데이터 보내고 받은 데이터 넘겨주기
            var popup = await uIManager.Show<ResultPanelController>("ResultPanel");
            popup.SetData(true);
            PoolManager.getInstance.RemoveObjectPool<EnemyObject>();
            PoolManager.getInstance.RemoveObjectPool<Bullet>();
            PoolManager.getInstance.RemoveObjectPool<DamageText>();
        }
        else
        {
            var popup = await uIManager.Show<InGameShopPanelController>("InGameShopPanel");
            popup.SetData(StartNextWave, gameWave);
        }

        joypadController.OnJoypadUp();
        isTouch = false;

        timeManager.PauseTime();
    }

    private void CheckGameWaveEnd(bool _endWave)
    {
        if (_endWave && gameWave != endWave)
        {
            timeManagerCancel.Cancel();
            monsterMoveCancel.Cancel();
            regenHpCancel.Cancel();
            monsterCheckCollisionCancel.Cancel();

            EndGameWave();
        }
        else if (_endWave && gameWave == endWave)
        {
            //Boss등장 UI 만들어서 띄워주기..UI등장과 함께 2초(임시)후 출현..
            BossAppearanceUI();
            CreateBossMonster().Forget();
        }
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
        sb.Append(string.Format("{0}:{1:N3}", (int)timeManager.GetTime / 60, timeManager.GetTime % 60));
        timeText.text = sb.ToString();
        sb.Clear();
    }
    /// <summary>
    /// 보스 몬스터 등장 경고 팝업 생성.
    /// </summary>
    private async void BossAppearanceUI()
    {
        var popup = await uIManager.Show<BossAppearancePopupController>("BossAppearancePopup");
    }
    /// <summary>
    /// 보스 몬스터 생성 함수.
    /// </summary>
    /// <returns></returns>
    private async UniTask CreateBossMonster()
    {
        await UniTask.Delay(4500);

        bossHp.SetActive(true);
        var obj = (EnemyObject)monsterPool.GetObject();
        obj.transform.localPosition = Vector3.zero;
        obj.OnActivate();
        obj.Init(EnemyTable.getInstance.GetEnemyInfoByIndex(2));
        bossMonster = obj;
        bossMonster.SetBossTarget(playerTransform);
        BossMonsterAttackStart().Forget();
    }
    /// <summary>
    /// 보스 몬스터 공격범위의 따른 공격가능 여부와 공격패턴 시작.
    /// </summary>
    /// <returns></returns>
    private async UniTask BossMonsterAttackStart()
    {
        while (true)
        {
            int pattern = Random.Range(0, (int)BossMonsterAttackPattern.Max);
            BossMonsterAttackPattern attackPattern = (BossMonsterAttackPattern)pattern;
            bool attackPossible = await bossMonster.BossAttackRange(pattern);
            if (attackPossible)
            {
                switch (attackPattern)
                {
                    case BossMonsterAttackPattern.BulletFire:
                        FireBossMonsterBullet(bossMonster, bossMonster.GetAttackRangeDirection());
                        break;
                    case BossMonsterAttackPattern.BodyAttack:
                        BossBodyAttack(bossMonster, bossMonster.GetAttackRangeDirection());
                        break;
                    case BossMonsterAttackPattern.Max:
                        break;
                }
            }
            await UniTask.Delay(2000);
        }
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
            obj.Init(EnemyTable.getInstance.GetEnemyInfoByIndex(stage.MonsterInfo[monsterCount]));
            obj.WaveEnhanceMonster(gameWave);
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
            await UniTask.Delay(5000, cancellationToken: _cancellationToken.Token); // 몬스터 리젠 주기..5초
            for (int i = 0; i < regenCount; i++)
            {
                var obj = (EnemyObject)monsterPool.GetObject();
                if (obj != null)
                {
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
                    obj.Init(EnemyTable.getInstance.GetEnemyInfoByIndex(stage.MonsterInfo[monsterCount]));
                    obj.WaveEnhanceMonster(gameWave);
                    monsterList.Add(obj);
                    monsterCount++;
                }
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
                    if (type == MonsterType.Long)
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
            // 임시 보스 공격
            if (bossMonster != null)
            {
                var isCollision = bossMonster.OnCheckCollision(localPlayerController.GetPlayerAABB);

                if (isCollision)
                {
                    SetDamageText(bossMonster.GetAttackPower(), PlayerHUDTransform.position, Color.red).Forget();
                    currentPlayerHp -= bossMonster.GetAttackPower();
                    SetHpText(currentPlayerHp);
                    await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token); // 공격 받은 후 무적시간 1초                    
                }
            }
            else
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
                        await UniTask.Delay(1000, cancellationToken: _cancellationToken.Token); // 공격 받은 후 무적시간 1초                    
                    }
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
        if (bossMonster != null)
        {
            if ((bossMonster.transform.position - playerTransform.position).magnitude <= _range)
            {
                return bossMonster;
            }
        }
        else
        {
            int count = monsterList.Count;
            for (int i = 0; i < count; i++)
            {
                if ((monsterList[i].transform.position - playerTransform.position).magnitude <= _range)
                {
                    return monsterList[i];
                }
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
            if ((playerTransform.position - _enemy.transform.position).magnitude <= _range)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 플레이어 원거리 공격 함수.
    /// </summary>
    /// <param name="_enemy">Target Enemy(가장 가까운 몬스터)</param>
    /// <param name="_weapon">원거리 무기 객체</param>
    /// <returns></returns>
    public async UniTaskVoid FireBullet(EnemyObject _enemy, WeaponSlot _weapon)
    {
        var obj = (Bullet)bulletPool.GetObject();
        var type = _weapon.GetWeaponType();
        obj.transform.position = _weapon.transform.position;
        var direction = _enemy.transform.position - obj.transform.position;
        obj.SetBulletSprite(type, _enemy.transform);
        obj.OnActivate();

        bool isMove = true;

        while (isMove)
        {
            if (obj == null)
            {
                isMove = false;
            }

            obj.transform.position += (direction.normalized * BULLET_SPEED) * Time.deltaTime;

            // 원거리 무기의 경우 여기서 AABB 적용
            // 충돌체크
            var isCheck = CheckMonsterAttack(_weapon, obj);
            if (isCheck)
            {
                isMove = FireBulletKill(obj);
            }

            // 플레이어와 거리체크 소멸
            float distance = Vector3.Distance(playerTransform.position, obj.transform.position);
            if (distance >= 25)
            {
                isMove = FireBulletKill(obj);
            }

            await UniTask.Yield();
        }
    }
    /// <summary>
    /// 불렛 객체 return ObjectPool
    /// </summary>
    /// <param name="_bullet">Bullet 객체</param>    
    /// <returns></returns>
    private bool FireBulletKill(Bullet _bullet)
    {
        bulletPool.EnqueueObject(_bullet);

        return false;
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
        }
        await UniTask.Delay(1500);
        _enemy.SetState(MonsterState.Chase);
    }
    /// <summary>
    /// 보스 몬스터 전용 총알공격 함수.
    /// </summary>
    /// <param name="_enemy">bossMonster</param>
    /// <param name="_direction">공격 범위 방향</param>
    public void FireBossMonsterBullet(EnemyObject _enemy, Vector3 _direction)
    {
        _enemy.SetState(MonsterState.Attack);
        for (int i = -1; i <= 1; i++)
        {
            var obj = (Bullet)bulletPool.GetObject();
            obj.transform.position = _enemy.transform.position;
            var direction = _direction;
            var quaternion = Quaternion.Euler(0, 0, (i * 45));
            var newDirection = quaternion * direction;
            obj.SetBossMonsterBulletSprite(_direction, i + 2);
            obj.OnActivate();
            BossBulletMove(obj, newDirection).Forget();
        }
        _enemy.SetState(MonsterState.Chase);
    }

    private void BossBodyAttack(EnemyObject _enemy, Vector3 _direction)
    {
        _enemy.SetState(MonsterState.Attack);
        var obj = (Bullet)bulletPool.GetObject();
        obj.transform.position = _enemy.transform.position;
        obj.SetBossMonsterBodyAttackSprite();
        obj.OnActivate();
        BossBulletMove(obj, _direction, 2f).Forget();
        _enemy.SetState(MonsterState.Chase);
    }

    /// <summary>
    /// 보스 몬스터 전용 총알들 이동 함수.
    /// </summary>
    /// <param name="_bullet">Bullet 객체</param>
    /// <param name="_direction">각 총알의 방향</param>
    /// <returns></returns>
    private async UniTask BossBulletMove(Bullet _bullet, Vector3 _direction, float _bulletSpeed = 1f)
    {
        bool isMove = true;
        while (isMove)
        {
            if (_bullet == null)
                isMove = false;
            else
            {
                _bullet.transform.position += (_direction.normalized * (BULLET_SPEED * _bulletSpeed)) * Time.deltaTime;

                // 충돌체크
                var isCheck = _bullet.OnCheckCollision(localPlayerController.GetPlayerAABB);
                if (isCheck)
                {
                    bulletPool.EnqueueObject(_bullet);
                    isMove = false;
                    SetDamageText(bossMonster.GetAttackPower(), PlayerHUDTransform.position, Color.red).Forget();
                    currentPlayerHp -= bossMonster.GetAttackPower();
                    SetHpText(currentPlayerHp);
                }

                float distance = Vector3.Distance(bossMonster.transform.position, _bullet.transform.position);
                if (distance >= 15)
                {
                    bulletPool.EnqueueObject(_bullet);
                    isMove = false;
                }
            }
            await UniTask.Yield();
        }
    }
    /// <summary>
    /// 원거리 무기 AABB
    /// </summary>
    /// <param name="_bullet"></param>
    /// <returns></returns>
    private bool CheckMonsterAttack(WeaponSlot _weapon, Bullet _bullet)
    {
        // 라인그리기
        var aabb = _bullet.GetBulletAABB;
        var leftTop = new Vector3(aabb.GetLeft, aabb.GetTop, 0);
        var rightTop = new Vector3(aabb.GetRight, aabb.GetTop, 0);
        var leftBottom = new Vector3(aabb.GetLeft, aabb.GetBottom, 0);
        var rightBottom = new Vector3(aabb.GetRight, aabb.GetBottom, 0);

        // 45도로 회전한 AABB의 네 꼭짓점 계산
        Vector3 leftTop2 = aabb.RotatePoint(leftTop);
        Vector3 rightTop2 = aabb.RotatePoint(rightTop);
        Vector3 leftBottom2 = aabb.RotatePoint(leftBottom);
        Vector3 rightBottom2 = aabb.RotatePoint(rightBottom);

        // 회전된 AABB를 라인으로 그리기
        Debug.DrawLine(leftTop2, rightTop2, Color.blue);
        Debug.DrawLine(rightTop2, rightBottom2, Color.blue);
        Debug.DrawLine(rightBottom2, leftBottom2, Color.blue);
        Debug.DrawLine(leftBottom2, leftTop2, Color.blue);

        // 여기서 보스 공격확인
        if (bossMonster != null)
        {
            var isCollision = bossMonster.OnCheckCollision(_bullet.GetBulletAABB);

            if (isCollision)
            {
                BossMonsterAttack(_weapon);
                return true;
            }
        }
        else
        {
            for (int i = 0; i < monsterList.Count; i++)
            {

                var isCollision = monsterList[i].OnCheckCollision(_bullet.GetBulletAABB);
                if (isCollision)
                {
                    AttackMonster(_weapon, i);
                    return true;
                }
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

        if (bossMonster != null)
        {
            var isCollision = bossMonster.OnCheckCollision(_weapon.GetWeaponAABB);

            if (isCollision)
            {
                BossMonsterAttack(_weapon);
                return true;
            }
        }
        else
        {
            for (int i = 0; i < monsterList.Count; i++)
            {
                var isCollision = monsterList[i].OnCheckCollision(_weapon.GetWeaponAABB);
                if (isCollision)
                {
                    AttackMonster(_weapon, i);
                    return false;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 충돌된 몬스터
    /// hp 적용필요
    /// </summary>
    /// <param name="_index"></param>
    private void AttackMonster(WeaponSlot _weapon, int _index)
    {
        if (weapons == null)
            return;

        var monster = monsterList[_index];

        monster.SetState(MonsterState.Hit);
        monster.SetAttack(playerTransform);

        var weapon = _weapon.GetWeaponInfo();

        float ENHANCE_POWER;
        if (_weapon.GetWeaponType() == WeaponType.gun || _weapon.GetWeaponType() == WeaponType.ninjastar)
        {
            ENHANCE_POWER = WeaponTable.RANGED_WEAPON_ENHANCE_POWER;
        }
        else
        {
            ENHANCE_POWER = WeaponTable.MELEE_WEAPON_ENHANCE_POWER;
        }

        var damage = weapon.attackPower + ((weapon.enhance * ENHANCE_POWER) * weapon.attackPower) + (weapon.attackPower * playerManager.GetPlayerDamage);

        monster.SetDamage(damage);

        if (monster.IsDie())
        {
            monster.SetState(MonsterState.Die);
            monsterPool.EnqueueObject(monsterList[_index]);
            monsterList.RemoveAt(_index);
            deathCount++;
            playerManager.CurrentGold += 100;
            SetGoldText(playerManager.CurrentGold);
            CheckGameWaveEnd(deathCount >= MAX_WAVE_MONSTER);
        }
        SetDamageText(damage, monster.GetHUDTransform().position, Color.black).Forget();
    }

    private void BossMonsterAttack(WeaponSlot _weapon)
    {
        if (weapons == null)
            return;

        bossMonster.SetState(MonsterState.Hit);
        bossMonster.SetBossAttack();

        var weapon = _weapon.GetWeaponInfo();

        float ENHANCE_POWER;
        if (_weapon.GetWeaponType() == WeaponType.gun || _weapon.GetWeaponType() == WeaponType.ninjastar)
        {
            ENHANCE_POWER = WeaponTable.RANGED_WEAPON_ENHANCE_POWER;
        }
        else
        {
            ENHANCE_POWER = WeaponTable.MELEE_WEAPON_ENHANCE_POWER;
        }

        var damage = weapon.attackPower + ((weapon.enhance * ENHANCE_POWER) * weapon.attackPower) + (weapon.attackPower * playerManager.GetPlayerDamage);

        bossMonster.SetDamage(damage);

        if (bossMonster.IsDie())
        {
            bossMonster.SetState(MonsterState.Die);
            monsterPool.EnqueueObject(bossMonster);
            // TODO :: 외부 상점용 재화 올라가는 기능 추가 필요..
            EndGameWave();
        }
        SetDamageText(damage, bossMonster.GetHUDTransform().position, Color.black).Forget();
        bossHpBar.fillAmount = (bossMonster.GetCurrentHp() / bossMonster.GetMaxHp());
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
        if (text != null)
        {
            var transform = Camera.main.WorldToScreenPoint(_position);
            text.SetDamage(_attackPower, transform, _damageColor);

            await UniTask.Delay(1600, cancellationToken: damageTextCancel.Token);

            text.ResetText();
            damageTextPool.EnqueueObject(text);
        }
    }

    private bool TopTouch()
    {
        if (topPanel < Input.mousePosition.y)
        {
            return true;
        }

        return false;
    }

    private void EndWaveActiveBulletObjectEnqueue()
    {
        foreach (Transform item in BulletPoolRoot)
        {
            if (item.gameObject.activeSelf)
            {
                if (item.gameObject.TryGetComponent(out Bullet bullet))
                {
                    bulletPool.EnqueueObject(bullet);
                }
            }
        }
    }

    private void EndWaveActiveDamageTextObjectEnqueue()
    {
        foreach (Transform item in damageTextRoot)
        {
            if (item.gameObject.activeSelf)
            {
                if (item.gameObject.TryGetComponent(out DamageText damageText))
                {
                    damageText.ResetText();
                    damageTextPool.EnqueueObject(damageText);
                }
            }
        }
    }

    private async void CheckGameOver()
    {
        // 게임을 일시 정지 시킬지 고민 필요
        if (currentPlayerHp <= 0)
        {
            timeManagerCancel.Cancel();
            monsterMoveCancel.Cancel();
            monsterCheckCollisionCancel.Cancel();
            monsterRegenCancel.Cancel();
            damageTextCancel.Cancel();
            regenHpCancel.Cancel();

            bossMonster = null;

            SetPlaying(false);

            var popup = await uIManager.Show<ResultPanelController>("ResultPanel");
            popup.SetData();

            joypadController.OnJoypadUp();
            isTouch = false;

            // 씬 이동으로 인한 기존 ObjectPool 삭제 및 초기화.
            PoolManager.getInstance.RemoveObjectPool<EnemyObject>();
            PoolManager.getInstance.RemoveObjectPool<Bullet>();
            PoolManager.getInstance.RemoveObjectPool<DamageText>();

            timeManager.PauseTime();
        }
    }

    private async UniTaskVoid RegenHp(CancellationTokenSource _cancellationToken)
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            var regenHp = playerManager.GetPlayerRegenHp;
            if (regenHp > 0)
            {
                var maxHp = playerMaxHp + regenHp;
                if (currentPlayerHp < maxHp)
                {
                    currentPlayerHp += regenHp;
                    if (currentPlayerHp > maxHp)
                    {
                        currentPlayerHp = maxHp;
                    }
                }
            }
            await UniTask.Delay(5000, cancellationToken: _cancellationToken.Token);
        }
    }
}