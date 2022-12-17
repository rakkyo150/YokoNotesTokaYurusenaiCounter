using YokoNotesTokaYurusenaiCounter.UI;
using Zenject;

namespace YokoNotesTokaYurusenaiCounter.Installers
{
    internal class GameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<ImageViewSetter>().FromNewComponentOnNewGameObject().AsSingle();
            Container.Bind<ObstacleCounterUpdater>().AsSingle();
            Container.Bind<ConfirmorOfNumberOfDigit>().AsSingle();
        }
    }
}
