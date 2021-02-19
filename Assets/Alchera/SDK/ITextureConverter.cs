using System.Threading.Tasks;
using UnityEngine;

namespace Alchera
{

    public interface ITextureConverter
    {
        Task<ImageData> Convert(Texture texture);
    }
}