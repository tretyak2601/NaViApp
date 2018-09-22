using UnityEngine;
using UnityEngine.UI;

public class DisableButtons : MonoBehaviour {

    [SerializeField] ParseFromHTML parsing;

    [SerializeField] Toggle dotaTog;
    [SerializeField] Toggle csgoTog;
    [SerializeField] Toggle lolTog;
    [SerializeField] Button showLastGames;

    void Start () {
        parsing.OnPrefabsCreated += OnAll;
        parsing.OnUpdateStart += DisableAll;
	}

    private void DisableAll()
    {
        dotaTog.interactable = false;
        csgoTog.interactable = false;
        lolTog.interactable = false;
        showLastGames.interactable = false;
    }

    private void OnAll()
    {
        dotaTog.interactable = true;
        csgoTog.interactable = true;
        lolTog.interactable = true;
        showLastGames.interactable = true;
    }

    private void OnDisable()
    {
        parsing.OnPrefabsCreated -= OnAll;
        parsing.OnUpdateStart -= DisableAll;
    }
}
