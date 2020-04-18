using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace BrokenVector.FavoritesList
{
	public static class Constants
	{

        // Plugin Info
        public const string BRAND_NAME = "BrokenVector";
        public const string ASSET_NAME = "Favorites List";
	    public const string ASSET_PATH = "BrokenVector.FavoritesList";

        // Editor Window
        public const string WINDOW_PATH = "Tools/" + ASSET_NAME;
        public const string WINDOW_PATH_ALT = "Window/" + BRAND_NAME + "/" + ASSET_NAME;
	    public const string WINDOW_ICON = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACqElEQVRYCcWWTUhUURTHm0otM9AkaTMVWEQuElJkgqIaghZRUNoiKBeGJLkQahO2cteitlHLVkUrQ5BqERR9bQqiTVCE+BF9IogiamW/v7yRO9N97913Z2T+8Ju577x7zznvnnvve6nFqZZV5dTqcgZX7GISSDN+Eh7Jka+KSeAkQdfBXqgpRwKHCFoJ6+EweMl3BjYTbRtofBUcBy/5JpAhmpKQ1kATKJHE8k1gH5Hqg2gp/hugObhO9OeTgOq+HaqNSErAax34JLCLYFuN4GpuBNkT+0s8gCCtsBNMyY/OhcLEzD7W9lrDesDRwVH6bTLG5Zq7afTA+5wh5H8O+2sY1/2U8S64z/VpGVdYP/HfDYOKY87ATa7/QDtUwEpoBKe34E3OuZnAU4yf4R30wRYolf7i6Dlcg2cwA0syS5CzaZtlQZ33gPZ5MZpl8F0YgAnQLC/LtgvmufsQdLxqXSxnSzuJFun8Ay7BBRiFvOBcR+7bMe6fgyvwETSNrpqmo0p6DFTz32CVrQS2jvsxDkGt7abF9gCbnvqb5V6eyVaCvA7BhRanXjqu0lRrFmLlmsARPOm4dVWWjhtcOrsmoBMuiVSqgy4DXBKQs7YQZ5+w/7eyg769IWPyzC4JnGFEYf0XsA1DBk6APk617Uxp4cZ+K8YloMBdplfaU3AHdJ7/An0Vd0LhVtXYDohUXAKNjN5hePhOW/tap9qXwK4SPIbL8BbMPX+e6xSEKi6BU4zUV6+kgNfhBoyDKZ2eSuIq6FWrEklNkF5qhfxEJaApzILeDQreD7dBs2CTkngCOjlfgk7OatBDhCoqgSpGaRGNwFm4B6p/lDT9r+AivAD51/diqKKOYj25tqCeTIH1RK5S3etAb9Ov8AGsikrAOqDUxqgSlDqW1d8/WMVw/7/yGawAAAAASUVORK5CYII=";

        // Editor Pref Paths
        public const string DEBUG_PREF = "Debug";                               // has to match FavoritesListReference.DEBUG_PREF (with AssetPrefs)

    }
}
