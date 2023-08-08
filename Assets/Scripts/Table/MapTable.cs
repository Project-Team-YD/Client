using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HSMLibrary.Generics;
using HSMLibrary.Tables;
using Cysharp.Threading.Tasks;


public class MapTable : Singleton<MapTable>, ITable
{
    private MapInfo[] mapInfos = null;    

    public async UniTask<bool> Initialize()
    {
        mapInfos = await TableLoader.getInstance.LoadTableJson<MapInfo[]>("MapInfo");

        return true;
    }    

    /// <summary>
    /// MapInfo by Chapter
    /// </summary>
    /// <param name="_index">ChapterIndex</param>
    /// <returns></returns>
    public MapInfo GetMapInfoByIndex(int _index)
    {
        return mapInfos[_index];
    }

    public int GetMapCount()
    {
        return mapInfos.Length;
    }
}