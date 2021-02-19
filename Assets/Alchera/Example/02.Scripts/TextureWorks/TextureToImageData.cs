using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Alchera
{
    public class TextureToImageData : MonoBehaviour, ITextureConverter
    {
        Color32[] pixelBuffer;

        unsafe UInt64 GetBufferStart(Color32[] pixels)
        {
            fixed (Color32* ptr = &pixels[0])
                return (UInt64)ptr;
        }


        Task<ImageData> ITextureConverter.Convert(Texture texture)
        {
            var source = new TaskCompletionSource<ImageData>();

            if (texture is WebCamTexture)
                StartCoroutine(ConvertWebcamInMainThread(texture as WebCamTexture, source));
            else if (texture is Texture2D)
                throw new ArgumentException(
                    "Cropped Texture2D process not Implemented"
                );
            else
                throw new ArgumentException(
                    "Can't convert given texture type. Available: WebCamTexture, Texture2D"
                );
            return source.Task;
        }
        
        IEnumerator ConvertWebcamInMainThread(WebCamTexture webcam, TaskCompletionSource<ImageData> promise) 
        {
            if (pixelBuffer == null || pixelBuffer.Length != webcam.width * webcam.height)
            {
                pixelBuffer = new Color32[webcam.width * webcam.height];
                print($"WebcamTexture: {webcam.width} {webcam.height}");
            }
            webcam.GetPixels32(pixelBuffer);

            ImageData image = default(ImageData);
            image.Data = GetBufferStart(pixelBuffer);
            image.WebcamWidth = (UInt16)webcam.width;
            image.WebcamHeight = (UInt16)webcam.height;
            image.DetectionWidth = (UInt16)webcam.width;
            image.DetectionHeight = (UInt16)webcam.height;
            image.OffsetX = 0;
            image.OffsetY = 0;

            image.Degree = (UInt16)(ReadWebcam.instance.GetAdjustedVideoRotationAngle()); // 방향에 따라 내부에서 이미지를 다르게 처리한다. LandscapeLeft:0 Protrait:90 LandscapeRight,Editor:180 

            promise.SetResult(image);
            yield break;
        }
    }
}