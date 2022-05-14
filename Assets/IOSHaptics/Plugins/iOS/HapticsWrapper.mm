//
//  HapticsWrapper.m
//
//  Created by roc on 2022/3/15.
//

#import "HapticsHelper.h"
#include "HapticPatternPlayerWrapper.h"

#if __cplusplus
extern "C" {
#endif

bool HapticsHelper_IsSupported() {
    return [HapticsHelper isSupported];
}

void HapticsHelper_CreateEngine() {
    [HapticsHelper createEngine];
}
    
void HapticsHelper_SetLogCallback(HapticsHelper_Callback_t callback) {
    [HapticsHelper setLogCallback:callback];
}

void HapticsHelper_PlaySingleTap() {
    [HapticsHelper playSingleTap];
}

void HapticsHelper_PlayPatternFromFile(const char* filePath) {
    [HapticsHelper playPatternFromFile:[NSString stringWithUTF8String:filePath]];
}

HapticPatternPlayerWrapper* HapticsHelper_CreatePatternPlayerByJson(const char* json) {
    if (json) {
        return [HapticsHelper createHapticPatternPlayerByJson:[NSString stringWithUTF8String:json]];
    }
    return nullptr;
}

void HapticsHelper_DestroyPatternPlayer(HapticPatternPlayerWrapper* playerWrapper) {
    [HapticsHelper destroyPatternPlayer:playerWrapper];
}

int HapticsHelper_GetPatternPlayerID(HapticPatternPlayerWrapper* playerWrapper) {
    if (playerWrapper)
        return playerWrapper->getPlayerID();
    return -1;  // 有效id范围: 0～MAX_INT
}

void HapticsHelper_SetPatternPlayerCompletionHandler(HapticPatternPlayerWrapper* playerWrapper, HapticsHelper_PatternPlayerCompletionHandler_t completionHandler) {
    [HapticsHelper setPatternPlayerCompletionHandler:playerWrapper completionHandler:completionHandler];
}

bool HapticsHelper_StartPatternPlayer(HapticPatternPlayerWrapper* playerWrapper, float time) {
    return [HapticsHelper startPatternPlayer:playerWrapper atTime:time];
}

bool HapticsHelper_PausePatternPlayer(HapticPatternPlayerWrapper* playerWrapper, float time) {
    return [HapticsHelper pausePatternPlayer:playerWrapper atTime:time];
}

bool HapticsHelper_ResumePatternPlayer(HapticPatternPlayerWrapper* playerWrapper, float time) {
    return [HapticsHelper resumePatternPlayer:playerWrapper atTime:time];
}

bool HapticsHelper_StopPatternPlayer(HapticPatternPlayerWrapper* playerWrapper, float time) {
    return [HapticsHelper stopPatternPlayer:playerWrapper atTime:time];
}

bool HapticsHelper_SeekToOffsetPatternPlayer(HapticPatternPlayerWrapper* playerWrapper, float offsetTime) {
    return [HapticsHelper seekToOffsetPatternPlayer:playerWrapper offsetTime:offsetTime];
}

bool HapticsHelper_GetLoopEnabledPatternPlayer(HapticPatternPlayerWrapper* playerWrapper) {
    return [HapticsHelper getLoopEnabledPatternPlayer:playerWrapper];
}

void HapticsHelper_SetLoopEnabledPatternPlayer(HapticPatternPlayerWrapper* playerWrapper, bool enable) {
    [HapticsHelper setLoopEnabledPatternPlayer:playerWrapper loopEnabled:enable];
}

float HapticsHelper_GetLoopEndPatternPlayer(HapticPatternPlayerWrapper* playerWrapper) {
    return [HapticsHelper getLoopEndPatternPlayer:playerWrapper];
}

void HapticsHelper_SetLoopEndPatternPlayer(HapticPatternPlayerWrapper* playerWrapper, float time) {
    [HapticsHelper setloopEndPatternPlayer:playerWrapper loopEnd:time];
}

float HapticsHelper_GetPlaybackRatePatternPlayer(HapticPatternPlayerWrapper* playerWrapper) {
    return [HapticsHelper getPlaybackRatePatternPlayer:playerWrapper];
}

void HapticsHelper_SetPlaybackRatePatternPlayer(HapticPatternPlayerWrapper* playerWrapper, float playbackRate) {
    [HapticsHelper setPlaybackRatePatternPlayer:playerWrapper playbackRate:playbackRate];
}

#if __cplusplus
}
#endif
