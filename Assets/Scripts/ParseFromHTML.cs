using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text.RegularExpressions;

public class ParseFromHTML : MonoBehaviour {

    private const string naViSite = "http://game-tournaments.com/csgo/team/navi";
    private const string needID = "block_matches_current";

    private string html;
    private string[] tegs;
    private string temp;
    private string gameDiv;

    private ArrayList divs = new ArrayList();
    private ArrayList tds = new ArrayList();
    private List<GamePrefab> games = new List<GamePrefab>();
    private WebClient webSite;

    void Start()
    {
        webSite = new WebClient();
        html = webSite.DownloadString(naViSite);
        tegs = Regex.Split(html, @"(?<=[>])");
        DevideForTag("div", divs);
        
        foreach (string i in divs)
        {
            if (i.Contains(needID))
                gameDiv = i;
        }

        tegs = Regex.Split(gameDiv, @"(?<=[>])");
        DevideForTag("td", tds);

        foreach (string i in tegs)
            Debug.Log(i);
    }

    private void DevideForTag(string tag, ArrayList list)
    {
        for (int i = 0; i < tegs.Length; i++)
        {
            if (tegs[i].Contains("<" + tag))
            {
                for (int j = i; j < tegs.Length; j++)
                {
                    temp += tegs[j];

                    if (tegs[j].StartsWith("</" + tag))
                    {
                        list.Add(temp);
                        temp = string.Empty;
                        break;
                    }
                }
            }
        }
    }
}
