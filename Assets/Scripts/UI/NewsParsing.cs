using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Text;
using System.Net;
using System;
using UniRx;

public class NewsParsing : MonoBehaviour {

    private const string NewsHTML = "http://navi.gg/read/tree/2-news";
    private const string listTag = "list list-news";

    private byte[] bHtml;
    private bool newsAvailable = false;
    private string html;

    private string temp;
    private string[] tegs;
    private string[][] tableTags;
    private int needTag;

    private ArrayList divs = new ArrayList();
    private WebClient webSite;
    private GameObject[] prefabs;
    private GamePrefab prefabContent;

    void Start () {
        webSite = new WebClient();
        UpdatePage(NewsHTML);
	}

    private void UpdatePage(string page)
    {
        var observer = Observer.Create<byte[]>(
            x =>
            {
                bHtml = x;
            },
            ex => Debug.Log("Error"),
            () =>
            {
                html = Encoding.UTF8.GetString(bHtml);
                tegs = Regex.Split(html, @"(?<=[>])");

                for (int i = 0; i < tegs.Length; i++)
                {
                    if (tegs[i].Contains(listTag))
                    {
                        needTag = i;
                        newsAvailable = true;
                    }
                }

                if (newsAvailable)
                {
                    CountLastNews();

                    prefabs = new GameObject[divs.Count];
                    tableTags = new string[divs.Count][];

                    //CreatePrefabs();

                    foreach (string q in divs)
                        print(q);
                }
            });

        ObservableWWW.GetAndGetBytes(page).Subscribe(observer);
    }

    private void CountLastNews()
    {
        for (int i = needTag + 1; i < tegs.Length; i++)
        {
            if (tegs[i].Contains("</ul"))
                return;

            if (tegs[i].Contains("<li"))
            {
                for (int j = i; j < tegs.Length; j++)
                {
                    temp += tegs[j];

                    if (tegs[j].Contains("</li"))
                    {
                        divs.Add(temp);
                        temp = string.Empty;
                        break;
                    }
                }
            }
        }
    }
}
