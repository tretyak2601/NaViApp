using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenSelected : MonoBehaviour {
    
    [SerializeField] MenuPair[] menuPair;
    [SerializeField] GameObject Menu;
    [SerializeField] Image lastGamesButtonImage;

    [HideInInspector] GameObject choosedMenu;
    [HideInInspector] int numOfMenu;
    [HideInInspector] GameObject ShownMenu;

    public static Color choosed = new Color(1f, 0.964f, 0.164f, 1);
    public static Color unChoosed = new Color(1, 1, 1, 0.5f);
    private static Vector3 moove = new Vector3(1080, 0, 0);
    private const int speed = 5;
    private bool isStaying = true;
    private bool off = true;
    private Vector3 rightPos;

    void Start () {

	}

    void Update()
    {
        if (ShownMenu != null)
        {
            if (Menu.transform.localPosition != rightPos)
            {
                float distance = (Menu.transform.localPosition - rightPos).magnitude + 100;
                Menu.transform.localPosition = Vector3.MoveTowards(Menu.transform.localPosition, rightPos, Time.deltaTime * speed * distance);
            }
            else
                isStaying = true;            
        }

        if (isStaying && off)
        {
            for (int i = 0; i < menuPair.Length; i++)
                menuPair[i].menu.SetActive(false);

            menuPair[numOfMenu].menu.SetActive(true);
            off = false;
        }
    }

    public void Choose()
    {
        isStaying = false;
        off = true;

        choosedMenu = EventSystem.current.currentSelectedGameObject;

        foreach (var i in menuPair)
            i.menu.SetActive(true);

        switch (choosedMenu.name)
        {
            case "Games Button": numOfMenu = 0;
                break;
            case "News Button": numOfMenu = 1;
                break;
            case "Streams Button": numOfMenu = 2;
                break;
            default: numOfMenu = 0;
                break;
        }

        if (numOfMenu == 1)
            rightPos = Vector3.zero - moove;
        else if (numOfMenu == 2)
            rightPos = Vector3.zero - moove * 2;
        else
            rightPos = Vector3.zero;

        offOthers(menuPair[numOfMenu]);
    }

    private void offOthers(MenuPair pair)
    {
        foreach (var i in menuPair)
        {
            i.image.color = unChoosed;
            i.bottomImage.gameObject.SetActive(false);
        }
        
        ShownMenu = pair.menu;
        pair.image.color = choosed;
        pair.bottomImage.gameObject.SetActive(true);
    }

    public void SetColor()
    {
        if (ParseFromHTML.isPast)
            lastGamesButtonImage.color = choosed;
        else
            lastGamesButtonImage.color = unChoosed;
    }

    Vector3 deltaPos = Vector3.zero;

    public void SlideRightToLeft()
    {
        Menu.transform.localPosition = new Vector3(Input.mousePosition.x - 1080f, Menu.transform.localPosition.y, Menu.transform.localPosition.z)/10f;
    }

    public void OnEndDrag()
    {
        if (Menu.transform.localPosition.x < -50f)
        {



        }
    }
}
