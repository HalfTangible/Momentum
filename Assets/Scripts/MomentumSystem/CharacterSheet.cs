using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.AbilitySystem;
using RPG.StatSystem;


[CreateAssetMenu(fileName = "CharacterSheet", menuName = "RPG/CharacterSheet", order = 2)]
public class CharacterSheet : ScriptableObject
{/*
    [Header("Identity")]
    public string characterName = "Unnamed";

    [Header("Base Stats Template")]
    public Stat health = new Stat(100, StatType.Resource) { Name = "HEALTH" };
    public Stat momentum = new Stat(10, StatType.Unbounded) { Name = "MOMENTUM" };
    public Stat motive = new Stat(10, StatType.Character) { Name = "MOTIVE" };
    public Stat means = new Stat(10, StatType.Character) { Name = "MEANS" };
    public Stat skill = new Stat(3, StatType.Character) { Name = "SKILL" };

    //Character abilities
    [Header("Abilities")]
    [SerializeField] private List<Ability> abilities = new List<Ability>();


    public StatSheet CreateRuntimeInstance()
    {
        var runtime = new StatSheet
        {
            characterName = characterName,

            // Clone stats so runtime changes don't affect template
            health = health.Clone(),
            momentum = momentum.Clone(),
            motive = motive.Clone(),
            means = means.Clone(),
            skill = skill.Clone(),

            abilities = new List<Ability>(startingAbilities),

            // Runtime defaults
            wards = 0,
            counters = 0,
            shield = 0,
            continuingEffects = new List<ABehavior>()
        };

        return runtime;
    }

    */

}
