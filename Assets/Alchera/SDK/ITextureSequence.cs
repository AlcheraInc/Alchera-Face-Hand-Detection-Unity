using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{

    public interface ITextureSequence : IDisposable
    {
        Task<Texture> Capture();
        IEnumerable<Task<Texture>> Repeat();
    }

}