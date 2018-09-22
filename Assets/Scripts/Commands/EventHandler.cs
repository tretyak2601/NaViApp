using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour {
    
    [SerializeField] ParseFromHTML parsing;
    [SerializeField] ToggleGroupScript group;
    [SerializeField] ShowList showList;
    [SerializeField] Loading loading;

    [SerializeField] Image choosenGameImage;

    private ToggleObj toggleInfo;

    void Start()
    {
        group.OnChange += Group_OnChange;
    }

    private void Group_OnChange(Toggle newActive)
    {
        toggleInfo = newActive.GetComponent<ToggleObj>();

        choosenGameImage.sprite = toggleInfo.GameImage;
        loading.StartCour();
        parsing.NeedId = ParseFromHTML.futureGames;
        parsing.GameName = toggleInfo.GameName;
        parsing.UpdatePage(toggleInfo.GameHtml);
    }

    private void OnDisable()
    {
        group.OnChange -= Group_OnChange;
    }
}
