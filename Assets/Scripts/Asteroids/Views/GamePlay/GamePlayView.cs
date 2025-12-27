using System;
using System.Collections.Generic;
using Oryx;
using PG.Asteroids.Models.DataModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PG.Asteroids.Views.GamePlay
 {
    public class GamePlayView : MonoBehaviour
    {
        [SerializeField] SimpleStateComponent SimpleStateComponent;
        [SerializeField] public TMP_Text ScoreText;
        [SerializeField] public TMP_Text LivesText;
        [SerializeField] public Button ButtonRetry;
        
        public const string END_GAME_STATE_TRUE = "ShowEndGameTrue";
        public const string END_GAME_STATE_FALSE = "ShowEndGameFalse";
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void ShowEndGame(bool visibility)
        {
            SimpleStateComponent.PlayState(visibility ? END_GAME_STATE_TRUE : END_GAME_STATE_FALSE);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

