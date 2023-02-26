# Celesta

Celesta is a small scripting language that communicates with the Astralbrew Engine.

```basic
assign(Actor, bullet, id(0))
assign(Actor, enemy, id(1))
assign(Integer, score, id(2))

bullet.move(0, 1)

if (bullet.touches(enemy)) then
	score = score + 1
	show_dialog("Enemy hit");
else
	show_dialog("Try again");
endif
```
