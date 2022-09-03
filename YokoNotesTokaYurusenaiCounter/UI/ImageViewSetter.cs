using HMUI;
using UnityEngine;
using UnityEngine.UI;
using YokoNotesTokaYurusenaiCounter.Configuration;
using Zenject;

namespace YokoNotesTokaYurusenaiCounter.UI
{
    public class ImageViewSetter : MonoBehaviour
    {
        private AssetLoader _assetLoader;

        private ImageView yokoNote;

        // InstantiateするときはInjectアトリビュートつけるコンストラクタじゃないと動かない
        [Inject]
        public void Construct(AssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public void Initialize()
        {
            yokoNote = new GameObject("yokoNoteImageViewSetter").AddComponent<ImageView>();
            yokoNote.transform.SetParent(transform, false);
            yokoNote.rectTransform.localScale = Vector3.one * 0.045f;
            yokoNote.rectTransform.localPosition = Vector3.zero;
            yokoNote.sprite = _assetLoader.yokoNotesWithVerticalSlash;
            yokoNote.type = Image.Type.Simple;
            yokoNote.material = _assetLoader.mat_UINoGlow;
        }

        public void SetTMPTransform(Transform tMPTransform)
        {
            yokoNote.transform.SetParent(tMPTransform, false);
            yokoNote.transform.position = yokoNote.transform.position + (Vector3.down * 0.15f);

            if (PluginConfig.Instance.SeparateSaber)
            {
                yokoNote.transform.position = yokoNote.transform.position + (Vector3.left * 0.3f);
                return;
            }

            yokoNote.transform.position = yokoNote.transform.position + (Vector3.left * 0.15f);
        }

        public void MoveLeft()
        {
            yokoNote.transform.position = yokoNote.transform.position + (Vector3.left * 0.05f);
        }
    }
}
