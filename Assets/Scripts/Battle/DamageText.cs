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

    public void ResetText()
    {
        transitionManager.KillSequence(TransitionManager.TransitionType.Invisible);
        damageText.alpha = 1f;
        OnDeactivate();
    }
    
    public void OnActivate()
    {
        gameObject.SetActive(true);
    }

    public void OnDeactivate()
    {
        gameObject.SetActive(false);
    }
}
