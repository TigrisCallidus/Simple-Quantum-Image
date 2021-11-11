using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteAnimation : MonoBehaviour
{

    public ImageCreation ImageCreator;
    public TerrainGenerator Generator;
    public MeshRenderer Renderer;
    public Texture2D TargetImage;
    public Transform Terrain;
    public MeshRenderer TerrainRenderer;
    public GameObject Camera;
    public Transform[] TargetPositions;


    public bool AutoStart = true;

    public bool GenerateNewTerrain = false;

    public bool DoGrayscale = true;
    public bool DoBlur = true;
    public bool DoGrow = true;
    public bool DoColor = true;
    public bool DoFly = true;

    public float AnimationDuration = 3f;

    public float GrowDuration = 5f;

    public float FlyDuration = 5f;

    public float endScale = 0.12f;
    public float endRotation = 0.5f;

    public float firstWaitTime = 0f;
    public float secondWaitTime = 0.5f;
    public float thirdWaitTime = 0.01f;
    public float forthWaitTime = 0.01f;



    Material targetMat;
    Material terrainMat;

    private void Awake() {
        targetMat = Renderer.material;
        terrainMat = TerrainRenderer.material;
        targetMat.mainTexture = TargetImage;
        if (GenerateNewTerrain) {
            Generator.TextureToBlur = TargetImage;
            Generator.BlurRotation = endRotation;
            Generator.ApplyBlur();
            Generator.GenerateMesh();
        }

        if (AutoStart) {
            StartCoroutine(GreyAnimation());
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(GreyAnimation());

        }
    }


    IEnumerator GreyAnimation() {
        yield return null;
        if (DoGrayscale) {

        }
        yield return new WaitForSeconds(firstWaitTime);
        StartCoroutine(BlurAnimation());
    }


    IEnumerator BlurAnimation() {

        Debug.Log("StartBlur");

        if (DoBlur) {



            float progress = 0;

            ImageCreator.InputTexture1 = TargetImage;

            while (progress < 1) {
                float rotation = progress * endRotation;
                ImageCreator.Rotation = rotation;
                ImageCreator.CreateBlur();
                targetMat.mainTexture = ImageCreator.OutputTexture;

                progress += Time.deltaTime / AnimationDuration;
                yield return null;
            }

            progress = 1;
            ImageCreator.Rotation = endRotation;
            ImageCreator.CreateBlur();
            targetMat.mainTexture = ImageCreator.OutputTexture;
        }
        Debug.Log("Endblur");
        yield return new WaitForSeconds(secondWaitTime);

        StartCoroutine(GrowAnimation());
    }

    IEnumerator GrowAnimation() {

        Debug.Log("StartGrow");


        if (DoGrow) {



            float progress = 0;



            Vector3 startScale = Terrain.localScale;

            while (progress < 1) {
                float scale = progress*progress*progress * endScale;
                Terrain.localScale = new Vector3(startScale.x, scale, startScale.z);

                progress += Time.deltaTime / GrowDuration;
                yield return null;
            }

            progress = 1;
            Terrain.localScale = new Vector3(startScale.x, endScale, startScale.z);

        }
        Debug.Log("EndGrow");
        yield return new WaitForSeconds(thirdWaitTime);

        StartCoroutine(ColorAnimation());
    }


    IEnumerator ColorAnimation() {

        Debug.Log("startColor");

        if (DoColor) {



            float progress = 0;


            while (progress < 1) {
                terrainMat.SetFloat("_ColorScale", progress);

                progress += Time.deltaTime / AnimationDuration;
                yield return null;
            }

            progress = 1;
            terrainMat.SetFloat("_ColorScale", progress);
        }
        Debug.Log("EndColor");

        yield return new WaitForSeconds(forthWaitTime);

        StartCoroutine(FlyAnimation());
    }


    IEnumerator FlyAnimation() {

        Debug.Log("startFly");

        if (DoFly) {


            float[] lengths = new float[TargetPositions.Length];
            float totalLength = 0;

            Vector3 lastPos = Camera.transform.position;
            for (int i = 0; i < TargetPositions.Length; i++) {
                float length = (TargetPositions[i].position - lastPos).magnitude;
                totalLength += length;
                lengths[i] = length;
            }

            float lastLength = 0;

            for (int i = 0; i < TargetPositions.Length; i++) {
                lengths[i] = lastLength+lengths[i]/totalLength;
                lastLength= lengths[i];
            }


            float progress = 0;

            lastPos = Camera.transform.position;
            Quaternion lastRot = Camera.transform.rotation;


            Vector3 targetPos = TargetPositions[0].position;
            Quaternion targetRot = TargetPositions[0].rotation;
            
            Quaternion rot;
            float lastprogress = 0;
            float currentProgress = 0;
            int count = 0;

            while (progress < 1) {





                Camera.transform.position = Vector3.Lerp(lastPos, targetPos, currentProgress);
                rot= Quaternion.Lerp(lastRot, targetRot, currentProgress);
                Camera.transform.rotation = rot;
                
                progress += Time.deltaTime / FlyDuration;


                if (progress > lengths[count]) {
                    lastprogress = lengths[count];
                    count++;
                    if (count >= lengths.Length) {
                        break;
                    }
                    lastPos = targetPos;
                    lastRot = targetRot;

                    Debug.Log(progress + " " + count + " " + lengths[count] + " " + lastprogress);
                    targetPos = TargetPositions[count].position;
                    targetRot = TargetPositions[count].rotation;
                }

                currentProgress = (progress - lastprogress) / (lengths[count] - lastprogress);


                yield return null;
            }

            progress = 1;
            Camera.transform.position = targetPos;
            Camera.transform.rotation = targetRot;
        }
        Debug.Log("EndColor");
    }

}
