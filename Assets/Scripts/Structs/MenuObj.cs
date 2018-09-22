using UnityEngine;
using UnityEngine.UI;
using System;

public class MenuObj : MonoBehaviour {

    [SerializeField] public Button Button;
    [SerializeField] public int menuNum;
    [SerializeField] public Vector3 menuPosition;
    [SerializeField] public Image mainImage;
    [SerializeField] public Image bottomImage;

    public static event Action<Button> OnMenuChanged;

	void Start () {
        Button.onClick.AddListener(() => MoveMenu(Button));
	}

    public void MoveMenu(Button b)
    {
        if (OnMenuChanged != null)
            OnMenuChanged(b);
    }
}
