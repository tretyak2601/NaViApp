using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UniRx;
using System;

public class FullNewsPage : MonoBehaviour, IPage {

    [SerializeField] public Button backButton;
    [SerializeField] public Image mainImage;
    [SerializeField] public Text mainText;

    private WebClient web;
    private GameObject[] prefabs;
    public ArrayList objectList = new ArrayList();

    private byte[] bHtml;

    private string html;
    private string[] tegs;
    private string[][] tableTags;

    private const string topTag = "header-description";
    private const string contentTag = "content";

    private int needTagContent;
    private int needTagTop;
    private bool newsAvailable = false;

    public static event Action OnLoadingStart;
    public static event Action OnLoadingEnded;

    public void UpdatePage(string page)
    {
        web = new WebClient();

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
                            if (tegs[i].Contains(contentTag))
                            {
                                needTagContent = i;
                                newsAvailable = true;
                            }

                            if (tegs[i].Contains(topTag))
                            {
                                byte[] image = new byte[0];
                                string[] temp = Regex.Split(tegs[i + 1], "src=\"");
                                string link = temp[1].Substring(0, temp[1].Length - 10);

                                if (link.Length > 10)
                                {
                                    image = web.DownloadData(link);
                                    Texture2D texture = new Texture2D(1080, 360);
                                    texture.LoadImage(image);
                                    Sprite mySprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
                                    mainImage.sprite = mySprite;
                                }
                            }
                        }

                        if (newsAvailable)
                        {/*
                            CountLast();

                            prefabs = new GameObject[objectList.Count];
                            tableTags = new string[objectList.Count][];

                            OnLoadingEnded();*/
                        }
                    });

        ObservableWWW.GetAndGetBytes(page).Subscribe(observer);
    }

    public void CountLast()
    {
        string temp = string.Empty;

        for (int i = needTagContent + 1; i < tegs.Length; i++)
        {
            if (tegs[i].Contains("</div"))
                return;

            if (tegs[i].Contains("<p"))
            {
                for (int j = i; j < tegs.Length; j++)
                {
                    temp += tegs[j];

                    if (tegs[j].Contains("</p"))
                    {
                        objectList.Add(temp);
                        temp = string.Empty;
                        break;
                    }
                }
            }
        }
    }

    public void CreatePrefabs()
    {
        throw new System.NotImplementedException();
    }

    public void ChangeSprite(Image img, string cname)
    {

    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }
}
