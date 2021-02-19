using UnityEngine;
using System.Collections;

namespace Alchera
{
    public class ReadWebcam : MonoBehaviour
    {
        public static ReadWebcam instance = null;

        protected WebCamDevice[] devices;
        [HideInInspector] public WebCamTexture current, front, rear;
        [HideInInspector] public bool isCameraFront;
        bool[] CamerasFR;
        int FRIndex = 0;
        [SerializeField] protected AutoBackgroundQuad quad;

        public int mirror3D { get; set; }
        public bool prepared { get; set; }

        public IEnumerator AfterInit()
        {
            prepared = false;
            yield return new WaitForSeconds(1.0f);
            prepared = true;
        }

        void Awake()
        {
            instance = this;

            //  Layer가 SkipRendering으로 설정된 FaceObjPrefab은 렌더링되지 않는다.
            int SkipRenderingLayerBit = LayerMask.NameToLayer("Skip Rendering");
            int isCameraSkipRenderingOn = (Camera.main.cullingMask & (1 << SkipRenderingLayerBit)) >> SkipRenderingLayerBit;
            if (isCameraSkipRenderingOn == 1)
            {
                Camera.main.cullingMask ^= 1 << LayerMask.NameToLayer("Skip Rendering");
            }
        }

        void Start()
        {
            var RequestWidth = 1280;
            var RequestHeight = 720;
            var RequestedFPS = 120; //카메라가 들어오는 프레임 조절. 
            prepared = false;
            devices = WebCamTexture.devices;
            CamerasFR = new bool[Mathf.Min(devices.Length, 2)];
            int i=0;
            foreach (var device in devices)
            {
                print($"{device.name} {device.isFrontFacing}");
                if (front != null && rear != null) break; // 멀티카메라인 경우 일반 카메라가 첫번째 인덱스에 있다고 생각.

#if UNITY_EDITOR || UNITY_STANDALONE
                if (front == null){ //에디터 카메라는 front 이다.
                    front = new WebCamTexture(device.name, RequestWidth, RequestHeight, RequestedFPS);
                    CamerasFR[i] = true;
                    FRIndex = i;
                    isCameraFront = CamerasFR[i];
                }
                else { 
                    rear = new WebCamTexture(device.name, RequestWidth, RequestHeight, RequestedFPS);
                    CamerasFR[i] = true;
                }
#else
                if (device.isFrontFacing) {
                    front = new WebCamTexture(device.name, RequestWidth, RequestHeight, RequestedFPS);
                    CamerasFR[i] = true;
                    FRIndex = i;
                    isCameraFront = CamerasFR[i];
                }
                else { 
                    rear = new WebCamTexture(device.name, RequestWidth, RequestHeight, RequestedFPS);
                    CamerasFR[i] = false;
                }
#endif
                i++;
            }
            current = (front == null) ? rear : front; //전면이 기본값.
            print($"current Camera : {current.deviceName}");
        }
        protected bool GetNextCameraFR()
        {
            return CamerasFR[(++FRIndex) % 2]; 
        }
        public void GetMirrorValue(out int mirrorX, out int mirrorY)
        {
            mirrorX = -1;
            mirrorY = -1;
            if (isCameraFront)
                mirrorX *= -1;
        }
        public int GetAdjustedVideoRotationAngle()
        {
            return (current.videoRotationAngle + 180) % 360;
        }
        protected void OnDestroy()
        {
            Debug.LogWarning("ReadWebcam.OnDestroy");
            current.Stop();
            if (front != null && front.isPlaying)
                front.Stop();
            if (rear != null && rear.isPlaying)
                rear.Stop();
        }
    }
}
