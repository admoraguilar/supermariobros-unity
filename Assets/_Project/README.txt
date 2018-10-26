WORKFLOW:
* Objects should only have one "Controller" script which drives the whole thing.
	* The "Controller" will have "Component" which could be shared with other "Controllers".
	* This is a kind of similar approach to Unreal's Actor-Component system.
	* Readings: http://slides.com/cratesmith/how-to-use-unity#/11/2
* You should make objects of type "Controller" through the Scene view.
	* Do not add "Controller" via the Project View as some controllers requires their components be their child object.
* As much as possible everything should be a prefab to make changes easier.
* References fields are exposed in the inspector in case some default reference values are missing, but it shouldn't be touched as much as possible.

=========================================

BUGS:
* Transitioning from small to big, when picking up a mushroom timescale stop and animation doesn't work.


=========================================

BUG FIXES:
* The issue of "Ghost vertices" on Box2D games:
	* Solution:
		* CompositeCollider2D - make the colliders as one surface instead of separate colliders per object.
		  This way there are no edges or seams to take into account.
	* Readings:
		* https://www.iforce2d.net/b2dtut/ghost-vertices
		* https://forum.unity.com/threads/boxcollider2d-colliding-with-edgecollider2d.223054/#post-1488478
		* https://gamedev.stackexchange.com/questions/146898/floor-is-made-up-of-tiles-with-their-own-box-colliders-pushable-item-gets-stuck
		* https://www.youtube.com/watch?v=NwPIoVW65pE
		* https://answers.unity.com/questions/1066094/sprite-with-box-collider-gets-stuck-on-nothing.html
* Stop falling through floors: (e.g Falling from a high ground and having the colliders stuck on ground contact)
	* Solution:
		* Make rigidbody collision detection "Continuous", than "Discrete". 
	* Readings:
		* http://johnstejskal.com/wp/how-to-stop-falling-through-floors-in-unity/