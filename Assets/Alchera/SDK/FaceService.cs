using System;
using UnityEngine;
using UnityEngine.Profiling;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Alchera
{
    public class FaceService : MonoBehaviour, IDetectService
    {
        private class Translator : ITranslator
        {
            public UInt16 count;
            public FaceLib.Context context;
            public Face3DLib.Context3D context3D;
            public FaceData[] storage;
            bool need3D;
            AutoBackgroundQuad quad;

            internal unsafe void Init(int _maxCount, bool _need3D, LevelOf3DProcess _levelOf3DProcess)
            {
                context = default(FaceLib.Context);
                context.detectableSize = 128f;
                context.logStateMode = 0;
                context.maxCount = (UInt16)_maxCount;
                context.InitPath =  Application.persistentDataPath;

                storage = new FaceData[context.maxCount];
                need3D = _need3D;

                quad = FindObjectOfType<AutoBackgroundQuad>();

                FaceLib.Init(ref context);

                if (need3D)
                {
                    context3D.levelOf3DProcess = _levelOf3DProcess;
                    Face3DLib.Init(ref context3D);
                }
            }

            int CheckMirror(ref Vector2 p0, ref Vector2 p16, ref Vector2 p32)
            {
                Vector2 diff = (p32 + p0) / 2.0f - p16;
                Vector2 dir = p0 - p16;

                float v = dir[0] * diff[1] - dir[1] * diff[0];
                if (v < 0) return -1;
                return 1;
            }

            unsafe IEnumerable<T> ITranslator.Fetch<T>(IEnumerable<T> result)
            {
                // Only `FaceData` can be fetched
                if (typeof(T) != typeof(FaceData))
                    throw new NotSupportedException($"Available types: `{typeof(FaceData).FullName}`");

                // this code doesn't implement memory reusage
                Profiler.BeginSample("FaceService.Translator.Fetch");
                // apply fitting for each faces
                if (need3D && ReadWebcam.instance.prepared)
                {
                    Face3DLib.SetCameraInfo(ref context3D, Screen.width, Screen.height, Camera.main.fieldOfView);

                    float[] camMat = new float[6];
                    float v = 1.0f / (float)Math.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) * Screen.height;
                    camMat[0] = v * 0.5f; camMat[1] = 0.0f; camMat[2] = Screen.width * 0.5f;
                    camMat[3] = 0; camMat[4] = v * 0.5f; camMat[5] = Screen.height * 0.5f;

                    float screenRatio = (float)Screen.width / Screen.height;
                    var height = quad.texture.height < 16 ? 1 : quad.texture.height; //divide by 0
                    float adjustment = System.Math.Abs(quad.transform.localScale.y / height);

                    float centerX = quad.texture.width / 2;
                    float centerY = quad.texture.height / 2;
                    if (ReadWebcam.instance.GetAdjustedVideoRotationAngle() % 180 != 0) // width < height
                    {
                        centerX = quad.texture.height / 2;
                        centerY = quad.texture.width / 2;
                    }
                    ReadWebcam.instance.GetMirrorValue(out int mirrorX, out int mirrorY);
                    //to resolve rear flip
                    if (!ReadWebcam.instance.isCameraFront)
                    {
                        mirrorX *= -1;
                    }

                    for (int i = 0; i < count; ++i)
                    {
                        for (int j = 0; j < FaceData.NumLandmark; ++j)
                        {
                            var posX = mirrorX * (storage[i].Landmark[j].x - centerX) * adjustment;
                            var posY = mirrorY * (storage[i].Landmark[j].y - centerY) * adjustment;
                            var posZ = quad.transform.localPosition.z;

                            Vector3 pos = new Vector3(posX, posY, posZ);

                            float rx = pos[0] * camMat[0] + pos[1] * camMat[1] + pos[2] * camMat[2];
                            float ry = pos[0] * camMat[3] + pos[1] * camMat[4] + pos[2] * camMat[5];
                            rx /= pos[2];
                            ry /= pos[2];

                            storage[i].Landmark[j].x = rx;
                            storage[i].Landmark[j].y = ry;
                        }
                        ReadWebcam.instance.mirror3D = CheckMirror(ref storage[i].Landmark[0], ref storage[i].Landmark[16], ref storage[i].Landmark[32]);
                        //apply flip for x direction
                        if (ReadWebcam.instance.mirror3D == -1)
                        {
                            for (int j = 0; j < FaceData.NumLandmark; ++j)
                            {
                                storage[i].Landmark[j].x = Screen.width - storage[i].Landmark[j].x;
                            }
                        }
                        Face3DLib.Process(ref context3D, ref storage[i]);
                    }
                }
                Profiler.EndSample();

                // segment of reserved storage
                return result = new ArraySegment<FaceData>(storage, 0, count)
                    as IEnumerable<FaceData>    // double casting. 
                    as IEnumerable<T>;          // usually this is bad 
            }
            public void Dispose() { }
        }

        public int maxCount;
        public bool need3D;
        public LevelOf3DProcess levelOf3DProcess;

        TaskCompletionSource<ITranslator> promise;
        Translator translator;
        void Start()
        {
            try
            {
                translator = new Translator();
                translator.Init(maxCount, need3D, levelOf3DProcess);
                StartCoroutine(ReadWebcam.instance.AfterInit());
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
        }

        unsafe Task<ITranslator> IDetectService.Detect(ref ImageData image)
        {
            Profiler.BeginSample("FaceService.DetectAsync");

            // too small image
            if (image.WebcamWidth < 40)
            {
                Debug.LogWarning("FaceService.DetectAsync: image is too small");
                goto DetectDone;
            }

            if (promise != null)
                promise.TrySetCanceled();
            // the number of detected faces
            fixed (FaceData* facePtr = translator.storage)
            {
                translator.count = (UInt16)FaceLib.Detect(ref translator.context, ref image, facePtr);
            }

        DetectDone:
            // return result in translator form
            promise = new TaskCompletionSource<ITranslator>();
            promise.SetResult(translator);

            Profiler.EndSample();
            return promise.Task;
        }

        void IDisposable.Dispose()
        {
            Debug.LogWarning("FaceService.Dispose");

            FaceLib.Release(ref translator.context);
            if (need3D)
            {
                Face3DLib.Release(ref translator.context3D);
            }
            if (promise == null)
                return;

            promise.TrySetCanceled();
            promise = null;
        }
    }
}