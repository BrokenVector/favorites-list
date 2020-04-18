using UnityEngine;
using System.Collections;
using UnityEditor;

namespace BrokenVector.FavoritesList
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ListData))]
    public class ListDataInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Favorites Lists Data");
        }
    }
}
