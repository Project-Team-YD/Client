using HSMLibrary.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BasePlayerController
{
    private const float HALF = 0.5f;

    private float speed = 10f;
    private float hp = 100f;

    private float mapSizeWidth = 0;
    private float mapSizeHeight = 0;

    private SpriteRenderer playerSprite;
    private Animator playerAnim;

    private PlayerManager playerManager = null;

    private AABB curAABB;

    public AABB GetPlayerAABB
    {
        get
        {
            if (curAABB == null)
            {
                curAABB = new AABB(this.transform, playerSprite.size);
            }

#if UNITY_EDITOR
            var leftTop = new Vector3(curAABB.GetLeft, curAABB.GetTop, 0);
            var rightTop = new Vector3(curAABB.GetRight, curAABB.GetTop, 0);
            var leftBottom = new Vector3(curAABB.GetLeft, curAABB.GetBottom, 0);
            var rightBottom = new Vector3(curAABB.GetRight, curAABB.GetBottom, 0);

            Debug.DrawLine(leftTop, rightTop, Color.black);
            Debug.DrawLine(rightTop, rightBottom, Color.black);
            Debug.DrawLine(rightBottom, leftBottom, Color.black);
            Debug.DrawLine(leftBottom, leftTop, Color.black);
#endif
            return curAABB;
        }
    }

    public PlayerController(Transform _transform) : base(_transform)
    {
        playerSprite = _transform.gameObject.GetComponent<SpriteRenderer>();
        playerAnim = _transform.gameObject.GetComponent<Animator>();

        playerSprite.sprite = Resources.Load<Sprite>($"Player/player_0");

        playerManager = PlayerManager.getInstance;
    }

    public Vector3 SetMapSize
    {
        set
        {
            //mapSize = value * 0.5f;
            // TODO:: 임시로 캐릭터 크기는 scale로 추후 수정필요
            mapSizeWidth = value.x * HALF - (transform.localScale.x + HALF); // * -> + 로 변경..맵 끝과 플레이어 자기 자신의 크기 반절만큼 덜 가기위해...
            mapSizeHeight = value.y * HALF - (transform.localScale.y + HALF); // * -> + 로 변경..맵 끝과 플레이어 자기 자신의 크기 반절만큼 덜 가기위해...
        }
    }
    /// <summary>
    /// 플레이어 최대 hp 반환.
    /// </summary>
    /// <returns>hp</returns>
    public float GetMaxHP()
    {
        return hp;
    }

    public override void OnMove(float _rot, float _speed)
    {
        // TODO:: map check로인해 base 사용하지 않음
        float speed = _speed * Time.deltaTime;

        Vector3 curPos = transform.position;
        curPos.x = Mathf.Clamp(curPos.x + (Mathf.Cos(_rot) * speed), -mapSizeWidth, mapSizeWidth);
        curPos.y = Mathf.Clamp(curPos.y + (Mathf.Sin(_rot) * speed), -mapSizeHeight, mapSizeHeight);

        transform.position = curPos;
    }

    public override void OnMove(Vector3 _dir, float _speed)
    {
        bool isLeft = _dir.x < 0f;
        playerSprite.flipX = !isLeft;

        _speed *= speed + (speed * playerManager.GetPlayerSpeed);

        base.OnMove(_dir, _speed);
    }

    public Transform GetPlayerTransform { get { return transform; } }
}
