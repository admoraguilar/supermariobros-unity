


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