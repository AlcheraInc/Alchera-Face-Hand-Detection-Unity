using UnityEngine;

public class FaceMaskLine : MonoBehaviour
{
    void Update()
    {
        float ratio;
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            ratio = Input.mousePosition.x / Screen.width;
#else
        if (Input.touchCount > 0)
        {
            ratio = Input.GetTouch(0).position.x / Screen.width;
#endif
            GetComponent<RectTransform>().anchorMin = new Vector2(ratio, 0.04f);
            GetComponent<RectTransform>().anchorMax = new Vector2(ratio, 0.96f);
        }
    }
}
