using System.Threading.Tasks;
using UnityEngine;

namespace Alchera
{
    public class CaptureSceneBehavior : MonoBehaviour
    {
        ITextureSequence sequence;
        ITextureConverter converter;
        IConsumer<Texture> consumer;

        async void Start()
        {
            sequence = GetComponent<ITextureSequence>();
            converter = GetComponent<ITextureConverter>();
            consumer = GetComponent<IConsumer<Texture>>();

            foreach (Task<Texture> request in sequence.Repeat())
            {
                if (request == null)
                    continue;
                var texture = await request;
                var image = await converter.Convert(texture);
                if (consumer != null) consumer.Consume(ref image, texture);
            }
        }
    }
}
