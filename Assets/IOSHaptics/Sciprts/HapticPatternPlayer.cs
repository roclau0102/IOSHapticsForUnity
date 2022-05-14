using System;
using UnityEngine;
using IOSHaptics.Core;

namespace IOSHaptics
{
    /// <summary>
    /// Haptic Pattern 播放器
    /// </summary>
    public class HapticPatternPlayer
    {
        /// <summary>
        /// 当设置为true时，会在到达‘loopEnd’设定的时间后循环播放。默认为false。
        /// </summary>
        public bool LoopEnabled
        {
            get { return HapticsWrapper.GetLoopEnabledPatternPlayer(_intPtr); }
            set { HapticsWrapper.SetLoopEnabledPatternPlayer(_intPtr, value); }
        }

        /// <summary>
        /// 开始循环播放的时间，单位为秒。默认值为当前pattern数据的播放总时长。
        /// </summary>
        public float LoopEnd
        {
            get { return HapticsWrapper.GetLoopEndPatternPlayer(_intPtr); }
            set { HapticsWrapper.SetLoopEndPatternPlayer(_intPtr, value); }
        }

        /// <summary>
        /// 播放速率，默认值为1。可以在播放前或播放期间进行调整。
        /// </summary>
        public float PlaybackRate
        {
            get { return HapticsWrapper.GetPlaybackRatePatternPlayer(_intPtr); }
            set { HapticsWrapper.SetPlaybackRatePatternPlayer(_intPtr, value); }
        }

        #region Private fields
        private IntPtr _intPtr = IntPtr.Zero;
        private int _playerID;
        private bool _isPlaying;
        private bool _toDestroy;
        private Action<string> _completionHandler;
        #endregion

        #region Private methods
        private HapticPatternPlayer(IntPtr intPtr, int playerID)
        {
            _intPtr = intPtr;
            _playerID = playerID;

            HapticsWrapper.SetPatternPlayerCompletionHandler(_intPtr, _playerID, OnCompletion);
        }

        private void OnCompletion(string error)
        {
            _isPlaying = false;
            _completionHandler?.Invoke(error);

            if (_toDestroy)
                DestroyInternal();
        }

        private void DestroyInternal()
        {
            HapticsWrapper.DestroyPatternPlayer(_intPtr, _playerID);
            _intPtr = IntPtr.Zero;
        }
        #endregion

        /// <summary>
        /// 传入hapticData数据对象，创建Haptic Pattern播放器实例。若hapticData不符合AHAP结构，将返回null。
        /// </summary>
        /// <param name="hapticData"></param>
        /// <returns></returns>
        public static HapticPatternPlayer Create(HapticData hapticData)
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer) return null;

            IntPtr intPtr = HapticsWrapper.CreatePatternPlayerByJson(JsonHelper.ToJson(hapticData));
            if (intPtr == IntPtr.Zero)
                return null;

            int playerID = HapticsWrapper.GetPatternPlayerID(intPtr);
            return new HapticPatternPlayer(intPtr, playerID);
        }

        /// <summary>
        /// 传入AHAP内容（json字符串），创建Haptic Pattern播放器实例。若json字符串不符合AHAP结构，将返回null。
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static HapticPatternPlayer Create(string json)
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer) return null;

            IntPtr intPtr = HapticsWrapper.CreatePatternPlayerByJson(json);
            if (intPtr == IntPtr.Zero)
                return null;

            int playerID = HapticsWrapper.GetPatternPlayerID(intPtr);
            return new HapticPatternPlayer(intPtr, playerID);
        }

        /// <summary>
        /// 销毁Haptic Pattern播放器实例。重要：必须与 PatternPlayer.Create() 成对使用，不然会造成内存泄漏。若 PatternPlayer 尚为停止（暂停不算），将触发强制停止并回调 CompletionHandler。
        /// </summary>
        public void Destroy()
        {
            if (Application.platform != RuntimePlatform.IPhonePlayer) return;

            if (_isPlaying)
            {
                _toDestroy = true;
                Stop();
                return;
            }

            DestroyInternal();
        }

        /// <summary>
        /// 在特定时间后开始播放震动。如果time为0，将立刻播放。
        /// </summary>
        public bool Start(float time = 0)
        {
            var result = HapticsWrapper.StartPatternPlayer(_intPtr, time);
            if (result)
                _isPlaying = true;
            return result;
        }

        /// <summary>
        /// 在特定时间后停止播放震动。如果time为0，将立刻停止。
        /// </summary>
        public bool Stop(float time = 0)
        {
            var result = HapticsWrapper.StopPatternPlayer(_intPtr, time);
            if (result)
                _isPlaying = false;
            return result;
        }

        /// <summary>
        /// 在特定时间后暂停播放震动。如果time为0，将立刻暂停。
        /// </summary>
        public bool Pause(float time = 0)
        {
            return HapticsWrapper.PausePatternPlayer(_intPtr, time);
        }

        /// <summary>
        /// 在特定时间后继续播放被暂停的震动。如果time为0，将立刻继续。只会对已暂停的PatternPlayer起作用。
        /// </summary>
        public bool Resume(float time = 0)
        {
            return HapticsWrapper.ResumePatternPlayer(_intPtr, time);
        }

        /// <summary>
        /// 设置一个正在播放的PatternPlayer的播放位置到某个特定的偏移时间。当offsetTime为0时，PatternPlayer会重头开始播放。
        /// 当offsetTime大于或等于单次播放时长，对于非循环播放的PatternPlayer，将立刻停止播放；对于循环播放的PatternPlayer，将重头播放。
        /// </summary>
        /// <param name="offsetTime"></param>
        /// <returns></returns>
        public bool SeekToOffset(float offsetTime)
        {
            return HapticsWrapper.SeekToOffset(_intPtr, offsetTime);
        }

        /// <summary>
        /// 设置PatternPlayer播放结束的回调
        /// </summary>
        /// <param name="completionHandler"></param>
        public void SetCompletionHandler(Action<string> completionHandler)
        {
            _completionHandler = completionHandler;
        }
    }
}