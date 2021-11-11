using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessincScript : MonoBehaviour {

    public Material PostProcessingMaterial;

    public bool IsRunning = true;

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (IsRunning) {
            Graphics.Blit(source, destination, PostProcessingMaterial);
        } else {
            Graphics.Blit(source, destination);
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            IsRunning = !IsRunning;
        }
    }
}
