using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor;
#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
#endif
using Object = UnityEngine.Object;

namespace BrokenVector.FavoritesList.Utils
{
    public static class AssetUtils
    {

        /// <summary>
        /// Returns a unique indetifier for a Scene object
        /// </summary>
#if UNITY_5_4_OR_NEWER
        public static string GetSceneGUID(Scene scene)
        {
            return AssetDatabase.AssetPathToGUID(scene.path);
        }
#else
        public static string GetSceneGUID(string path)
        {
            return AssetDatabase.AssetPathToGUID(path);
        }
#endif

        /// <summary>
        /// Returns the local identifier of an object
        /// </summary>
        /// <remarks>
        /// might be used to indetify object references in scenes or prefabs
        /// </remarks>
        public static int GetLocalIdentifier(Object obj)
        {
            var serializedObject = new SerializedObject(obj);

            PropertyInfo inspectorMode = serializedObject.GetType().GetProperty("inspectorMode", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            inspectorMode.GetSetMethod(true).Invoke(serializedObject, new object[] { InspectorMode.Debug });

            return serializedObject.FindProperty("m_LocalIdentfierInFile").intValue;
        }

        /// <summary>
        /// Finds all (also inactive) objects of the active scene
        /// </summary>
        public static GameObject[] FindAllSceneObjects()
        {
            // return SceneManager.GetActiveScene().GetRootGameObjects().SelectMany(go => go.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject)).ToArray();
            // performance wise better:
            var list = new List<GameObject>();

#if UNITY_5_4_OR_NEWER
            foreach (var rootGo in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (rootGo == null)
                    continue;
#else
            foreach (var rootGo in GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[])
            {
                if (rootGo == null || rootGo.transform.parent != null)
                    continue;
#endif

                list.Add(rootGo);

                foreach (var childCmp in rootGo.GetComponentsInChildren<Transform>(true))
                {
                    list.Add(childCmp.gameObject);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Looks up the path for an asset named fileName.
        /// If it doesnt find, it looks for an aset of the type fallbackType.
        /// </summary>
        public static string GetAssetLocation(string fileName, Type fallbackType)
        {
            string path = fileName;
            var search = AssetDatabase.FindAssets(fileName);
            if (search.Length > 0)
            {
                path = AssetDatabase.GUIDToAssetPath(search[0]);
            }
            else
            {
                var className = fallbackType.Name;
                var search2 = AssetDatabase.FindAssets(className);
                if (search2.Length > 0)
                {
                    var classPath = AssetDatabase.GUIDToAssetPath(search2[0]);
                    var folderPath = classPath.Substring(0, classPath.Length - className.Length - 3); // 3 = extension (".cs")
                    path = Path.Combine(folderPath, fileName + ".asset");
                }
            }

            return path;
        }

        /// <summary>
        /// Returns the icon and name of any object
        /// </summary>
        public static GUIContent GetGUIContent(Object obj, Texture defaultIcon = null)
        {
            var content = EditorGUIUtility.ObjectContent(obj, obj.GetType());

            if (content == null)
            {
                content = new GUIContent
                {
                    image = defaultIcon,
                    text = obj.name
                };
            }

            return content;
        }

    }
}
