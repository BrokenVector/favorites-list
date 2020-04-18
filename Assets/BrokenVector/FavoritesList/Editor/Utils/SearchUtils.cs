using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BrokenVector.FavoritesList.Utils
{
   public static class SearchUtils
    {

        public interface ISearchable
        {
            string GetSearchName();
            string[] GetTypes();
        }

        private const string SKIN_TOOLBAR = "Toolbar";
        private const string SKIN_SEARCH = "ToolbarSeachTextField";
        private const string SKIN_BUTTON = "ToolbarButton";
        private const string SKIN_CANCELBUTTON = "ToolbarSeachCancelButton";


        public static string BeginSearchbar(EditorWindow window, string searchText)
        {
            GUILayout.BeginHorizontal(SKIN_TOOLBAR);

            searchText = GUILayout.TextField(searchText, SKIN_SEARCH);
            if (GUILayout.Button("", SKIN_CANCELBUTTON) || Event.current.keyCode == KeyCode.Escape)
            {
                searchText = "";
                GUI.FocusControl(null);
                window.Repaint();
            }

            return searchText;
        }

        public static void EndSearchbar()
        {
            GUILayout.EndHorizontal();
        }

        public static bool Button(GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.Button(content, SKIN_BUTTON, options);
        }

        public static bool IsSearched(ISearchable searchable, string searchQuery)
        {
            if (searchable == null)
                return false;

            var typeQuery = SearchTypes(ref searchQuery);

            bool typeQueryEmpty = typeQuery.Count == 0;
            bool searchQueryEmpty = string.IsNullOrEmpty(searchQuery);


            if (searchQueryEmpty)
            {
                if (typeQueryEmpty)
                    return true;
                else
                {
                    // filter asset with types
                }

            }
            else
            {
                if (ContainsWord(searchQuery, searchable.GetSearchName()))
                {
                    if (typeQueryEmpty)
                        return true;
                    else
                    {
                        // fillter asset with type
                    }
                }
                else
                {
                    return false;
                }
            }

            //if (!typeQueryEmpty) // expresssion is always true, because of the code above
            {
                foreach (var type in typeQuery)
                {
                    foreach (var ct in searchable.GetTypes())
                    {
                        if (ContainsWord(type, ct))
                            return true;
                    }
                }

                return false;
            }
        }

        public static bool IsSearched(UnityEngine.Object obj, string searchQuery)
        {
            if (obj == null)
                return false;

            var typeQuery = SearchTypes(ref searchQuery);

            bool typeQueryEmpty = typeQuery.Count == 0;
            bool searchQueryEmpty = string.IsNullOrEmpty(searchQuery);

            if (searchQueryEmpty && typeQueryEmpty)
                return true;

            if (!ContainsWord(searchQuery, obj.name))
                return false;

            if (!typeQueryEmpty)
            {
                if (obj is GameObject)
                {
                    foreach (var type in typeQuery)
                        if (FilterComponents(type, (GameObject) obj))
                            return true;
                }
                else
                {
                    foreach (var type in typeQuery)
                        if (type.Equals(obj.GetType().Name, StringComparison.InvariantCultureIgnoreCase))
                            return true;
                }
            }

            return false;
        }

        private static List<string> SearchTypes(ref string searchQuery)
        {
            // search query with type search support (works the same as the unity built-in search)
            List<string> typeQuery = new List<string>();
            if (searchQuery.Contains("t:"))
            {
                bool ignoreFirst = !searchQuery.StartsWith("t:");

                var splitted = searchQuery.Split(new[] { "t:" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < splitted.Length; i++)
                {
                    if (ignoreFirst && i == 0)
                        continue;

                    string part = splitted[i];
                    var idx = part.IndexOf(' ');
                    if (idx > 0)
                        part = part.Substring(0, idx);

                    if (!string.IsNullOrEmpty(part))
                    {
                        typeQuery.Add(part);
                        searchQuery = searchQuery.Replace(part, "");  // remove typequery from searchquery
                    }

                }
                searchQuery = searchQuery.Replace("t:", "");
            }

            return typeQuery;
        }

        private static bool ContainsWord(string input, string containedWord)
        {
            var splitted = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in splitted)
            {
                if (containedWord.IndexOf(part, 0, StringComparison.OrdinalIgnoreCase) == -1)
                    return false;
            }
            return true;
        }

        private static bool FilterComponents(string searchQuery, GameObject go)
        {
            foreach (var component in go.GetComponents<Component>())
            {
                var type = component.GetType().Name;
                if (searchQuery.Equals(type, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

    }
}
