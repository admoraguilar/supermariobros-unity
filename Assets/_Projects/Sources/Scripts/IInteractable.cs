using UnityEngine;
using System;


public interface IInteractable {
    event Action<string> OnInteract;

    void Interact<T>(T details, string message);
}
