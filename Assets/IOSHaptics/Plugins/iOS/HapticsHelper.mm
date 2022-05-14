//
//  HapticsHelper.m
//
//  Created by roc on 2022/3/15.
//

#import "HapticsHelper.h"
#import <CoreHaptics/CoreHaptics.h>

@implementation HapticsHelper

API_AVAILABLE(ios(13.0))
static CHHapticEngine* HHEngine = nil;
static HapticsHelper_Callback_t HHLogCallback = NULL;

void HHLog(NSString* format, ...) {
    va_list args;
    va_start(args, format);
    NSString* log = [[NSString alloc] initWithFormat:format arguments:args];
    NSString* formatLog = [NSString stringWithFormat:@"[HapticsHelper] %@", log];
    if (HHLogCallback) {
        HHLogCallback([formatLog UTF8String]);
    } else {
        NSLog(@"%@", formatLog);
    }
    va_end(args);
}

BOOL HHIsValid() {
    if (@available(iOS 13.0, *)) {
        BOOL isSupported = CHHapticEngine.capabilitiesForHardware.supportsHaptics;
        if (!isSupported) {
            HHLog(@"Core Haptics不支持iPhone8以下或iPad设备!");
            return NO;
        }
    } else {
        HHLog(@"Core Hapitics不支持iOS13以下系统!");
        return NO;
    }
    
    return YES;
}

+ (void)setLogCallback:(HapticsHelper_Callback_t)callback {
    HHLogCallback = callback;
}

+ (void)createEngine API_AVAILABLE(ios(13.0)) {
    if (!HHIsValid()) {
        return;
    }
    
    NSError* initError;
    HHEngine = [[CHHapticEngine alloc] initAndReturnError:&initError];
    if (initError) {
        HHLog(@"ERROR HapticsHelper couldn't init engine! %@", [initError localizedDescription]);
        return;
    }
    
    [HHEngine setStoppedHandler:^(CHHapticEngineStoppedReason reason){
        HHLog(@"HapticsHelper Engine STOPPED!");
        switch (reason)
        {
            case CHHapticEngineStoppedReasonAudioSessionInterrupt: {
                HHLog(@"REASON: Audio Session Interrupt");
                break;
            }
            case CHHapticEngineStoppedReasonApplicationSuspended: {
                HHLog(@"REASON: Application Suspended");
                break;
            }
            case CHHapticEngineStoppedReasonIdleTimeout: {
                HHLog(@"REASON: Idle Timeout");
                break;
            }
            case CHHapticEngineStoppedReasonNotifyWhenFinished: {
                HHLog(@"REASON: Notify When Finished");
                break;
            }
            case CHHapticEngineStoppedReasonEngineDestroyed: {
                HHLog(@"REASON: Destroyed");
                break;
            }
            case CHHapticEngineStoppedReasonGameControllerDisconnect: {
                HHLog(@"REASON: GameController Disconnect");
                break;
            }
            case CHHapticEngineStoppedReasonSystemError: {
                HHLog(@"REASON: System Error");
                break;
            }
        }
    }];
    
    [HHEngine setResetHandler:^{
        HHLog(@"Haptic Engine RESET!");
        NSError* startError;
        [HHEngine startAndReturnError:&startError];
        if (startError) {
            HHLog(@"ERROR: HapticsHelper couldn't restart engine! %@", [startError localizedDescription]);
        }
    }];
    
    HHLog(@"Created Haptic Engine.");
}

+ (BOOL)isSupported {
    return HHIsValid();
}

+ (void)playSingleTap API_AVAILABLE(ios(13.0)) {
    if (!HHIsValid) {
        return;
    }
    
    NSDictionary* hapticDict = @{
        CHHapticPatternKeyPattern: @[
            @{ CHHapticPatternKeyEvent: @{
                CHHapticPatternKeyEventType: CHHapticEventTypeHapticTransient,
                CHHapticPatternKeyTime: @(CHHapticTimeImmediate),
                CHHapticPatternKeyEventDuration: @1.0 },
            },
        ],
    };
    
    NSError* error;
    CHHapticPattern* pattern = [[CHHapticPattern alloc] initWithDictionary:hapticDict error:&error];
    if (error) {
        HHLog(@"ERROR HapticEngine could't create pattern! %@", [error localizedDescription]);
    }
    
    id<CHHapticPatternPlayer> player = [HHEngine createPlayerWithPattern:pattern error:&error];
    if (error) {
        HHLog(@"ERROR HapticEngine could't create pattern player! %@", [error localizedDescription]);
    }
    
    [HHEngine startWithCompletionHandler:^(NSError * _Nullable error) {
        [player startAtTime:0 error:&error];
        if (error) {
            HHLog(@"ERROR HapticEngine could't start pattern player! %@", [error localizedDescription]);
        }
    }];
    
    HHLog(@"Play a single tap.");
}

