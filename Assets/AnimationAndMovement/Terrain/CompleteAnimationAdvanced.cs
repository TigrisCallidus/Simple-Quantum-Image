using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteAnimationAdvanced : MonoBehaviour
{

    public ImageCreation ImageCreator;
    public TerrainGenerator Generator;
    public MeshRenderer Renderer;
    public Texture2D TargetImage;
    public Texture2D BlackWhiteImage;
    public Transform Terrain;
    public MeshRenderer TerrainRenderer;
    public GameObject Camera;
    public Transform[] TargetPositions;

    public GameObject UsedLight;
    public Transform StartLightRotation;
    public Transform EndLightRotation;


    public bool AutoStart = true;

    public bool GenerateNewTerrain = false;

    public bool DoGrayscale = true;
    //public bool DoTransform = false;

    public bool DoBlur = true;
    public bool DoGrow = true;
    public bool DoColor = true;
    public bool DoFly = true;
    public bool DoLightning = true;

    public float OverSaturation = 3f;
    public float GreyDuration = 3f;
    public float BlurDuration = 3f;

    public float GrowDuration = 5f;
    public AnimationCurve GrowCurve;
    public float LightningDuration = 3f;
    public float ColorDuration = 3f;

    public float FlyDuration = 5f;

    public float endScale = 1f;
    public float endBlurRotation = 1f;

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
        Renderer.transform.localScale = new Vector3(0.1f* TargetImage.width, 1,0.1f* TargetImage.height);
        if (GenerateNewTerrain) {
            Generator.TextureToBlur = BlackWhiteImage;
            Generator.BlurRotation = endBlurRotation;
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

        if (0 < TargetPositions.Length && TargetPositions[0]!=null) {
            Debug.Log("Change camera");
            Camera.transform.position = TargetPositions[0].position;
            Camera.transform.rotation = TargetPositions[0].rotation;

        }

        if (DoGrow) {
            Terrain.localScale = new Vector3(Terrain.localScale.x, 0, Terrain.localScale.z);

        }

        if (DoGrow || DoGrayscale || DoBlur) {
            Terrain.gameObject.SetActive(false);
            Renderer.gameObject.SetActive(true);
        }

        Debug.Log("StartGrey");


        yield return null;
        if (DoGrayscale) {


            float progress = 0;


            while (progress < 1) {

                targetMat.SetFloat("_Saturation", (1 - progress)* OverSaturation);


                progress += Time.deltaTime / GreyDuration;
                yield return null;
            }

            progress = 1;
            targetMat.SetFloat("_Saturation", (1 - progress) * OverSaturation);


        }

        /*
        yield return new WaitForSeconds(firstWaitTime);

        if (BlackWhiteImage!=null && DoTransform) {

            targetMat.SetTexture("_Texture2", BlackWhiteImage);

            float progress = 0;


            while (progress < 1) {

                targetMat.SetFloat("_Progress", (progress) );


                progress += Time.deltaTime / GreyDuration;
                yield return null;
            }

            progress = 0;

            targetMat.mainTexture = BlackWhiteImage;


            targetMat.SetFloat("_Progress", (progress));


        }
        */
        Debug.Log("EndGrey");


        yield return new WaitForSeconds(firstWaitTime);
        StartCoroutine(BlurAnimation());
    }


    IEnumerator BlurAnimation() {

        Debug.Log("StartBlur");

        if (DoBlur) {



            
            if (BlackWhiteImage!=null) {
                ImageCreator.InputTexture1 = BlackWhiteImage;
            } else {
                ImageCreator.InputTexture1 = TargetImage;
            }
            

            float progress = 0;


            while (progress < 1) {
                float rotation = progress * endBlurRotation;
                ImageCreator.Rotation = rotation;
                ImageCreator.CreateBlur();
                targetMat.mainTexture = ImageCreator.OutputTexture;

                progress += Time.deltaTime / BlurDuration;
                yield return null;
            }

            progress = 1;
            ImageCreator.Rotation = endBlurRotation;
            ImageCreator.CreateBlur();
            targetMat.mainTexture = ImageCreator.OutputTexture;
        }
        Debug.Log("Endblur");
        yield return new WaitForSeconds(secondWaitTime);

        StartCoroutine(GrowAnimation());
    }

    IEnumerator GrowAnimation() {

        Terrain.gameObject.SetActive(true);


        if (DoLightning) {
            UsedLight.transform.rotation = StartLightRotation.rotation;
        }

        Debug.Log("StartGrow");
        if (DoColor) {
            terrainMat.SetFloat("_Saturation", 0);
        }

        if (DoGrow) {



            float progress = 0;



            Vector3 startScale = Terrain.localScale;

            while (progress < 1) {
                float usedProgress = GrowCurve.Evaluate(progress);
                float scale = usedProgress * endScale;
                Terrain.localScale = new Vector3(startScale.x, scale, startScale.z);

                progress += Time.deltaTime / GrowDuration;
                yield return null;
            }

            progress = 1;
            Terrain.localScale = new Vector3(startScale.x, endScale, startScale.z);

        }
        Debug.Log("EndGrow");
        yield return new WaitForSeconds(thirdWaitTime);

        Renderer.gameObject.SetActive(false);


        StartCoroutine(ColorAnimation());
    }


    IEnumerator ColorAnimation() {

        Debug.Log("startColor");

        if (DoColor) {



            float progress = 0;
            terrainMat.SetFloat("_Saturation", progress);


            while (progress < 1) {
                terrainMat.SetFloat("_Saturation", progress);

                progress += Time.deltaTime / ColorDuration;
                yield return null;
            }

            progress = 1;
            terrainMat.SetFloat("_Saturation", progress);
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


            float progress = 1;

            lastPos = Camera.transform.position;
            Quaternion lastRot = Camera.transform.rotation;

            int count = 1;

            Vector3 targetPos = TargetPositions[count].position;
            Quaternion targetRot = TargetPositions[count].rotation;
            
            Quaternion rot;
            float lastprogress = 0;
            float currentProgress = 0;

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
        Debug.Log("end fly");

        StartCoroutine(ColorAnLightningAnimationimation());

    }

    IEnumerator ColorAnLightningAnimationimation() {

        Debug.Log("startLightning");

        if (DoLightning) {



            float progress = 0;


            while (progress < 1) {

                UsedLight.transform.rotation = Quaternion.Lerp(StartLightRotation.rotation, EndLightRotation.rotation, progress);
                progress += Time.deltaTime / LightningDuration;
                yield return null;
            }

            progress = 1;
            UsedLight.transform.rotation = EndLightRotation.rotation;
        }
        Debug.Log("EndLightning");

        yield return new WaitForSeconds(forthWaitTime);
    }

}
