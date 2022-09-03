using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace YokoNotesTokaYurusenaiCounter.UI
{
    public class AssetLoader
    {
        internal Sprite yokoNotesWithVerticalSlash { get; }

        internal Material mat_UINoGlow { get; }

        public AssetLoader()
        {
            yokoNotesWithVerticalSlash = LoadSpriteFromResource("YokoNotesTokaYurusenaiCounter.Images.yoko.png");

            mat_UINoGlow = new Material(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").First())
            {
                name = "UINoGlowEvenMoreCustomThanBSMLLOLnojk"
            };
        }

        private static Sprite LoadSpriteFromResource(string path)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(path))
            {
                if (stream == null) return null;

                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                Texture2D tex = new Texture2D(2, 2);

                if (!tex.LoadImage(data)) return null;

                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0), 100);
                return sprite;
            }
        }
    }
}
