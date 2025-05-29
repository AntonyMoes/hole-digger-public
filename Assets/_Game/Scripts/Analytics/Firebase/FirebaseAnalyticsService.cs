using GeneralUtils.Processes;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using Firebase.Extensions;
#endif

namespace _Game.Scripts.Analytics.Firebase {
    public class FirebaseAnalyticsService : IAnalyticsService {
        public Process Init() {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            return new AsyncProcess(onDone => {
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
                    if (task.Result == DependencyStatus.Available) {
                        // Create and hold a reference to your FirebaseApp,
                        // where app is a Firebase.FirebaseApp property of your application class.
                        // Crashlytics will use the DefaultInstance, as well;
                        // this ensures that Crashlytics is initialized.
                        var _ = FirebaseApp.DefaultInstance;
                        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                        // FirebaseAnalytics.SetUserId(TODO);
                        Crashlytics.IsCrashlyticsCollectionEnabled = true;
                        if (UnityEngine.Debug.isDebugBuild) {
                            FirebaseApp.LogLevel = LogLevel.Verbose;
                        }

                        onDone?.Invoke();
                    } else {
                        UnityEngine.Debug.LogError("Could not resolve all Firebase dependencies : " + task.Result);
                    }
                });
            });
#else
            return new DummyProcess();
#endif
        }

        public IServiceLogger CreateLogger() => new FirebaseServiceLogger();
    }
}