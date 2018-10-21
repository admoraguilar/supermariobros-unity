using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using ActionDelegate = System.Action;


namespace WishfulDroplet {
    public static class ActionTemplates {
        public static void RunActionNextFrame(string name, ActionDelegate action) {
            var act = new ActionQueue(name);
            act.AddAction(new WaitForEndOfFrame());
            act.AddAction(action);
            act.Start();
        }

        public static void RunActionAfterSeconds(string name, float seconds, ActionDelegate action) {
            var act = new ActionQueue(name);
            act.AddAction(new WaitForSeconds(seconds));
            act.AddAction(action);
            act.Start();
        }
    }

    /// <summary>
    /// Can be used for easily making a coroutine outside a monobehaviour callable.
    /// 
    /// Example usage:
    /// ActionQueue actions = new ActionQueue();
    /// actions.AddAction(MyCoroutine());
    /// actions.Start();
    /// </summary>
    public class ActionQueue {
        public ActionQueue(string name = "Action", bool dontDestroyOnLoad = true) {
            ActionName = string.Format("[Action]: {0}", name);
            DontDestroyOnLoad = dontDestroyOnLoad;
        }

        public MonoBehaviour ActionRunner { get; private set; }

        private Queue<Action> actions = new Queue<Action>();
        private Coroutine routineAction;

        public string ActionName { get; private set; }
        public bool DontDestroyOnLoad { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsPaused { get; private set; }


        public ActionDelegate AddAction(ActionDelegate action) {
            actions.Enqueue(new Action(action));
            return action;
        }

        public IEnumerator AddAction(IEnumerator action) {
            actions.Enqueue(new Action(action));
            return action;
        }

        public YieldInstruction AddAction(YieldInstruction action) {
            actions.Enqueue(new Action(action));
            return action;
        }

        public Coroutine AddAction(Coroutine action) {
            actions.Enqueue(new Action(action));
            return action;
        }

        /// <summary>
        /// Starts the action.
        /// </summary>
        /// <returns>The action runner, useful if you need to do something with the runner like attaching a component or something.</returns>
        public MonoBehaviour Start() {
            Assert.IsFalse(IsStarted, "ActionQueue has already been started.");
            if(IsStarted) return ActionRunner;

            IsPaused = false;
            IsStarted = true;

            if(!ActionRunner) {
                ActionRunner = new GameObject(ActionName).AddComponent<ActionRunner>();
                if(DontDestroyOnLoad) Object.DontDestroyOnLoad(ActionRunner);
            }

            routineAction = ActionRunner.StartCoroutine(RoutineRunQueue());
            return ActionRunner;
        }

        /// <summary>
        /// Stops the action.
        /// </summary>
        public void Stop() {
            IsPaused = false;
            IsStarted = false;

            actions.Clear();
            if(ActionRunner) {
                ActionRunner.StopCoroutine(routineAction);
                Object.Destroy(ActionRunner.gameObject);
            }
        }

        /// <summary>
        /// Resumes the action.
        /// </summary>
        public void Resume() {
            IsPaused = false;
        }

        /// <summary>
        /// Pauses the action.
        /// </summary>
        public void Pause() {
            IsPaused = true;
        }

        private IEnumerator RoutineRunQueue() {
            while(actions.Count != 0) {
                if(IsPaused) yield return null;

                Action a = actions.Dequeue();

                switch(a.GetActionType()) {
                    case "Delegate":
                        a.GetActionDelegate()();
                        yield return null;
                        break;
                    case "Routine":
                        yield return ActionRunner.StartCoroutine(a.GetActionRoutine());
                        break;
                    case "YieldInstruction":
                        yield return a.GetActionYieldInstruction();
                        break;
                    case "Coroutine":
                        yield return a.GetActionCoroutine();
                        break;
                }
            }

            IsStarted = false;
            IsPaused = false;

            Object.Destroy(ActionRunner.gameObject);
        }


        public class Action {
            public Action(ActionDelegate action) {
                actionDelegate = action;
                actionType = "Delegate";
            }

            public Action(IEnumerator action) {
                actionRoutine = action;
                actionType = "Routine";
            }

            public Action(YieldInstruction action) {
                actionYieldInstruction = action;
                actionType = "YieldInstruction";
            }

            public Action(Coroutine action) {
                actionCoroutine = action;
                actionType = "Coroutine";
            }

            private ActionDelegate actionDelegate;
            private IEnumerator actionRoutine;
            private YieldInstruction actionYieldInstruction;
            private Coroutine actionCoroutine;
            private string actionType;


            public string GetActionType() {
                return actionType;
            }

            public ActionDelegate GetActionDelegate() {
                return actionDelegate;
            }

            public IEnumerator GetActionRoutine() {
                return actionRoutine;
            }

            public YieldInstruction GetActionYieldInstruction() {
                return actionYieldInstruction;
            }

            public Coroutine GetActionCoroutine() {
                return actionCoroutine;
            }
        }
    }


    /// <summary>
    /// Dummy component for ActionQueue.
    /// </summary>
    public class ActionRunner : MonoBehaviour { }
}