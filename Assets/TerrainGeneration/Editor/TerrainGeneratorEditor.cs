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

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
//Custom Editor for the QuantumBlurUnity class, adding some buttons and a representation of the Maze
public class TerrainGeneratorEditor : Editor {

    TerrainGenerator targetScript;

    void OnEnable() {
        targetScript = target as TerrainGenerator;
    }

    public override void OnInspectorGUI() {

        // Let the default inspecter draw all the values
        DrawDefaultInspector();

        // Spawn buttons

        if (GUILayout.Button("Apply Blur effect to the Texture to Blur")) {
            targetScript.ApplyBlur();
        }
        
        //if (GUILayout.Button("Apply your own effect to the Texture to Blur")) {
            //targetScript.ApplyYourOwnEffect();
        //}

        /*

        if (GUILayout.Button("Generate 2D Data")) {
            targetScript.Generate2DData();
        }

        if (GUILayout.Button("Generate 3D Data")) {
            targetScript.Generate3DDataFrom2DData();
        }

        */

        if (GUILayout.Button("Generate a terrain with the chosen visualisation method")) {
            targetScript.GenerateMesh();
        }

        if (GUILayout.Button("Color the terrain with the current settings")) {
            targetScript.ColorMesh();
        }

        if (GUILayout.Button("Save the terrain as a mesh")) {
            AssetDatabase.CreateAsset(targetScript.GeneratedMesh, targetScript.GenerateSavePath());
            AssetDatabase.SaveAssets();
        }

        if (GUILayout.Button("Save settings to local settings file")) {
            targetScript.SaveSettings();
        }

        if (GUILayout.Button("Load settings from local settings file")) {
            targetScript.LoadSettings();
        }


        if (GUILayout.Button("Export settings to json")) {
            targetScript.ExportSettingsFile();
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Import settings from json")) {
            targetScript.ImportSettingsFile();
        }

    }
}