using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IPage {
    
    void UpdatePage(string page);
    void CountLast();
    void CreatePrefabs();
    void ChangeSprite(Image img, string cname);
    void Clear();
}
