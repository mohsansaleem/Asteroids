using PG.Asteroids.Models.RemoteDataModels;
using UnityEngine;
using UnityEngine.UI;

namespace PG.Asteroids.Views.Startup
{
    public class StartupView : MonoBehaviour
    {
        [Header("References")]
        public Slider ProgressBar;

        public void SetProgress(float progress)
        {
            ProgressBar.value = progress;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

