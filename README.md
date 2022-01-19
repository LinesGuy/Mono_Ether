# Mono_Ether
Full rewrite of Mono Ether


- Enemies should produce particle effects upon being killed 

- Program should detect if enemies collide with walls and bounce the enemies back 

- Enemies that use A* pathfinding should navigate around walls (as opposed to trying to move through walls) 

- A* pathfinding should be used sparingly, not every frame. E.g twice per second when close to the player, and every few seconds when far from the player 

- Enemies that use A* pathfinding should be able to store its current path in a queue and follow its path smoothly 

- Some enemy types should set their sprite orientation towards the direction it's moving (i.e enemies should "face forwards" when moving) 

- Some enemy types should be able to dodge bullets 

- Some enemy types should have a "tail"; a chain of smaller entities that follows the enemy (like a snake with its head and tail) 

- Level with a boss should have a "boss bar"; an indication of how much health the current boss has 

- Main levels should have a unique boss enemy with unique attack and movement patterns 

- Upon a boss' health hitting zero, the level should end with a "you won" screen 

- Program should spawn random enemy types at random intervals 

- Program should spawn enemies in "appropriate" locations; close-ish to the player but NOT inside walls or outside of the play area 

- The interval between enemy spawns should decrease as the game time increases, effectively increasing the number of enemies over time 

- The program should determine which "level" the user selected from the level selection screen and load the correct level data and settings accordingly 

- Program should have music playing in the background during gameplay, which loops upon ending 

- Program should unload all entities and the map upon exiting a level 

- Player should be able to pause the game by pressing ESC during gameplay 

- Program should automatically pause if the window loses focus (e.g if the user opens a different window) during gameplay 

- Program should have a pause screen with buttons to return to the level selection screen or resume gameplay 

- Program should have an editor mode, which can be accessed by pressing P during gameplay 

- When entering editor mode, all entities (except the player) should disappear, and enemy spawning should be disabled to remove any distractions during editing 

- In editor mode, user should be able to add or remove tiles from the map with the mouse 

- Whenever a tile is added or removed, the collision data for that tile and surrounding tiles should update accordingly 

- User should be able to press R during editor mode to save the tilemap data to a .txt file 

- User should be able to press P again during editor mode to resume to normal gameplay (enabling enemy spawners etc) 

- When an enemy is destroyed, the score gained from killing that enemy should appear as bouncing text from where the enemy died 

- This bouncing text should automatically disappear after a few seconds 

- Program should keep track of active geoms 

- If the player gets near-ish to a geom, the geom should move toward the player, as if attracted towards it like a magnet 

- When a player and a geom collide, the geom disappears and the player's geom counter is incremented, and a pickup sound is played 

- When an enemy dies, an random enemy death sound is played 

- When the player dies, a unique player death sound is played, and an especially large number of particles are emitted 

- When an enemy is spawned, a random enemy spawn sound is played 

- When the player shoots a bullet, a random bullet shot sound is played 

- The HUD (heads up display) should display the player's current score, highscore, geom count, score multiplier, and remainig lives 

- The HUD should also display any active power packs the player has with an indicator for each power pack that shows how much time remains for that power pack 

- The program should keep a circular array of any active particles, with a maximum number of particles (the array length) 

- When trying to add a particle when the list is full, the oldest particle is removed, to save memory and improve performance 

- Pause menu should slide in and out smoothly, not just appear and disappear in one frame 

- When changing between screens, the screen should fade to black and fade in to the new screen 

- When the player dies, all enemies must disappear, and the player must not spawn again for a set interval (e.g 2 seconds) 

- Player should be able to use WASD to move around on the screen 

- Program should store and use the player's acceleration, velocity, position and friction so the player moves around smoothly and not jagged-ly 

- When the player is accelerating in a direction, "exhaust fire" particles should be emitted in the opposite direction 

- Program should keep track of the current highscore, and when a game ends with a new highscore, this highscore should be saved to a .txt file 

- Game should have various powerpacks that allow the player to temporarily gain increased firepower or movement speed 

- Game should have a tutorial level which teahes the player basics like movement and shooting 

- Program should have a credit screen to showcase the creators 

- Program should have a level selection screen 

- Level selection screen should have a list of levels the user can go to 

- Level selection screen should also have a difficulty selection section 

- Program should have a Title screen, with buttons to advance to other screens and a fancy background 

- Program should have a settings screen to adjust various settings 

- Program should have a doom-style (pseudo-3d) secret section, which can be entered by picking up a certain powerpack 

- The doom section requires the player to find a certain tile to return back to gameplay 

- The faster the player completes this task, the longer they will have increased firepower 

- The program must not crash or freeze at any point 

-The program should be able to run with at least 60 frames per second during gameplay 

-Program should be able to process hundreds of enemies and/or particles efficiently without lagging 

-Any complex algorithms or sections of code that will be run many times per frame should be optimised to minimise cpu/memory usage 

-Code should be clean, easy to understand and easy to expand upon, using comments to explain code blocks that may be difficult to understand. 

-Classes should be neatly organised into the correct namespaces, files and folders to make the codebase easier to navigate 