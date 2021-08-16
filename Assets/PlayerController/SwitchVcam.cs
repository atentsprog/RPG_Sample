using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
public class SwitchVcam : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput = default;
    [SerializeField]
    private int priorityBoostAmount = 10;
    [SerializeField] private CinemachineVirtualCamera normalVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    private InputAction aimAction;
    [SerializeField] private Canvas normalCanvas;
    [SerializeField] private Canvas aimCanvas;
    void Awake()
    {
        aimVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        aimAction = playerInput.actions["Aim"];
    }
    private void OnEnable()
    {
        aimAction.performed += StartAim;
        aimAction.canceled += CancelAim;
        //aimAction.performed += _ => StartAim1();
    }

    //void StartAim1()
    //{
    //    virtualCamera.Priority += priorityBoostAmount;
    //}
    void StartAim(InputAction.CallbackContext obj)
    {
        //aimVirtualCamera.transform.position = normalVirtualCamera.transform.position;
        aimVirtualCamera.Priority += priorityBoostAmount;
        normalCanvas.enabled = false;
        aimCanvas.enabled = true;
    } 
    void CancelAim(InputAction.CallbackContext obj)
    {
        //normalVirtualCamera.transform.position = aimVirtualCamera.transform.position;
        aimVirtualCamera.Priority -= priorityBoostAmount;
        normalCanvas.enabled = true;
        aimCanvas.enabled = false;
    }
}
