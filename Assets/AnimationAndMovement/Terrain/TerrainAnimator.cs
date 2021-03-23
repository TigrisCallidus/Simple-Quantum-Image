using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainAnimator : MonoBehaviour {

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

    public float MicrophoneScale = 100f;

    const int recordedFrames = 60;
    int currposition = 0;

    public bool HeighPerLoudness = true;

    float[] lastLoudness = new float[recordedFrames];

    public Mesh[] GeneratedMeshes;

    const int FrameRate = 30;

    // Start is called before the first frame update
    void Start() {

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
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        if (HeighPerLoudness) {
            float Loudness = MicInput.MicLoudness * MicrophoneScale;

            if (Loudness > 1) {
                Loudness = 1;
            }

            lastLoudness[currposition] = Loudness;
            currposition++;
            currposition = currposition % recordedFrames;

            Loudness = 0;


            for (int i = 0; i < lastLoudness.Length; i++) {
                Loudness += lastLoudness[i];
            }
            Loudness = Loudness / recordedFrames;

            //Debug.Log(Loudness);


            float height = Mathf.Lerp(MinHeight, MaxHeight, Loudness);
            Generator.GenerateMesh(true, height);
        }
    }


    IEnumerator ColorAnimation() {
        yield return null;

        Generator.MixGradiants = true;
        Generator.HeighGradient = Gradient1;
        Generator.HeighGradient2 = Gradient2;

        float progress = 0;

        while (progress < 1) {
            progress += Time.deltaTime / AnimationDuration;
            yield return null;
            Generator.GradientMixvalue = progress;
            Generator.ColorMesh();
        }
        yield return null;
        progress = 1;
        Generator.GradientMixvalue = 1;
        Generator.ColorMesh();
        yield return new WaitForSeconds(WaitTime);

        if (Reverse) {


            while (progress > 0) {
                progress -= Time.deltaTime / AnimationDuration;
                yield return null;
                Generator.GradientMixvalue = progress;
                Generator.ColorMesh();
            }
            yield return null;
            progress = 0;
            Generator.GradientMixvalue = 0;
            Generator.ColorMesh();
            yield return new WaitForSeconds(WaitTime);
        }


        if (Looping) {
            StartCoroutine(ColorAnimation());
        }
    }

    IEnumerator HeightAnimation() {
        yield return null;


        float progress = 0;
        float height;
        while (progress < 1) {
            progress += Time.deltaTime / AnimationDuration;
            yield return null;
            height = Mathf.Lerp(MinHeight, MaxHeight, progress);
            Generator.GenerateMesh(true, height);
        }
        yield return null;
        progress = 1;
        height = Mathf.Lerp(MinHeight, MaxHeight, progress);
        Generator.GenerateMesh(true, height);
        yield return new WaitForSeconds(WaitTime);

        if (Reverse) {


            while (progress > 0) {
                yield return null;
                height = Mathf.Lerp(MinHeight, MaxHeight, progress);
                Generator.GenerateMesh(true, height);
                progress -= Time.deltaTime / AnimationDuration;
            }
            progress = 0.000001f;
            height = Mathf.Lerp(MinHeight, MaxHeight, progress);
            Generator.GenerateMesh(true, height);
            yield return new WaitForSeconds(WaitTime);
        }


        if (Looping) {
            StartCoroutine(HeightAnimation());
        }
    }



    IEnumerator BlurAnimation() {
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


        if (Looping) {
            StartCoroutine(BlurAnimation());
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
        if (WaitTime>0) {
            yield return new WaitForSeconds(WaitTime);
        }

        if (Reverse) {
            for (int i = GeneratedMeshes.Length-1; i >=0; i--) {
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
        Blur
    }

}
