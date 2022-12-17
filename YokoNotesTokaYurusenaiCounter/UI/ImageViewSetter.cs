using HMUI;
using TMPro;
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

        public void InitializeYokoNoteIcon()
        {
            yokoNote = new GameObject("yokoNoteImageViewSetter").AddComponent<ImageView>();
            yokoNote.sprite = _assetLoader.yokoNotesWithVerticalSlash;

            yokoNote = SetData(yokoNote);
        }

        public void InitializeSquatIcon()
        {
            squat = new GameObject("squatImageViewSetter").AddComponent<ImageView>();
            squat.sprite = _assetLoader.irasutoyaSquat;

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

        public void SetTMPTransformForYokoNote(Transform tMPTransform)
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

        public void SetTMPTransformForObstacle(Transform tMPTransform)
        {
            squat.transform.SetParent(tMPTransform, false);
            
            if(PluginConfig.Instance.CounterType == CounterTypeEnum.All)
            {
                squat.transform.position = squat.transform.position + (Vector3.down * 1.1f);
            }
            else if(PluginConfig.Instance.CounterType == CounterTypeEnum.YokoNotesAndObstacles
                    || PluginConfig.Instance.CounterType == CounterTypeEnum.BombsAndObstacles)
            {
                squat.transform.position = squat.transform.position + (Vector3.down * 0.625f);
            }
            else
            {
                squat.transform.position = squat.transform.position + (Vector3.down * 0.15f);
            }


            if (!PluginConfig.Instance.IsObstacleTimeEnable)
            {
                squat.transform.position = squat.transform.position + (Vector3.left * 0.1f);
                return ;
            }
            
            squat.transform.position = squat.transform.position + (Vector3.left * 0.3f);

            if (PluginConfig.Instance.ObstacleTimeType != ObstacleTimeTypeEnum.Second) return;

            // 小数点のコンマの分
            if (PluginConfig.Instance.ObstacleSecondPrecision > 0) MoveSquatLeft();
            
            for (int i = 0; i < PluginConfig.Instance.ObstacleSecondPrecision; i++)
            {
                MoveSquatLeft();
            }
        }

        
        internal void CreateIcons(TMP_Text counter)
        {
            if (!PluginConfig.Instance.IsYokoNoteIconDisabled())
            {
                InitializeYokoNoteIcon();
                SetTMPTransformForYokoNote(counter.transform);
            }

            if (PluginConfig.Instance.IsObstacleIconDisabled()) return;

            InitializeSquatIcon();
            SetTMPTransformForObstacle(counter.transform);
        }

        public void MoveYokoNoteLeft()
        {
            yokoNote.transform.position = yokoNote.transform.position + (Vector3.left * 0.05f);
        }

        public void MoveSquatLeft()
        {
            squat.transform.position = squat.transform.position + (Vector3.left * 0.08f);
        }

        public void OnDestroy()
        {
            // いらないはずだけど一応
            _assetLoader = null;
        }
    }
}
