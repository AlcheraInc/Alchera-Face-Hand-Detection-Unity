using UnityEngine;

namespace Alchera
{
    public class FaceStickerPrefab : MonoBehaviour, IFace
    {
        public float lerpRatio = 0.8f;
        new Transform transform;

        AutoBackgroundQuad quad;

        public Pose HeadPose
        {
            set
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, value.position, lerpRatio);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, value.rotation, lerpRatio);
            }
        }

        void Start()
        {
            transform = GetComponent<Transform>();
        }

        public void UseFaceData(ref ImageData image, ref FaceData face)
        {
            if (quad == null)
            {
                quad = FindObjectOfType<AutoBackgroundQuad>();
                return;
            }
            ReadWebcam.instance.GetMirrorValue(out int mirrorX, out int mirrorY);
            mirrorX = mirrorX * ReadWebcam.instance.mirror3D;
            transform.parent.localScale = new Vector3(mirrorX, mirrorY, -1.0f);

            var pose = face.HeadPose;
            pose.position *= 100;
            HeadPose = pose;
        }
    }
}