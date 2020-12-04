Issues: Sometimes when many players are on the server at once, there can be issues with firing at a consitent speed. The game will instead fire at an uneven pace
This is probably due to some sort of issue where the server is too slow to handle many projectiles at once.

We decided to create a player class in order to keep track of things like tank ids, socket states, and the fire rate delays. This object is stored by the server until a player diosconnects.

There are no additional features added due to time restrictions. 

We decided to have the server not print anything into the console, as it is mostly useful for debugging purposes.

The server can read settings from files, but they must be called "settings.xml" and it must be located in the Resources folder of the solution. 
It is capable of reading all game settings from the file, but it only requires world size, frame rate, frames per shot, and respwan rate to be defined in the file. The rest will set itself to default values if not included.