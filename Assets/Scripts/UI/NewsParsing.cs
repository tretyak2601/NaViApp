using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Text;
using System.Net;
using System;
using UniRx;

public class NewsParsing : MonoBehaviour, IPage {

    [SerializeField] GameObject tablePrefab;
    [SerializeField] GameObject content;
    [SerializeField] Sprite[] disciplines;

    private const string NewsHTML = "http://navi.gg/read/tree/2-news";
    private const string listTag = "list list-news";

    private byte[] bHtml;
    private bool newsAvailable = false;
    private string html;

    private string temp;
    private string[] tegs;
    private string[][] tableTags;
    private int needTag;

    public ArrayList objectList = new ArrayList();
    private WebClient webSite;
    private GameObject[] prefabs;
    private NewsStruct prefabContent;

    void Start () {
        webSite = new WebClient();
        UpdatePage(NewsHTML);
	}

    public void UpdatePage(string page)
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
                    CountLast();

                    prefabs = new GameObject[objectList.Count];
                    tableTags = new string[objectList.Count][];

                    CreatePrefabs();
                }
            });

        ObservableWWW.GetAndGetBytes(page).Subscribe(observer);
    }

    public void CountLast()
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
        for (int i = 0; i < objectList.Count; i++)
        {
            tableTags[i] = Regex.Split((string)objectList[i], @"(?<=[>])");
            prefabs[i] = Instantiate(tablePrefab, tablePrefab.transform.localPosition, Quaternion.identity, content.transform);
        }

        for (int i = 0; i < objectList.Count; i++)
        {
            prefabContent = prefabs[i].GetComponent<NewsStruct>();

            for (int j = 0; j < tableTags[i].Length; j++)
            {
                if (tableTags[i][j].Contains("item-news-bg")) // Main Image
                {
                    string[] temp = Regex.Split(tableTags[i][j], "http");
                    string link = "http" + temp[1];
                    link = link.Substring(0, link.Length - 4);

                    byte[] image = webSite.DownloadData(link);
                    Texture2D texture = new Texture2D(350, 300);
                    texture.LoadImage(image);
                    Sprite mySprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                    prefabContent.mainImage.sprite = mySprite;

                }

                if (tableTags[i][j].Contains("icon-discipline")) // Discipline
                {
                    string[] temp = Regex.Split(tableTags[i][j], "data-game=\"");
                    string dis = temp[1].Substring(0, temp[1].Length - 2);

                    ChangeSprite(prefabContent.gameImage, dis);
                }

                if (tableTags[i][j].Contains("header-item-news")) // Header Text
                {
                    string temp = tableTags[i][j+1];
                    temp = temp.Substring(0, temp.Length - 7);

                    prefabContent.headerText.text = temp;
                }

                if (tableTags[i][j].Contains("description-news")) // Description
                {
                    string temp = tableTags[i][j + 1];
                    temp = temp.Substring(0, temp.Length - 7);

                    prefabContent.contentText.text = temp;
                }

                if (tableTags[i][j].Contains("data-news")) // Time
                {
                    string temp = tableTags[i][j + 1];
                    string time = temp.Substring(0, temp.Length - 7);

                    prefabContent.timeText.text = time;
                }
            }
        }
    }

    public void ChangeSprite(Image img, string cname)
    {
        switch (cname)
        {
            case "d2":
                img.sprite = disciplines[0];
                break;
            case "csgo":
                img.sprite = disciplines[1];
                break;
            case "lol":
                img.sprite = disciplines[3];
                break;
            default:
                img.sprite = disciplines[0];
                break;
        }
    }


    public void Clear()
    {

    }
}
