using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MenuCode : MonoBehaviour
{
//    bool menuCurrentlyOpen;
    // Start is called before the first frame update
    void Start()
    {
  //      menuCurrentlyOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        //In the object itself, do menu buttons
        //If you try to make it activate and turn itself on/off
        //Then it won't be able to turn back on.
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Before key: " + menuCurrentlyOpen);
            menuCurrentlyOpen = !menuCurrentlyOpen;
            gameObject.SetActive(menuCurrentlyOpen);
            Debug.Log("After key: " + menuCurrentlyOpen);
        }*/
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }


    }
}
