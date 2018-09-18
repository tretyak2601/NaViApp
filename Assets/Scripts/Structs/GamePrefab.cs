using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePrefab : MonoBehaviour {
    private int id;
    [HideInInspector] public string bestOf;

    public Image naviLogo;
    public Image enemyLogo;
    public Text date;
    public Text c1name;
    public Text c2name;
}
