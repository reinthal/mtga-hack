using UnityEngine;
namespace Hack3
{
    public class Loader
    {
        private static GameObject Load;

        public static void Init()
        {
            
            Loader.Load = new GameObject();
            Loader.Load.AddComponent<CheatGUI>();
            UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
        }

    }
}
