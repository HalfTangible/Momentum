Last updated 12/19/2024

# Momentum

Marked as Momentum_MVP in my files, this is the beginnings of a project using the Momentum system.

The main purpose of this project for the moment is to create tools in Unity based in C# that will allow me to create a wide variety of customizable abilities, dialogue, and anything else a JRPG system requires. I've been trying to future-proof the editors to work with any future updates.

## Animations and Art

Currently on the backburner. I have a template I'm working on... and now that I think on it it might be worth it to put the aseprite files directly into the Momentum_MVP folder just in case.

## Dialogue

Dialogues can be created from scratch and nodes added to it through an editor system. Still have not finished creating a way to display the information, but this set of files does have an older version of a text box that I will be using as a reference guide until completion.

## Combat

Statsheets are prepared. Still need to create full-on character sheets and character managers to unite the statsheet, animations, art, etc.

## Overworld tilemaps

Not even begun.

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