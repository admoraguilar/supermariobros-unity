using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;


[ExecuteInEditMode()]
public class BoxCastCollider2DDetector : Collider2DDetector {
    [Header("BoxCast Info")]
    public Placement CenterPlacement = Placement.FromPosition;
    public Vector2 Center = Vector2.zero;
    public Vector2 Offset = Vector2.zero;
    public Vector2 Size = Vector2.one;
    public Vector2 CastDirection = Vector2.one;
    public float SizeMultiplier = 1f;
    public float Distance = 1f;

    private List<Collider2D> oldColliders = new List<Collider2D>();


    protected override void UpdateDetector() {
        // Update center
        switch(CenterPlacement) {
            case Placement.FromPosition:
                Center = thisTransform.position;
                break;
        }

        oldColliders.Clear();
        oldColliders.AddRange(hitColliders);

        // Cast the box
        hitColliders.Clear();
        RaycastHit2D[] rHits = Physics2D.BoxCastAll(Center + Offset, Size * SizeMultiplier, 0f, CastDirection, Distance);

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
        Gizmos.DrawWireCube(Center + Offset, Size);
    }


    public enum Placement {
        Free,
        FromPosition
    }
}
