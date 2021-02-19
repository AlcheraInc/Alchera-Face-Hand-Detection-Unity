using UnityEngine;

namespace Alchera
{
    public interface IHand
    {
        void UseHandData(ref ImageData image, ref HandData hand, bool isLeft);
    }

    public interface IHandFactory
    {
        IHand Create(out GameObject obj, bool isLeft);
    }
}
