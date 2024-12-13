using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TestBattle1 : MonoBehaviour
{
    /*
    //With this, we're going to test if the BasicAttack and HeavyAttack classes actually work as they should.
    StatSheet player = new StatSheet();
    StatSheet enemy = new StatSheet();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        Stat health1 = new Stat(100);
        Stat health2 = new Stat(20);

        Stat m1 = new Stat(20);
        Stat m2 = new Stat(10);

        player.SetHealth(health1);
        enemy.SetHealth(health2);

        player.SetMomentum(m1);
        enemy.SetMomentum(m2);

        BasicAttackTest();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BasicAttackTest()
    {
        //No longer works with our current setup

        /*
        Debug.Log("Begin BasicAttack test");
        //The enemy will perform a BasicAttack against the user.
        //Print out player health and max before this point as well as the enemy

        Debug.Log("Player HP: " + player.getHealthCurrent() + " / " + player.getHealthMax());
        Debug.Log("Player Momentum: " + player.getMomentumCurrent());

        Debug.Log("Enemy HP: " + enemy.getHealthCurrent() + " / " + enemy.getHealthMax());
        Debug.Log("Enemy Momentum: " + enemy.getMomentumCurrent());
        
        //Seems to be an issue in the BasicAttack class itself, line 51
        BasicAttack basicAttack = new BasicAttack();
        Debug.Log("basicAttack created");
        basicAttack.UseAbility(player, enemy);

        //Cannot create a new object for a monobehavior.
        //Need to either convert BasicAttack into a scriptable object, or remove the monobehavior from Damage. Can we do the latter?
        //Can, but we get the same error. So it's not the damage monobehavior that's doing it.

        //Now print current totals.

        Debug.Log("Player HP: " + player.getHealthCurrent() + " / " + player.getHealthMax());
        Debug.Log("Player Momentum: " + player.getMomentumCurrent());

        Debug.Log("Enemy HP: " + enemy.getHealthCurrent() + " / " + enemy.getHealthMax());
        Debug.Log("Enemy Momentum: " + enemy.getMomentumCurrent());
    }

    void HeavyAttack()
    {
        //The user will perform a HeavyAttack against the enemy.
    }

    void OverWhelm() 
    {
        //User gains a ton of extra momentum and Overwhelms the attacker.
    }*/
}
