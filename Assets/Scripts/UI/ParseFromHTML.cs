using System.Collections;
using UnityEngine;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Text;
using UniRx;
using System;

public class ParseFromHTML : MonoBehaviour, IPage {

    private const string NaViCsGo = "http://game-tournaments.com/csgo/team/navi";
    private const string NaViDota = "http://game-tournaments.com/dota-2/team/navi";
    private const string NaViLOL = "http://game-tournaments.com/lol/team/navi";

    private static string[] Month = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
    public const string futureGames = "block_matches_current";
    public  const string pastGames = "block_matches_past";
    private const string NaVI = "Na`Vi";
    private const string resultButton = "mbutton tresult";

    public static bool isPast = false;
    public static string needID = "block_matches_current";
    public static bool gamesAvailable = false;
    public static int timeLocalization = 3;

    private string gameName = "csgo";
    private byte[] bHtml;
    private string html;
    private string[] tegs;
    private string[][] tableTags;
    private int needTag;
    private string s;
    private string gameDiv;
    
    private ArrayList divs = new ArrayList();
    private WebClient webSite;
    private GameObject[] prefabs;
    private GamePrefab prefabContent;

    [SerializeField] Sprite noName;
    [SerializeField] ImageDict[] logos;
    [SerializeField] GameObject tablePrefab;
    [SerializeField] GameObject content;
    [SerializeField] Image TimeBackButtonImage;
    [SerializeField] Text NoGamesText;
    [SerializeField] Text NoInternet;

    public event Action OnUpdateStart;
    public event Action OnPrefabsCreated;
    
    public string GameName
    {
        set
        {
            this.gameName = value;
        }
        get
        {
            return this.gameName;
        }
    }
    
    public string NeedId
    {
        set
        {
            isPast = false;
            needID = value;
        }
        get
        {
            return needID;
        }
    }

    void Start()
    {
        webSite = new WebClient();
        UpdatePage(NaViCsGo);
    }

    public void UpdatePage(string page)
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            if (OnUpdateStart != null)
                OnUpdateStart();

            TimeBackButtonImage.color = ScreenSelected.unChoosed;
            NoInternet.gameObject.SetActive(false);
            NoGamesText.gameObject.SetActive(false);
            gamesAvailable = false;
            Clear();