+ (void)playPatternFromFile:(NSString*)filePath API_AVAILABLE(ios(13.0)) {
    if (!HHIsValid()) {
        return;
    }

    NSError* error;
    [HHEngine startAndReturnError:&error];
    if (error) {
        HHLog(@"ERROR: HapticsHelper couldn't start Haptic Engine! %@", [error localizedDescription]);
        return;
    }

    NSURL* url = [NSURL fileURLWithPath:filePath];
    [HHEngine playPatternFromURL:url error:&error];
    if (error) {
        HHLog(@"ERROR: HapticsHelper couldn't play pattern from file! %@", [error localizedDescription]);
        return;
    }

    HHLog(@"Play pattern from file:%@", filePath);
}

+ (CHHapticPattern*)initPatternByJson:(NSString*)json API_AVAILABLE(ios(13.0)) {
    if (!json) {
        return nil;
    }
    
    NSError* error;
    NSDictionary* hapticDict = [NSJSONSerialization JSONObjectWithData:[json dataUsingEncoding:NSUTF8StringEncoding] options:0 error:&error];
    if (error) {
        HHLog(@"ERROR HapticEngine could't init NSDictionary from json: %@! %@", json, [error localizedDescription]);
        return nil;
    }
    
    CHHapticPattern* pattern = [[CHHapticPattern alloc] initWithDictionary:hapticDict error:&error];
    if (error) {
        HHLog(@"ERROR HapticEngine could't create pattern! %@", [error localizedDescription]);
        return nil;
    }
    
    return pattern;
}

+ (HapticPatternPlayerWrapper*)createHapticPatternPlayerByJson:(NSString *)json API_AVAILABLE(ios(13.0)) {
    if (!HHIsValid) {
        return nullptr;
    }
    
    CHHapticPattern* pattern = [self initPatternByJson:json];
    
    if (!pattern) {
        return nullptr;
    }
    
    NSError* error;
    id<CHHapticAdvancedPatternPlayer> player = [HHEngine createAdvancedPlayerWithPattern:pattern error:&error];
    if (error) {
        HHLog(@"ERROR HapticEngine could't create player by json! %@", [error localizedDescription]);
        return nullptr;
    }
    
    HapticPatternPlayerWrapper* patternPlayer = new HapticPatternPlayerWrapper(player);
    HHLog(@"Created pattern player with id: %d by json.", patternPlayer->getPlayerID());
    return patternPlayer;
}

+ (void)destroyPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper {
    if (playerWrapper) {
        HHLog(@"Destroied pattern player with id: %d.", playerWrapper->getPlayerID());
        delete playerWrapper;
    }
}

+ (BOOL)startPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper atTime:(float)time API_AVAILABLE(ios(13.0)) {
    if (!HHIsValid) {
        return NO;
    }
    
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't playPatternPlayer, patternPlayer pointer was null!");
        return NO;
    }
    
    NSError* error = nil;
    [HHEngine startAndReturnError:&error];
    
    if (error) {
        HHLog(@"ERROR HapticEngine startWithCompletionHandler failed! %@", [error localizedDescription]);
        return NO;
    }
    
    id<CHHapticAdvancedPatternPlayer> player = playerWrapper->getPlayer();
    if (!player) {
        HHLog(@"ERROR HapticEngine get null player!");
        return NO;
    }
    
    if (player) {
        [player startAtTime:time error:&error];
        if (error) {
            HHLog(@"ERROR HapticEngine startAtTime failed! %@", [error localizedDescription]);
            return NO;
        }
        return YES;
    }
    
    return NO;
}

+ (BOOL)pausePatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper atTime:(float)time API_AVAILABLE(ios(13.0)) {
    if (!HHIsValid) {
        return NO;
    }
    
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't pausePatternPlayer, patternPlayer pointer was null!");
        return NO;
    }
    
    NSError* error = nil;
    [HHEngine startAndReturnError:&error];
    if (error) {
        HHLog(@"ERROR HapticEngine startWithCompletionHandler failed! %@", [error localizedDescription]);
        return NO;
    }
    
    id<CHHapticAdvancedPatternPlayer> player = playerWrapper->getPlayer();
    if (!player) {
        HHLog(@"ERROR HapticEngine get null player!");
        return NO;
    }
    
    if (player) {
        [player pauseAtTime:time error:&error];
        if (error) {
            HHLog(@"ERROR HapticEngine startAtTime failed! %@", [error localizedDescription]);
            return NO;
        }
        return YES;
    }
    
    return NO;
}

