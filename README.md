# MM_Pong
A simple Pong game made in Unity and C# for practice.

Play the web build at: https://mmacken42.github.io/MM_Pong/

Game design:
- Player and Enemy AI have same top speed (feels more fair and fun to me)
- Paddles have three sections, top, middle, and bottom
- If the ball hits the middle, it is somewhat slowed down and fires straight ahead, making it trivially easy for the other paddle to hit it
- If the ball hits the top or bottom of the paddle, this speeds up the ball and makes it bounce off the top or bottom walls
- If ball hits the top of paddle, it always goes up to the top wall
- If ball hits the bottom of paddle, it always goes down to bottom wall
- This offers players a greater degree of control over their return shot. Instead of just “being in the way” of the incoming ball, they are encouraged to think about the best return shot that may beat the enemy AI.
- This design utilizes classic Risk vs. Reward setup: players are encouraged to try to risk hitting the ball with the top or bottom of their paddle (and potentially miss it altogether) instead of taking the safe route of hitting the ball with the middle of the paddle.

Design of Enemy AI:
- Enemy AI uses raycasts fired from the ball to predict where the ball will land after the next 1-2 bounces and moves to that position
- Errors are then deliberately injected into the AI’s target position so that it isn’t perfect and can be beat
- AI errors increase as the ball speed increases, so it plays worse the longer the round goes on
- AI isn’t allowed to do repeated middle-of-paddle shots because that’s boring for the player

Note:
- There are still some bugs in this game around collision detection once the ball gets going at very high speeds. This is a classic issue with Unity's built-in physics. To solve it, I would disable the built-in collision and switch to a raycast-based system to detect objects for the ball to bounce off, but honestly I'd rather work on other projects right now and I don't think anyone will play this anyway. My main goal in working on this Pong game was to play around with how to make a fallible enemy AI and I'm pretty happy with how that turned out so I'm moving on.
