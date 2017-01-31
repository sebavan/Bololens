using System;
using UnityEngine;

/// <summary>
/// In the root namespace for ease of access.
/// </summary>
namespace Bololens
{
    /// <summary>
    /// Redirection of the debug log to allow injection of the time stamp.
    /// </summary>
    public static class BotDebug
    {
        /// <summary>
        /// Turns on/off the debug log level.
        /// </summary>
        public static bool DebugLog = true;

        /// <summary>
        /// Gets the time in string to automatically inject in the log messages.
        /// </summary>
        /// <returns></returns>
        private static string GetTimeString()
        {
            return string.Format("{0:[HH:mm:ss.fff]} ", DateTime.Now);
        }

        #region Unity Log Proxy
        //The region is not commented as it is only a proxy to the unity debug log system.

        public static void Log(object message)
        {
            if (DebugLog)
            {
                Debug.Log(GetTimeString() + message);
            }
        }
        public static void LogFormat(string format, params object[] args)
        {
            if (DebugLog)
            {
                Debug.LogFormat(GetTimeString() + format, args);
            }
        }

        public static void LogWarning(object message)
        {
            Debug.LogWarning(GetTimeString() + message);
        }
        public static void LogWarningFormat(string format, params object[] args)
        {
            Debug.LogWarningFormat(GetTimeString() + format, args);
        }

        public static void LogError(object message)
        {
            Debug.LogError(GetTimeString() + message);
        }
        public static void LogErrorFormat(string format, params object[] args)
        {
            Debug.LogErrorFormat(GetTimeString() + format, args);
        }

        public static void LogException(Exception exception)
        {
            Debug.LogException(exception);
        }
        #endregion
    }
}