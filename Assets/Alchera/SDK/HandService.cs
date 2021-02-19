using System;
using UnityEngine;
using UnityEngine.Profiling;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Alchera
{
    public class HandService : MonoBehaviour, IDetectService
    {
        private class Translator : ITranslator
        {
            public UInt16 count;
            public HandLib.Context context;
            public Hand3DLib.Context3D context3D;
            public HandData[] storage;
            bool need3D;

            internal unsafe void Init(int _maxCount, bool _need3D)
            {
                context = default(HandLib.Context);
                context.minHandSize = 60;
                context.maxCount = (UInt16)_maxCount;

                storage = new HandData[context.maxCount];
                need3D = _need3D;

                string str = Application.persistentDataPath;
                fixed (byte* path = context.modelPath.folderPath)
                {
                    for (int i = 0; i < str.Length; i++)
                    {
                        path[i] = (byte)str[i];
                    }
                }

                HandLib.Init(ref context);

                if (need3D)
                {
                    Hand3DLib.Init(ref context3D);
                }
            }

            unsafe IEnumerable<T> ITranslator.Fetch<T>(IEnumerable<T> result)
            {
                // Only `HandData` can be fetched
                if (typeof(T) != typeof(HandData))
                    throw new NotSupportedException($"Available types: `{typeof(HandData).FullName}`");

                // this code doesn't implement memory reusage
                Profiler.BeginSample("HandTranslator.Fetch");
                // apply 3D fitting for each hands if needed
                if (need3D && ReadWebcam.instance.prepared)
                {
                    
                    Hand3DLib.SetCameraInfo(ref context3D, (UInt16)ReadWebcam.instance.current.requestedWidth, (UInt16)ReadWebcam.instance.current.requestedHeight, (UInt16)Screen.width, (UInt16)Screen.height);
                    fixed (HandData* hands = storage)
                    {
                        for (var i = 0; i < count; ++i)
                            Hand3DLib.Process(ref context3D, ref storage[i]);
                        for (var p = 0; p < HandData.NumPoints; ++p)
                            hands[0].Points[p] *= 10;
                    }
                }
                Profiler.EndSample();

                // segment of reserved storage
                return result = new ArraySegment<HandData>(storage, 0, count)
                    as IEnumerable<HandData>    // double casting. 
                    as IEnumerable<T>;         // usually this is bad 
            }
            public void Dispose() { }
        }

        public int maxCount;
        public bool need3D;

        TaskCompletionSource<ITranslator> promise;
        Translator translator;

        unsafe void Start()
        {
            try
            {
                translator = new Translator();
                translator.Init(maxCount, need3D);
                StartCoroutine(ReadWebcam.instance.AfterInit());
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.ToString());
            }

        }

        unsafe Task<ITranslator> IDetectService.Detect(ref ImageData image)
        {
            Profiler.BeginSample("HandService.DetectAsync");

            // too small image
            if (image.WebcamWidth < 40)
            {
                Debug.LogWarning("HandService.DetectAsync: image is too small");
                goto DetectDone;
            }
            if (promise != null)
                promise.TrySetCanceled();
            // the number of detected hands
            fixed (HandData* handPtr = translator.storage)
            {
                translator.count = (UInt16)HandLib.Detect(ref translator.context, ref image, handPtr);
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
            Debug.LogWarning("HandService.Dispose");

            HandLib.Release(ref translator.context);
            if (need3D)
            {
                Hand3DLib.Release(ref translator.context3D);
            }
            if (promise == null)
                return;

            promise.TrySetCanceled();
            promise = null;
        }
    }

}