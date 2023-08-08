using HSMLibrary.Manager;
using System.Collections.Generic;
using UnityEngine;


public class BaseSceneController : MonoBehaviour
{
    public string musicName = string.Empty;

    private ObjectPool<IPoolable> pool = null;
    private Queue<EnemyObject> test = new Queue<EnemyObject>();

    static protected int chapterIndex; // 임시..나중에 로비에서 선택한 챕터와 캐릭터등을 매니저로 만들어서 게임씬에 넘겨줘야 될 것 같음..
    protected MapInfo chapterMap;
    private Sprite mapImage = null;

    #region Joypad
    protected UIJoyPad joypadController = null;
    [SerializeField] protected RectTransform joypadBackgroundRectTransform = null;
    [SerializeField] protected RectTransform joypadStickRectTransform = null;
    #endregion

    #region Player
    protected PlayerController localPlayerController = null;
    [SerializeField] protected Transform playerTransform = null;

    // TODO:: 플레이어 정보 가지고 있어야함
    #endregion

    #region Camera
    [SerializeField] protected CameraController cameraController = null;
    #endregion

    public virtual void OnResponsePacket(string _responsePacket)
    {
        //.. TODO :: ???? ????
    }

    private void Awake()
    {
        //PoolManager.getInstance.RegisterObjectPool<EnemyObject>(new ObjectPool<IPoolable>());
    }

    private void Start()
    {
        //pool = PoolManager.getInstance.GetObjectPool<EnemyObject>();
        //pool.Initialize("Prefabs/EnemyObject", EnemyTable.getInstance.GetDataCount());
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
        //    pool.EnqueueObject(test.Dequeue());
        //}

    }

    public virtual void OnActivate()
    {

    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnDeactivate()
    {

    }        

    protected Sprite GetMapSprite(string _imageName)
    {
        mapImage = Resources.Load<Sprite>($"Image/{_imageName}");
        if (mapImage != null)
        {
            return mapImage;
        }
        return null;
    }
}
