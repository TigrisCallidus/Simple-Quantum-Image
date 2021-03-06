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
using Qiskit;
using QuantumImage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class QuantumPaint : MonoBehaviour {

    [Header("Important")]
    public ImageSize Size;
    public Color PaintColor = Color.white;

    public ReflectionType Type;
    [Range(0, 1)]
    public float Strength;
    public int AxisToReflect;



    //public Reflection ReflectionToAdd;

    Gate GateToAdd = new Gate();

    [Header("Advanced")]
    public bool ShowAdvanced;


    public bool UseSimpleEncoding = false;
    public bool RenormalizeImage = true;

    public List<Gate> Gates;
    public QuantumCircuit Circuit;
    public RawImage TargetImage;

    [Header("File Management")]
    public string FileName;
    [Tooltip("The folder name (under Assets) in which the file will be stored (when pressing saving file direct).")]
    public string FolderName = "Visuals";

    public int logSize = 2;

    public TextAsset Text;

    public GameObject[] Vertical;
    public GameObject[] Horizontal;

    public string QiskitString;

    public CircuitHolder Holder;

    Texture2D startTexture;
    Texture2D currentTexture;
    int size;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Test() {
        Debug.Log("Test");
        TargetImage.texture = CircuitToTexture();
    }


    public Texture2D CircuitToTexture() {


        Texture2D outPutTexture;

        //generating the quantum circuits encoding the color channels of the image

        QuantumCircuit red;
        //QuantumCircuit green;
        //QuantumCircuit blue;


        double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(startTexture, ColorChannel.R);
        if (UseSimpleEncoding) {
            red = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red = QuantumImageHelper.HeightToCircuit(imageData);
        }


        Circuit = red;


        red.Gates = Gates;
        //blue.Gates = Gates;
        //green.Gates = Gates;

        double[,] redData;//,  greenData, blueData;

        if (UseSimpleEncoding) {
            redData = QuantumImageHelper.CircuitToImage(red, size, size, RenormalizeImage);
            //greenData = QuantumImageHelper.CircuitToImage(green, size, size);
            //blueData = QuantumImageHelper.CircuitToImage(blue, size, size);

        } else {
            redData = QuantumImageHelper.CircuitToHeight2D(red, size, size, RenormalizeImage);
            //greenData = QuantumImageHelper.CircuitToHeight2D(green, size, size);
            //blueData = QuantumImageHelper.CircuitToHeight2D(blue, size, size);
        }

        //outPutTexture = QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);
        outPutTexture = QuantumImageHelper.CalculateColorTexture(redData, redData, redData, PaintColor.r, PaintColor.g, PaintColor.b);
        outPutTexture.filterMode = FilterMode.Point;
        outPutTexture.wrapMode = TextureWrapMode.Clamp;
        QiskitString = red.GetQiskitString(true);

        return outPutTexture;

    }

    public void InitializeImage() {

        size = 4;

        switch (Size) {
            case ImageSize._4x4:
                logSize = 2;
                size = 4;
                //Quantumcircuit 4
                break;
            case ImageSize._8x8:
                logSize = 3;
                size = 8;
                //Quantumcircuit 6
                break;
            case ImageSize._16x16:
                logSize = 4;
                size = 16;
                //Quantumcircuit 8
                break;
            case ImageSize._32x32:
                logSize = 5;
                size = 32;
                //Quantumcircuit 10
                break;
            case ImageSize._64x64:
                logSize = 6;
                size = 64;
                //Quantumcircuit 12
                break;
            case ImageSize._128x128:
                logSize = 7;
                size = 128;
                //Quantumcircuit 14
                break;
            default:
                break;
        }

        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < size; i++) {
            for (int j = 0; j < size; j++) {
                texture.SetPixel(i, j, Color.black);
            }
        }

        texture.SetPixel(0, 0, PaintColor);

        startTexture = texture;
        Gates = new List<Gate>();

        texture.Apply();

        TargetImage.texture = startTexture;
        currentTexture = startTexture;
        ShowPreview();
        ApplyGates();

    }

    public void AddGate() {
        GateToAdd = new Gate();
        GateToAdd.CircuitType = CircuitType.RX;
        GateToAdd.Theta = Strength * Mathf.PI;
        GateToAdd.First = AxisToReflect - 1;
        if (Type == ReflectionType.Vertical) {
            GateToAdd.First = AxisToReflect - 1 + logSize;
        }
        Gates.Add(GateToAdd);
        ApplyGates();
    }


    public void Undo() {
        if (Gates.Count > 0) {
            Gates.RemoveAt(Gates.Count - 1);
            ApplyGates();
        }
    }

    public void ApplyGates() {
        Holder.Gates = Gates;
        currentTexture = CircuitToTexture();
        TargetImage.texture = currentTexture;

    }

    public void CreateCircuitFromHolder() {
        Gates = Holder.Gates;
        currentTexture = CircuitToTexture();
        TargetImage.texture = currentTexture;

    }



    public void OptimizeGates() {

        int gateCount = Gates.Count;
        if (gateCount < 2) {
            return;
        }

        List<Gate> newGates = new List<Gate>();
        Gate lastGate = Gates[0];
        double TwoPi = MathHelper.Pi * 2;

        for (int i = 1; i < gateCount; i++) {
            Gate nextGate = Gates[i];
            if (lastGate.CircuitType == CircuitType.RX && nextGate.CircuitType == CircuitType.RX && lastGate.First == nextGate.First) {
                lastGate.Theta = lastGate.Theta + nextGate.Theta;
            } else {
                if (lastGate.CircuitType == CircuitType.RX) {
                    lastGate.Theta = lastGate.Theta % TwoPi;
                    if (lastGate.Theta > MathHelper.Pi) {
                        //TODO better improve
                        //lastGate.Theta = TwoPi - lastGate.Theta;
                    }
                }
                newGates.Add(lastGate);
                lastGate = nextGate;
            }
        }
        if (lastGate.CircuitType == CircuitType.RX) {
            lastGate.Theta = lastGate.Theta % TwoPi;
            if (lastGate.Theta > MathHelper.Pi) {
                //TODO better improve
                //lastGate.Theta = TwoPi - lastGate.Theta;
            }
        }
        newGates.Add(lastGate);
        Gates = newGates;
        ApplyGates();


    }

    public void ConstructGateFromString() {

        string text = Text.text;

        Debug.Log(text);

        string[] lines = text.Split(new string[] { "\n" , "\r\n", "\r"}, StringSplitOptions.None);

        Debug.Log("Text: " + lines.Length);

        if (lines.Length < 2) {
            return;
        }
        string tmp = lines[0].Replace("qc = QuantumCircuit(", "");
        tmp = tmp.Replace(")", "");
        tmp = tmp.Split(new string[] { ", ", "," }, StringSplitOptions.None)[0];
        int qubits = 0;

        if (!int.TryParse(tmp, out qubits)) {
            Debug.Log("cant parse " + tmp );
            return;
        }

        Debug.Log("Needing " + qubits + " qubits");
        switch (qubits) {
            case 4:
                Size = ImageSize._4x4;
                break;
            case 6:
                Size = ImageSize._8x8;
                break;
            case 8:
                Size = ImageSize._16x16;
                break;
            case 10:
                Size = ImageSize._32x32;
                break;
            case 12:
                Size = ImageSize._64x64;
                break;
            case 14:
                Size = ImageSize._128x128;
                break;
            default:
                Debug.Log("Strange qubit number " + qubits);
                return;
        }
        InitializeImage();

        for (int i = 1; i < lines.Length; i++) {
            ParseGate(lines[i]);
        }
        ApplyGates();

    }

    public void ParseGate(string line) {
        if (!line.StartsWith("qc.")) {
            Debug.Log("line starts wrong: " + line);
            return;
        }

        if (line.StartsWith("qc.x(")) {
            line = line.Replace("qc.x(", "");
            line = line.Replace(")", "");
            int first;
            if (int.TryParse(line, out first)) {
                Gate gate = new Gate();
                gate.CircuitType = CircuitType.X;
                gate.First = first;
                Gates.Add(gate);
            }

        }else if (line.StartsWith("qc.rx(")) {
            line = line.Replace("qc.rx(", "");
            line = line.Replace(")", "");
            string[] lines = line.Split(new string[] { ", " }, StringSplitOptions.None);
            double theta;
            int first;
            if (double.TryParse(lines[0], out theta)) {
                if (int.TryParse(lines[1], out first)) {
                    Gate gate = new Gate();
                    gate.CircuitType = CircuitType.RX;
                    gate.First = first;
                    gate.Theta = theta;
                    Gates.Add(gate);
                }
            }

        }else if (line.StartsWith("qc.h(")) {
            line = line.Replace("qc.h(", "");
            line = line.Replace(")", "");
            int first;
            if (int.TryParse(line, out first)) {
                Gate gate = new Gate();
                gate.CircuitType = CircuitType.H;
                gate.First = first;
                Gates.Add(gate);
            }

        }else if (line.StartsWith("qc.cx(")) {
            line = line.Replace("qc.cx(", "");
            line = line.Replace(")", "");
            string[] lines = line.Split(new string[] { ", " }, StringSplitOptions.None);
            int second;
            int first;
            if (int.TryParse(lines[0], out first)) {
                if (int.TryParse(lines[1], out second)) {
                    Gate gate = new Gate();
                    gate.CircuitType = CircuitType.CX;
                    gate.First = first;
                    gate.Second = second;
                    Gates.Add(gate);
                }
            }

        } else if (line.StartsWith("qc.crx(")) {
            //not yet implemented
        } else {
            Debug.Log("line is strange: " + line);
        }

    }


    public Texture2D CreateBlur(Texture2D inputeTExture, float rotation) {

        Texture2D outPutTexture;


        //generating the quantum circuits encoding the color channels of the image

        QuantumCircuit red;
        QuantumCircuit green;
        QuantumCircuit blue;

        double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(inputeTExture, ColorChannel.R);
        if (UseSimpleEncoding) {
            red = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(inputeTExture, ColorChannel.G);
        if (UseSimpleEncoding) {
            green = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            green = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(inputeTExture, ColorChannel.B);
        if (UseSimpleEncoding) {
            blue = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            blue = QuantumImageHelper.HeightToCircuit(imageData);
        }

        //QuantumCircuit red = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.R);
        //QuantumCircuit green = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.G);
        //QuantumCircuit blue = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.B);

        //applying the rotation generating the blur effect
        ApplyPartialQ(red, rotation);
        ApplyPartialQ(green, rotation);
        ApplyPartialQ(blue, rotation);

        Gates = red.Gates;
        double[,] redData, greenData, blueData;

        if (UseSimpleEncoding) {
            redData = QuantumImageHelper.CircuitToImage(red, inputeTExture.width, inputeTExture.height, RenormalizeImage);
            greenData = QuantumImageHelper.CircuitToImage(green, inputeTExture.width, inputeTExture.height, RenormalizeImage);
            blueData = QuantumImageHelper.CircuitToImage(blue, inputeTExture.width, inputeTExture.height, RenormalizeImage);

        } else {
            redData = QuantumImageHelper.CircuitToHeight2D(red, inputeTExture.width, inputeTExture.height, RenormalizeImage);
            greenData = QuantumImageHelper.CircuitToHeight2D(green, inputeTExture.width, inputeTExture.height, RenormalizeImage);
            blueData = QuantumImageHelper.CircuitToHeight2D(blue, inputeTExture.width, inputeTExture.height, RenormalizeImage);
        }

        outPutTexture = QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);
        return outPutTexture;

    }

    public Texture2D ApplyGates(Texture2D InputTexture1) {

        Texture2D outPutTexture;


        //generating the quantum circuits encoding the color channels of the image

        QuantumCircuit red;
        QuantumCircuit green;
        QuantumCircuit blue;

        double[,] imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.R);
        if (UseSimpleEncoding) {
            red = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            red = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.G);
        if (UseSimpleEncoding) {
            green = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            green = QuantumImageHelper.HeightToCircuit(imageData);
        }

        imageData = QuantumImageHelper.GetHeightArrayDouble(InputTexture1, ColorChannel.B);
        if (UseSimpleEncoding) {
            blue = QuantumImageHelper.ImageToCircuit(imageData);
        } else {
            blue = QuantumImageHelper.HeightToCircuit(imageData);
        }

        //QuantumCircuit red = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.R);
        //QuantumCircuit green = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.G);
        //QuantumCircuit blue = QuantumImageCreator.GetCircuitDirect(InputTexture1, ColorChannel.B);

        red.Gates = Gates;
        blue.Gates = Gates;
        green.Gates = Gates;

        double[,] redData, greenData, blueData;

        if (UseSimpleEncoding) {
            redData = QuantumImageHelper.CircuitToImage(red, InputTexture1.width, InputTexture1.height);
            greenData = QuantumImageHelper.CircuitToImage(green, InputTexture1.width, InputTexture1.height);
            blueData = QuantumImageHelper.CircuitToImage(blue, InputTexture1.width, InputTexture1.height);

        } else {
            redData = QuantumImageHelper.CircuitToHeight2D(red, InputTexture1.width, InputTexture1.height);
            greenData = QuantumImageHelper.CircuitToHeight2D(green, InputTexture1.width, InputTexture1.height);
            blueData = QuantumImageHelper.CircuitToHeight2D(blue, InputTexture1.width, InputTexture1.height);
        }

        outPutTexture = QuantumImageHelper.CalculateColorTexture(redData, greenData, blueData);

        return outPutTexture;
    }

    public void ShowPreview() {
        GameObject[] highlights = Horizontal;
        switch (Type) {
            case ReflectionType.Horizontal:
                highlights = Horizontal;
                break;
            case ReflectionType.Vertical:
                highlights = Vertical;
                break;
            default:
                break;
        }
        int target = logSize - AxisToReflect;
        for (int i = 0; i < Horizontal.Length; i++) {
            Horizontal[i].SetActive(false);
        }
        for (int i = 0; i < Vertical.Length; i++) {
            Vertical[i].SetActive(false);
        }
        if (target <= highlights.Length) {
            highlights[target].SetActive(true);
        }


    }


    /// <summary>
    /// Applies a partial rotation (in radian) to each qubit of a quantum circuit.
    /// </summary>
    /// <param name="circuit">The quantum circuit to which the rotation is applied</param>
    /// <param name="rotation">The applied rotation. Rotation is in radian (so 2PI is a full rotation)</param>
    public static void ApplyPartialQ(QuantumCircuit circuit, float rotation) {
        for (int i = 0; i < circuit.NumberOfQubits; i++) {
            circuit.RX(i, rotation);
        }
    }

    public void SaveFile() {
        string path = Path.Combine(Application.dataPath, FolderName, FileName + ".png");
        File.WriteAllBytes(path, currentTexture.EncodeToPNG());
    }

    public enum ImageSize {
        _4x4,
        _8x8,
        _16x16,
        _32x32,
        _64x64,
        _128x128
    }

    public enum ReflectionType {
        Horizontal,
        Vertical
    }

    /*
    [System.Serializable]
    public class Reflection {
        public ReflectionType Type;
        [Range(0, 1)]
        public float Strength;
        [Range(0, 1)]
        public int AxisToReflect;
    }
    */
}


