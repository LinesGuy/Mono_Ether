- The interval between enemy spawns should decrease as the game time increases, effectively increasing the number of enemies over time 
- The program should determine which "level" the user selected from the level selection screen and load the correct level data and settings accordingly 
- Program should have music playing in the background during gameplay, which loops upon ending 
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
- When an enemy dies, an random enemy death sound is played 
- When the player dies, a unique player death sound is played, and an especially large number of particles are emitted 
- When an enemy is spawned, a random enemy spawn sound is played 
- When the player shoots a bullet, a random bullet shot sound is played 
- The HUD (heads up display) should display the player's current score, highscore, geom count, score multiplier, and remainig lives 
- The HUD should also display any active power packs the player has with an indicator for each power pack that shows how much time remains for that power pack 
- The program should keep a circular array of any active particles, with a maximum number of particles (the array length) 
- When trying to add a particle when the list is full, the oldest particle is removed, to save memory and improve performance 
- When changing between screens, the screen should fade to black and fade in to the new screen 
- When the player is accelerating in a direction, "exhaust fire" particles should be emitted in the opposite direction 
- Program should keep track of the current highscore, and when a game ends with a new highscore, this highscore should be saved to a .txt file 
- Game should have various powerpacks that allow the player to temporarily gain increased firepower or movement speed 
- Game should have a tutorial level which teahes the player basics like movement and shooting 
- Program should have a credit screen to showcase the creators 
- Level selection screen should also have a difficulty selection section 
- Program should have a doom-style (pseudo-3d) secret section, which can be entered by picking up a certain powerpack 
- The doom section requires the player to find a certain tile to return back to gameplay 
- The faster the player completes this task, the longer they will have increased firepower 