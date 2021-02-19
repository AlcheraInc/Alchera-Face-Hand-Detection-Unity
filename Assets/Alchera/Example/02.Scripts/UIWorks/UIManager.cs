using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Alchera;
public class UIManager : MonoBehaviour
{
    [SerializeField] FadeController fadeScreen = null;
    [Space]
    [SerializeField] CanvasGroup mainUI = null;
    [SerializeField] CanvasGroup inGameUI = null;
    [Space]
    [SerializeField] RectTransform footerPanel = null;
    [SerializeField] RectTransform titlePanel = null;
    [SerializeField] RectTransform itemPanel = null;
	[SerializeField] RectTransform itemBGPanel = null;
    [Space]
    [SerializeField] Button[] sceneButtons = null;
    [Space]
    [SerializeField] Button swapButton = null;
    [SerializeField] Button backButton = null;
    [Space]
    [SerializeField] RectTransform loadingImage = null;
    [SerializeField] RectTransform swapImage = null;
    [SerializeField] float animSpeed = 0.15f;
    [SerializeField] float alphaSpeed = 0.1f;

    AsyncOperation operater;
    CanvasGroup loadingGroup;
    CanvasGroup swapGroup;

    bool isMainStatus = true;

	int loadingMoveState;
	int loadingAlphaState;
	int swapMoveState;

	void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        DontDestroyOnLoad(this);

        fadeScreen.gameObject.SetActive(true);
        fadeScreen.FadeOut(0.4f);

