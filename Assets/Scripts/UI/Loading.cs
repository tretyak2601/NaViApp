using System.Collections;
using UnityEngine;

public class Loading : MonoBehaviour {
    
    [SerializeField] GameObject loading;
    [SerializeField] ParseFromHTML parsing;
    [SerializeField] GameObject background;
    [SerializeField] GameObject secondBackground;

    public static bool isLoading = false;

    private GameObject loadingInit;

    void Start () {
        parsing.OnPrefabsCreated += StopCour;
	}

    public void StartCour()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            isLoading = true;
            StartCoroutine(Load());
        }
    }

    public void StopCour()
    {
        StopAllCoroutines();

        if (loadingInit != null)
            Destroy(loadingInit);
    }

    IEnumerator Load()
    {
        if (background.activeSelf)
            loadingInit = Instantiate(loading, background.transform);
        else
            loadingInit = Instantiate(loading, secondBackground.transform);

        while (true)
        {
            loadingInit.transform.Rotate(Vector3.back * 15f);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDisable()
    {
        parsing.OnPrefabsCreated -= StopCour;
    }
}