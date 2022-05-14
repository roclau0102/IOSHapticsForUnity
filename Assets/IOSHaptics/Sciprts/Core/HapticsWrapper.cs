using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;

namespace IOSHaptics.Core
{
    public class HapticsWrapper
    {
#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void HapticsHelper_CreateEngine();

        [DllImport("__Internal")]
        private static extern bool HapticsHelper_IsSupported();

        [DllImport("__Internal")]
        private static extern void HapticsHelper_PlaySingleTap();

        [DllImport("__Internal")]
        private static extern void HapticsHelper_SetLogCallback(IntPtr ptr);

        [DllImport("__Internal")]
        private static extern void HapticsHelper_PlayPatternFromFile(string path);

        [DllImport("__Internal")]
        private static extern IntPtr HapticsHelper_CreatePatternPlayerByJson(string json);

        [DllImport("__Internal")]
        private static extern void HapticsHelper_DestroyPatternPlayer(IntPtr patternPlayer);

        [DllImport("__Internal")]
        private static extern int HapticsHelper_GetPatternPlayerID(IntPtr patternPlayer);

        [DllImport("__Internal")]
        private static extern void HapticsHelper_SetPatternPlayerCompletionHandler(IntPtr patternPlayer, IntPtr completionHandler);

        [DllImport("__Internal")]
        private static extern bool HapticsHelper_StartPatternPlayer(IntPtr patternPlayer, float time);

        [DllImport("__Internal")]
        private static extern bool HapticsHelper_PausePatternPlayer(IntPtr patternPlayer, float time);

        [DllImport("__Internal")]
        private static extern bool HapticsHelper_ResumePatternPlayer(IntPtr patternPlayer, float time);

        [DllImport("__Internal")]
        private static extern bool HapticsHelper_StopPatternPlayer(IntPtr patternPlayer, float time);

        [DllImport("__Internal")]
        private static extern bool HapticsHelper_SeekToOffsetPatternPlayer(IntPtr patternPlayer, float offsetTime);

        [DllImport("__Internal")]
        private static extern bool HapticsHelper_GetLoopEnabledPatternPlayer(IntPtr patternPlayer);

        [DllImport("__Internal")]
        private static extern void HapticsHelper_SetLoopEnabledPatternPlayer(IntPtr patternPlayer, bool enable);

        [DllImport("__Internal")]
        private static extern float HapticsHelper_GetLoopEndPatternPlayer(IntPtr patternPlayer);

        [DllImport("__Internal")]
        private static extern void HapticsHelper_SetLoopEndPatternPlayer(IntPtr patternPlayer, float time);

        [DllImport("__Internal")]
        private static extern float HapticsHelper_GetPlaybackRatePatternPlayer(IntPtr patternPlayer);

        [DllImport("__Internal")]
        private static extern void HapticsHelper_SetPlaybackRatePatternPlayer(IntPtr patternPlayer, float playbackRate);
#endif

        private static Action<string> _logCallback;

        private static Dictionary<int, Action<string>> _patternPlayerCompletionHanlders = new Dictionary<int, Action<string>>();

