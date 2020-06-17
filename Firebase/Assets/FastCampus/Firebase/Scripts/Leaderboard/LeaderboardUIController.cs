using FastCampus.Firebase.Leaderboard;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;

namespace FastCampus.Firebase.Leaderboard
{


    public class LeaderboardUIController : MonoBehaviour
    {
        public LeaderboardHandler leaderboardHandler;
        public TMP_Text outputText;

        private enum TopScoreElement
        {
            Username = 1,
            Timestamp = 2,
            Score = 3
        }

        public int MaxRetrievableScores = 100;
        public RectTransform scoreContentContainer;
        public GameObject scorePrefab;
        /// <summary>
        /// List of top score prefabs created in the ScoreContainer scroll view.
        /// </summary>
        private List<GameObject> scoreObjects = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(CreateTopscorePrefabs());
        }

        private void OnEnable()
        {
            leaderboardHandler.OnAddedScore += OnAddedUserScore;
            leaderboardHandler.OnUpdatedScore += OnUpdatedUserScore;
            leaderboardHandler.OnRetrievedScore += OnRetrievedUserScore;
            leaderboardHandler.OnUpdatedLeaderboard += OnUpdatedLeaderboard;
        }

        private void OnDisable()
        {
            leaderboardHandler.OnAddedScore -= OnAddedUserScore;
            leaderboardHandler.OnUpdatedScore -= OnUpdatedUserScore;
            leaderboardHandler.OnRetrievedScore -= OnRetrievedUserScore;
            leaderboardHandler.OnUpdatedLeaderboard -= OnUpdatedLeaderboard;
        }

        // Update is called once per frame
        #region Leaderboard events
        private void OnAddedUserScore(object sender, UserScoreArgs args)
        {
            outputText.text = args.message;
        }

        private void OnUpdatedUserScore(object sender, UserScoreArgs args)
        {
            outputText.text = args.message;
        }

        private void OnRetrievedUserScore(object sender, UserScoreArgs args)
        {
            outputText.text = args.message;
        }

        private void OnUpdatedLeaderboard(object sender, LeaderboardArgs args)
        {
            var scores = args.scores;
            for (var i = 0; i<Math.Min(scores.Count, scoreObjects.Count); i++)
            {
                var score = scores[i];

                var scoreObject = scoreObjects[i];
                scoreObject.SetActive(true);

                var textElements = scoreObject.GetComponentsInChildren<TMP_Text>();
                textElements[(int)TopScoreElement.Username].text = String.IsNullOrEmpty(score.userName) ? score.userId : score.userName;
                textElements[(int)TopScoreElement.Timestamp].text = score.ShortDateString;
                textElements[(int)TopScoreElement.Score].text = score.score.ToString();
            }

            for (var i = scores.Count; i< scoreObjects.Count; i++)
            {
                scoreObjects[i].SetActive(false);
            }
        }
        #endregion Leaderboard events

        #region Methods
        private IEnumerator CreateTopscorePrefabs()
        {
            var textElements = scorePrefab.GetComponentsInChildren<TMP_Text>();
            var topScoreElementValues = Enum.GetValues(typeof(TopScoreElement));
            var lastTopScoreElementValue = (int)topScoreElementValues.GetValue(topScoreElementValues.Length - 1);
            if (textElements.Length < lastTopScoreElementValue)
            {
                throw new InvalidOperationException(String.Format(
                    "At least {0} Text components must be present on TopScorePrefab. Found {1}",
                    lastTopScoreElementValue,
                    textElements.Length));
            }

            for (int i = 0; i < MaxRetrievableScores; i++)
            {
                GameObject scoreObject = Instantiate(scorePrefab, scoreContentContainer.transform);
                scoreObject.GetComponentInChildren<TMP_Text>().text = (i + 1).ToString();
                scoreObject.name = "Leaderboard Score Record " + i;
                scoreObject.SetActive(false);

                scoreObjects.Add(scoreObject);

                yield return null;
            }
        }
        #endregion Methods
    }
}