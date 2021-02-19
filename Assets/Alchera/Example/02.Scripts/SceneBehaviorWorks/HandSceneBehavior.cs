using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{
    public class HandSceneBehavior : MonoBehaviour
    {
        ITextureSequence sequence;
        ITextureConverter converter;
        IDetectService detector;

        [SerializeField] GameObject HandConsumer = null;
        IHandListConsumer consumer;

        async void Start()
        {
            sequence = GetComponent<ITextureSequence>();
            converter = GetComponent<ITextureConverter>();
            detector = GetComponent<IDetectService>();
            consumer = HandConsumer.GetComponent<IHandListConsumer>();

            // start a logic loop
            IEnumerable<HandData> hands = null;

            foreach (Task<Texture> request in sequence.Repeat())
            {
                if (request == null)
                    continue;

                var texture = await request; 
                var image = await converter.Convert(texture);
                var translator = await detector.Detect(ref image);

                hands = translator.Fetch<HandData>(hands);
                if (hands != null) consumer.Consume(ref image, hands);

                //release holding resources for detection
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