using UnityEngine;
using System;
using System.Collections.Generic;


public class AnimationEventsHook : MonoBehaviour {
    private Dictionary<string, List<Action>> events = new Dictionary<string, List<Action>>(); 


    public void FireEvent(string name) {
        if(!events.ContainsKey(name)) return;

        foreach(Action animationEvent in events[name]) {
            if(animationEvent != null) {
                animationEvent();
            }
        }
    }

    public void AddEvent(string name, Action onEventFire) {
        if(!events.ContainsKey(name)) {
            events[name] = new List<Action>();
        }

        events[name].Add(onEventFire);
    }

    public void RemoveEvent(string name, Action onEventFire) {
        List<Action> animationEvents = null;
        if(events.TryGetValue(name, out animationEvents)) {
            animationEvents.Remove(onEventFire);
        }
    }
}
