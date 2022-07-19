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
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
//Custom Editor for the QuantumBlurUnity class, adding some buttons and a representation of the Maze
public class TerrainGeneratorEditor : Editor {

    TerrainGenerator targetScript;

    SerializedProperty objectNumber;


    SerializedProperty fileName;
    SerializedProperty generatedMesh;
    SerializedProperty usedProfile;
    SerializedProperty jsonFileName;


    void OnEnable() {
        targetScript = target as TerrainGenerator;

        fileName = serializedObject.FindProperty("MeshName");
        generatedMesh = serializedObject.FindProperty("GeneratedMesh");
        usedProfile = serializedObject.FindProperty("UsedProfile");
        jsonFileName = serializedObject.FindProperty("JsonFileName");
        objectNumber = serializedObject.FindProperty("ObjectNumber");
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
        /*

        if (targetScript.TextureForTransparency!=null) {
            if (GUILayout.Button("Add Transparency")) {
                targetScript.ApplyTransparency();
            }
        }

        */
        

        if (targetScript.TextureForCut != null) {
            if (GUILayout.Button("Apply Cut")) {
                targetScript.ApplyCut();
            }
        }



        if (GUILayout.Button("Generate a terrain with the chosen visualisation method")) {
            targetScript.GenerateMesh();
        }

        if (GUILayout.Button("Color the terrain with the current settings")) {
            targetScript.ColorMesh();
        }



        GUILayout.BeginHorizontal();
        GUILayout.Label("Prepare A Merge", GUILayout.Width(145));
        targetScript.prepareMerge = EditorGUILayout.Toggle(targetScript.prepareMerge);
        GUILayout.EndHorizontal();

        if (targetScript.prepareMerge) {
            if (GUILayout.Button("Reset View")) {
                targetScript.ResetView();
            }

            EditorGUILayout.PropertyField(objectNumber, new GUIContent("ObjectNumber : "));


            if (GUILayout.Button("Prepare Merge")) {
                targetScript.PrepareMerge();
            }

           
        }


            



            GUILayout.BeginHorizontal();
        GUILayout.Label("Show Advanced Setting", GUILayout.Width(145));
        targetScript.showAdvanced = EditorGUILayout.Toggle(targetScript.showAdvanced);
        GUILayout.EndHorizontal();



        if (targetScript.showAdvanced) {


            EditorGUILayout.PropertyField(generatedMesh, new GUIContent("GeneratedMesh : "));






            //Todo do it using a better fitting object
            //EditorGUILayout.LabelField("Handling Saving and Loading");

            EditorGUILayout.PropertyField(fileName, new GUIContent("Mesh Name : "));


            if (GUILayout.Button("Save the terrain as a mesh")) {
                AssetDatabase.CreateAsset(targetScript.GeneratedMesh, targetScript.GenerateSavePath());
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.PropertyField(usedProfile, new GUIContent("UsedProfile : "));

            if (GUILayout.Button("Save settings to local settings file")) {
                targetScript.SaveSettings();
            }

            if (GUILayout.Button("Load settings from local settings file")) {
                targetScript.LoadSettings();
            }

            EditorGUILayout.PropertyField(jsonFileName, new GUIContent("JsonFileName : "));

            if (GUILayout.Button("Export settings to json")) {
                targetScript.ExportSettingsFile();
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("Import settings from json")) {
                targetScript.ImportSettingsFile();
            }

            if (GUILayout.Button("Import settings from json")) {
                targetScript.ImportSettingsFile();
            }

            if (GUILayout.Button("PrepareNewMaterial")) {
                targetScript.PrepareNewMaterial();
            }


        }

        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();


    }
}