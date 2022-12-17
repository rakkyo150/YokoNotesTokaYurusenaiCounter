using YokoNotesTokaYurusenaiCounter.UI;
using Zenject;

namespace YokoNotesTokaYurusenaiCounter.Installers
{
    internal class GameInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<ImageViewSetter>().AsSingle();
            Container.Bind<ObstacleCounterUpdater>().AsSingle();
        }
    }
}
