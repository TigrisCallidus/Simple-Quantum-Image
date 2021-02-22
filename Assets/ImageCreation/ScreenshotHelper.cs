using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

[ExecuteInEditMode]
public class ScreenshotHelper : MonoBehaviour
{
    
    public const string FolderName = "Screenshots";

    [Tooltip("The factor by which the resolution is upscaled.")]
    public int Upscaling = 4;
    [Tooltip("If one should be able to capture screenshots during play mode with a button press.")]
    public bool CaptureInPlayMode = true;
    [Tooltip("The button which will capture a screenshot during play mode")]
    public KeyCode ScreenCaptureButton = KeyCode.P;

    FirstPerson firstPerson;

    public void CaptureScreenshot() {
        string time = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second +".png";

        string fileName = Path.Combine(Application.dataPath, FolderName, time);
        ScreenCapture.CaptureScreenshot(fileName, Upscaling);

        Invoke(nameof(Refresh), 1.0f);
        //StopAllCoroutines();
        //StartCoroutine(refresh());
    }


    private void Update() {
        if (Input.GetKeyDown (ScreenCaptureButton)) {
            CaptureScreenshot();
        }
    }


    public void Refresh() {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public IEnumerator refresh() {
        yield return new WaitForSeconds(2);
        Refresh();
    }

    public void MoveCamera(Transform target) {
        if (firstPerson==null) {
            firstPerson = FindObjectOfType<FirstPerson>();
        }
        firstPerson.MoveCamera(target);

    }
}
