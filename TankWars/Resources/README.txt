Issues: When holding two movement keys, and the first key is released, the tank will pause for a frame before continuing to move in the direction of the second key.


We decided to have the movement keys be stored in a stack. When a key is pressed, it is added to the satck, and removed when the key is released. 
When you hold multiple keys, the most recently pressed key goes on top of the stack, and only the top value on the stack is sent as a control command to the server.

The draw methods for projectiles, beams, and tank turrets all incorporate parts of DrawObjectWithTrasnform instead of just using that method.
This is becasue those sprites need to be rotated, and some additional translations need to be made before and after the rotation in order to preserve the sprites location.

The example view program, and the tankwars server all seem to mis-represent the walls as far as we can tell. In the settings file for the server, the innermost right and left wall are defined as 2 segments long.
The example view program and the tankwars.eng.utah.edu server all treat these walls as if they are 3 segments long, which is incorrect as far as we can see. Out program treats these walls as 2 segments long.

