using UnityEditor;
using UnityEngine;

namespace BrokenVector.FavoritesList.Utils
{
    /// <summary>
    /// A wrapper for EditorPrefs, which prefixes the assetname
    /// </summary>
	public class AssetPrefs
	{
        private const char SEPERATOR = '.';

        private string asset;

        private string prefix
        {
            get
            {
                return asset + SEPERATOR;
            }
        }

        public AssetPrefs(string asset)
        {
            this.asset = asset;
        }

        #region string
        public void Set(string key, string val)
        {
            EditorPrefs.SetString(CreatePath(key), val);
        }

        public string Get(string key, string def)
        {
            return EditorPrefs.GetString(CreatePath(key), def);
        }
        #endregion

        #region float
        public void Set(string key, float val)
        {
            EditorPrefs.SetFloat(CreatePath(key), val);
        }

        public float Get(string key, float def)
        {
            return EditorPrefs.GetFloat(CreatePath(key), def);
        }
        #endregion

        #region bool
        public void Set(string key, bool val)
        {
            EditorPrefs.SetBool(CreatePath(key), val);
        }

        public bool Get(string key, bool def)
        {
            return EditorPrefs.GetBool(CreatePath(key), def);
        }
        #endregion

        #region rect
        public void Set(string key, Rect val)
        {
            EditorPrefs.SetFloat(CreatePath(key + ".x"), val.x);
            EditorPrefs.SetFloat(CreatePath(key + ".y"), val.y);
            EditorPrefs.SetFloat(CreatePath(key + ".width"), val.width);
            EditorPrefs.SetFloat(CreatePath(key + ".height"), val.height);
        }

        public Rect Get(string key, Rect def)
        {
            var rect = new Rect();

            rect.x = EditorPrefs.GetFloat(CreatePath(key + ".x"), def.x);
            rect.y = EditorPrefs.GetFloat(CreatePath(key + ".y"), def.y);
            rect.width = EditorPrefs.GetFloat(CreatePath(key + ".width"), def.width);
            rect.height = EditorPrefs.GetFloat(CreatePath(key + ".height"), def.height);

            return rect;
        }
        #endregion

        private string CreatePath(string key)
        {
            return prefix + key;
        }
	}
}
