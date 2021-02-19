using System.Collections;
using UnityEngine;

namespace Alchera
{
    public class AutoBackgroundQuad : MonoBehaviour
    {
        [SerializeField] Material webcamMaterial = null;
        [HideInInspector] public Texture texture;

        WaitForSeconds ws = new WaitForSeconds(0.5f);

        void Start()
        {
            Debug.Log(webcamMaterial.ToString());
            transform.localPosition = new Vector3(0, 0, 300);

            StartCoroutine(UpdateQuad());
        }

        public IEnumerator UpdateQuad()
        {
            texture = new Texture2D(0, 0);

            while (texture.width < 20)
            {
                if (!ReadWebcam.instance.current.isPlaying)
                {
                    yield return ws;
                    continue;
                }
                var request = (ReadWebcam.instance as ITextureSequence).Capture();
                yield return request;
                texture = request.Result;
                yield return ws;
            }
            webcamMaterial.mainTexture = texture;
        }

        void Update()
        {
            if (texture.width < 20)
                return;

            SetQuadSize();
            SetQuadMirror();
            SetQuadRotation();
        }

        void SetQuadSize()
        {
            var zScale = Mathf.Tan(Mathf.Deg2Rad * (Camera.main.fieldOfView / 2.0f)) * transform.localPosition.z * 2.0f;

            float w = texture.width;
            float h = texture.height;
            float screenRatio = (float)Screen.width / Screen.height;

            if (screenRatio > 1) // width > height
            {
                if (screenRatio > (w / h))
                {
                    float scaleRatio = screenRatio * h / w;
                    transform.localScale = new Vector3(zScale * (w / h) * scaleRatio, zScale * scaleRatio, 1);
                }
                else
                    transform.localScale = new Vector3(zScale * (w / h), zScale, 1);
            }
            else
            {
                if (screenRatio > (h / w))
                {
                    float scaleRatio = screenRatio * w / h;
                    transform.localScale = new Vector3(zScale * scaleRatio, zScale * scaleRatio * (h / w), 1);
                }
                else
                    transform.localScale = new Vector3(zScale, zScale * (h / w), 1);
            }
        }

        void SetQuadMirror()
        {
            ReadWebcam.instance.GetMirrorValue(out int mirrorX, out int mirrorY);

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID
#elif UNITY_IOS
            mirrorY*=-1;
#endif
            if (!ReadWebcam.instance.isCameraFront && ReadWebcam.instance.GetAdjustedVideoRotationAngle() % 180 != 0)
            {
                mirrorX *= -1;
                mirrorY *= -1;
            }

            transform.localScale = new Vector3(mirrorX * transform.localScale.x, mirrorY * transform.localScale.y, 1);
        }

        void SetQuadRotation()
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, ReadWebcam.instance.GetAdjustedVideoRotationAngle()));
        }

    }
}