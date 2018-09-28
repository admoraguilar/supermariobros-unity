using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;


[ExecuteInEditMode()]
public class RayCastCollider2DDetector : Collider2DDetector {
    [Header("Raycast Info")]
    public Placement OriginPlacement = Placement.FromPosition;
    public Vector2 Origin = Vector2.zero;
    public Vector2 Offset = Vector2.zero;
    public Vector2 CastDirection = Vector2.up;
    public float Distance = 1f;

    private List<Collider2D> oldColliders = new List<Collider2D>();


    protected override void UpdateDetector() {
        // Update origin
        switch(OriginPlacement) {
            case Placement.FromPosition:
                Origin = thisTransform.position;
                break;
        }

        oldColliders.Clear();
        oldColliders.AddRange(hitColliders);

        // Cast the box
        hitColliders.Clear();
        RaycastHit2D[] rHits = Physics2D.RaycastAll(Origin + Offset, CastDirection, Distance);

        // Filter cast hits
        for(int i = 0; i < rHits.Length; i++) {
            if(toIgnores.Contains(rHits[i].collider)) continue;
            hitColliders.Add(rHits[i].collider);
        }

        // Call events
        for(int i = 0; i < oldColliders.Count; i++) {
            if(!hitColliders.Contains(oldColliders[i])) RaiseOnColliderExit(oldColliders[i]);
        }

        for(int i = 0; i < hitColliders.Count; i++) {
            if(oldColliders.Contains(hitColliders[i])) RaiseOnColliderStay(hitColliders[i]);
            else RaiseOnColliderEnter(hitColliders[i]);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Origin + Offset, CastDirection * Distance);
    }


    public enum Placement {
        Free,
        FromPosition
    }
}
