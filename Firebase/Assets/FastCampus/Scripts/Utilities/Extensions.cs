using UnityEngine;

namespace FastCampus
{
    public static class ExtensionMethods
    {
        public static Transform FindInChildren(this Transform self, string name)
        {
            int count = self.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform child = self.GetChild(i);
                if (child.name == name) return child;
                Transform subChild = child.FindInChildren(name);
                if (subChild != null) return subChild;
            }
            return null;
        }

        public static GameObject FindInChildren(this GameObject self, string name)
        {
            Transform transform = self.transform;
            Transform child = transform.FindInChildren(name);
            return child != null ? child.gameObject : null;
        }

        public static bool IsContain(this LayerMask self, int layer)
        {
            if (((1 << layer) & self) != 0)
            {
                return true;
            }

            return false;
        }
    }
}