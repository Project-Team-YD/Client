using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController
{
    private const float HALF = 0.5f;
    private const float correctionMap = 0.5f;

    private readonly Vector3 correctionCameraPosition = new Vector3(0f, 0f, -10f);

    private Camera mainCamera = null;

    // 카메라 보간 사용시 플레이어 스피드와 같은 속도를 주는게 좋아보임
    private float cameraMoveSpeed = 10f;

    private float cameraWidth = 0f;
    private float cameraHeight = 0f;

    private float mapHalfSizeWidth = 0f;
    private float mapHalfSizeHeight = 0f;

    private bool isLoopCamera = false;

    public CameraController(Camera _camera)
    {
        mainCamera = _camera;

        // TODO:: camera projection 물어보기
        // 카메라 사이즈
        cameraHeight = Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Screen.width / Screen.height;
    }

    /// <summary>
    /// Camera Fix Check
    /// </summary>
    public bool SetFixCamera { set { isLoopCamera = value; } }

    /// <summary>
    /// Camera Show Update
    /// </summary>
    /// <param name="_transform"></param>   Player Position
    public void UpdateCamera(Transform _transform)
    {
        // 옵션 delegate 고민 필요
        if (isLoopCamera == true)
        {
            mainCamera.transform.position = _transform.position + correctionCameraPosition;
        }
        else
        {
            // 기본
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, _transform.position + correctionCameraPosition,
                                      Time.deltaTime * cameraMoveSpeed);
        }

        var mainCameraPos = mainCamera.transform.position;
        // 카메라 최대 가로길이
        float limitCameraWidth = mapHalfSizeWidth - cameraWidth;
        float clampWidth = Mathf.Clamp(mainCameraPos.x, -limitCameraWidth, limitCameraWidth);

        // 카메라 최대 높이
        float limitCameraHeight = mapHalfSizeHeight - cameraHeight;
        float clampHeight = Mathf.Clamp(mainCameraPos.y, -limitCameraHeight, limitCameraHeight);

        mainCamera.transform.position = new Vector3(clampWidth, clampHeight, mainCameraPos.z);
    }

    public void SetMapSize(float _width, float _height)
    {
        mapHalfSizeWidth = _width * HALF + correctionMap;
        mapHalfSizeHeight = _height * HALF + correctionMap;
    }

    /// <summary>
    /// Camera Shake
    /// </summary>
    /// <param name="_time"></param>    Shake Time
    /// <param name="_range"></param>   Shake Range
    /// <param name="_speed"></param>   Shake Speed
    public async void CameraShake(float _time, float _range, float _speed = 0.5f)
    {
        var cameraPos = mainCamera.transform.localPosition;
        float timer = 0f;

        while (timer <= _time)
        {
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, Random.insideUnitSphere * _range + cameraPos, _speed);

            timer += Time.deltaTime * _speed;
            await UniTask.WaitForEndOfFrame();
        }

        mainCamera.transform.localPosition = cameraPos;
    }
}