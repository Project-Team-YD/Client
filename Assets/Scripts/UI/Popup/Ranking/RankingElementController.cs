using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HSMLibrary.Generics;
using HSMLibrary.UI;
using System;
using HSMLibrary.Extensions;
using Cysharp.Threading.Tasks;
using HSMLibrary.Manager;
using TMPro;

public class RankingElementController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText = null;
    [SerializeField] private TextMeshProUGUI nameText = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;

    public void SetData(string _rankText, string _nameText, string _scoreText)
    {
        rankText.text = _rankText;
        nameText.text = _nameText;
        scoreText.text = _scoreText;
    }
}
