using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
#endif

using BrokenVector.FavoritesList.Utils;

namespace BrokenVector.FavoritesList
{
    public class MainWindow : EditorWindow
    {

        // Constants
        private const float ICON_SIZE = 18;
        private const float CLEAR_BUTTON_WIDTH = 80f;
        private const float ICON_BUTTON_WIDTH = 30f;
        private const string SKIN_TOOLBAR = "Toolbar";
        private const string SKIN_BUTTON = "ToolbarButton";
        private const string BUTTON_ICON_ADD = "CreateAddNew";
        private const string BUTTON_ICON_SELECT = "Animation.FilterBySelection";

        // Statics
        private static Vector2 ICON_SPACE;
        private static float BORDER_SPACE;
        internal static Texture DEFAULT_ICON;
        private static Texture CLOSE_ICON;
        private static Texture WINDOW_ICON;

        // Fields
        private static ListData list;
        private static List<string> listNames = new List<string>();
        private static List<string> pathList = new List<string>();
        private static string selectedName;
        private static int index = 0;

        #region Editor Window
        [MenuItem(Constants.WINDOW_PATH), MenuItem(Constants.WINDOW_PATH_ALT)]
        private static void ShowWindow()
        {
            LoadResources();

            //var window = GetWindow(typeof(MainWindow)); // refocus an already opened window
            var window = CreateInstance<MainWindow>(); // allows creating multiple windows

#if UNITY_5_4_OR_NEWER
            window.titleContent = new GUIContent(Constants.ASSET_NAME, WINDOW_ICON);
#else
            window.title = Constants.ASSET_NAME;
#endif

            window.minSize = new Vector2(150, 200);
            window.position = Globals.Prefs.Get("position", new Rect(50, 50, 250, 375));

            window.Show();
        }

        private static void LoadResources()
        {
            pathList = ListData.GetAssetsLocation();

            if (index > pathList.Count - 1) {
                index = 0;
            }

            listNames.Clear();

            for (int i = 0; i < pathList.Count; i++)
            {
                var match = Regex.Match(pathList[i], @".*[\/\\](.*)\..*$");

                if (match.Success && match.Groups.Count >= 2)
                {
                    string name = match.Groups[1].Value;

                    if (listNames.Contains(name))
                        listNames.Add(pathList[i].Replace("/", "\u200A\u2215\u200B"));
                    else
                        listNames.Add(name);
                }
                else
                {
                    listNames.Add(pathList[i].Replace("/", "\u200A\u2215\u200B"));
                }
            }

            if (DEFAULT_ICON == null)
            {
                BORDER_SPACE = ICON_SPACE.x - ICON_SIZE * 2 - 25; // 25 because of scrollbar
                DEFAULT_ICON = EditorGUIUtility.IconContent("cs Script Icon").image;
                CLOSE_ICON = EditorGUIUtility.FindTexture("winbtn_mac_close_a");
                ICON_SPACE = GUIStyle.none.CalcSize(new GUIContent(DEFAULT_ICON));

#if UNITY_5_4_OR_NEWER
                WINDOW_ICON = Base64.FromBase64(Constants.WINDOW_ICON);
#endif
            }
        }
        #endregion

        #region Unity Events
        // opening the window / Unity
        private void OnEnable()
        {
            Initialize();
        }

        // called when closing the window
        private void OnDisable()
        {
            CleanUp();
        }

        // called when closing Unity
        private void OnDestroy()
        {
            CleanUp();
        }
        #endregion

        #region Instance Lifecycle
        private void Initialize()
        {
            LoadOrCreateData();
        }

        private void LoadOrCreateData()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        private void CleanUp()
        {
            Globals.Prefs.Set("position", this.position);

            // nothing to clean up here
        }
        #endregion

