Task:
1) On the stage there is a tank (with health, protection and speed of movement)
2) The tank must be able to move around the stage, turn (arrows on the keyboard)
 and be able to shoot (button X)
3) Weapons at the tank should be of several types (with different appearance and
 damage)
4) The tank should have the ability to change the weapon (buttons Q and W)

1) Monsters should be of several types (with different appearance, amount of health,
 damage, protection and speed of movement)
2) Monsters should be born randomly behind the scene and go to the tank
3) There must be no more than 10 monsters at a time, at the death of one should be
 born the next
4) When a bullet hits a monster, his health should decrease correspondingly to the
 damage from the bullet and the defense of the monster.
5) In the event of a collision with a tank, the health of the tank should decrease
 accordingly with the protection of the tank and damage from the monster

Scene size should be limited. Optionally, various obstacles can be found on the
 stage. The appearance can be indicated by a picture or a color. Calculation of
 damage: health = health - damage * protection (0 ... 1).

Implemented:
Control: Left, Right, Up, Down
Weapon Fire: x, mouseLeft
Weapon Change: 1, 2, 3
Weapon Change cyclically: q, w