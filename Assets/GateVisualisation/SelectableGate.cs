﻿// -*- coding: utf-8 -*-

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
using UnityEngine.UI;

public class SelectableGate : MonoBehaviour {

    public GameObject UI;

    public int PosX;

    public int PosY;

    public GameObject Visuals;
    public Text Name;
    public Text Rotation;

    public bool IsSet = true;

    public GameObject HGate;
    public GameObject XGate;
    public GameObject RXGate;
    public GameObject Dot;

    public void Initialise(int x, int y) {
        PosX = x;
        PosY = y;
        UI.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public void Select() {
        Debug.Log("Select");
        GateUIController.Instance.Select(this);
    }


    public void SetActive(bool active) {
        UI.SetActive(active);
    }

    public void SetName(string name) {
        Name.text = name;
    }

    public void SetRotation(string rotation) {
        Rotation.text = rotation;
    }

    public void ActivateVisuals() {
        Debug.Log("Activate Visuals");
        Visuals.SetActive(true);
        HGate.SetActive(false);
        XGate.SetActive(false);
        RXGate.SetActive(false);
        Dot.SetActive(false);
    }

    public void Reset() {
        //Debug.Log("Reset");
        UI.SetActive(true);
        Visuals.SetActive(false);
        Name.text = "";
        Rotation.text = "";
        IsSet = false;
        Visuals.SetActive(false);
        HGate.SetActive(false);
        XGate.SetActive(false);
        RXGate.SetActive(false);
        Dot.SetActive(false);
    }


    public void ActivateH() {
        Debug.Log("Activate H");
        Visuals.SetActive(false);
        HGate.SetActive(true);
        XGate.SetActive(false);
        RXGate.SetActive(false);
        Dot.SetActive(false);
    }

    public void ActivateX() {
        Debug.Log("Activate X");
        Visuals.SetActive(false);
        HGate.SetActive(false);
        XGate.SetActive(true);
        RXGate.SetActive(false);
        Dot.SetActive(false);
    }

    public void ActivateRX() {
        Debug.Log("Activate RX");
        Visuals.SetActive(false);
        HGate.SetActive(false);
        XGate.SetActive(false);
        RXGate.SetActive(true);
        Dot.SetActive(false);
    }

    public void ActivateDot() {
        Debug.Log("Activate Dot");
        Visuals.SetActive(false);
        HGate.SetActive(false);
        XGate.SetActive(false);
        RXGate.SetActive(false);
        Dot.SetActive(true);
    }


}
