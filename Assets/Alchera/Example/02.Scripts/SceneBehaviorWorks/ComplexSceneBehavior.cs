using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{
    public class ComplexSceneBehavior : MonoBehaviour
    {
        ITextureSequence sequence;
        ITextureConverter converter;
        IDetectService faceService;
        IDetectService handService;

        [SerializeField] GameObject FaceConsumer = null;
        [SerializeField] GameObject HandConsumer = null;
        IFaceListConsumer faceConsumer;
        IHandListConsumer handConsumer;

       
        async void Start()
        {
            sequence = GetComponent<ITextureSequence>();
            converter = GetComponent<ITextureConverter>();

            var detectors = GetComponents<IDetectService>();
            Debug.Log("Number of Detectors: " + detectors.Length);
            faceService = detectors[0];
            handService = detectors[1];

            faceConsumer = FaceConsumer.GetComponent<IFaceListConsumer>();
            handConsumer = HandConsumer.GetComponent<IHandListConsumer>();

            // start a logic loop
            IEnumerable<FaceData> faces = null;
            IEnumerable<HandData> hands = null;

            foreach (Task<Texture> request in sequence.Repeat())
            {
                if (request == null)
                    continue;

                var texture = await request; 

                var image = await converter.Convert(texture);
                var faceTranslator = await faceService.Detect(ref image);
                faces = faceTranslator.Fetch<FaceData>(faces);
                if (faces != null) faceConsumer.Consume(ref image, faces);
                //release holding resources for detection
                faceTranslator.Dispose();

                image = await converter.Convert(texture);
                var handTranslator = await handService.Detect(ref image);
                hands = handTranslator.Fetch<HandData>(hands);
                if (faces != null) handConsumer.Consume(ref image, hands);
                //release holding resources for detection
                handTranslator.Dispose();
            }
        }

        void OnDestroy()
        {
            sequence.Dispose();
            faceService.Dispose();
            handService.Dispose();
        }
    }

}
