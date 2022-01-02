using Firebase.Analytics;

public class AnalyticsManager
{
    private static void LogEvent(string eventName, params Parameter[] parameters)
    {
        //Main method to send Analytic
        FirebaseAnalytics.LogEvent(eventName, parameters); 
    }

    public static void LogUpgradeEvent(int resourceIndex, int level)
    {
        /*Log using event and parameter provided byy Firebase
        //so it can be shownn on the data report*/
        LogEvent(
            FirebaseAnalytics.EventLevelUp, 
            new Parameter(FirebaseAnalytics.ParameterIndex, resourceIndex.ToString()), 
            new Parameter(FirebaseAnalytics.ParameterLevel, level)
            );
    }

    public static void LogUnlockEvent(int resourceIndex)
    {
        LogEvent(
            FirebaseAnalytics.EventUnlockAchievement,
            new Parameter(FirebaseAnalytics.ParameterIndex, resourceIndex.ToString())
            );
    }

    public static void SetUserProperties(string name, string value)
    {
        FirebaseAnalytics.SetUserProperty(name, value);
    }
}
