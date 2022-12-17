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
        private ImageView squat;

        // InstantiateするときはInjectアトリビュートつけるコンストラクタじゃないと動かない
        [Inject]
        public void Construct(AssetLoader assetLoader)
        {
            _assetLoader = assetLoader;
        }

        public void Initialize()
        {
            yokoNote = new GameObject("yokoNoteImageViewSetter").AddComponent<ImageView>();
            yokoNote.sprite = _assetLoader.yokoNotesWithVerticalSlash;
            squat = new GameObject("squatImageViewSetter").AddComponent<ImageView>();
            squat.sprite = _assetLoader.irasutoyaSquat;

            yokoNote = SetData(yokoNote);
            squat = SetData(squat);
        }

        private ImageView SetData(ImageView imageView)
        {            
            imageView.transform.SetParent(transform, false);
            imageView.rectTransform.localScale = Vector3.one * 0.045f;
            imageView.type = Image.Type.Simple;
            imageView.material = _assetLoader.mat_UINoGlow;

            return imageView;
        }

        public void SetTMPTransform(Transform tMPTransform)
        {
            // メソッド化必須
            yokoNote.transform.SetParent(tMPTransform, false);
            yokoNote.transform.position = yokoNote.transform.position + (Vector3.down * 0.15f);

            if (PluginConfig.Instance.SeparateSaber)
            {
                yokoNote.transform.position = yokoNote.transform.position + (Vector3.left * 0.3f);
                return;
            }

            yokoNote.transform.position = yokoNote.transform.position + (Vector3.left * 0.15f);
        }

        public void SetTMPTransform2(Transform tMPTransform)
        {
            squat.transform.SetParent(tMPTransform, false);
            squat.transform.position = squat.transform.position + (Vector3.down * 1.1f);

            squat.transform.position = squat.transform.position + (Vector3.left * 0.35f);
            if (PluginConfig.Instance.ObstacleTimeType != ObstacleTimeTypeEnum.Second) return;

            // 小数点のコンマの分
            if (PluginConfig.Instance.ObstacleSecondPrecision > 0) MoveSquatLeft();
            
            for (int i = 0; i < PluginConfig.Instance.ObstacleSecondPrecision; i++)
            {
                MoveSquatLeft();
            }
        }

        public void MoveYokoNoteLeft()
        {
            yokoNote.transform.position = yokoNote.transform.position + (Vector3.left * 0.05f);
        }

        public void MoveSquatLeft()
        {
            squat.transform.position = squat.transform.position + (Vector3.left * 0.05f);
        }

        public void OnDestroy()
        {
            // いらないはずだけど一応
            _assetLoader = null;
        }
    }
}
