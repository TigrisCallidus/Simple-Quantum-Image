using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainAnimator;

public class AnimationSequencer : MonoBehaviour {
    public AnimationSequence[] Sequence;

    [HideInInspector]
    public TerrainAnimator Animator;

    TerrainGenerator generator;
    Mesh startMesh;

    int currentCount = 0;


    AnimationSequence lastSeq;
    AnimationSequence newSeq;

    private void Awake() {
        Animator.Looping = false;
        Animator.Reverse = false;
        generator = Animator.Generator;
        startMesh = generator.TargetMesh.mesh;
        for (int i = 0; i < Sequence.Length; i++) {
            if (Sequence[i].Settings != null) {

                generator.UsedProfile = Sequence[i].Settings;
                generator.LoadSettings();
                generator.ApplyBlur();
                generator.GenerateMesh();
                Debug.Log("generate Mesh");
                Sequence[i].Mesh = generator.TargetMesh.mesh;
                Sequence[i].Mesh.name = "Sequence: " + i;
            }
        }

        //Start condition again
        generator.TargetMesh.mesh = startMesh;
        if (Sequence[0].Mesh!=null) {
            generator.TargetMesh.mesh = Sequence[0].Mesh;
        }
        generator.UsedProfile = Sequence[0].Settings;
        generator.LoadSettings();
        generator.ApplyBlur();


        NextAnimation();
    }

    // Start is called before the first frame update
    void Start() {

    }

    public void NextAnimation() {
        if (currentCount>=Sequence.Length) {
            return;
        }

        Debug.Log("StartAnimation: " + currentCount);

        Animator.AnimationDuration = Sequence[currentCount].AnimationDuration;
        Animator.WaitTime = Sequence[currentCount].WaitTime;
        Animator.MinHeight = Sequence[currentCount].StartHeight;
        Animator.MaxHeight = Sequence[currentCount].EndHeight;


        generator.UsedProfile = Sequence[currentCount].Settings;
        generator.LoadSettings();

        if (Sequence[currentCount].Mesh!=null) {

            //generator.TargetMesh.mesh = Sequence[currentCount].Mesh;
            lastSeq = newSeq;
            newSeq = Sequence[currentCount];
            if (lastSeq!=null) {
                Animator.Mesh1 = lastSeq.Mesh;
                Animator.Mesh2 = newSeq.Mesh;
                Animator.Position1 = lastSeq.Settings.CameraPosition;
                Animator.Rotation1 = lastSeq.Settings.CameraRotation.eulerAngles;
                Animator.Position2 = newSeq.Settings.CameraPosition;
                Animator.Rotation2 = newSeq.Settings.CameraRotation.eulerAngles;

            }
            if (newSeq.Type!= AnimationType.Transition) {
                generator.TargetMesh.mesh = Sequence[currentCount].Mesh;
            }
        }

        Animator.Startanimation(Sequence[currentCount].Type, NextAnimation);
        currentCount++;
    }

    [System.Serializable]
    public class AnimationSequence {
        public AnimationType Type;
        public float AnimationDuration;
        public float WaitTime;
        public MeshCreationSettings Settings;
        public int StartHeight;
        public int EndHeight;

        //[HideInInspector]
        public Mesh Mesh;


    }
}




