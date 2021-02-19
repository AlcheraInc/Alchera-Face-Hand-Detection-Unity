using UnityEngine;

public class LoadingImage : MonoBehaviour
{
    [SerializeField] RectTransform topCover = null;
    [SerializeField] RectTransform bottomCover = null;

    void Update()
    {
        var circleHeight = Screen.width * 0.4f;
        var coverHeight = (Screen.height - circleHeight) / 2;

        topCover.sizeDelta = new Vector2(0, coverHeight);
        topCover.anchoredPosition = new Vector2(0, Screen.height - coverHeight / 2);
        bottomCover.sizeDelta = new Vector2(0, coverHeight);
        bottomCover.anchoredPosition = new Vector2(0, coverHeight / 2);
    }
}
