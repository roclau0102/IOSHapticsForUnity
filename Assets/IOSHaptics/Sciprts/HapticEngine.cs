using System;
using UnityEngine;
using IOSHaptics.Core;

namespace IOSHaptics
{
    /// <summary>
    /// iOS Haptics震动功能
    /// </summary>
    public class HapticEngine
    {
        /// <summary>
        /// 检查当前设备是否支持Haptics震动功能。注：只支持iOS13.0或以上系统且iPhone8或以上的设备。
        /// </summary>
        /// <returns></returns>
        public static bool IsSupported()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer) return false;
            return HapticsWrapper.IsSupported();
        }

        /// <summary>
        /// 创建Haptics震动引擎（初始化功能）。
        /// </summary>
        public static void CreateEngine()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer) return;
            HapticsWrapper.CreateEngine();

            UnityMainThreadDispatcher.Instance.Init();
        }

        /// <summary>
        /// 传入项目打印回调，用于接收iOS系统层打印。比如传入UnityEngine.Debug.Log。
        /// </summary>
        /// <param name="logCallback"></param>
        public static void SetLogCallback(Action<string> logCallback)
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer) return;
            HapticsWrapper.SetLogCallback(logCallback);
        }

        /// <summary>
        /// 播放一次轻微的震动。
        /// </summary>
        public static void PlaySingleTap()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer) return;
            HapticsWrapper.PlaySingleTap();
        }

        /// <summary>
        /// 根据传入的AHAP配置文件进行播放。path应为绝对路径，比如 Application.streamingAssetsPath + "/example.ahap"。
        /// </summary>
        /// <param name="path"></param>
        public static void PlayPatternFromFile(string path)
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer) return;
            HapticsWrapper.PlayPatternFromFile(path);
        }
    }
}
