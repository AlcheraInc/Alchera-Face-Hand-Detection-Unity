using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{
    public class AnimojiPrefab : MonoBehaviour, IFace
    {
        SkinnedMeshRenderer smr;
        Transform leftEye;
        Transform rightEye;
        new Transform transform;

        AutoBackgroundQuad quad;
        Dictionary<string, int> blendIndices;

        public Pose HeadPose
        {
            set
            {
                var lerpRatio = 0.5f;  
                transform.localPosition = Vector3.Lerp(transform.localPosition, value.position, lerpRatio);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, value.rotation, lerpRatio);
            }
        }

        string[] exprIndices = {
            "eyeBlinkRight",
            "eyeBlinkLeft",
            "jawOpen",
            "mouthClose",
            "mouthFunnel",
            "mouthPucker",
            "mouthLeft",
            "mouthRight",
            "mouthFrownRight",
            "mouthShrugLower",
            "mouthShrugUpper",
            "mouthUpperUpLeft",
            "mouthUpperUpRight",
            "browDownLeft",
            "browDownRight",
            "browInnerUp"
        };

        void Start()
        {
            leftEye = base.transform.Find("eye_Left");
            rightEye = base.transform.Find("eye_Right");
            transform = GetComponent<Transform>();
            
            // Acquire renderer for blendshape
            smr = this.GetComponentInChildren<SkinnedMeshRenderer>();

            blendIndices = new Dictionary<string, int>();
            for (int i = 0; i < smr?.sharedMesh.blendShapeCount; ++i) {
                var BSName = smr?.sharedMesh.GetBlendShapeName(i).ToLower();
                blendIndices.Add(BSName, i);
            }
        }

        public unsafe void UseFaceData(ref ImageData image, ref FaceData face)
        {
            if (smr == null) return;
            if (quad == null)
            {
                quad = FindObjectOfType<AutoBackgroundQuad>();
                return;
            }

            ReadWebcam.instance.GetMirrorValue(out int mirrorX, out int mirrorY);
            mirrorX *= ReadWebcam.instance.mirror3D;

            transform.parent.localScale = new Vector3(mirrorX, mirrorY, -1.0f);

            var pose = face.HeadPose;
            pose.position *= 100;
            HeadPose = pose;

            float* weights = stackalloc float[FaceData.NumAnimationWeights];
            face.GetAnimation(weights);
            SetAnimation(weights);

            SetCustomEyeBlink(ref face);
        }
        // set custom eye weight for AnimojiModel 
        unsafe void SetCustomEyeBlink(ref FaceData face)
        {
            var left = GetEyeValue(ref face, 52, 72, 55, 73); //0~100
            var right = GetEyeValue(ref face, 58, 75, 61, 76); //0~100

            SetBlendShapeWeight("PandaBlendshape.eyeBlinkLeft", left);
            SetBlendShapeWeight("PandaBlendshape.eyeBlinkRight", right);
        }
        unsafe float GetEyeValue(ref FaceData face, int l, int t, int r,int b)
        {
            Vector2
                left = face.Landmark[l],
                top = face.Landmark[t],
                right = face.Landmark[r],
                bottom = face.Landmark[b];

            float horDist = Vector2.Distance(left, right);
            float verDist = Vector2.Distance(top, bottom);
            
            var value = verDist / horDist;
            value = (-value + 0.25f) * 1000;
            value = Mathf.Clamp(value, 0, 100);
            return value;
        }
        void SetBlendShapeWeight(string key,float value)
        {
            int idx = blendIndices[key.ToLower()];
            var previous = smr.GetBlendShapeWeight(idx);
            smr.SetBlendShapeWeight(idx, Mathf.Lerp(previous, value, 0.4f));
        }
        public void SetEyeRotation(ref Quaternion left, ref Quaternion right)
        {
            if (leftEye == null || rightEye == null)
                return;
            leftEye.localRotation = left;
            rightEye.localRotation = right;
        }

        // Since weight index might be different for each fbx model
        // be cautious when implementing new AnimojiModel
        public unsafe void SetAnimation(float* weights)
        {
            if (blendIndices == null)
                return;
            for (int i = 0; i < exprIndices.Length; ++i)
            {
                string key = "PandaBlendshape." + exprIndices[i];
                float value = weights[i];
                value = Mathf.Clamp(value, 0, 100);

                //use custom function for eye weighting..
                if (key == "PandaBlendshape.eyeBlinkLeft" || key == "PandaBlendshape.eyeBlinkRight"|| key == "PandaBlendshape.browDownLeft" || key == "PandaBlendshape.browDownRight")
                    continue;
                    
                if (blendIndices.ContainsKey(key.ToLower()))
                {
                    int idx = blendIndices[key.ToLower()];
                    smr.SetBlendShapeWeight(idx, value);
                }
                else
                    Debug.LogError($"{key} is not found..");
            }
        }
    }
}
