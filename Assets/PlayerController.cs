using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    AudioSource audioSource;
    private bool isFiring;
    private float speedWhileShooting = 3;
    private float speed = 5;

    public float mouseSensitivity = 40f;
    public Transform cameraTr;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        cameraTr = Camera.main.transform;
    }

    void Update()
    {
        Move();

        CameraRotate();

        Shortcut();
    }

    private void Shortcut()
    {
        if (Input.GetKeyDown(KeyCode.Minus))
            mouseSensitivity -= 1f;
        if (Input.GetKeyDown(KeyCode.Plus))
            mouseSensitivity += 1f;
    }

    private void CameraRotate()
    {
        // 카메라 로테이션을 바꾸자. -> 마우스 이동량에 따라.
        float mouseMoveX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseMoveY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        //cameraTr
        var worldUp = cameraTr.InverseTransformDirection(Vector3.up);
        var rotation = cameraTr.rotation *
                       Quaternion.AngleAxis(mouseMoveX, worldUp) *
                       Quaternion.AngleAxis(mouseMoveY, Vector3.left);
        transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
        cameraTr.rotation = rotation;
    }

    private void Move()
    {
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) move.z = 1;
        if (Input.GetKey(KeyCode.S)) move.z = -1;   // 누름
        if (Input.GetKey(KeyCode.A)) move.x = -1;
        if (Input.GetKey(KeyCode.D)) move.x = 1;
        if (move != Vector3.zero)
        {
            Vector3 relateMove;
            relateMove = Camera.main.transform.forward * move.z; // 0, -1, 0
            relateMove += Camera.main.transform.right * move.x;
            relateMove.y = 0;
            move = relateMove;

            move.Normalize(); // z : -1, x : 0

            float _speed = isFiring ? speedWhileShooting : speed;
            transform.Translate(move * _speed * Time.deltaTime, Space.World);

            float forwardDegree = transform.forward.VectorToDegree();
            float moveDegree = move.VectorToDegree();
            float dirRadian = (moveDegree - forwardDegree + 90) * Mathf.PI / 180; //라디안값
            Vector3 dir;
            dir.x = Mathf.Cos(dirRadian);// 
            dir.z = Mathf.Sin(dirRadian);//

            animator.SetFloat("DirX", dir.x);
            animator.SetFloat("DirY", dir.z);
        }

        audioSource.enabled = move.sqrMagnitude > 0;
        animator.SetFloat("Speed", move.sqrMagnitude);
    }
}
