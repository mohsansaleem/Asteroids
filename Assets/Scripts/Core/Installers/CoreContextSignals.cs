using System;
using UnityEngine.SceneManagement;

namespace PG.Core.Installers
{
    public class LoadSceneSignal
    {
        public string Scene;
        public LoadSceneMode LoadSceneMode;
    }

    public class UnloadSceneSignal
    {
        public string Scene;
    }

    public class CommandExecutedSignal
    {
        public Type CommandType;
    }
}