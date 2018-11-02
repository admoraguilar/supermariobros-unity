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
        public MonoBehaviour			actionRunner { get; private set; }

        private Queue<Action>			_actions = new Queue<Action>();
        private Coroutine				_routineAction;

        public string					actionName { get; private set; }
        public bool						dontDestroyOnLoad { get; private set; }
        public bool						isStarted { get; private set; }
        public bool						isPaused { get; private set; }


		public ActionQueue(string name = "Action", bool dontDestroyOnLoad = true) {
			actionName = string.Format("[Action]: {0}", name);
			this.dontDestroyOnLoad = dontDestroyOnLoad;
		}

		public ActionDelegate AddAction(ActionDelegate action) {
            _actions.Enqueue(new Action(action));
            return action;
        }

        public IEnumerator AddAction(IEnumerator action) {
            _actions.Enqueue(new Action(action));
            return action;
        }

        public YieldInstruction AddAction(YieldInstruction action) {
            _actions.Enqueue(new Action(action));
            return action;
        }

        public Coroutine AddAction(Coroutine action) {
            _actions.Enqueue(new Action(action));
            return action;
        }

        /// <summary>
        /// Starts the action.
        /// </summary>
        /// <returns>The action runner, useful if you need to do something with the runner like attaching a component or something.</returns>
        public MonoBehaviour Start() {
            Assert.IsFalse(isStarted, "ActionQueue has already been started.");
            if(isStarted) return actionRunner;

            isPaused = false;
            isStarted = true;

            if(!actionRunner) {
                actionRunner = new GameObject(actionName).AddComponent<ActionRunner>();
                if(dontDestroyOnLoad) Object.DontDestroyOnLoad(actionRunner);
            }

            _routineAction = actionRunner.StartCoroutine(RoutineRunQueue());
            return actionRunner;
        }

        /// <summary>
        /// Stops the action.
        /// </summary>
        public void Stop() {
            isPaused = false;
            isStarted = false;

            _actions.Clear();
            if(actionRunner) {
                actionRunner.StopCoroutine(_routineAction);
                Object.Destroy(actionRunner.gameObject);
            }
        }

        /// <summary>
        /// Resumes the action.
        /// </summary>
        public void Resume() {
            isPaused = false;
        }

        /// <summary>
        /// Pauses the action.
        /// </summary>
        public void Pause() {
            isPaused = true;
        }

        private IEnumerator RoutineRunQueue() {
            while(_actions.Count != 0) {
                if(isPaused) yield return null;

                Action a = _actions.Dequeue();

                switch(a.GetActionType()) {
                    case "Delegate":
                        a.GetActionDelegate()();
                        yield return null;
                        break;
                    case "Routine":
                        yield return actionRunner.StartCoroutine(a.GetActionRoutine());
                        break;
                    case "YieldInstruction":
                        yield return a.GetActionYieldInstruction();
                        break;
                    case "Coroutine":
                        yield return a.GetActionCoroutine();
                        break;
                }
            }

            isStarted = false;
            isPaused = false;

            Object.Destroy(actionRunner.gameObject);
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