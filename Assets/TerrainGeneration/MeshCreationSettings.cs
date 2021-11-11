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

using UnityEngine;

/// <summary>
/// Scriptable object to store settings (like a profile) for mesh creation.
/// This way for different kinds of images, different settings can be stored and easily changed.
/// </summary>
[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/MeshCreationSettings", order = 1)]
public class MeshCreationSettings : ScriptableObject {

    public bool Locked = false;

    public Texture2D Texture;

    public float BlurRotation = 0.25f;
    public int MaxHeight = 10;
    public bool Invert;
    public float Threshold = 0.5f;
    public Gradient HeighGradient;
    public bool AlwaysDrawBottomCube = true;
    public float ColorTranslation = 0;
    public float ColorScaling = 1;
    public TerrainGenerator.VisualitationType VisualisationMethod;

    public Vector3 CameraPosition;
    public Quaternion CameraRotation;

    public Vector3 SunPosition;
    public Quaternion SunRotation;
    public Color SunColor;
    public float SunIntensity;

    public Vector3 Light1Position;
    public Quaternion Light1Rotation;
    public Color Light1Color;
    public float Light1Intensity;
    public float Light1Range;
    public LightType Light1Type;

    public Vector3 Light2Position;
    public Quaternion Light2Rotation;
    public Color Light2Color;
    public float Light2Intensity;
    public float Light2Range;
    public LightType Light2Type;

    public Vector3 Light3Position;
    public Quaternion Light3Rotation;
    public Color Light3Color;
    public float Light3Intensity;
    public float Light3Range;
    public LightType Light3Type;



}

[System.Serializable]
public class MeshCreationExportSettings {
    public float BlurRotation = 0.25f;
    public int MaxHeight = 10;
    public bool Invert;
    public float Threshold = 0.5f;
    public Gradient HeighGradient;
    public bool AlwaysDrawBottomCube = true;
    public float ColorTranslation = 0;
    public float ColorScaling = 1;
    public TerrainGenerator.VisualitationType VisualisationMethod;
}