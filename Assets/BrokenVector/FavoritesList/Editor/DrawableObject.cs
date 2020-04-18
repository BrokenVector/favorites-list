using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BrokenVector.FavoritesList
{

    // provides access to data of the object, for drawing the editor window
    [Serializable]
    public struct DrawableObjectData
    {
        public String Name;
        public Texture Icon;
        public DrawableSceneData SceneData;
    }

    [Serializable]
    public struct DrawableSceneData
    {
        public Boolean IsScene;
#if UNITY_5_4_OR_NEWER
        public SceneAsset Scene;    // might be empty
#endif
    }

    public interface IDrawableObjectDataProvider
    {
        DrawableObjectData GetProvidedData();
    }

}
