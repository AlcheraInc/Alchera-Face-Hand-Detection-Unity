using UnityEngine;

namespace Alchera
{
    public interface IFace
    {
        void UseFaceData(ref ImageData image, ref FaceData face);
    }

    public interface IFaceFactory
    {
        IFace Create(out GameObject obj);
    }
}
