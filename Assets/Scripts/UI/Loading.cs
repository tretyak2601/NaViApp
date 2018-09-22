using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

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
        isLoading = true;
        StartCoroutine(Load());
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
            yield return new WaitForSeconds(0.001f);
        }
    }

    private void OnDisable()
    {
        parsing.OnPrefabsCreated -= StopCour;
    }
}