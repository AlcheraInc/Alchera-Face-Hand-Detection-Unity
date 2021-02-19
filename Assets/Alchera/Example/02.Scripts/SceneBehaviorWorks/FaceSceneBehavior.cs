using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{
    public class FaceSceneBehavior : MonoBehaviour
    {
        ITextureSequence sequence;
        ITextureConverter converter;
        IDetectService detector;

        [SerializeField] GameObject FaceConsumer = null;
        IFaceListConsumer consumer = null;


        async void Start()
        {
            sequence = GetComponent<ITextureSequence>();
            converter = GetComponent<ITextureConverter>();
            detector = GetComponent<IDetectService>();
            consumer = FaceConsumer.GetComponent<IFaceListConsumer>();

            // start a logic loop
            IEnumerable<FaceData> faces = null;

            foreach (Task<Texture> request in sequence.Repeat())
            {
                if (request == null)
                    continue;

                var texture = await request; 
                var image = await converter.Convert(texture);
                var translator = await detector.Detect(ref image);

                faces = translator.Fetch<FaceData>(faces);
                if (faces != null) consumer.Consume(ref image, faces);

                // release holding resources for detection
                translator.Dispose();
            }
        }

        void OnDestroy()
        {
            sequence.Dispose();
            detector.Dispose();
        }

    }
}

