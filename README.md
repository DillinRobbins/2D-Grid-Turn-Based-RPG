# 2D-Grid-Turn-Based-RPG
Old school turn-based RPG with grid-based horde combat.

This system features fully modular encounters, characters, combat actions, and totems using the decorator pattern and Unity's Scriptable object class. 
This allows designers to customize gameplay from within the inspector. 
The turn system works using managers to ensure proper order and timing of actions according to character speed and combat actions' speed modifiers. 
The grid system works by generating points centered on and between each cell to detect mouse position and highlight cells according to a combat action's area of effect data. 	Systems that aren't dependent on timing of execution are decoupled using the observer pattern. 
If I had to recreate some aspects of this, I would definitely use interfaces and I would manage the encapsulation of Character Lists across classes better.
The gameplay hadnt reached a level of complexity to require more than abstract and inherited classes.
