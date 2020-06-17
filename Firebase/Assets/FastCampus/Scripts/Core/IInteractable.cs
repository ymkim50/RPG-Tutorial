using UnityEngine;

namespace FastCampus.Core
{
    public interface IInteractable
    {
        float Distance
        {
            get;
        }

        void Interact(GameObject other);
        void StopInteract(GameObject other);
    }
}
