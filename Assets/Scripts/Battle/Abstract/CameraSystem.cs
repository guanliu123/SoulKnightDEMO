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
        FindCamera();
        
        EventManager.Instance.SingOn(EventId.MAP_GENERATION_COMPLETED,FollowPlayer);
    }

    private void FollowPlayer()
    {
        SetFollowTarget(AbstractManager.Instance.GetController<PlayerController>().MainPlayer.transform);
        ChangeCamera(CustomCameraType.FollowCamera);
    }

    private void FindCamera()
    {
        StaticCamera = GameObject.Find("StaticCamera")?.GetComponent<CinemachineVirtualCamera>();
        SelectCamera = GameObject.Find("SelectCamera")?.GetComponent<CinemachineVirtualCamera>();
        FollowCamera = GameObject.Find("FollowCamera")?.GetComponent<CinemachineVirtualCamera>();
    }

    public void SetSelectTarget(Transform t)
    {
        if (SelectCamera == null)
        {
            FindCamera();
        }
        SelectCamera.Follow = t;
    }

    public void SetFollowTarget(Transform t)
    {
        if (FollowCamera == null)
        {
            FindCamera();
        }
        FollowCamera.Follow = t;
    }

    public void ChangeCamera(CustomCameraType type)
    {
        //FindCamera();
        StaticCamera?.gameObject.SetActive(type==CustomCameraType.StaticCamera);
        SelectCamera?.gameObject.SetActive(type==CustomCameraType.SelectCamera);
        FollowCamera?.gameObject.SetActive(type==CustomCameraType.FollowCamera);
    }
}