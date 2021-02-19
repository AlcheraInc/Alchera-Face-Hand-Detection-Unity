using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alchera
{
    public class FaceStickerChange : MonoBehaviour
    {
        [SerializeField] Draw3DSticker draw3DSticker = null;
        int currStickerIndex = 0;

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                currStickerIndex += 1;
#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began)
                {
                    currStickerIndex += 1;
                }
#endif
                if (currStickerIndex >= draw3DSticker.Prefabs.Count)
                {
                    currStickerIndex = 0;
                }
                draw3DSticker.SetStickerByIndex(currStickerIndex);
            }
        }
    }
}
