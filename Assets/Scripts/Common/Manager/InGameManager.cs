using HSMLibrary.Generics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : Singleton<InGameManager>
{
    private int currentStage;

    public int CurrentStage { get { return currentStage; } set { currentStage = value; } }
}
