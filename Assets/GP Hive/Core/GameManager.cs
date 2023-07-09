using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GPHive.Game;
using UnityEngine.UI;


namespace GPHive.Core
{
    public enum GameState
    {
        Idle,
        Playing,
        End
    }

    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private TextMeshProUGUI levelText, kilText, moneyText;

        [SerializeField] private GameObject winScreen;
        [SerializeField] private GameObject loseScreen;

        [SerializeField] private float OpenFinalUIAfterTime, uIAfterTimeWin, uIAfterTimeLose;

        private GameState gameState;
        public GameState GameState { get { return gameState; } }

        [HideInInspector] public int kilCount, moneyCount;

        public GameObject Tutorial, Relese, Hold;

        public Image TapToStart;

        private void Start()
        {
            SetLevelText();
        }

        private void SetLevelText()
        {
            levelText.SetText($"LEVEL { SaveLoadManager.GetLevel() }");
        }
        public void StartLevel()
        {
            EventManager.StartLevel(SaveLoadManager.GetLevel());

            gameState = GameState.Playing;
        }

        public void NextLevel()
        {
            Destroy(LevelManager.Instance.ActiveLevel);
            LevelManager.Instance.ActiveLevel = LevelManager.Instance.CreateLevel();

            gameState = GameState.Idle;
            SetLevelText();

        }

        /// <summary>
        /// Call when level is successfully finished.
        /// </summary>
        public void WinLevel()
        {
            Shooter.Instance.ShootStop = true;
            OpenFinalUIAfterTime = uIAfterTimeWin;
            EventManager.SuccessLevel(SaveLoadManager.GetLevel());
            SaveLoadManager.IncreaseLevel();

            gameState = GameState.End;

            kilText.text = kilCount.ToString();
            moneyText.text = moneyCount.ToString();

            StartCoroutine(CO_OpenUIDelayed(winScreen));

        }

        /// <summary>
        /// Call when level is failed.
        /// </summary>
        public void LoseLevel()
        {
            Shooter.Instance.ShootStop = true;
            OpenFinalUIAfterTime = uIAfterTimeLose;
            EventManager.FailLevel(SaveLoadManager.GetLevel());

            gameState = GameState.End;
            StartCoroutine(CO_OpenUIDelayed(loseScreen));
        }

        IEnumerator CO_OpenUIDelayed(GameObject UI)
        {
            yield return BetterWaitForSeconds.Wait(OpenFinalUIAfterTime);
            UI.SetActive(true);
        }
    }
}
