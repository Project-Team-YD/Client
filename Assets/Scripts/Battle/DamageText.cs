using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour, IPoolable
{
    TextMeshProUGUI damageText = null;
    TransitionManager transitionManager = null;
    [SerializeField] Material outlineWhite = null;
    [SerializeField] Material outlineBlack = null;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
        transitionManager = TransitionManager.getInstance;
    }
    /// <summary>
    /// 데미지 셋팅 함수.
    /// </summary>
    /// <param name="_damage">데미지 수치</param>
    /// <param name="_position">HUD Position</param>
    /// <param name="_color">데미지 텍스트 컬러</param>
    public void SetDamage(float _damage, Vector3 _position, Color _color)
    {
        gameObject.transform.position = _position;
        damageText.text = _damage.ToString();
        if(_color == Color.red)
        {
            damageText.fontMaterial = outlineWhite;
        }
        else
        {
            damageText.fontMaterial = outlineBlack;
        }
        damageText.color = _color;
        OnActivate();
        transitionManager.Play(TransitionManager.TransitionType.Invisible, 1.5f, Vector3.zero, gameObject);                
    }
    /// <summary>
    /// Damage Text 리셋 함수.
    /// </summary>
    public void ResetText()
    {
        transitionManager.KillSequence(TransitionManager.TransitionType.Invisible);
        damageText.alpha = 1f;
        OnDeactivate();
    }
    /// <summary>
    /// SetActive(true)
    /// </summary>
    public void OnActivate()
    {
        gameObject.SetActive(true);
    }
    /// <summary>
    /// SetActive(false)
    /// </summary>
    public void OnDeactivate()
    {
        gameObject.SetActive(false);
    }
}
