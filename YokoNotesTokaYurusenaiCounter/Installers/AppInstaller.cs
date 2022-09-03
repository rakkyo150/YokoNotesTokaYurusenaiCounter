using YokoNotesTokaYurusenaiCounter.UI;
using Zenject;

namespace YokoNotesTokaYurusenaiCounter.Installers
{
    internal class AppInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<AssetLoader>().AsSingle();
        }
    }
}
