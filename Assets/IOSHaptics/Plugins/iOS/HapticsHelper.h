//
//  HapticsHelper.h
//
//  Created by roc on 2022/3/15.
//

#import <Foundation/Foundation.h>
#import <CoreHaptics/CoreHaptics.h>
#include "HapticPatternPlayerWrapper.h"

NS_ASSUME_NONNULL_BEGIN

typedef void(*HapticsHelper_Callback_t)(const char* msg);
typedef void(*HapticsHelper_PatternPlayerCompletionHandler_t)(int playerId, const char* error);

@interface HapticsHelper : NSObject

+ (void)setLogCallback:(HapticsHelper_Callback_t)callback;
+ (BOOL)isSupported;
+ (void)createEngine;
+ (void)playSingleTap;
+ (void)playPatternFromFile:(NSString*)filePath;

// create & destroy
+ (HapticPatternPlayerWrapper *)createHapticPatternPlayerByJson:(NSString*)json;
+ (void)destroyPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper;

// finished callback
+ (void)setPatternPlayerCompletionHandler:(HapticPatternPlayerWrapper*)playerWrapper
                        completionHandler:(HapticsHelper_PatternPlayerCompletionHandler_t)completionHandler;

// Controlling Playback
+ (BOOL)startPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper atTime:(float)time;
+ (BOOL)pausePatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper atTime:(float)time;
+ (BOOL)resumePatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper atTime:(float)time;
+ (BOOL)stopPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper atTime:(float)time;
+ (BOOL)seekToOffsetPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper offsetTime:(float)offsetTime;

// loopEnabled
+ (BOOL)getLoopEnabledPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper;
+ (void)setLoopEnabledPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper loopEnabled:(BOOL)loopEnabled;

// loopEnd
+ (float)getLoopEndPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper;
+ (void)setloopEndPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper loopEnd:(float)loopEnd;

// playbackRate
+ (float)getPlaybackRatePatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper;
+ (void)setPlaybackRatePatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper playbackRate:(float)playbackRate;

@end

NS_ASSUME_NONNULL_END