        [AOT.MonoPInvokeCallback(typeof(Action<string>))]
        private static void Log(string content)
        {
            _logCallback?.Invoke(content);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int, IntPtr>))]
        private static void PatternPlayerCompletionHandler(int playerID, IntPtr intPtr)
        {
            if (!_patternPlayerCompletionHanlders.ContainsKey(playerID))
            {
                Debug.LogWarning($"HapticsWrapper's _patternPlayerCompletionHanlders doesn't contains key {playerID}");
                return;
            }

            // 防止线程同步后intPtr的内存已被释放
            string error = Marshal.PtrToStringAnsi(intPtr);

            UnityMainThreadDispatcher.Instance.Enqueue(() =>
            {
                _patternPlayerCompletionHanlders[playerID]?.Invoke(error);
            });
        }

        // ---------------------------------------------------------------------------
        // Public Interfaces
        // ---------------------------------------------------------------------------

        public static bool IsSupported()
        {
#if UNITY_IOS
            return HapticsHelper_IsSupported();
#else
            return false;
#endif
        }

        public static void CreateEngine()
        {
#if UNITY_IOS
            HapticsHelper_CreateEngine();
#endif
        }

        public static void SetLogCallback(Action<string> logCallback)
        {
#if UNITY_IOS
            _logCallback = logCallback;
            IntPtr ptr = Marshal.GetFunctionPointerForDelegate<Action<string>>(Log);
            HapticsHelper_SetLogCallback(ptr);
#endif
        }

        public static void PlaySingleTap()
        {
#if UNITY_IOS
            HapticsHelper_PlaySingleTap();
#endif
        }

        public static void PlayPatternFromFile(string path)
        {
#if UNITY_IOS
            HapticsHelper_PlayPatternFromFile(path);
#endif
        }

        public static IntPtr CreatePatternPlayerByJson(string json)
        {
#if UNITY_IOS
            return HapticsHelper_CreatePatternPlayerByJson(json);
#else
            return IntPtr.Zero;
#endif
        }

        public static void DestroyPatternPlayer(IntPtr intPtr, int playerID)
        {
#if UNITY_IOS
            if (_patternPlayerCompletionHanlders.ContainsKey(playerID))
                _patternPlayerCompletionHanlders.Remove(playerID);

            HapticsHelper_DestroyPatternPlayer(intPtr);
#endif
        }

        public static int GetPatternPlayerID(IntPtr intPtr)
        {
#if UNITY_IOS
            return HapticsHelper_GetPatternPlayerID(intPtr);
#else
            return -1;
#endif
        }

        public static void SetPatternPlayerCompletionHandler(IntPtr intPtr, int patternPlayerID, Action<string> completionHandler)
        {
#if UNITY_IOS
            if (intPtr == IntPtr.Zero) return;

            if (_patternPlayerCompletionHanlders.ContainsKey(patternPlayerID))
            {
                _patternPlayerCompletionHanlders[patternPlayerID] = completionHandler;
            }
            else
            {
                _patternPlayerCompletionHanlders.Add(patternPlayerID, completionHandler);
            }

            var handlerPointer = Marshal.GetFunctionPointerForDelegate<Action<int, IntPtr>>(PatternPlayerCompletionHandler);
            HapticsHelper_SetPatternPlayerCompletionHandler(intPtr, handlerPointer);
#endif
        }

        public static bool StartPatternPlayer(IntPtr intPtr, float time)
        {
#if UNITY_IOS
            return HapticsHelper_StartPatternPlayer(intPtr, time);
#else
            return false;
#endif
        }

        public static bool PausePatternPlayer(IntPtr intPtr, float time)
        {
#if UNITY_IOS
            return HapticsHelper_PausePatternPlayer(intPtr, time);
#else
            return false;
#endif
        }

        public static bool ResumePatternPlayer(IntPtr intPtr, float time)
        {
#if UNITY_IOS
            return HapticsHelper_ResumePatternPlayer(intPtr, time);
#else
            return false;
#endif
        }

        public static bool StopPatternPlayer(IntPtr intPtr, float time)
        {
#if UNITY_IOS
            return HapticsHelper_StopPatternPlayer(intPtr, time);
#else
            return false;
#endif
        }

        public static bool SeekToOffset(IntPtr intPtr, float offsetTime)
        {
#if UNITY_IOS
            return HapticsHelper_SeekToOffsetPatternPlayer(intPtr, offsetTime);
#else
            return false;
#endif
        }

        public static bool GetLoopEnabledPatternPlayer(IntPtr intPtr)
        {
#if UNITY_IOS
            return HapticsHelper_GetLoopEnabledPatternPlayer(intPtr);
#else
            return false;
#endif
        }

        public static void SetLoopEnabledPatternPlayer(IntPtr intPtr, bool enable)
        {
#if UNITY_IOS
            HapticsHelper_SetLoopEnabledPatternPlayer(intPtr, enable);
#endif
        }

        public static float GetLoopEndPatternPlayer(IntPtr intPtr)
        {
#if UNITY_IOS
            return HapticsHelper_GetLoopEndPatternPlayer(intPtr);
#else
            return 0f;
#endif
        }

        public static void SetLoopEndPatternPlayer(IntPtr intPtr, float time)
        {
#if UNITY_IOS
            HapticsHelper_SetLoopEndPatternPlayer(intPtr, time);
#endif
        }

        public static float GetPlaybackRatePatternPlayer(IntPtr intPtr)
        {
#if UNITY_IOS
            return HapticsHelper_GetPlaybackRatePatternPlayer(intPtr);
#else
            return 0f;
#endif
        }

        public static void SetPlaybackRatePatternPlayer(IntPtr intPtr, float playbackRate)
        {
#if UNITY_IOS
            HapticsHelper_SetPlaybackRatePatternPlayer(intPtr, playbackRate);
#endif
        }
    }
}
