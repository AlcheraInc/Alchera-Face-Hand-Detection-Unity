using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{
    public sealed class ReadWebcamInSequence : ReadWebcam, ITextureSequence
    {
        TaskCompletionSource<Texture> promise;
        WaitForSeconds ws = new WaitForSeconds(0.05f);
        IEnumerable<Task<Texture>> ITextureSequence.Repeat()
        {
            promise = new TaskCompletionSource<Texture>();

            if (current.isPlaying == false)
                current.Play();

            StartCoroutine(SendTextureFrom()); 

            while (true) //TaskCompletionSource 
            {
                if (current != null && current.isPlaying)
                {
                    yield return promise.Task; 
                    promise = new TaskCompletionSource<Texture>();
                }
                else
                    yield return null;
            }
        }

        IEnumerator SendTextureFrom()
        {
            do
            {
                if (promise != null && current.isPlaying) {
                    promise.TrySetResult(current as Texture); //성공하면 texture 가 들어간다.
                }
                //yield return ws;
                yield return null;
            }
            while (true);
        }

        Task<Texture> ITextureSequence.Capture()
        {
            if (current.isPlaying == false) {
                current.Play();
            }
            var source = new TaskCompletionSource<Texture>();
            source.SetResult(this.current as Texture);
            return source.Task;
        }

        public void Swap()
        {
            Debug.LogFormat("Number of Device: {0}", devices.Length);
            if (devices.Length >= 2)
            {
                current.Stop();
                current = (current == rear) ? front : rear;
                current.Play();
                isCameraFront = GetNextCameraFR();
            }

            StartCoroutine(quad.UpdateQuad());
            StartCoroutine(AfterInit());
        }
      
        new void OnDestroy()
        {
            base.OnDestroy();
            (this as IDisposable).Dispose();
        }

        void IDisposable.Dispose()
        {
            if (promise == null)
                return;

            promise.TrySetCanceled();
            promise = null;
        }
    }
}