            //bHtml = webSite.DownloadData(page);
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
                        if (tegs[i].Contains(needID))
                        {
                            needTag = i;
                            gamesAvailable = true;
                        }
                    }

                    if (gamesAvailable)
                    {
                        CountLast();

                        prefabs = new GameObject[divs.Count];
                        tableTags = new string[divs.Count][];

                        CreatePrefabs();
                    }
                    else
                        NoGamesText.gameObject.SetActive(true);

                    Loading.isLoading = false;
                    if (OnPrefabsCreated != null)
                        OnPrefabsCreated();
                });

            ObservableWWW.GetAndGetBytes(page).Subscribe(observer);
        }
        else
        {
            NoGamesText.gameObject.SetActive(false);
            NoInternet.gameObject.SetActive(true);
        }
    }

    public void ShowLastGames()
    {
        if (!isPast)
        {
            needID = pastGames;
            isPast = true;
        }
        else
        {
            needID = futureGames;
            isPast = false;
        }

        switch (gameName)
        {
            case "dota":
                UpdatePage(NaViDota);
                break;
            case "csgo":
                UpdatePage(NaViCsGo);
                break;
            case "lol":
                UpdatePage(NaViLOL);
                break;
        }

    }

    public void CountLast()
    {
        for (int i = needTag + 1; i < tegs.Length; i++)
        {
            if (tegs[i].Contains("</table"))
                return;

            if (tegs[i].Contains("<tr"))
            {
                for (int j = i; j < tegs.Length; j++)
                {
                    s += tegs[j];

                    if (tegs[j].Contains("</tr"))
                    {
                        divs.Add(s);
                        s = string.Empty;
                        break;
                    }
                }
            }
        }
    }

    public void CreatePrefabs()
    {
        for (int i = 0; i < divs.Count; i++)
        {
            tableTags[i] = Regex.Split((string)divs[i], @"(?<=[>])");
            prefabs[i] = Instantiate(tablePrefab, tablePrefab.transform.localPosition, Quaternion.identity, content.transform);
        }

        for (int i = 0; i < divs.Count; i++)
        {
            prefabContent = prefabs[i].GetComponent<GamePrefab>();

            for (int j = 0; j < tableTags[i].Length; j++)
            {
                if (tableTags[i][j].Contains("teamname c1")) // First team
                {
                    if (tableTags[i][j + 2].Contains("Na") && tableTags[i][j + 2].Contains("Vi"))
                        prefabContent.c1name.text = NaVI;
                    else
                    {
                        prefabContent.c1name.text = tableTags[i][j + 2].Substring(0, tableTags[i][j + 2].Length - 4);
                        ChangeSprite(prefabContent.naviLogo, prefabContent.c1name.text);
                        Exceptions(prefabContent.naviLogo);
                    }
                }

                if (tableTags[i][j].Contains("teamname c2")) // Second team
                {
                    if (tableTags[i][j + 2].Contains("Na") && tableTags[i][j + 2].Contains("Vi"))
                        prefabContent.c2name.text = NaVI;
                    else
                    {
                        prefabContent.c2name.text = tableTags[i][j + 2].Substring(0, tableTags[i][j + 2].Length - 4);
                        ChangeSprite(prefabContent.enemyLogo, prefabContent.c2name.text);
                        Exceptions(prefabContent.enemyLogo);
                    }
                }
                
                if (tableTags[i][j].Contains("sct")) // DateTime
                {
                    prefabContent.date.text = string.Join("", tableTags[i][j + 1].Substring(0, tableTags[i][j + 1].Length - 7).Split(','));
                    prefabContent.date.text = prefabContent.date.text.Insert(prefabContent.date.text.Length - 5, "\n");
                    prefabContent.date.text = prefabContent.date.text.Remove(prefabContent.date.text.Length - 12, 6);

                    if (isPast)
                        prefabContent.date.gameObject.transform.localPosition -= new Vector3(100f, 0f, 0f);

                    DateTime date = ToDateTime(prefabContent.date.text);
                    prefabContent.date.text = Month[date.Month - 1].ToString() + " " + date.Day + "\n" + date.ToShortTimeString();
                }

                if (tableTags[i][j].Contains(resultButton)) // SCORE
                {
                    prefabContent.score.SetActive(true);
                    string[] temp = Regex.Split(tableTags[i][j], "data-score=\"(.+)\"");
                    foreach (var s in temp)
                    {
                        string[] scores = s.Split(':');
                        if (scores.Length == 2)
                        {
                            string left = scores[0].Trim();
                            string right = scores[1].Trim();
                            int index = right.IndexOf('"');
                            right = right.Substring(0, index);
                            prefabContent.bestOf = left.Trim() + " : " + right.Trim();
                        }
                    }
                }
            }
        }
    }

    public void ChangeSprite(Image image, string cname)
    {
        for (int i = 0; i < logos.Length; i++)
        {
            if (logos[i].sprite.name == cname)
            {
                image.sprite = logos[i].sprite;
                return;
            }
        }
        image.sprite = noName;
    }

    public void Clear()
    {
        if (prefabs != null)
        {
            content.transform.localPosition = Vector3.zero;

            foreach (GameObject obj in prefabs)
                Destroy(obj);

            divs.Clear();

            for (int i = 0; i < content.transform.childCount; i++)
            {
                Destroy(content.transform.GetChild(i).gameObject);
            }
        }
    }

    private void Exceptions(Image logo)
    {
        if (prefabContent.c1name.text.Contains("VP"))
            ChangeSprite(logo, "Virtus.Pro");
        if (prefabContent.c1name.text.Contains("Fnatic"))
            ChangeSprite(logo, "fnatic");
    }
    private DateTime ToDateTime(string s)
    {
        DateTime date = new DateTime();
        string[] dateInString = s.Split(' ', '\n', ':');

        int month = 1;
        double day = double.Parse(dateInString[1]);
        double hours = double.Parse(dateInString[2]);
        double minutes = double.Parse(dateInString[3]);

        for (int i = 0; i < Month.Length; i++)
        {
            if (Month[i].Equals(dateInString[0]))
            {
                month = i;
                break;
            }
        }
        
        date = date.AddMonths(month);
        date = date.AddDays(day - 1);
        date = date.AddHours(hours + timeLocalization);
        date = date.AddMinutes(minutes);
        
        return date;
    }
}
