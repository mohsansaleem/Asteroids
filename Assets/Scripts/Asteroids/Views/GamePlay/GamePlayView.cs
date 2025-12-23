using System;
using System.Collections.Generic;
using PG.Asteroids.Models.DataModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PG.Asteroids.Views.GamePlay
 {
    public class GamePlayView : MonoBehaviour
    {
        [SerializeField] public TMP_Text ScoreText;
        [SerializeField] public TMP_Text LivesText;
        [SerializeField] public CanvasGroup EndGameCanvasGroup;
        [SerializeField] public Button ButtonRetry;
        
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

