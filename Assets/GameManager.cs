using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }
    // Still need to make the Character class.
    public List<Character> playerParty;

    // Story events: Dictionary<Arc, Dictionary<EventName, EventStatus>>
    private Dictionary<string, Dictionary<string, bool>> storyEvents = new Dictionary<string, Dictionary<string, bool>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManagers
            Debug.Log("Duplicate GameManager destroyed");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
