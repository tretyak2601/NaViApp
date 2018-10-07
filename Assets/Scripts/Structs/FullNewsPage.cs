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
    [SerializeField] private GameObject content;

    private WebClient web;
    private GameObject[] prefabs;
    public ArrayList objectList = new ArrayList();

    private byte[] bHtml;

    private string html;
    private string[] tegs;
    private string[][] tableTags;

    private const string topTag = "header-description";
    private const string contentTag = "<div class=\"content\">";

    private int needTagContent;
    private int needTagTop;
    private bool newsAvailable = false;

    public static event Action OnLoadingStart;
    public static event Action OnLoadingEnded;

    [SerializeField] GameObject textPrefab;
    [SerializeField] GameObject flagPrefab;
    [SerializeField] GameObject imagePrefab;

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
                        {
                            CountLast();
                            //prefabs = new GameObject[objectList.Count];
                            //tableTags = new string[objectList.Count][];

                            //OnLoadingEnded();
                        }
                    });

        ObservableWWW.GetAndGetBytes(page).Subscribe(observer);
    }

    bool isCenter = false;

    public void CountLast()
    {
        string temp = string.Empty;

        for (int i = needTagContent + 1; i < tegs.Length; i++)
        {
            if (isCenter)
            {
                if (tegs[i].Contains("</center>"))
                    isCenter = false;
                continue;
            }

            if (tegs[i].Contains("<center>"))
            {
                isCenter = true;
                continue;
            }

            if (tegs[i].Contains("</div"))
                return;

            if (tegs[i].Contains("<p"))
            {
                for (int j = i; j < tegs.Length; j++)
                {
                    temp += tegs[j];

                    if (tegs[j].Contains("</p"))
                    {
                        Obr(temp);
                        objectList.Add(temp);
                        temp = string.Empty;
                        break;
                    }
                }
            }
            else if (tegs[i].Contains("<h2"))
            {
                for (int j = i; j < tegs.Length; j++)
                {
                    temp += tegs[j];

                    if (tegs[j].Contains("</h2"))
                    {
                        objectList.Add(temp);
                        temp = string.Empty;
                        break;
                    }
                }
            }
            else if (tegs[i].Contains("<h4"))
            {
                for (int j = i; j < tegs.Length; j++)
                {
                    temp += tegs[j];

                    if (tegs[j].Contains("</h4 "))
                    {
                        objectList.Add(temp);
                        temp = string.Empty;
                        break;
                    }
                }
            }
        }
    }

    public void Obr(string s)
    {
        string[] temp = Regex.Split(s, @"(?<=[>])");

        foreach (var k in temp)
        {
            string[] str = Regex.Split(k, "<");

            foreach (var element in str)
            {
                if (element.Trim() != string.Empty && !element.Contains(">")) //Standart sentences.
                {
                    if (content.transform.childCount != 0 && content.transform.GetChild(content.transform.childCount - 1).GetComponent<Text>())
                        content.transform.GetChild(content.transform.childCount - 1).GetComponent<Text>().text += element;
                    else
                    {
                        GameObject w = Instantiate(textPrefab, content.transform);
                        w.GetComponent<Text>().text = element;
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
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }
}
