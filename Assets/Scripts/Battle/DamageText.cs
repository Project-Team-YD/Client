using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour, IPoolable
{
    TextMeshProUGUI damageText = null;
    TransitionManager transitionManager = null;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
        transitionManager = TransitionManager.getInstance;
    }

    public void SetDamage(float _damage, Vector3 _position)
    {
        gameObject.transform.position = _position;
        damageText.text = _damage.ToString();
        damageText.color = Color.red;
        OnActivate();
        transitionManager.Play(TransitionManager.TransitionType.Invisible, 1f, Vector3.zero, gameObject);                
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
