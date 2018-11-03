using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace WishfulDroplet {
	public class OperationQueueHelper {
		private static List<OperationQueueAndRunnerWrapper> _wrappers = new List<OperationQueueAndRunnerWrapper>();
		private static Transform _operationsContainer;


		public static void RunActionAfterSeconds(string name, float seconds, Action action) {
			OperationQueueAndRunnerWrapper wrapper = GetOrMakeOperationQueueAndRunnerWrapper();
			wrapper.operationQueue.name = name;

			wrapper.operationQueue.AddOperation(RoutineRunActionAfterSeconds(seconds, action));

			wrapper.operationQueue.Start(wrapper.operationRunner);
		}

		#region CONVENIENCE PRIVATE METHODS

		private static IEnumerator RoutineRunActionAfterSeconds(float seconds, Action action) {
			float timer = 0f;

			while(timer < seconds) {
				timer += Time.deltaTime;
				yield return null;
			}

			action();
		}

		#endregion

		private static OperationQueueAndRunnerWrapper GetOrMakeOperationQueueAndRunnerWrapper() {
			// Get a wrapper or make if there's none in the 
			// buffer
			if(_wrappers.Count > 0) {
				OperationQueueAndRunnerWrapper wrapper = _wrappers[0];
				_wrappers.RemoveAt(0);
				return wrapper;
			} else {
				return MakeOperationQueueAndRunnerWrapper();
			}
		}

		private static OperationQueueAndRunnerWrapper MakeOperationQueueAndRunnerWrapper() {
			// Make a non-destroyable container of the wrappers in the hierarchy
			// for a clean display
			if(_operationsContainer == null) {
				_operationsContainer = Utilities.CreateOrGetObject("OperationQueueHelper").GetComponent<Transform>();
				UnityEngine.Object.DontDestroyOnLoad(_operationsContainer.gameObject);
			}

			// Makes a wrapper and puts it in a container in the hierarchy
			OperationQueueAndRunnerWrapper wrapper = new OperationQueueAndRunnerWrapper() {
				operationQueue = new OperationQueue(),
				operationRunner = new GameObject("_Idle").AddComponent<OperationQueue.OperationRunner>(),
			};

			wrapper.operationRunner.GetComponent<Transform>().SetParent(_operationsContainer);

			return wrapper;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void RunOnStartup() {
			int initOperationsCount = 20;

			for(int i = 0; i < initOperationsCount; i++) {
				OperationQueueAndRunnerWrapper wrapper = MakeOperationQueueAndRunnerWrapper();
				_wrappers.Add(wrapper);

				// Callbacks
				wrapper.operationQueue.OnStart += () => {
					wrapper.operationRunner.name = wrapper.operationQueue.name;
				};

				wrapper.operationQueue.OnStop += () => {
					wrapper.operationRunner.name = "_Idle";
					_wrappers.Add(wrapper);
				};

				wrapper.operationQueue.OnFinish += () => {
					wrapper.operationRunner.name = "_Idle";
					_wrappers.Add(wrapper);
				};
			}
		}


		private class OperationQueueAndRunnerWrapper {
			public OperationQueue operationQueue;
			public OperationQueue.OperationRunner operationRunner;
		}
	}


	public class OperationQueue {
		public event Action				OnStart = delegate { };
		public event Action				OnStop = delegate { };
		public event Action				OnFinish = delegate { };
		public event Action				OnPause = delegate { };
		public event Action				OnResume = delegate { };

		private Queue<object>			_operations = new Queue<object>();
		private MonoBehaviour			_operationRunner;
		private Coroutine				_operationRoutine;

		public string					name { get; set; }
		public OperationQueueState		state { get; private set; }


		public object AddOperation(object operation) {
			_operations.Enqueue(operation);
			return operation;
		}

		public Action AddOperation(Action operation) {
			_operations.Enqueue(operation);
			return operation;
		}

		public IEnumerator AddOperation(IEnumerator operation) {
			_operations.Enqueue(operation);
			return operation;
		}

		public YieldInstruction AddOperation(YieldInstruction operation) {
			_operations.Enqueue(operation);
			return operation;
		}

		public Coroutine AddOperation(Coroutine operation) {
			_operations.Enqueue(operation);
			return operation;
		}

		public void Start(MonoBehaviour operationRunner) {
			if(state == OperationQueueState.Playing) return;

			state = OperationQueueState.Playing;
			OnStart();

			_operationRunner = operationRunner;
			_operationRoutine = _operationRunner.StartCoroutine(RoutineRunOperationQueue());
		}

		public void Stop() {
			if(state == OperationQueueState.Stopped) return;

			_operations.Clear();
			_operationRunner.StopCoroutine(_operationRoutine);

			state = OperationQueueState.Stopped;
			OnStop();
		}

		public void Resume() {
			state = OperationQueueState.Playing;
			OnResume();
		}

		public void Pause() {
			state = OperationQueueState.Paused;
			OnPause();
		}

		private IEnumerator RoutineRunOperationQueue() {
			while(_operations.Count != 0) {
				if(state == OperationQueueState.Paused) yield return null;

				object operation = _operations.Dequeue();

				Action action = operation as Action;
				if(action != null) {
					action();
					yield return null;
					continue;
				}

				IEnumerator enumerator = operation as IEnumerator;
				if(enumerator != null) {
					yield return _operationRunner.StartCoroutine(enumerator);
					continue;
				}

				YieldInstruction yieldInstruction = operation as YieldInstruction;
				if(yieldInstruction != null) {
					yield return yieldInstruction;
					continue;
				}

				Coroutine coroutine = operation as Coroutine;
				if(coroutine != null) {
					yield return coroutine;
					continue;
				}
			}

			state = OperationQueueState.Finished;
			OnFinish();
		}


		public enum OperationQueueState {
			Stopped,
			Playing,
			Paused,
			Finished
		}


		/// <summary>
		/// Dummy component to run operations.
		/// </summary>
		public class OperationRunner : MonoBehaviour { }
	}
}