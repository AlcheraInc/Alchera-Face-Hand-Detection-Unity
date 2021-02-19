using UnityEngine;

public class UISingleton : MonoBehaviour
{
    [SerializeField] UIManager UIprefab = null;
 
    void Awake()
    {
        if (!GameObject.Find(UIprefab.name))
        {
            var prefab = Instantiate(UIprefab);
            prefab.name = UIprefab.name;
        }
    }
}
