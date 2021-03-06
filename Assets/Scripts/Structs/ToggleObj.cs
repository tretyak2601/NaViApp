﻿using UnityEngine;

public class ToggleObj : MonoBehaviour {
    
    [SerializeField] Sprite gameImage;
    [SerializeField] string gameHtml;
    [SerializeField] string gameName;
    
    public Sprite GameImage
    {
        get
        {
            return gameImage;
        }
    }

    public string GameHtml
    {
        get
        {
            return gameHtml;
        }
    }

    public string GameName
    {
        get
        {
            return gameName;
        }
    }
}
