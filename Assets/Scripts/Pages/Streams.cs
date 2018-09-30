using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Net;
using UniRx;
using System.Text;

public class Streams : MonoBehaviour, IPage
{
    [SerializeField] GameObject tablePrefab;
    [SerializeField] GameObject content;
    [SerializeField] Sprite[] disciplines;
    [SerializeField] Toggle videoToggle;
    [SerializeField] Toggle streamToggle;

    private const string site = "http://navi.gg";
    private const string videosHTML = "http://navi.gg/watch/record/";
    private const string streamsHTML = "http://navi.gg/watch/stream/";
    private const string listVideoTag = "list list-video";
    private const string listStreamTag = "list-stream list";

    private string currentHTML = videosHTML;
    private string currentTag = listVideoTag;

    private string html;
    private byte[] bHtml;
    private bool streamsAvailable = false;

    private string temp;
    private string[] tegs;
    private string[][] tableTags;
    private int needTag;

    public ArrayList objectList = new ArrayList();
    private WebClient webSite;
    private GameObject[] prefabs;
    private StreamObj prefabContent;

    public static event Action OnLoadingStart;
    public static event Action OnLoadingEnded;

    private void Start()
    {
        videoToggle.onValueChanged.AddListener( (value) => OnToggleChange(0, videoToggle) );
        streamToggle.onValueChanged.AddListener( (value) => OnToggleChange(1, streamToggle) );

        webSite = new WebClient();
        UpdatePage(currentHTML);
        OnLoadingEnded += CreatePrefabs;
    }

    public void UpdatePage(string page)
    {
        if (OnLoadingStart != null)
            OnLoadingStart();

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
                    if (tegs[i].Contains(currentTag))
                    {
                        needTag = i;
                        streamsAvailable = true;
                    }
                }
                
                if (streamsAvailable)
                {
                    CountLast();

                    prefabs = new GameObject[objectList.Count];
                    tableTags = new string[objectList.Count][];
                    
                    if (OnLoadingEnded != null)
                        OnLoadingEnded();
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
            prefabContent = prefabs[i].GetComponent<StreamObj>();

            for (int j = 0; j < tableTags[i].Length; j++)
            {
                if (tableTags[i][j].Contains("class=\"item-video\"")) // Link
                {
                    string[] temp = Regex.Split(tableTags[i][j], "href=\"");
                    string tLink = temp[1];
                    string[] temp2 = tLink.Split('"');
                    prefabContent.linkToStream = site + temp2[0];
                }

                if (tableTags[i][j].Contains("item-video-bg")) // Background
                {
                    string[] temp = Regex.Split(tableTags[i][j], "src=\"");
                    string dis = temp[1].Substring(0, temp[1].Length - 10);

                    byte[] image = webSite.DownloadData(dis);
                    Texture2D texture = new Texture2D(960, 540);
                    texture.LoadImage(image);
                    Sprite mySprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

                    prefabContent.mainImage.sprite = mySprite;
                }

                if (tableTags[i][j].Contains("icon-discipline")) // Discipline
                {
                    string[] temp = Regex.Split(tableTags[i][j], "data-game=\"");
                    string dis = temp[1].Substring(0, temp[1].Length - 2);

                    ChangeSprite(prefabContent.dicsiplineIMG , dis);
                }
                
                if (tableTags[i][j].Contains("header-item-video")) // Description
                {
                    string temp = tableTags[i][j + 1];
                    temp = temp.Substring(0, temp.Length - 7);

                    prefabContent.streamName.text = temp;
                }

                if (tableTags[i][j].Contains("data-news")) // Time
                {
                    string temp = tableTags[i][j + 1];
                    string time = temp.Substring(0, temp.Length - 6);

                    prefabContent.timeText.text = time;
                }

                if (tableTags[i][j].Contains("numeric-comments")) // Time
                {
                    string temp = tableTags[i][j + 1];
                    temp = temp.Substring(0, temp.Length - 7);
                    prefabContent.commentsCount.text = temp;
                }
            }
        }
    }

    public void ChangeSprite(Image img, string cname)
    {
        switch (cname)
        {
            case "csgo":
                img.sprite = disciplines[1];
                break;
            case "lol":
                img.sprite = disciplines[2];
                break;
            case "pubg":
                img.sprite = disciplines[3];
                break;
            default:
                img.sprite = disciplines[0];
                break;
        }
    }

    public void Clear()
    {
        if (prefabs != null)
        {
            content.transform.localPosition = Vector3.zero;

            foreach (GameObject obj in prefabs)
                Destroy(obj);

            objectList.Clear();

            for (int i = 0; i < content.transform.childCount; i++)
                Destroy(content.transform.GetChild(i).gameObject);
        }
    }

    public void OnToggleChange(int ID, Toggle tog)
    {
        if (tog.isOn)
        {
            if (ID == 0)
            {
                currentHTML = videosHTML;
                currentTag = listVideoTag;
            }
            else
            {
                currentHTML = streamsHTML;
                currentTag = listStreamTag;
            }

            Clear();
            UpdatePage(currentHTML);
        }
    }
}
