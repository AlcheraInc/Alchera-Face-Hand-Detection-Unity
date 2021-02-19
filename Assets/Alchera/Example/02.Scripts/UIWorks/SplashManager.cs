using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.Android;

public class SplashManager : MonoBehaviour
{
    [SerializeField] FadeController fadeScreen = null;
    [SerializeField] GameObject PermissionDialog = null;

    VideoPlayer vp;
#if UNITY_ANDROID && !UNITY_EDITOR
    bool endReached;
    bool askPermission;
    bool IsPermitted
    {
        get{
            return Permission.HasUserAuthorizedPermission(Permission.Camera) &&
                Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) &&
                Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
        }
    }
#endif
    void Start()
    {
        PermissionDialog.SetActive(false);
        vp = GetComponentInChildren<VideoPlayer>();
        vp.loopPointReached += EndReached;
    }
    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        endReached = true;


        if (!IsPermitted)
            PermissionDialog.SetActive(true);
        else
            GoToDemoUI();
        return;
#endif
        GoToDemoUI();
    }

    void GoToDemoUI()
    {
        fadeScreen.FadeIn(0.1f, () =>
        {
            SceneManager.LoadSceneAsync(1);
        });
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    void Update()
    {
        if (askPermission)
        {
            GetPermissions();
            askPermission = false;
        }
    }
    void OnApplicationFocus(bool focusStatus)
    {
        if (endReached == false || focusStatus == false) return;

        if (Application.platform != RuntimePlatform.Android) return;
        if (!IsPermitted)
            askPermission = true;
        else
            GoToDemoUI();
        
    }

    public void GetPermissions()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            Permission.RequestUserPermission(Permission.Camera);
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            Permission.RequestUserPermission(Permission.ExternalStorageRead);
    }
#endif
}
