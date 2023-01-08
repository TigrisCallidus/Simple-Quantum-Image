// -*- coding: utf-8 -*-

// This code is part of Qiskit.
//
// (C) Copyright IBM 2020.
//
// This code is licensed under the Apache License, Version 2.0. You may
// obtain a copy of this license in the LICENSE.txt file in the root directory
// of this source tree or at http://www.apache.org/licenses/LICENSE-2.0.
//
// Any modifications or derivative works of this code must retain this
// copyright notice, and modified files need to carry a notice indicating
// that they have been altered from the originals.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson : MonoBehaviour {

#if CINEMACHINE_PRESENT

    public Cinemachine.CinemachineBrain Cinemachine;

#endif


    public bool Active = true;
    public float turnSpeed = 4.0f;
    public float moveSpeed = 2.0f;

    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;

    public float TurboIncrease = 3f;
    private float rotX;

    void Awake() {

#if CINEMACHINE_PRESENT

        if (Cinemachine != null) {
            Cinemachine.enabled = false;
        }
#endif

    }

    void Update() {
        if (!Active) {
            return;
        }
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

    public void MoveCamera(Transform target) {
#if CINEMACHINE_PRESENT
        if (Cinemachine!=null) {
            Cinemachine.enabled = false;
        }
#endif
        this.transform.position = target.transform.position;
        this.transform.rotation = target.transform.rotation;
    }
}
