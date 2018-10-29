using UnityEngine;
using System;


namespace WishfulDroplet.Components {
	public class CollisionEvents2D : MonoBehaviour {
		public event Action<Collision2D>		OnCollisionEnter2DCallback = delegate { };
		public event Action<Collision2D>		OnCollisionStay2DCallback = delegate { };
		public event Action<Collision2D>		OnCollisionExit2DCallback = delegate { };
		public event Action<Collider2D>			OnTriggerEnter2DCallback = delegate { };
		public event Action<Collider2D>			OnTriggerStay2DCallback = delegate { };
		public event Action<Collider2D>			OnTriggerExit2DCallback = delegate { };


		private void OnCollisionEnter2D(Collision2D collision) {
			OnCollisionEnter2DCallback(collision);
		}

		private void OnCollisionStay2D(Collision2D collision) {
			OnCollisionStay2DCallback(collision);
		}

		private void OnCollisionExit2D(Collision2D collision) {
			OnCollisionExit2DCallback(collision);
		}

		private void OnTriggerEnter2D(Collider2D collision) {
			OnTriggerEnter2DCallback(collision);
		}

		private void OnTriggerStay2D(Collider2D collision) {
			OnTriggerStay2DCallback(collision);
		}

		private void OnTriggerExit2D(Collider2D collision) {
			OnTriggerExit2DCallback(collision);
		}
	}
}