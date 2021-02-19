using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Alchera
{
    public interface ITranslator : IDisposable
    {
        IEnumerable<T> Fetch<T>(IEnumerable<T> reuse = null) where T : struct;
    }

    public interface IDetectService : IDisposable
    {
        Task<ITranslator> Detect(ref ImageData image);
    }


}