using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FastCampus.Datas
{
    [CreateAssetMenu]
    public class GameEventObject : ScriptableObject
    {
        private List<GameEventListener> eventListeners = new List<GameEventListener>();

        public void Raise()
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
            {
                eventListeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnRegisterListener(GameEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
}
