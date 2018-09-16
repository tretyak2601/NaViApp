﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Text;

public class ParseFromHTML : MonoBehaviour {

    private const string NaViCsGo = "http://game-tournaments.com/csgo/team/navi";
    private const string NaViDota = "http://game-tournaments.com/dota-2/team/navi";
    private const string NaViLOL = "http://game-tournaments.com/lol/team/navi";

    private const string needID = "block_matches_past";
    private const string NaVI = "Na`Vi";

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

    void Start()
    {
        webSite = new WebClient();
        UpdatePage(NaViCsGo);
    }

    public void UpdatePage(string page)
    {
        Clear();

        bHtml = webSite.DownloadData(page);
        html = Encoding.UTF8.GetString(bHtml);
        tegs = Regex.Split(html, @"(?<=[>])");

        for (int i = 0; i < tegs.Length; i++)
        {
            if (tegs[i].Contains(needID))
                needTag = i;
        }

        CountLastGames();

        prefabs = new GameObject[divs.Count];
        tableTags = new string[divs.Count][];

        for (int i = 0; i < divs.Count; i++)
        {
            tableTags[i] = Regex.Split((string)divs[i], @"(?<=[>])");
            prefabs[i] = Instantiate(tablePrefab, tablePrefab.transform.localPosition, Quaternion.identity, content.transform);
        }

        CreatePrefabs();
    }

    private void CountLastGames()
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

    private void CreatePrefabs()
    {
        for (int i = 0; i < divs.Count; i++)
        {
            prefabContent = prefabs[i].GetComponent<GamePrefab>();

            for (int j = 0; j < tableTags[i].Length; j++)
            {
                if (tableTags[i][j].Contains("teamname c1"))
                {
                    if (tableTags[i][j + 2].Contains("Na") && tableTags[i][j + 2].Contains("Vi"))
                        prefabContent.c1name.text = NaVI;
                    else
                    {
                        prefabContent.c1name.text = tableTags[i][j + 2].Substring(0, tableTags[i][j + 2].Length - 4);
                        ChangeSprite(prefabContent.naviLogo, prefabContent.c1name.text);
                        Exceptions(true);
                    }
                }

                if (tableTags[i][j].Contains("teamname c2"))
                {
                    if (tableTags[i][j + 2].Contains("Na") && tableTags[i][j + 2].Contains("Vi"))
                        prefabContent.c2name.text = NaVI;
                    else
                    {
                        prefabContent.c2name.text = tableTags[i][j + 2].Substring(0, tableTags[i][j + 2].Length - 4);
                        ChangeSprite(prefabContent.enemyLogo, prefabContent.c2name.text);
                        Exceptions(false);
                    }
                }

                if (tableTags[i][j].Contains("sct"))
                {
                    prefabContent.date.text = string.Join("", tableTags[i][j + 1].Substring(0, tableTags[i][j + 1].Length - 7).Split(','));
                    prefabContent.date.text = prefabContent.date.text.Insert(prefabContent.date.text.Length - 5, "\n");
                }
            }
        }
    }

    private void ChangeSprite(Image image, string cname)
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

    private void Clear()
    {
        if (prefabs != null)
        {
            foreach (GameObject obj in prefabs)
                Destroy(obj);

            divs.Clear();

            for (int i = 0; i < content.transform.childCount; i++)
            {
                Destroy(content.transform.GetChild(i).gameObject);
            }
        }
    }

    private void Exceptions(bool left)
    {
        if (left)
        {
            if (prefabContent.c1name.text.Contains("VP"))
                ChangeSprite(prefabContent.naviLogo, "Virtus.Pro");
            if (prefabContent.c1name.text.Contains("Fnatic"))
                ChangeSprite(prefabContent.naviLogo, "fnatic");

        }
        else
        {
            if (prefabContent.c1name.text.Contains("VP"))
                ChangeSprite(prefabContent.enemyLogo, "Virtus.Pro");
            if (prefabContent.c1name.text.Contains("Fnatic"))
                ChangeSprite(prefabContent.enemyLogo, "fnatic");
        }
    }
    
}
