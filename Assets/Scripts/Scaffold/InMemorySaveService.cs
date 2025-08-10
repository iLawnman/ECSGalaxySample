using Scaffold;

namespace Services
{
    public class InMemorySaveService : ISaveService
    {
        private SceneConfig cachedConfig = new SceneConfig();

        public SceneConfig LoadSceneConfig()
        {
            return cachedConfig;
        }

        public void SaveSceneConfig(SceneConfig config)
        {
            cachedConfig = config ?? new SceneConfig();
        }
    }
}