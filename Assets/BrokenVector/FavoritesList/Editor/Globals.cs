using System.Collections;
using System.Collections.Generic;
using BrokenVector.FavoritesList.Utils;
using UnityEngine;

namespace BrokenVector.FavoritesList
{
    public static class Globals
    {

        public static readonly AssetPrefs Prefs = new AssetPrefs(Constants.ASSET_PATH);

        public static bool Debug
        {
            get { return Prefs.Get(Constants.DEBUG_PREF, false); }
            set { Prefs.Set(Constants.DEBUG_PREF, value); }
        }

    }
}
