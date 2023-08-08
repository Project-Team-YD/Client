using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;

[CustomEditor(typeof(MapTool))]
[CanEditMultipleObjects]
public class MapToolEditor : Editor
{
    #region Map
    private MapInfo map;
    private MapInfo mapData;
    private SerializedProperty prefab;
    private SerializedProperty background;
    private SerializedProperty backgroundColor;
    private SerializedProperty mask;
    private SerializedProperty maskColor;
    private SerializedProperty spriteRenderer;
    private SerializedProperty id;
    private SerializedProperty size;    
    private SerializedProperty mapImage;
    private SerializedProperty boundaryImage;
    private SerializedProperty playerRespawnPosition;
    private MapTool maptool;
    #endregion

    #region Obstruction    
    int obstructionsCount;
    private SerializedProperty obstructionPrefab;
    private SerializedProperty toolObstructions;
    private ObstructionToolInfo[] toolinfos;
    private ObstructionInfo[] infos;
    #endregion

    private void OnEnable()
    {
        #region Map
        prefab = serializedObject.FindProperty("prefab");
        background = serializedObject.FindProperty("mapBackground");
        backgroundColor = serializedObject.FindProperty("mapBackgroundColor");
        mask = serializedObject.FindProperty("mapBoundary");
        maskColor = serializedObject.FindProperty("mapBoundaryColor");
        spriteRenderer = serializedObject.FindProperty("spriteRenderer");
        id = serializedObject.FindProperty("mapId");
        size = serializedObject.FindProperty("mapSize");        
        mapImage = serializedObject.FindProperty("mapImage");
        boundaryImage = serializedObject.FindProperty("boundaryImage");
        playerRespawnPosition = serializedObject.FindProperty("playerRespawnPosition");
        maptool = target as MapTool;
        #endregion

        #region Obstruction
        obstructionPrefab = serializedObject.FindProperty("obstructionPrefab");
        toolObstructions = serializedObject.FindProperty("toolObstructions");        
        #endregion        
    }

    public override void OnInspectorGUI()
    {        
        #region Map
        GUIStyle style = EditorStyles.helpBox;
        EditorGUILayout.LabelField("MapSettings", EditorStyles.boldLabel);
        GUILayout.BeginVertical(style);
        EditorGUILayout.PropertyField(prefab);
        //EditorGUILayout.PropertyField(background);
        
        EditorGUILayout.PropertyField(backgroundColor);
        map.MapBackgroundColor = backgroundColor.colorValue;
        
        //EditorGUILayout.PropertyField(spriteRenderer);
        //EditorGUILayout.PropertyField(mask);
        
        EditorGUILayout.PropertyField(maskColor);
        map.BoundaryColor = maskColor.colorValue;
        
        EditorGUILayout.PropertyField(id);
        map.MapId = id.intValue;
        
        EditorGUILayout.PropertyField(size);
        map.MapWidth = size.vector2Value.x;
        map.MapHeight = size.vector2Value.y;
       
        EditorGUILayout.PropertyField(mapImage);
        if (mapImage.objectReferenceValue != null)
        {
            map.MapImage = mapImage.objectReferenceValue.name.ToString();
        }
        else
        {
            map.MapImage = string.Empty;
        }
        
        EditorGUILayout.PropertyField(boundaryImage);
        if(boundaryImage.objectReferenceValue != null)
        {
            map.BoundaryImage = boundaryImage.objectReferenceValue.name.ToString();
        }
        else
        {
            map.BoundaryImage = string.Empty;
        }
        
        EditorGUILayout.PropertyField(playerRespawnPosition);
        map.CharacterStartPositionX = playerRespawnPosition.vector2Value.x;
        map.CharacterStartPositionY = playerRespawnPosition.vector2Value.y;
        
        GUILayout.EndVertical();
        if(GUILayout.Button("CreateMap"))
        {
            maptool.Initialize();
            CreateMap();
        }
        #endregion
        GUILayout.Space(20f);

        #region Obstruction        
        EditorGUILayout.LabelField("ObstructionSettings", EditorStyles.boldLabel);
        GUILayout.BeginVertical(style);
        EditorGUILayout.PropertyField(obstructionPrefab);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(toolObstructions, true);
        if (EditorGUI.EndChangeCheck())
        {
            
        }
        GUILayout.EndVertical();
        if(GUILayout.Button("CreateObstruction"))
        {
            CreateObstruction();
        }
        GUILayout.Space(20f);
        if(GUILayout.Button("CreateJsonFile"))
        {
            mapData = map;
            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;            
            string mapjson = JsonConvert.SerializeObject(mapData, settings);

            CreateJsonFile(string.Format($"{Application.dataPath}/Resources/Table"), "MapInfo", mapjson);            
        }
        #endregion
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateMap()
    {
        maptool.CreateMap();
        maptool.MapInfoInit(map);
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateObstruction()
    {
        obstructionsCount = toolObstructions.arraySize;
        toolinfos = new ObstructionToolInfo[obstructionsCount];
        infos = new ObstructionInfo[obstructionsCount];
        for (int i = 0; i < obstructionsCount; i++)
        {            
            var element = toolObstructions.GetArrayElementAtIndex(i);            
            toolinfos[i].obstructionSize = element.FindPropertyRelative("obstructionSize").vector2Value;
            toolinfos[i].obstructionPosition = element.FindPropertyRelative("obstructionPosition").vector2Value;
            toolinfos[i].obstructionImage = (Sprite)element.FindPropertyRelative("obstructionImage").objectReferenceValue;
        }
        for (int i = 0; i < obstructionsCount; i++)
        {
            maptool.CreateObstruction();            
            infos[i].obstructionWidth = toolinfos[i].obstructionSize.x;
            infos[i].obstructionHeight = toolinfos[i].obstructionSize.y;
            infos[i].obstructionPositionX = toolinfos[i].obstructionPosition.x;
            infos[i].obstructionPositionY = toolinfos[i].obstructionPosition.y;
            infos[i].obstructionImageName = toolinfos[i].obstructionImage.name;
            maptool.ObstructionInit(infos[i]);
        }
        map.ObstructionInfos = infos;
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateJsonFile (string createPath, string fileName, string jsonData)
    {
        FileInfo file = new FileInfo(string.Format("{0}/{1}.json", createPath, fileName));                
        if (!file.Exists)
        {
            FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Create);
            StreamWriter sw = new StreamWriter(fileStream);
            sw.Write("[");
            //byte[] data = Encoding.UTF8.GetBytes(jsonData);
            //fileStream.Write(data, 0, data.Length);
            sw.Write(jsonData);
            sw.Write("]");
            sw.Close();
            fileStream.Close();
        }
        else
        {
            FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", createPath, fileName), FileMode.Open);
            fileStream.Position = fileStream.Seek(-1, SeekOrigin.End);
            StreamWriter sw = new StreamWriter(fileStream);
            sw.Write("," + jsonData + "]");            
            sw.Close();
            //byte[] data = Encoding.UTF8.GetBytes(jsonData);            
            //fileStream.Write(data, 0, data.Length);
            fileStream.Close();
        }
    }
}
