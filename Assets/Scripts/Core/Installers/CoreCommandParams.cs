using System;
using UnityEngine.SceneManagement;

namespace PG.Core.Installers
{
    public class LoadSceneParams
    {
        public string Scene;
        public LoadSceneMode LoadSceneMode;
    }

    public class UnloadSceneParams
    {
        public string Scene;
    }
}