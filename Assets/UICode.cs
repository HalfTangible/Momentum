using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class UICode : MonoBehaviour
{
    GameObject menu;
    // Start is called before the first frame update
    void Start()
    {
        menu = GameObject.Find("Menu");
        menu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && menu.activeSelf == false)
        {
            menu.SetActive(true);
            //If the menu is called, then the rest of our possible commands cannot execute
        }
        else if (menu.activeSelf == true)
        {
            //Prevent any actions that don't affect the menu. Like moving dialogue along.
        }
    }
}
