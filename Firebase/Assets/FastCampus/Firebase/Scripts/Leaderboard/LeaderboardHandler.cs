using Firebase;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace FastCampus.Firebase.Leaderboard
{
    /// <summary>
    /// An EventArgs calss used for LeaderboardHandler to notify listeners
    /// when a User's scores
    /// </summary>
    public class UserScoreArgs : EventArgs
    {
        public UserScore score;
        public string message;

        public UserScoreArgs(UserScore score, string message)
        {
            this.score = score;
            this.message = message;
        }
    }

    public class LeaderboardArgs : EventArgs
    {
        public DateTime startDate;
        public DateTime endDate;
        public List<UserScore> scores;
    }

    public class LeaderboardHandler : MonoBehaviour
    {
        #region Fields
        private bool readyToInitialize = false;
        private bool initialized = false;

        private bool lowestFirst = false;

        private DatabaseReference databaseRef;

        public bool sendInitializedEvent = false;
        public event EventHandler OnInitialized;

        private bool sendAddedScoreEvent = false;
        private UserScoreArgs addedScoreArgs;
        public event EventHandler<UserScoreArgs> OnAddedScore;

        private bool sendUpdatedScoreEvent = false;
        private UserScoreArgs updatedScoreArgs;
        public event EventHandler<UserScoreArgs> OnUpdatedScore;

        private bool getUserScoreCallQueued = false;
        private bool gettingUserScore = false;

        private bool sendRetrievedScoreEvent = false;
        private UserScoreArgs retrievedScoreArgs;
        public event EventHandler<UserScoreArgs> OnRetrievedScore;

        /// <summary>
        /// (Readonly) Contains the last set of user top scores requested.
        /// </summary>
        private List<UserScore> topScores = new List<UserScore>();
        public List<UserScore> TopScores => topScores;

        private Dictionary<string, UserScore> userScores = new Dictionary<string, UserScore>();

        private Query currentNewScoreQuery;

        private bool gettingTopScores = false;
        private int scoresToRetrieve = 20;

        private bool sendUpdatedLeaderbardEvent = false;
        public event EventHandler<LeaderboardArgs> OnUpdatedLeaderboard;


        public TMP_InputField userIdInputField;
        public TMP_InputField userNameInputField;
        public TMP_InputField scoreInputField;
        public TMP_Text outputText;


        public UDateTime startDateTime;
        private long StartTimeTicks => startDateTime.dateTime.Ticks / TimeSpan.TicksPerSecond;

        public UDateTime endDateTime;
        private long EndTimeTicks
        {
            get
            {
                long endTimeTicks = endDateTime.dateTime.Ticks / TimeSpan.TicksPerSecond;
                if (endTimeTicks <= 0)
                {
                    endTimeTicks = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
                }

                return endTimeTicks;
            }
        }

        /// <summary>
        /// True when there are any async calls in progress.
        /// </summary>
        public bool TasksProcessing
        {
            get
            {
                return readyToInitialize || gettingTopScores || gettingUserScore || addingUserScore;
            }
        }

        #endregion Fields

        #region Firebase Database Path
        /// <summary>
        /// Path to store all scores in Firebase database.
        /// </summary>
        private string internalAllScoreDataPath = "all_scores";

        public string AllScoreDataPath
        {
            get => internalAllScoreDataPath;
        }

        #endregion Firebase Database Path

        #region Get User Score Variables
        private
        #endregion Get User Score Variables

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            FirebaseInitializer.Initialize(dependencyStatus =>
            {
                if (dependencyStatus == DependencyStatus.Available)
                {
                    readyToInitialize = true;
                    InitializeDatabase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

        private void Update()
        {
            if (sendAddedScoreEvent)
            {
                sendAddedScoreEvent = false;
                OnAddedScore(this, addedScoreArgs);
            }

            if (sendUpdatedScoreEvent)
            {
                sendUpdatedScoreEvent = false;
                OnUpdatedScore(this, updatedScoreArgs);
            }

            if (sendRetrievedScoreEvent)
            {
                sendRetrievedScoreEvent = false;
                OnRetrievedScore(this, retrievedScoreArgs);
            }

            if (sendUpdatedLeaderbardEvent)
            {
                sendUpdatedLeaderbardEvent = false;

                OnUpdatedLeaderboard(this, new LeaderboardArgs
                {
                    scores = topScores,
                    startDate = startDateTime,
                    endDate = endDateTime
                });
            }
        }


        private void OnDisable()
        {
            if (currentNewScoreQuery != null)
            {
                currentNewScoreQuery.ChildAdded -= OnScoreAdded;
                currentNewScoreQuery.ChildRemoved -= OnScoreRemoved;
            }
        }

        #endregion Unity Methods

        #region Methods
        private void InitializeDatabase()
        {
            if (initialized)
            {
                return;
            }

            FirebaseApp app = FirebaseApp.DefaultInstance;

            //if (app.Options.DatabaseUrl != null)
            //{
            //    app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
            //}

            databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

            initialized = true;
            readyToInitialize = false;
            OnInitialized(this, null);
        }

        public Task AddScore(string userId, string userName, int score, long timestamp = -1L, Dictionary<string, object> otherData = null)
        {
            if (timestamp <= 0)
            {
                timestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
            }

            var userScore = new UserScore(userId, userName, score, timestamp, otherData);
            return AddScore(userScore);
        }

        private bool addingUserScore = false;

        public Task<UserScore> AddScore(UserScore userScore)
        {
            if (addingUserScore)
            {
                Debug.LogError("Running add user score task!");
                return null;
            }

            var scoreDictinary = userScore.ToDictionary();
            addingUserScore = true;

            return Task.Run(() =>
            {
                //// Waits to ensure the FirebaseLeaderboard is initialized before adding the new score.
                while (!initialized)
                {
                }

                var newEntry = databaseRef.Child(AllScoreDataPath).Push();

                return newEntry.SetValueAsync(scoreDictinary).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Debug.LogWarning("Exception adding score: " + task.Exception);
                        return null;
                    }

                    if (!task.IsCompleted)
                    {
                        return null;
                    }

                    addingUserScore = false;

                    addedScoreArgs = new UserScoreArgs(userScore, userScore.ToString() + " Added!");
                    sendAddedScoreEvent = true;

                    return userScore;
                }).Result;
            });
        }

        public Task UpdateScore(string userId, string userName, int score, long timestamp = -1L, Dictionary<string, object> otherData = null)
        {
            if (timestamp <= 0)
            {
                timestamp = DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond;
            }

            var userScore = new UserScore(userId, userName, score, timestamp, otherData);
            return UpdateScore(userScore);
        }

        private bool updatingUserScore = false;

        public Task<UserScore> UpdateScore(UserScore userScore)
        {
            if (updatingUserScore)
            {
                Debug.LogError("Running add user score task!");
                return null;
            }

            var scoreDictinary = userScore.ToDictionary();
            updatingUserScore = true;

            return Task.Run(() =>
            {
                //// Waits to ensure the FirebaseLeaderboard is initialized before adding the new score.
                while (!initialized)
                {
                }

                string testKey = "/-M9JbQnc5c0xu_08pa_Y";
                var newEntry = databaseRef.Child(AllScoreDataPath + testKey);

                return newEntry.UpdateChildrenAsync(scoreDictinary).ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Debug.LogWarning("Exception adding score: " + task.Exception);
                        return null;
                    }

                    if (!task.IsCompleted)
                    {
                        return null;
                    }

                    updatingUserScore = false;

                    updatedScoreArgs = new UserScoreArgs(userScore, userScore.ToString() + " Updated!");
                    sendUpdatedScoreEvent = true;

                    return userScore;
                }).Result;
            });
        }

        public void GetUserScore(string userId)
        {
            if (!initialized && !getUserScoreCallQueued)
            {
                Debug.LogWarning("GetUserScore called before Firebase initialized. Waiting for initialization...");
                getUserScoreCallQueued = true;
                StartCoroutine(GetUserScoreWhenInitialized(userId));
                return;
            }

            if (getUserScoreCallQueued)
            {
                Debug.LogWarning("Still waiting for initialization...");
                return;
            }

            gettingUserScore = true;

            // Get user scores within tiem frame, then sort by score to find the highest one.
            databaseRef.Child(AllScoreDataPath)
                .OrderByChild(UserScore.userIdPath)
                .StartAt(userId)
                .EndAt(userId)
                .GetValueAsync().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }

                if (!task.IsCompleted)
                {
                    return;
                }

                if (task.Result.ChildrenCount == 0)
                {
                    retrievedScoreArgs = new UserScoreArgs(null, String.Format("No scores for User {0}", userId));
                }
                else
                {
                    // Find the User's scores within the time range.
                    var scores = ParseValidUserScoreRecords(task.Result, StartTimeTicks, EndTimeTicks).ToList();

                    if (scores.Count == 0)
                    {
                        retrievedScoreArgs = new UserScoreArgs(null, String.Format("No scores for User {0} within time range ({1} - {2})", userId, StartTimeTicks, EndTimeTicks));
                    }
                    else
                    {
                        var orderedScores = scores.OrderBy(score => score.score);
                        var userScore = lowestFirst ? orderedScores.First() : orderedScores.Last();

                        retrievedScoreArgs = new UserScoreArgs(userScore, userScore.ToString() + " Received!");
                    }
                }

                gettingUserScore = false;
                sendRetrievedScoreEvent = true;
            });
        }

        private List<UserScore> ParseValidUserScoreRecords(DataSnapshot snapshot, long startTS, long endTS)
        {
            return snapshot.Children
                .Select(scoreRecord => UserScore.CreateScoreFromRecord(scoreRecord))
                .Where(score => score != null && score.timestamp > startTS && score.timestamp <= endTS)
                .Reverse()
                .ToList();
        }

        private IEnumerator GetUserScoreWhenInitialized(string userId)
        {
            while (!initialized)
            {
                yield return null;
            }

            getUserScoreCallQueued = false;
            GetUserScore(userId);
        }

        private void GetInitialTopScores(long batchEnd)
        {
            gettingTopScores = true;

            var query = databaseRef.Child(AllScoreDataPath).OrderByChild("score");
            query = lowestFirst ? query.StartAt(batchEnd).LimitToFirst(scoresToRetrieve) : query.EndAt(batchEnd).LimitToLast(scoresToRetrieve);

            query.GetValueAsync().ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    SetTopScores();
                    return;
                }
                else if (!task.IsCompleted)
                {
                    return;
                }

                if (!task.Result.HasChildren)
                {
                    // No scores left to retrieve.
                    SetTopScores();
                    return;
                }

                var scores = ParseValidUserScoreRecords(task.Result, StartTimeTicks, EndTimeTicks);
                foreach (var userScore in scores)
                {
                    if (!userScores.ContainsKey(userScore.userId))
                    {
                        userScores[userScore.userId] = userScore;
                    }
                    else
                    {
                        var bestScore = GetBestScore(userScores[userScore.userId], userScore);
                        userScores[userScore.userId] = bestScore;
                    }

                    if (userScores.Count == scoresToRetrieve)
                    {
                        SetTopScores();
                        return;
                    }
                }

                // Until we have found scoresToRetrieve unique user scores or run out of
                // scores to retrieve, get another page of score recordds by ending the next batch
                // (ordered by score) at the worst score found so far.
                long nextEndAt = lowestFirst ? scores.First().score + 1L : scores.Last().score - 1L;

                try
                {
                    GetInitialTopScores(nextEndAt);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
                finally
                {
                    SetTopScores();
                }
            });
        }

        /// <summary>
        /// Gets the best of the provided scores.
        /// </summary>
        /// <param name="scores"></param>
        /// <returns>Best of scores, depending on whether lower or highier sores are better.</returns>
        private UserScore GetBestScore(params UserScore[] scores)
        {
            if (scores.Length == 0)
            {
                return null;
            }

            UserScore bestScore = null;
            foreach (var score in scores)
            {
                if (bestScore == null)
                {
                    bestScore = score;
                }
                else if (lowestFirst && score.score < bestScore.score)
                {
                    bestScore = score;
                }
                else if (!lowestFirst && score.score > bestScore.score)
                {
                    bestScore = score;
                }
            }

            return bestScore;
        }


        /// <summary>
        /// When finished retrieving as many valid user scores that can be found given the
        /// scoresToRetrieve, EndTime, and Interval constraints, this methods stores the scores found
        /// and queues an invocation of TopScoreUpdated on the next Update call.
        /// </summary>
        private void SetTopScores()
        {
            topScores.Clear();

            // Reset top scores and unsubscrie from OnScoreAdded if alreay listening.
            if (currentNewScoreQuery != null)
            {
                currentNewScoreQuery.ChildAdded -= OnScoreAdded;
                currentNewScoreQuery.ChildRemoved -= OnScoreRemoved;
            }

            topScores.AddRange(lowestFirst
                ? userScores.Values.OrderBy(score => score.score)
                : userScores.Values.OrderByDescending(score => score.score));

            // Subscribe to any score added that is greater than the lowest current top score.
            currentNewScoreQuery = databaseRef.Child(AllScoreDataPath).OrderByChild("score");
            if (topScores.Count > 0)
            {
                currentNewScoreQuery = lowestFirst ? currentNewScoreQuery.EndAt(topScores.Last().score) : currentNewScoreQuery.StartAt(topScores.Last().score);
            }

            // If the end datae is now, subscribe to future score added events.
            if (endDateTime.dateTime.Ticks <= 0)
            {
                currentNewScoreQuery.ChildAdded += OnScoreAdded;
            }

            currentNewScoreQuery.ChildRemoved += OnScoreRemoved;

            sendUpdatedLeaderbardEvent = true;
            gettingTopScores = false;
        }

        #endregion Methods

        #region Query Events
        /// <summary>
        /// Callback when a score record is added with a score high enough for a spot on the
        /// leaderboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnScoreAdded(object sender, ChildChangedEventArgs args)
        {
            if (args.Snapshot == null || !args.Snapshot.Exists)
            {
                return;
            }

            var score = UserScore.CreateScoreFromRecord(args.Snapshot);
            if (score == null)
            {
                return;
            }

            // Verify that score is within start/end times, and isn't already in topScores.
            if (topScores.Contains(score))
            {
                return;
            }

            if (StartTimeTicks > 0 || EndTimeTicks > 0)
            {
                var endTimeTicks = endDateTime.dateTime.Ticks > 0
                    ? EndTimeTicks
                    : (DateTime.UtcNow.Ticks / TimeSpan.TicksPerSecond);
                var startTime = startDateTime.dateTime.Ticks > 0
                    ? endTimeTicks - StartTimeTicks
                    : 0;
                if (score.timestamp > endTimeTicks || score.timestamp < startTime)
                {
                    return;
                }
            }

            // Don't add if the same user already has a better score.
            // If the same user has a worse score, remove it.
            var existingScore = topScores.Find(inScore => inScore.userId == score.userId);
            if (existingScore != null)
            {
                var bestStore = GetBestScore(existingScore, score);
                if (existingScore == bestStore)
                {
                    return;
                }

                topScores.Remove(existingScore);
            }

            if (topScores.Any(inScore => inScore.userId == score.userId))
            {
                return;
            }

            topScores.Add(score);
            topScores = lowestFirst
                ? topScores.OrderBy(inScore => inScore.score).Take(scoresToRetrieve).ToList()
                : topScores.OrderByDescending(inScore => inScore.score).Take(scoresToRetrieve).ToList();

            sendUpdatedLeaderbardEvent = true;
        }

        /// <summary>
        /// Callback when a score record is removed from the database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnScoreRemoved(object sender, ChildChangedEventArgs args)
        {
            if (args.Snapshot == null || !args.Snapshot.Exists)
            {
                return;
            }

            var score = UserScore.CreateScoreFromRecord(args.Snapshot);
            if (score == null)
            {
                return;
            }

            if (topScores.Contains(score))
            {
                topScores.Remove(score);
                RefreshScore();
            }
        }
        #endregion Query Events

        #region UI Methods
        public void AddScore()
        {
            AddScore(userIdInputField.text, userNameInputField.text, int.Parse(scoreInputField.text));
        }

        public void UpdateUserScore()
        {
            UpdateScore(userIdInputField.text, userNameInputField.text, int.Parse(scoreInputField.text));
        }

        public void GetUerScore()
        {
            GetUserScore(userIdInputField.text);
        }


        public void RefreshScore()
        {
            if (initialized)
            {
                GetInitialTopScores(lowestFirst ? Int64.MinValue : Int64.MaxValue);
            }
        }
        #endregion UI Methods
    }
}