using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.AbilitySystem;
using RPG.StatSystem;


[CreateAssetMenu(fileName = "CharacterSheet", menuName = "RPG/CharacterSheet", order = 2)]
public class CharacterSheet : ScriptableObject
{
    //Character Image

    //Character animations

    //Character stats
    [Header("Stats")]
    [SerializeField] StatSheet statSheet = new StatSheet();

    //Character abilities
    [Header("Abilities")]
    [SerializeField] private List<Ability> abilities = new List<Ability>();

    /*
     [Header("Character Identity")]
    [SerializeField] private string characterName;
    [SerializeField] private Sprite characterPortrait; // For menus
    [SerializeField] private GameObject characterModel; // For combat scenes or 3D use

    [Header("Stats")]
    [SerializeField] private StatSheet statSheet;

    [Header("Combat Attributes")]
    [SerializeField] private int attackPower;
    [SerializeField] private int defensePower;
    [SerializeField] private float speed;

    [Header("Abilities")]
    [SerializeField] private List<Ability> abilities; // Define an Ability class for skills/spells

    [Header("AI Attributes (For Enemies)")]
    [SerializeField] private bool isPlayerControlled = true;
    [SerializeField] private AIBehaviorType aiBehavior; // Enum to define behavior patterns like Aggressive, Defensive, etc.

    // Getters and Setters
    public string CharacterName => characterName;
    public Sprite CharacterPortrait => characterPortrait;
    public GameObject CharacterModel => characterModel;
    public StatSheet StatSheet => statSheet;
    public int AttackPower => attackPower;
    public int DefensePower => defensePower;
    public float Speed => speed;
    public List<Ability> Abilities => abilities;
    public bool IsPlayerControlled => isPlayerControlled;
    public AIBehaviorType AIBehavior => aiBehavior;
     
     */
}
