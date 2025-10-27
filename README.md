ReadMe Last updated 10/27/2025

# Momentum

Marked as Momentum_MVP in my files, this is the beginnings of a project using the Momentum system.

The main purpose of this project for the moment is to create tools in Unity based in C# that will allow me to create a wide variety of customizable abilities, dialogue, and anything else a JRPG system requires. I've been trying to future-proof the editors to work with any future updates.

## Camera

1/1/24: Adjusted so that the sprites now appear at the proper size. Might need adjustment later but works for now.

## Animations and Art

12/23/24: Began doing grass overworld tiles. Planning to do a 4x4 grid of 16x16 tiles, each copying the same grass pattern but slightly unique individually. Aseprite files (character base not complete) moved into the git folder.
12/30/24: That took way too long. Next time, do like 4 tiles instead to see if it'll be any good.
12/31/24: Rule tile created for grass. Grid layers made and can be drawn but their size is not consistent with the character sprite.
1/1/25: Consistency issue resolved.
REMEMBER: Crop your sprits down so there isn't any extra canvas space. Unity assumes this is the case and will mess up your sprite size accordingly.
10/27/25: Added basic tilemap by Majek on Fiverr

## Dialogue

Dialogues can be created from scratch and nodes added to it through an editor system. Dialogues now contain a list of speakers, though at the moment the speakers must be entered manually through the inspector.

12/30/24: Dialogue can be displayed now through the DisplayDialogue script. Currently only shows a test dialogue script.
1/1/24: PROBLEM: The dialogue box no longer cooresponds with the player's view. 

## Combat

Statsheets are prepared. Still need to create full-on character sheets and character managers to unite the statsheet, animations, art, etc.

## Overworld tilemaps

## Tests

There is a test that verifies the damage system works as it is supposed to. No others exist as of yet, nor does the full combat system.

# The Momentum System

The Momentum combat system is similar to an action point system. The characters, both ally and enemy, have access to a resource called Momentum that can be spent in order to perform attacks. The next character to act in combat is the one with the highest Momentum score.

When a character makes an attack, and their skill + momentum surpass a certain threshold, the attack becomes Overwhelming, and has additional effects based on the attack being used and the character using them.

Example: A basic damaging ability that overwhelms would deal double the damage.

# Design Speculation

## Stances

Concept: at the beginning of each round, the player and enemies can choose between a number of stances that will affect how an Overwhelming attack works. An aggressive stance, for example, will sacrifice Skill on defense but add it on offense. Defensive, same deal in reverse. Or... hrm, seems too simplistic.

Permutations: Perhaps the player and enemies get one free stance change per round instead? Give it a try.

# UI

## BattleUI

3/5/2025: BattleUI and BattleScene set up. (Size doesn't work, scale works at 0.53x for now)
3/6/2025: Battle test now set up and working, uses Ability and Behavior classes. Abilities selected at random, still needs a UI.