        inGameUI.alpha = 0;
        inGameUI.interactable = false;
        loadingGroup = loadingImage.GetComponent<CanvasGroup>();
        swapGroup = swapImage.GetComponent<CanvasGroup>();
        sceneButtons[0].onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(ShowScene("ComplexScene")); });
        sceneButtons[1].onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(ShowScene("Hand2DSkeleton")); });
        sceneButtons[2].onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(ShowScene("Face2Dfacemark")); });
        sceneButtons[3].onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(ShowScene("Face3DSticker")); });
        sceneButtons[4].onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(ShowScene("Face3DMask")); });
        backButton.onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(ReturnScene()); });
		swapButton.onClick.AddListener(() => { StopAllCoroutines(); StartCoroutine(ShowSwapCover()); });
	}
    private void FixedUpdate()
    {
        if (GetComponent<Canvas>().worldCamera == null) 
            GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		MoveLoadingPanelIfNeed();
		MoveSwapPanelIfNeed();
		if (isMainStatus == false)
            return;
        if (Screen.width < Screen.height) //portrait
        {
            footerPanel.anchorMin = new Vector2(0, 0);
            footerPanel.anchorMax = new Vector2(1, 0.1355f);

            titlePanel.anchorMin = new Vector2(0, 0.814f);
            titlePanel.anchorMax = new Vector2(1, 1);

            itemPanel.anchorMin = new Vector2(0, 0.1355f);
            itemPanel.anchorMax = new Vector2(1, 0.814f);

			itemBGPanel.anchorMin = new Vector2(0, 0.1355f);
			itemBGPanel.anchorMax = new Vector2(1, 0.814f);
		}
        else //landscape
        {
            footerPanel.anchorMin = new Vector2(0, 0);
            footerPanel.anchorMax = new Vector2(0.45f, 0.3f);

            titlePanel.anchorMin = new Vector2(0, 0.3f);
            titlePanel.anchorMax = new Vector2(0.45f, 1);

            itemPanel.anchorMin = new Vector2(0.45f, 0);
            itemPanel.anchorMax = new Vector2(1, 1f);
			itemBGPanel.anchorMin = new Vector2(0.45f, 0);
			itemBGPanel.anchorMax = new Vector2(1, 1f);
		}
    }
	void MoveLoadingPanelIfNeed()
	{
		if (loadingMoveState == -1) //왼쪽으로 이동
		{
			var minX = Mathf.Lerp(loadingImage.anchorMin.x, 0, animSpeed);
			var maxX = Mathf.Lerp(loadingImage.anchorMax.x, 1, animSpeed);
			loadingImage.anchorMin = new Vector2(minX, loadingImage.anchorMin.y);
			loadingImage.anchorMax = new Vector2(maxX, loadingImage.anchorMax.y);
			if (!(loadingImage.anchorMin.x > 0.001f && loadingImage.anchorMax.x > 1.001f))
				loadingMoveState = 0;
		}
		else if (loadingMoveState == 1) //왼쪽으로 이동
		{
			var minX = Mathf.Lerp(loadingImage.anchorMin.x, 1, animSpeed);
			var maxX = Mathf.Lerp(loadingImage.anchorMax.x, 2, animSpeed);
			loadingImage.anchorMin = new Vector2(minX, loadingImage.anchorMin.y);
			loadingImage.anchorMax = new Vector2(maxX, loadingImage.anchorMax.y);
			if (!(loadingImage.anchorMin.x < 0.999f && loadingImage.anchorMax.x < 1.999f))
				loadingMoveState = 0;
		}
		if (loadingAlphaState == -1) //투명해지는중
		{
            loadingGroup.alpha = Mathf.Lerp(loadingGroup.alpha, 0, alphaSpeed);
			if (loadingGroup.alpha <= 0.001f)
				loadingAlphaState = 0;
		}
		else if (loadingAlphaState == 1) //불투명해지는
		{
            loadingGroup.alpha = Mathf.Lerp(loadingGroup.alpha, 1, alphaSpeed);
			if (loadingGroup.alpha >= 0.999f)
				loadingAlphaState = 0;
		}
	}
	void MoveSwapPanelIfNeed()
	{
		if (swapMoveState == -1) //왼쪽으로 이동
		{
			var minY = Mathf.Lerp(swapImage.anchorMin.y, -0.002f, animSpeed);
			var maxY = Mathf.Lerp(swapImage.anchorMax.y, 1, animSpeed);
			swapImage.anchorMin = new Vector2(swapImage.anchorMin.x, minY);
			swapImage.anchorMax = new Vector2(swapImage.anchorMax.x, maxY);
			if (!(swapImage.anchorMin.y > -0.002f && swapImage.anchorMax.y > 1.001f))
				swapMoveState = 0;
		}
		else if (swapMoveState == 1) //오른쪽으로 이동
		{
			var minY = Mathf.Lerp(swapImage.anchorMin.y, 1, animSpeed);
			var maxY = Mathf.Lerp(swapImage.anchorMax.y, 2, animSpeed);
			swapImage.anchorMin = new Vector2(swapImage.anchorMin.x, minY);
			swapImage.anchorMax = new Vector2(swapImage.anchorMax.x, maxY);
            if (!(swapImage.anchorMin.y < 0.999f && swapImage.anchorMax.y < 1.999f))
				swapMoveState = 0;
		}
	}
	IEnumerator ShowSwapCover()
    {
        swapImage.anchorMin = new Vector2(swapImage.anchorMin.x, 1);
        swapImage.anchorMax = new Vector2(swapImage.anchorMax.x, 2);
        swapGroup.blocksRaycasts = true;

		swapMoveState = -1;
		while (swapMoveState == -1)
            yield return null;

        swapImage.anchorMin = new Vector2(swapImage.anchorMin.x, 0);
        swapImage.anchorMax = new Vector2(swapImage.anchorMax.x, 1);

		FindObjectOfType<ReadWebcamInSequence>().Swap();
		yield return new WaitForSeconds(0.6f);

		swapMoveState = 1;
		while (swapMoveState == 1)
			yield return null;
		
        swapImage.anchorMin = new Vector2(swapImage.anchorMin.x, 1);
        swapImage.anchorMax = new Vector2(swapImage.anchorMax.x, 2);

        swapGroup.blocksRaycasts = false;
    }
    IEnumerator ShowScene(string sceneName)
    {
		isMainStatus = false;
        operater = SceneManager.LoadSceneAsync(sceneName);
        operater.allowSceneActivation = false;

        loadingGroup.blocksRaycasts = true;

		//로딩창 슬라이드
		loadingMoveState = -1;
		while (loadingMoveState == -1)
            yield return null;
       
        loadingImage.anchorMin = new Vector2(0, loadingImage.anchorMin.y);
        loadingImage.anchorMax = new Vector2(1, loadingImage.anchorMax.y);
        operater.allowSceneActivation = true;
        mainUI.alpha = 0;
        mainUI.interactable = false;
        mainUI.blocksRaycasts = false;
        inGameUI.alpha = 1;
        inGameUI.interactable = true;
        inGameUI.blocksRaycasts = true;

        yield return new WaitForSeconds(0.6f);

		//로딩창 투명
		loadingAlphaState = -1;
		while (loadingAlphaState == -1)
            yield return null;

        loadingGroup.alpha = 0;
        loadingGroup.blocksRaycasts = false;

    }
    IEnumerator ReturnScene()
    {
        operater = SceneManager.LoadSceneAsync("DemoUI");
        operater.allowSceneActivation = false;

        loadingGroup.blocksRaycasts = true;

		//로딩창 불투명
		loadingAlphaState = 1;
		while (loadingAlphaState == 1)
            yield return null;

        loadingGroup.alpha = 1;
        operater.allowSceneActivation = true;
        isMainStatus = true;
        mainUI.alpha = 1;
        mainUI.interactable = true;
        mainUI.blocksRaycasts = true;
        inGameUI.alpha = 0;
        inGameUI.interactable = false;
        inGameUI.blocksRaycasts = false;

		//로딩창 슬라이드
		loadingMoveState = 1;
		while (loadingMoveState == 1)
            yield return null;
        
        loadingImage.anchorMin = new Vector2(1, loadingImage.anchorMin.y);
        loadingImage.anchorMax = new Vector2(2, loadingImage.anchorMax.y);
        loadingGroup.blocksRaycasts = false;
    }
    
}
