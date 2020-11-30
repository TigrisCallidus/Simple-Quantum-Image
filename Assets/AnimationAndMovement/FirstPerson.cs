﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson : MonoBehaviour {

    public Cinemachine.CinemachineBrain Cinemachine;

    public float turnSpeed = 4.0f;
    public float moveSpeed = 2.0f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;

    public float TurboIncrease = 3f;
    private float rotX;

    void Awake() {
        Cinemachine.enabled = false;
    }

    void Update() {
        MouseAiming();
        KeyboardMovement();
    }

    void MouseAiming() {
        // get the mouse inputs
        float y = Input.GetAxis("Mouse X") * turnSpeed;
        rotX += Input.GetAxis("Mouse Y") * turnSpeed;

        // clamp the vertical rotation
        rotX = Mathf.Clamp(rotX, minTurnAngle, maxTurnAngle);

        // rotate the camera
        transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y + y, 0);
    }

    void KeyboardMovement() {
        Vector3 dir = new Vector3(0, 0, 0);

        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift)) {
            transform.Translate(dir * moveSpeed * TurboIncrease * Time.deltaTime);

        } else {
            transform.Translate(dir * moveSpeed * Time.deltaTime);
        }

    }
}