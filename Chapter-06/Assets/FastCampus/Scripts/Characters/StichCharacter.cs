using System.Collections.Generic;
using UnityEngine;

namespace FastCampus.Characters
{
    public class StichCharacter : MonoBehaviour
    {
        public List<GameObject> equipItemPrefabs = new List<GameObject>();

        //scripts
        private Stitcher stitcher;

        private List<GameObject> equippedItems = new List<GameObject>();
        //public GameObject equippedHead;

        #region Monobehaviour
        public void Awake()
        {
            stitcher = new Stitcher();
        }

        // Start is called before the first frame update
        void Start()
        {
            foreach (GameObject itemPrefab in equipItemPrefabs)
            {
                //GameObject equippedItem = EquipItem(itemPrefab, equippedItem)
                equippedItems.Add(EquipItem(itemPrefab));
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        private GameObject EquipItem(GameObject itemPrefab)
        {
            if (itemPrefab == null)
            {
                return null;
            }


            if (itemPrefab.GetComponentsInChildren<SkinnedMeshRenderer>().Length > 0)
            {
                GameObject item = (GameObject)GameObject.Instantiate(itemPrefab);
                GameObject equippedItem = stitcher.Stitch(item, this.gameObject);
                GameObject.Destroy(item);
            }

            MeshRenderer[] meshRenderers = itemPrefab.GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers.Length > 0)
            {
                stitcher.StichMeshRenderer(itemPrefab, this.gameObject);
            }

            return null;
        }
        #endregion MonoBehaviour
    }

    public class Stitcher
    {
        /// <summary>
        /// Stitch clothing onto an avatar.  Both clothing and avatar must be instantiated however clothing may be destroyed after.
        /// </summary>
        /// <param name="sourceClothing"></param>
        /// <param name="targetAvatar"></param>
        /// <returns>Newly created clothing on avatar</returns>

        public GameObject Stitch(GameObject sourceClothing, GameObject targetAvatar)
        {
            TransformCatalog boneCatalog = new TransformCatalog(targetAvatar.transform);
            SkinnedMeshRenderer[] skinnedMeshRenderers = sourceClothing.GetComponentsInChildren<SkinnedMeshRenderer>();
            GameObject targetClothing = AddChild(sourceClothing, targetAvatar.transform);
            foreach (SkinnedMeshRenderer sourceRenderer in skinnedMeshRenderers)
            {
                SkinnedMeshRenderer targetRenderer = AddSkinnedMeshRenderer(sourceRenderer, targetClothing);
                targetRenderer.bones = TranslateTransforms(sourceRenderer.bones, boneCatalog);
            }

            return targetClothing;
        }

        public GameObject StichMeshRenderer(GameObject item, GameObject targetAvatar)
        {
            MeshRenderer[] meshRenderers = item.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                Transform source = meshRenderer.transform;
                Debug.Log(source);

                // Find parent by name
                string parentName = meshRenderer.transform.parent.name;

                Transform parent = targetAvatar.transform.FindInChildren(parentName);

                //Transform parent = targetAvatar.transform.Find(parentName);
                GameObject instantiatedItem = GameObject.Instantiate<GameObject>(meshRenderer.gameObject, parent);
                //instantiatedItem.transform.parent = parent;
                Transform target = instantiatedItem.transform;
                Debug.Log(target);
            }

            return targetAvatar;
        }


        private GameObject AddChild(GameObject source, Transform parent)
        {
            GameObject target = new GameObject(source.name);
            target.transform.parent = parent;
            target.transform.localPosition = source.transform.localPosition;
            target.transform.localRotation = source.transform.localRotation;
            target.transform.localScale = source.transform.localScale;
            return target;
        }

        private SkinnedMeshRenderer AddSkinnedMeshRenderer(SkinnedMeshRenderer source, GameObject parent)
        {
            SkinnedMeshRenderer target = parent.AddComponent<SkinnedMeshRenderer>();
            target.sharedMesh = source.sharedMesh;
            target.materials = source.materials;
            return target;
        }

        private MeshRenderer AddMeshRenderer(MeshRenderer source, GameObject parent)
        {
            MeshRenderer target = parent.AddComponent<MeshRenderer>();
            MeshFilter filter = parent.AddComponent<MeshFilter>();
            //filter.mesh = source.mesh
            //target.sharedMesh = source.sharedMesh;
            target.materials = source.materials;
            return target;
        }

        private Transform[] TranslateTransforms(Transform[] sources, TransformCatalog transformCatalog)
        {
            Transform[] targets = new Transform[sources.Length];
            for (int index = 0; index < sources.Length; index++)
                targets[index] = DictionaryExtensions.Find(transformCatalog, sources[index].name);
            return targets;
        }


        #region TransformCatalog
        private class TransformCatalog : Dictionary<string, Transform>
        {
            #region Constructors
            public TransformCatalog(Transform transform)
            {
                Catalog(transform);
            }
            #endregion

            #region Catalog
            private void Catalog(Transform transform)
            {
                if (ContainsKey(transform.name))
                {
                    Remove(transform.name);
                    Add(transform.name, transform);
                }
                else
                    Add(transform.name, transform);
                foreach (Transform child in transform)
                    Catalog(child);
            }
            #endregion
        }
        #endregion


        #region DictionaryExtensions
        private class DictionaryExtensions
        {
            public static TValue Find<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key)
            {
                TValue value;
                source.TryGetValue(key, out value);
                return value;
            }
        }
        #endregion

    }
}