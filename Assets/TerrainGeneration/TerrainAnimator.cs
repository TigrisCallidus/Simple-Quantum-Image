using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainAnimator : MonoBehaviour
{

    public TerrainGenerator Generator;
    public AnimationType TypeOfAnimation;

    public float AnimationDuration=5;
    public float WaitTime = 1f;

    public bool Reverse = true;
    public bool Looping = true;

    public Gradient Gradient1;
    public Gradient Gradient2;

    public int MinHeight = 0;
    public int MaxHeight = 10;


    public float MinBlur = 0.2f;
    public float MaxBlur = 0.3f;

    // Start is called before the first frame update
    void Start()
    {

        switch (TypeOfAnimation) {
            case AnimationType.Color:
                StartCoroutine(ColorAnimation());
                break;
            case AnimationType.Height:
                StartCoroutine(HeightAnimation());
                break;
            case AnimationType.Blur:
                StartCoroutine(BlurAnimation());
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator ColorAnimation() {
        yield return null;

        Generator.MixGradiants = true;
        Generator.HeighGradient = Gradient1;
        Generator.HeighGradient2 = Gradient2;

        float progress = 0;

        while (progress<1) {
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


    public enum AnimationType {
        Color,
        Height,
        Blur
    }

}
