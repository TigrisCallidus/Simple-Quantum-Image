using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningController : MonoBehaviour {
    public Camera MainCamera;
    public Light MainLight;
    public Light Spotlight1;
    public Light Spotlight2;
    public Light Spotlight3;

    public CameraClearFlags BackgroundType;
    public Color BackgroundColor;

    //public UnityEngine.Rendering.AmbientMode AmbientLightType;

    public AmbientMode AmbientLightningType;

    [Tooltip("The texture which gets modified with the quantum effect. MUST HAVE READ/WRITE ENABLED! (Choose a small texture (256x256 at first)")]

    public float Intensity = 1;
    public Color AmbientMainColor;
    public Color EquatorColor;
    public Color GroundColor;

    public void SetBackgroundSettings() {
        MainCamera.clearFlags = BackgroundType;
        MainCamera.backgroundColor = BackgroundColor;
        RenderSettings.ambientSkyColor = AmbientMainColor;
        RenderSettings.ambientEquatorColor = EquatorColor;
        RenderSettings.ambientGroundColor = GroundColor;
        RenderSettings.ambientIntensity = Intensity;

        switch (AmbientLightningType) {
            case AmbientMode.Skybox:
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
                break;
            case AmbientMode.SingleColor:
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                break;
            case AmbientMode.Gradient:
                RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
                break;
            default:
                break;
        }
    }

    public void SetLayer(int layerMask) {
        MainLight.cullingMask = layerMask;
        Spotlight1.cullingMask = layerMask;
        Spotlight2.cullingMask = layerMask;
        Spotlight3.cullingMask = layerMask;
    }
    
    public enum AmbientMode {
        Skybox,
        SingleColor,
        Gradient
    }
}