        #region UI
        private string searchText = "";
        private Vector2 scrollPos = Vector2.zero;
        private void OnGUI()
        {
            LoadResources();

            if (pathList.Count <= 0) {
                if (GUILayout.Button(new GUIContent("Create a Favorites List"), GUILayout.MinHeight(50f)))
                {
                    EditorApplication.ExecuteMenuItem("Assets/Create/" + Constants.CREATE_MENU_OPTION);
                }
                return;
            }

            GUILayout.BeginHorizontal(SKIN_TOOLBAR);
            index = EditorGUILayout.Popup(index, listNames.ToArray(), EditorStyles.toolbarDropDown);

            selectedName = listNames[index];
            list = ListData.LoadList(pathList[index]);

            if (GUILayout.Button(EditorGUIUtility.FindTexture(BUTTON_ICON_SELECT), SKIN_BUTTON, GUILayout.MaxWidth(ICON_BUTTON_WIDTH)))
            {
                EditorGUIUtility.PingObject(list);
            }
            if (GUILayout.Button(EditorGUIUtility.FindTexture(BUTTON_ICON_ADD), SKIN_BUTTON, GUILayout.MaxWidth(ICON_BUTTON_WIDTH)))
            {
                EditorApplication.ExecuteMenuItem("Assets/Create/" + Constants.CREATE_MENU_OPTION);
            }
            GUILayout.EndHorizontal();

            searchText = SearchUtils.BeginSearchbar(this, searchText);
            if (SearchUtils.Button(new GUIContent("Remove All"), GUILayout.MaxWidth(CLEAR_BUTTON_WIDTH)))
            {
                list.Clear();

                Repaint();
            }
            SearchUtils.EndSearchbar();


            using (var scrollView = new GUILayout.ScrollViewScope(scrollPos, false, false))
            {
                scrollPos = scrollView.scrollPosition;

                for (int i = list.References.Count - 1; i >= 0; i--)
                {
                    var reference = list.References[i];

                    if (SearchUtils.IsSearched(reference, searchText))
                        if (DrawElement(reference))
                            list.RemoveReference(reference);
                }

                list.References.RemoveAll(reference => reference == null);
            }

            DetectDragNDrop();
        }

        // returns true if the element should be removed
        private bool DrawElement(ObjectReference reference)
        {
            if (reference == null)
                return true;

            var drawData = reference.GetProvidedData();
            var sceneData = drawData.SceneData;

            float inspectorWidth = EditorGUIUtility.currentViewWidth;
            float buttonWidth = inspectorWidth + BORDER_SPACE;

            buttonWidth = sceneData.IsScene ? (buttonWidth / 3) - 2.5f : buttonWidth;

            GUILayout.BeginHorizontal();

#if UNITY_5_4_OR_NEWER
            GUILayout.Label(drawData.Icon, GUILayout.Width(ICON_SIZE), GUILayout.Height(ICON_SIZE), GUILayout.MaxWidth(ICON_SIZE), GUILayout.MaxHeight(ICON_SIZE));
#else
            buttonWidth += ICON_SIZE + 10;
#endif

            if (GUILayout.Button(drawData.Name, GUILayout.MaxWidth(buttonWidth), GUILayout.Width(buttonWidth), GUILayout.ExpandWidth(false)))
            {
                reference.UpdateCachedData();
                reference.Select();
            }

#if UNITY_5_4_OR_NEWER
            if (sceneData.IsScene)
            {
                var scene = sceneData.Scene;
                // Playmode
                if (Application.isPlaying)
                {
                    if (GUILayout.Button("Load", GUILayout.MaxWidth(buttonWidth * 2 + 2.5f), GUILayout.Width(buttonWidth * 2 + 2.5f), GUILayout.ExpandWidth(false)))
                    {
                        SceneManager.LoadScene(AssetDatabase.GetAssetOrScenePath(scene));
                    }
                }
                else
                {
                    if (GUILayout.Button("Load", GUILayout.MaxWidth(buttonWidth), GUILayout.Width(buttonWidth), GUILayout.ExpandWidth(false)))
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(scene), OpenSceneMode.Single);
                    }

                    if (GUILayout.Button("Play", GUILayout.MaxWidth(buttonWidth), GUILayout.Width(buttonWidth), GUILayout.ExpandWidth(false)))
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            EditorSceneManager.OpenScene(AssetDatabase.GetAssetOrScenePath(scene), OpenSceneMode.Single);
                        EditorApplication.isPlaying = true;
                    }
                }
            }
#endif

            var alignmentBckp = GUIStyle.none.alignment;
            GUIStyle.none.alignment = TextAnchor.LowerCenter;
            if (GUILayout.Button(CLOSE_ICON, GUIStyle.none, GUILayout.Width(ICON_SIZE), GUILayout.Height(ICON_SIZE)))
                return true;

            GUIStyle.none.alignment = alignmentBckp;
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            return false;
        }

        private void DetectDragNDrop()
        {
            if (pathList.Count <= 0) {
                return;
            }

            var eventType = Event.current.type;
            if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (eventType == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    if (DragAndDrop.objectReferences.Length > 0)
                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            var reference = new ObjectReference(obj);
                            list.AddReference(reference);
                        }
                }

                Event.current.Use();
            }
        }
        #endregion

    }
}