+ (BOOL)resumePatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper atTime:(float)time API_AVAILABLE(ios(13.0)) {
    if (!HHIsValid) {
        return NO;
    }
    
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't resumePatternPlayer, patternPlayer pointer was null!");
        return NO;
    }
    
    NSError* error = nil;
    [HHEngine startAndReturnError:&error];
    if (error) {
        HHLog(@"ERROR HapticEngine startWithCompletionHandler failed! %@", [error localizedDescription]);
        return NO;
    }
    
    id<CHHapticAdvancedPatternPlayer> player = playerWrapper->getPlayer();
    if (!player) {
        HHLog(@"ERROR HapticEngine get null player!");
        return NO;
    }
    
    if (player) {
        [player resumeAtTime:time error:&error];
        if (error) {
            HHLog(@"ERROR HapticEngine startAtTime failed! %@", [error localizedDescription]);
            return NO;
        }
        return YES;
    }
    
    return NO;
}

+ (BOOL)stopPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper atTime:(float)time API_AVAILABLE(ios(13.0)) {
    if (!HHIsValid) {
        return NO;
    }
    
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't stopPatternPlayer, patternPlayer pointer was null!");
        return NO;
    }
    
    NSError* error = nil;
    [HHEngine startAndReturnError:&error];
    if (error) {
        HHLog(@"ERROR HapticEngine startWithCompletionHandler failed! %@", [error localizedDescription]);
        return NO;
    }
    
    id<CHHapticAdvancedPatternPlayer> player = playerWrapper->getPlayer();
    if (!player) {
        HHLog(@"ERROR HapticEngine get null player!");
        return NO;
    }
    
    if (player) {
        [player stopAtTime:time error:&error];
        if (error) {
            HHLog(@"ERROR HapticEngine startAtTime failed! %@", [error localizedDescription]);
            return NO;
        }
        return YES;
    }
    
    return NO;
}

+ (BOOL)seekToOffsetPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper offsetTime:(float)offsetTime {
    if (!HHIsValid) {
        return NO;
    }
    
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't seekToOffsetPatternPlayer, patternPlayer pointer was null!");
        return NO;
    }
    
    id<CHHapticAdvancedPatternPlayer> player = playerWrapper->getPlayer();
    if (!player) {
        HHLog(@"ERROR HapticEngine get null player!");
        return NO;
    }
    
    if (player) {
        NSError* error = nil;
        [player seekToOffset:offsetTime error:&error];
        if (error) {
            HHLog(@"ERROR HapticEngine seekToOffset failed! %@", [error localizedDescription]);
            return NO;
        }
        return YES;
    }
    
    return NO;
}

+ (BOOL)getLoopEnabledPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper {
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't getLoopEnabledPatternPlayer, patternPlayer pointer was null!");
        return NO;
    }
    return [playerWrapper->getPlayer() loopEnabled];
}

+ (void)setLoopEnabledPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper loopEnabled:(BOOL)loopEnabled {
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't setLoopEnabledPatternPlayer, patternPlayer pointer was null!");
        return;
    }
    [playerWrapper->getPlayer() setLoopEnabled:loopEnabled];
}

+ (float)getLoopEndPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper {
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't getLoopEndPatternPlayer, patternPlayer pointer was null!");
        return 0;
    }
    return [playerWrapper->getPlayer() loopEnd];
}

+ (void)setloopEndPatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper loopEnd:(float)loopEnd {
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't setloopEndPatternPlayer, patternPlayer pointer was null!");
        return;
    }
    [playerWrapper->getPlayer() setLoopEnd:loopEnd];
}

+ (float)getPlaybackRatePatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper {
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't getPlaybackRatePatternPlayer, patternPlayer pointer was null!");
        return 0;
    }
    return [playerWrapper->getPlayer() playbackRate];
}

+ (void)setPlaybackRatePatternPlayer:(HapticPatternPlayerWrapper*)playerWrapper playbackRate:(float)playbackRate {
    if (!playerWrapper) {
        HHLog(@"ERROR HapticEngine could't setloopEndPatternPlayer, patternPlayer pointer was null!");
        return;
    }
    [playerWrapper->getPlayer() setPlaybackRate:playbackRate];
}

+ (void)setPatternPlayerCompletionHandler:(HapticPatternPlayerWrapper*)playerWrapper
                        completionHandler:(HapticsHelper_PatternPlayerCompletionHandler_t)completionHandler {
    if (playerWrapper && completionHandler) {
        [playerWrapper->getPlayer() setCompletionHandler:^(NSError * _Nullable error) {
            HHLog(@"pattern player completion, player id: %d, error: %@", playerWrapper->getPlayerID(), [error localizedDescription]);
            const char* errMsg = error ? [[error localizedDescription] UTF8String] : "";
            completionHandler(playerWrapper->getPlayerID(), errMsg);
        }];
    }
}

@end
