using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayCharacterStats : MonoBehaviour
{

    StatSheet theCharacter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DisplayStats() {
        Stat[] theStats = theCharacter.getStats();
        foreach(Stat stat in theStats) { 
            //DisplayStat(stat) + "\n";
        }
    }

    string DisplayStat(Stat s)
    {
        return s.getName() + ": " + s.getCurrent() + " / " + s.getMax();
    }
}
