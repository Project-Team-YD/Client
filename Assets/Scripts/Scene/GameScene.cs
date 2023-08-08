using HSMLibrary.Manager;
using HSMLibrary.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : Scene
{
    private GameSceneController gameController = null;
    public override void OnActivate()
    {
        gameController = new GameSceneController();
    }

    public override void OnDeactivate()
    {
        
    }

    public override void OnUpdate()
    {
        
    }
    
    public void SetMapInfo(MapInfo _info)
    {
        
    }
}
