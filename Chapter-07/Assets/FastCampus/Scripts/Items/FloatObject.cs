using UnityEngine;

namespace FastCampus.Datas
{
    [CreateAssetMenu]
    public class FloatObject : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string developmentDescription = "";
#endif  // UNITY_EDITOR

        [SerializeField]
        private float value;

        public void SetValue(float value)
        {
            this.value = value;
        }

        public void SetValue(FloatObject objectValue)
        {
            this.value = objectValue.value;
        }

        public void AddValue(float amount)
        {
            this.value += amount;
        }

        public void AddValue(FloatObject objectValue)
        {
            this.value += objectValue.value;
        }
    }
}