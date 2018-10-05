using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UniRx;

public class FirstDownload : MonoBehaviour {

    [SerializeField] GameObject downloading;
    [SerializeField] ParseFromHTML parsing;
    [SerializeField] NewsParsing news;
    [SerializeField] Streams streams;

    public static bool isFirstDownload = true;
    private static List<int> menuList = new List<int>();

    void Start()
    {
        ParseFromHTML.OnPrefabsCreated += EnableScreen;
        NewsParsing.OnLoadingEnded += EnableScreen;
        Streams.OnLoadingEnded += EnableScreen;
    }

    public void EnableScreen()
    {
        menuList.Add(1);

        if (menuList.Count == 3)
            downloading.SetActive(false);
    }

    private void OnDisable()
    {
        ParseFromHTML.OnPrefabsCreated -= EnableScreen;
        NewsParsing.OnLoadingEnded -= EnableScreen;
        Streams.OnLoadingEnded -= EnableScreen;
    }
}
