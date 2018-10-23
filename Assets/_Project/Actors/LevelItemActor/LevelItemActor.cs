using UnityEngine;
using System;
using WishfulDroplet;


public class LevelItemActor : LevelItemActor<LevelItemActor, LevelItemActor.LevelItemBrain> {
    public abstract class LevelItemBrain : ActorBrain<LevelItemActor> {

    }
}


public class LevelItemActor<T, U> : Actor<T, U> 
    where T : Actor<T, U>
    where U : ActorBrain<T> {
    [InspectorNote("Level Item Actor")]
    public int test = 0;
}



//public class LevelItemActor : Actor<LevelItemActor, LevelItemActor.LevelItemBrain> {
//    [InspectorNote("Level Item Actor")]
//    [Header("Data")]
//    public float moveSpeed;


//    [Serializable]
//    public abstract class LevelItemBrain : ActorBrain<LevelItemActor> {

//    }


//    [Serializable]
//    public abstract class LevelItemState : State<LevelItemActor> {
//        public override void DoEnter(LevelItemActor owner) {}
//        public override void DoExit(LevelItemActor owner) {}
//    }


//    public class MoveStates {
//        [Serializable]
//        public class Move : LevelItemState {

//        }
//    }
//}