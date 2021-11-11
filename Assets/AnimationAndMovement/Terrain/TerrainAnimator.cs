using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainAnimator : MonoBehaviour {

    public bool Autostart = false;

    public TerrainGenerator Generator;
    public AnimationType TypeOfAnimation;
    public bool PrepareMeshesBefore = false;


    public float AnimationDuration = 5;
    public float WaitTime = 1f;

    public bool Reverse = true;
    public bool Looping = true;

    public Gradient Gradient1;
    public Gradient Gradient2;

    public int MinHeight = 0;
    public int MaxHeight = 10;


    public float MinBlur = 0.2f;
    public float MaxBlur = 0.3f;


    public bool InvertNoiseLevel = false;
    public float MicrophoneScale = 100f;

    const int recordedFrames = 60;
    int currposition = 0;

    public bool HeightPerLoudness = true;

    float[] lastLoudness = new float[recordedFrames];

    Mesh[] GeneratedMeshes;

    Vector3[][] Vertices;
    Vector3[][] Normals;
    int[][] Triangles;
    Color[][] Colors;


    const int FrameRate = 30;


    public Mesh Mesh1;
    public Vector3 Position1;
    public Vector3 Rotation1;

    public Mesh Mesh2;
    public Vector3 Position2;
    public Vector3 Rotation2;


    // Start is called before the first frame update
    void Start() {
        if (!Autostart) {
            return;
        }

        Application.targetFrameRate = FrameRate;

        switch (TypeOfAnimation) {
            case AnimationType.Color:
                StartCoroutine(ColorAnimation());
                break;
            case AnimationType.Height:
                if (PrepareMeshesBefore) {
                    PrepareHeightAnimation();
                    StartCoroutine(doPreparedAnimation());

                } else {
                    StartCoroutine(HeightAnimation());
                }
                break;
            case AnimationType.Blur:
                if (PrepareMeshesBefore) {
                    PrepareBlurMeshes();
                    StartCoroutine(doPreparedAnimation());

                } else {
                    StartCoroutine(BlurAnimation());
                }
                break;

            case AnimationType.Transition:
                StartCoroutine(TransitionAnimation());
                
                break;

            case AnimationType.TransitionWithCamera:
                StartCoroutine(TransitionAnimation());
                break;

            default:
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        if (TypeOfAnimation == AnimationType.HeightLoudness) {
            float Loudness = MicInput.MicLoudness * MicrophoneScale;

            if (Loudness > 1) {
                Loudness = 1;
            }
            //Debug.Log(Loudness);

            lastLoudness[currposition] = Loudness;
            currposition++;
            currposition = currposition % recordedFrames;

            Loudness = 0;


            for (int i = 0; i < lastLoudness.Length; i++) {
                Loudness += lastLoudness[i];
            }
            Loudness = Loudness / recordedFrames;

            //Debug.Log(Loudness);

            if (InvertNoiseLevel) {
                Loudness = 1 - Loudness;
            }
            float height = Mathf.Lerp(MinHeight, MaxHeight, Loudness);

            Generator.GenerateMesh(true, height);
        }
    }


    public void Startanimation(AnimationType type, Action callback = null) {
        TypeOfAnimation = type;
        StopAllCoroutines();
        switch (type) {
            case AnimationType.Color:
                StartCoroutine(ColorAnimation(callback));
                break;
            case AnimationType.Height:

                StartCoroutine(HeightAnimation(callback));
                break;
            case AnimationType.Blur:

                StartCoroutine(BlurAnimation(callback));
                break;

            case AnimationType.Transition:
                StartCoroutine(TransitionAnimation(callback));
                break;

            case AnimationType.TransitionWithCamera:
                StartCoroutine(TransitionAnimation(callback));
                break;

            default:
                break;
        }
    }


    IEnumerator ColorAnimation(Action callback = null) {

        Generator.MixGradiants = true;
        Generator.HeighGradient = Gradient1;
        Generator.HeighGradient2 = Gradient2;

        float progress = 0;

        while (progress < 1) {
            progress += Time.deltaTime / AnimationDuration;
            Generator.GradientMixvalue = progress;
            Generator.ColorMesh();
            yield return null;
        }
        yield return null;
        progress = 1;
        Generator.GradientMixvalue = 1;
        Generator.ColorMesh();
        yield return new WaitForSeconds(WaitTime);

        if (Reverse) {


            while (progress > 0) {
                progress -= Time.deltaTime / AnimationDuration;
                Generator.GradientMixvalue = progress;
                Generator.ColorMesh();
                yield return null;
            }
            yield return null;
            progress = 0;
            Generator.GradientMixvalue = 0;
            Generator.ColorMesh();
            yield return new WaitForSeconds(WaitTime);
        }

        if (callback!=null) {
            callback.Invoke();
        }

        if (Looping) {
            StartCoroutine(ColorAnimation());
        }
    }

    IEnumerator HeightAnimation(Action callback = null) {
        Debug.Log("Height Animation");

        float progress = 0;
        float height;
        while (progress < 1) {
            progress += Time.deltaTime / AnimationDuration;
            height = Mathf.Lerp(MinHeight, MaxHeight, progress);
            Generator.GenerateMesh(true, height);
            yield return null;
        }
        yield return null;
        progress = 1;
        height = Mathf.Lerp(MinHeight, MaxHeight, progress);
        Generator.GenerateMesh(true, height);
        yield return new WaitForSeconds(WaitTime);

        if (Reverse) {


            while (progress > 0) {
                height = Mathf.Lerp(MinHeight, MaxHeight, progress);
                Generator.GenerateMesh(true, height);
                progress -= Time.deltaTime / AnimationDuration;
                yield return null;
            }
            progress = 0.000001f;
            height = Mathf.Lerp(MinHeight, MaxHeight, progress);
            Generator.GenerateMesh(true, height);
            yield return new WaitForSeconds(WaitTime);
        }

        if (callback != null) {
            callback.Invoke();
        }

        if (Looping) {
            StartCoroutine(HeightAnimation());
        }
    }



    IEnumerator BlurAnimation(Action callback = null) {
        yield return null;


        float progress = 0;
        float blur;
        while (progress < 1) {
            progress += Time.deltaTime / AnimationDuration;
            yield return null;
            blur = Mathf.Lerp(MinBlur, MaxBlur, progress);
            Generator.BlurRotation = blur;
            Generator.ApplyBlur();
            Generator.GenerateMesh(true);
        }
        yield return null;
        progress = 1;
        blur = Mathf.Lerp(MinBlur, MaxBlur, progress);
        Generator.BlurRotation = blur;
        Generator.ApplyBlur();
        Generator.GenerateMesh(true);
        yield return new WaitForSeconds(WaitTime);

        if (Reverse) {


            while (progress > 0) {
                yield return null;
                blur = Mathf.Lerp(MinBlur, MaxBlur, progress);
                Generator.BlurRotation = blur;
                Generator.ApplyBlur();
                Generator.GenerateMesh(true);
                progress -= Time.deltaTime / AnimationDuration;
            }
            progress = 0.000001f;
            blur = Mathf.Lerp(MinBlur, MaxBlur, progress);
            Generator.BlurRotation = blur;
            Generator.ApplyBlur();
            Generator.GenerateMesh(true);
            yield return new WaitForSeconds(WaitTime);
        }

        if (callback != null) {
            callback.Invoke();
        }

        if (Looping) {
            StartCoroutine(BlurAnimation());
        }
    }

    public Mesh workingMesh;

    //Todo less repetitions call function
    IEnumerator TransitionAnimation(Action callback = null) {

        bool withCamera = TypeOfAnimation == AnimationType.TransitionWithCamera;
        Transform camera = Camera.main.transform;

        if (withCamera) {

            camera.position = Position1;
            camera.rotation = Quaternion.Euler(Rotation1);

        }

        yield return null;


        

        //prepare mesh
        workingMesh = new Mesh();
        workingMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        workingMesh.vertices = Mesh1.vertices;
        workingMesh.triangles = Mesh1.triangles;
        workingMesh.colors = Mesh1.colors;
        workingMesh.normals = Mesh1.normals;
        workingMesh.uv = Mesh1.uv;
        workingMesh.uv2 = Mesh1.uv2;
        workingMesh.name = "WorkingMesh";
        //workingMesh.RecalculateNormals();

        Vector3[] vert1 = Mesh1.vertices;
        Vector3[] normals1 = Mesh1.normals;
        Color[] colors1 = Mesh1.colors;

        Vector3[] vert2 = Mesh2.vertices;
        Vector3[] normals2 = Mesh2.normals;
        Color[] colors2 = Mesh2.colors;

        Vector3[] outVert = workingMesh.vertices;
        Vector3[] outNormals = workingMesh.normals;
        Color[] outColors = workingMesh.colors;

        Generator.TargetMesh.mesh = workingMesh;

        Vector3 position;
        Vector3 rotation;
        

        float progress = 0;


        while (progress < 1) {
            progress += Time.deltaTime / AnimationDuration;
            yield return null;

            LerpVectors(vert1, vert2, outVert, progress);
            LerpVectors(normals1, normals2, outNormals, progress);
            LerpColors(colors1, colors2, outColors, progress);

            workingMesh.vertices = outVert;
            workingMesh.normals = outNormals;
            workingMesh.colors = outColors;

            if (withCamera) {

                position = Vector3.Lerp(Position1, Position2, progress);
                rotation = Vector3.Lerp(Rotation1, Rotation2, progress);

                camera.position = position;
                camera.rotation = Quaternion.Euler(rotation);
            }

        }
        yield return null;
        progress = 1;
        LerpVectors(vert1, vert2, outVert, progress);
        LerpVectors(normals1, normals2, outNormals, progress);
        LerpColors(colors1, colors2, outColors, progress);

        workingMesh.vertices = outVert;
        workingMesh.normals = outNormals;
        workingMesh.colors = outColors;

        if (withCamera) {

            position = Vector3.Lerp(Position1, Position2, progress);
            rotation = Vector3.Lerp(Rotation1, Rotation2, progress);

            camera.position = position;
            camera.rotation = Quaternion.Euler(rotation);
        }

        yield return new WaitForSeconds(WaitTime);

        if (Reverse) {


            while (progress > 0) {
                yield return null;
                LerpVectors(vert1, vert2, outVert, progress);
                LerpVectors(normals1, normals2, outNormals, progress);
                LerpColors(colors1, colors2, outColors, progress);

                workingMesh.vertices = outVert;
                workingMesh.normals = outNormals;
                workingMesh.colors = outColors;

                if (withCamera) {

                    position = Vector3.Lerp(Position1, Position2, progress);
                    rotation = Vector3.Lerp(Rotation1, Rotation2, progress);

                    camera.position = position;
                    camera.rotation = Quaternion.Euler(rotation);
                }

                progress -= Time.deltaTime / AnimationDuration;
            }
            progress = 0.000001f;
            LerpVectors(vert1, vert2, outVert, progress);
            LerpVectors(normals1, normals2, outNormals, progress);
            LerpColors(colors1, colors2, outColors, progress);

            workingMesh.vertices = outVert;
            workingMesh.normals = outNormals;
            workingMesh.colors = outColors;

            if (withCamera) {

                position = Vector3.Lerp(Position1, Position2, progress);
                rotation = Vector3.Lerp(Rotation1, Rotation2, progress);

                camera.position = position;
                camera.rotation = Quaternion.Euler(rotation);
            }

            yield return new WaitForSeconds(WaitTime);
        }

        if (callback != null) {
            callback.Invoke();
        }

        if (Looping) {
            StartCoroutine(TransitionAnimation());
        }
        
    }


    public void LerpVectors(Vector3[] vec1, Vector3[] vec2, Vector3[] outVector, float percentage) {
        if (vec1.Length != vec2.Length || vec2.Length != outVector.Length) {
            Debug.LogError("mismatching dimensions");
            return;
        }
        int length = vec1.Length;
        for (int i = 0; i < length; i++) {
            outVector[i] = Vector3.Lerp(vec1[i], vec2[i], percentage);
        }

    }

    public void LerpColors(Color[] col1, Color[] col2, Color[] outCol, float percentage) {
        if (col1.Length != col2.Length || col2.Length != outCol.Length) {
            Debug.LogError("mismatching dimensions");
            return;
        }
        int length = col1.Length;
        for (int i = 0; i < length; i++) {
            outCol[i] = Color.Lerp(col1[i], col2[i], percentage);
        }

    }

    public void PrepareHeightAnimation() {

        Debug.Log("StartPrepareMeshes");

        int frames = Mathf.CeilToInt(AnimationDuration * FrameRate);
        int numberOFMeshes = frames;

        GeneratedMeshes = new Mesh[numberOFMeshes];
        float smallProgress = 1.0f / (frames - 1);
        float progress = 0;
        float height;
        Generator.GenerateMesh(true, MinHeight);
        GeneratedMeshes[0] = Generator.GeneratedMesh;

        for (int i = 1; i < numberOFMeshes - 1; i++) {
            progress += smallProgress;
            height = Mathf.Lerp(MinHeight, MaxHeight, progress);
            Generator.GenerateMesh(true, height);
            GeneratedMeshes[i] = Generator.GeneratedMesh;
        }
        Generator.GenerateMesh(true, MaxHeight);
        GeneratedMeshes[numberOFMeshes - 1] = Generator.GeneratedMesh;

        Debug.Log("Meshes Prepared");
    }

    public void PrepareBlurMeshes() {

        Debug.Log("StartPrepareMeshes");

        int frames = Mathf.CeilToInt(AnimationDuration * FrameRate);
        int numberOFMeshes = frames;

        GeneratedMeshes = new Mesh[numberOFMeshes];
        float smallProgress = 1.0f / (frames - 1);
        float progress = 0;
        float blur;

        Generator.BlurRotation = MinBlur;
        Generator.ApplyBlur();
        Generator.GenerateMesh(true);
        GeneratedMeshes[0] = Generator.GeneratedMesh;


        for (int i = 1; i < numberOFMeshes - 1; i++) {
            progress += smallProgress;
            blur = Mathf.Lerp(MinBlur, MaxBlur, progress);
            Generator.BlurRotation = blur;
            Generator.ApplyBlur();
            Generator.GenerateMesh(true);
            GeneratedMeshes[i] = Generator.GeneratedMesh;
        }
        Generator.BlurRotation = MaxBlur;
        Generator.ApplyBlur();
        Generator.GenerateMesh(true);
        GeneratedMeshes[numberOFMeshes - 1] = Generator.GeneratedMesh;

        Debug.Log("Meshes Prepared");
    }


    IEnumerator doPreparedAnimation() {

        Debug.Log("Start bluranimation");


        for (int i = 0; i < GeneratedMeshes.Length; i++) {
            Generator.TargetMesh.mesh = GeneratedMeshes[i];
            yield return null;
        }
        if (WaitTime > 0) {
            yield return new WaitForSeconds(WaitTime);
        }

        if (Reverse) {
            for (int i = GeneratedMeshes.Length - 1; i >= 0; i--) {
                Generator.TargetMesh.mesh = GeneratedMeshes[i];
                yield return null;
            }
            if (WaitTime > 0) {
                yield return new WaitForSeconds(WaitTime);
            }
        }
        if (Looping) {
            StartCoroutine(doPreparedAnimation());
        }

    }

    public enum AnimationType {
        None,
        Color,
        Height,
        Blur,
        HeightLoudness,
        Transition,
        TransitionWithCamera
    }

    public struct simplifiedMesh {
        public Vector3[] vertices;
        public Vector3[] normals;
        public int[] triangles;
        public Color[] colors;
    }

}
