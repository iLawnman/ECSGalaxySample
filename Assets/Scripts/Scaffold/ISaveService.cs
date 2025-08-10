using Scaffold;

namespace Services
{
    public interface ISaveService
    {
        SceneConfig LoadSceneConfig();
        void SaveSceneConfig(SceneConfig config);
    }

    public static class SaveServiceLocator
    {
        private static ISaveService current;

        public static ISaveService Current
        {
            get
            {
                if (current == null)
                {
                    current = new InMemorySaveService();
                }

                return current;
            }
        }

        public static void Set(ISaveService instance)
        {
            current = instance;
        }
    }
}