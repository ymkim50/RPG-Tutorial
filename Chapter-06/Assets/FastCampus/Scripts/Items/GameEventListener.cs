using UnityEngine;
using UnityEngine.Events;

namespace FastCampus.Datas
{
    public class GameEventListener : MonoBehaviour
    {
        public GameEventObject eventObject;
        public UnityEvent response;

        void OnEnable()
        {
            eventObject.RegisterListener(this);
        }

        void OnDisable()
        {
            eventObject.UnRegisterListener(this);
        }

        public void OnEventRaised()
        {
            response.Invoke();
        }
    }
}
