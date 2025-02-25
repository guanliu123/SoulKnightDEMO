using System;
using Cinemachine;
using UnityEngine;
using EnumCenter;

public class CameraSystem:AbstractSystem
{
    private CinemachineVirtualCamera StaticCamera;
    private CinemachineVirtualCamera SelectCamera;
    private CinemachineVirtualCamera FollowCamera;

    protected override void OnInit()
    {
        base.OnInit();
        StaticCamera = GameObject.Find("StaticCamera")?.GetComponent<CinemachineVirtualCamera>();
        SelectCamera = GameObject.Find("SelectCamera")?.GetComponent<CinemachineVirtualCamera>();
        FollowCamera = GameObject.Find("FollowCamera")?.GetComponent<CinemachineVirtualCamera>();
    }

    public void SetSelectTarget(Transform t)
    {
        SelectCamera.Follow = t;
    }

    public void SetFollowTarget(Transform t)
    {
        FollowCamera.Follow = t;
    }

    public void ChangeCamera(CustomCameraType type)
    {
        StaticCamera?.gameObject.SetActive(type==CustomCameraType.StaticCamera);
        SelectCamera?.gameObject.SetActive(type==CustomCameraType.SelectCamera);
        FollowCamera?.gameObject.SetActive(type==CustomCameraType.FollowCamera);
    }
}