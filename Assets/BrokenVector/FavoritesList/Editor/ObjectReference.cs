using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif
using Object = UnityEngine.Object;

using BrokenVector.FavoritesList.Utils;


namespace BrokenVector.FavoritesList
{
    [Serializable]
    public class ObjectReference : IDrawableObjectDataProvider, SearchUtils.ISearchable
    {

        [Serializable]
        internal enum ReferenceType
        {
            Scene, Project
        }

        [SerializeField]
        private ReferenceType type;
        [SerializeField]
        private Object actualReference;
        [SerializeField]
        private DrawableObjectData cachedDrawData;
        [SerializeField]
        private String[] cachedTypes;    // needed for the type search

        // Additional Scene Type Values
        [SerializeField]
        private string sceneGUID;
        [SerializeField]
        private int localIdentifier;


        public ObjectReference(Object reference)
        {
            actualReference = reference;

            UpdateCachedData();

            type = AssetDatabase.Contains(reference) ? ReferenceType.Project : ReferenceType.Scene;

            if (type == ReferenceType.Scene)
            {
                if (!(reference is GameObject))
                {
                    Debug.LogWarning(Constants.ASSET_NAME + ": Scene Object is not a GameObject");
                    return;
                }

#if UNITY_5_4_OR_NEWER
                sceneGUID = AssetUtils.GetSceneGUID(((GameObject) reference).scene);
#else
                sceneGUID = AssetUtils.GetSceneGUID(EditorApplication.currentScene);
#endif
                localIdentifier = AssetUtils.GetLocalIdentifier(reference);
            }
            else
            {
#if UNITY_5_4_OR_NEWER
                cachedDrawData.SceneData.IsScene = reference is SceneAsset;
                cachedDrawData.SceneData.Scene = reference as SceneAsset;
#endif
            }

        }

        public void UpdateCachedData()
        {
            if (actualReference == null)
                return;

            var content = AssetUtils.GetGUIContent(actualReference, MainWindow.DEFAULT_ICON);

            cachedDrawData.Name = content.text;
            cachedDrawData.Icon = content.image;

            var cachedTypes = new List<string>();
            cachedTypes.Add(actualReference.GetType().Name);
            var gameObject = actualReference as GameObject;
            if (gameObject != null)
            {
                foreach (var child in gameObject.GetComponents<Component>())
                {
                    cachedTypes.Add(child.GetType().Name);
                }
            }
            this.cachedTypes = cachedTypes.ToArray();
        }

        public void Select()
        {
            //if (!(type == ReferenceType.Scene && FindReferenceInScene()))
                //Debug.LogWarning(Constants.ASSET_NAME + ": Failed to load scene object refernce.");

            if (type == ReferenceType.Scene)
                FindReferenceInScene();

            EditorGUIUtility.PingObject(actualReference);
            Selection.activeObject = actualReference;
        }

        private bool LoadSceneOfObject()
        {
            if (type != ReferenceType.Scene)
                return false;

#if UNITY_5_4_OR_NEWER
            if (!AssetUtils.GetSceneGUID(SceneManager.GetActiveScene()).Equals(sceneGUID))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    var scene = EditorSceneManager.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGUID), OpenSceneMode.Single);
                    return scene.name != null;
                }
            }
#else
            if (!AssetUtils.GetSceneGUID(EditorApplication.currentScene).Equals(sceneGUID))
            {
                if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
                {
                    return EditorApplication.OpenScene(AssetDatabase.GUIDToAssetPath(sceneGUID));
                }
            }
#endif
            return true;
        }

        private bool FindReferenceInScene()
        {
            if (actualReference != null)
                return false;

            if (LoadSceneOfObject())
            {
                Object[] refs = AssetUtils.FindAllSceneObjects();
                actualReference = refs.FirstOrDefault(obj => AssetUtils.GetLocalIdentifier(obj).Equals(localIdentifier));
            }

            return actualReference != null;
        }

        // implements IDrawableObjectDataProvider
        public DrawableObjectData GetProvidedData()
        {
            // UpdateCachedData(); // called every frame, too expensive

            return cachedDrawData;
        }

        // implements SearchUtils.ISearchable
        public string GetSearchName()
        {
            return cachedDrawData.Name;
        }

        //implements SearchUtils.ISearchable
        public string[] GetTypes()
        {
            return cachedTypes;
        }

    }
}
