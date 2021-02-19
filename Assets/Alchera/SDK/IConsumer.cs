using System.Collections.Generic;

namespace Alchera
{
    public interface IConsumer<T>
    {
        void Consume(ref ImageData image, T obj);
    }
    public interface IHandListConsumer : IConsumer<IEnumerable<HandData>> {}
    public interface IFaceListConsumer : IConsumer<IEnumerable<FaceData>> {}
}