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
    private Queue<EnemyObject> test = new Queue<EnemyObject>();
    private List<EnemyObject> monsterList = new List<EnemyObject>();
    private int createCount;
    private int regenCount;
    private List<Vector2> summonPosition = new List<Vector2>();
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

    #region UniTask
    private CancellationToken monsterCreateCancel = new CancellationToken();
    private CancellationToken monsterRegenCancel = new CancellationToken();
    private CancellationToken monsterMoveCancel = new CancellationToken();
    #endregion

    private void Awake()
    {
        PoolManager.getInstance.RegisterObjectPool<EnemyObject>(new ObjectPool<IPoolable>());
    }

    private async void Start()
    {
        createCount = EnemyTable.getInstance.GetCreateCount();
        regenCount = EnemyTable.getInstance.GetRegenCount();

        monsterPool = PoolManager.getInstance.GetObjectPool<EnemyObject>();
        monsterPool.Initialize("Prefabs/EnemyObject", createCount, monsterPoolRoot);

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

        // �����е� �ӽ�
        OnClickJoypad();
    }

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
            obj.Init(EnemyTable.getInstance.GetEnemyInfoByIndex(0)); // �ϴ� �ٰŸ� ������..���� ���� �߰� �ɼ��� Random�Լ��� �̿��� ������ ���� ������ ���� �����ǰ�..
            monsterList.Add(obj);
        }
        summonPosition.Clear();
    }

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
            obj.Init(EnemyTable.getInstance.GetEnemyInfoByIndex(0)); // �ϴ� �ٰŸ� ������..���� ���� �߰� �ɼ��� Random�Լ��� �̿��� ������ ���� ������ ���� �����ǰ�..
            monsterList.Add(obj);
        }
        summonPosition.Clear();
    }

    private async void StartMoveMonster()
    {
        await MoveMonster();
    }

    // ���� �ǽð� �÷��̾� ��ġ�� �̵�..���� �����ؾ���.
    private async UniTask MoveMonster()
    {
        while (true)
        {
            for (int i = 0; i < monsterList.Count; i++)
            {
                if (monsterList[i].gameObject.activeSelf)
                    // playerTarget ������ �־��ִ� ��� �����غ���
                    monsterList[i].OnMoveTarget(playerTransform.gameObject);
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
        #region Map ���� ������� üũ �� ������ ����..
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
}
