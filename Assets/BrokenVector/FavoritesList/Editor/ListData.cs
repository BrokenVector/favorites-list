using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace BrokenVector.FavoritesList
{
    [CreateAssetMenu(fileName = Constants.DEFAULT_FILENAME, menuName = Constants.CREATE_MENU_OPTION, order = 1)]
    [System.Serializable]
    public class ListData : ScriptableObject
    {
        public List<ObjectReference> References = new List<ObjectReference>();

        private static List<ListData> listData = new List<ListData>();

        public void AddReference(ObjectReference obj)
        {
            References.Add(obj);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public void RemoveReference(ObjectReference obj)
        {
            References.Remove(obj);
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public void Clear()
        {
            References.Clear();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public static ListData LoadList(string path)
        {
            return AssetDatabase.LoadAssetAtPath(path, typeof(ListData)) as ListData;
        }

        public static List<string> GetAssetsLocation()
        {
            var guid = AssetDatabase.FindAssets("t:" + typeof(ListData).Name);
            var pathList = new List<string>();

            for (int i = guid.Length - 1; i >= 0; i--)
            {
                pathList.Add(AssetDatabase.GUIDToAssetPath(guid[i]));
            }

            return pathList;
        }

        public static void CreateNewListData()
        {
            ListData list = CreateInstance<ListData>();
            AssetDatabase.CreateAsset(list, "Assets/" + Constants.DEFAULT_FILENAME + ".asset");
            AssetDatabase.SaveAssets();
        }

    }
}
