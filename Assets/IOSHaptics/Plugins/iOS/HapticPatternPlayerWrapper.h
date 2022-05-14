//
//  HapticPatternPlayer.h
//
//  Created by roc on 2022/5/5.
//

#ifndef HapticPatternPlayerWrapper_h
#define HapticPatternPlayerWrapper_h

#import <CoreHaptics/CoreHaptics.h>

class HapticPatternPlayerWrapper {
public:
    HapticPatternPlayerWrapper(id<CHHapticAdvancedPatternPlayer> player) {
        m_player = player;
        static int seed = 0;
        m_id = seed++;
    }
    
    ~HapticPatternPlayerWrapper() {
        m_player = nil;
    }
    
    id<CHHapticAdvancedPatternPlayer> getPlayer() {
        return m_player;
    }
    
    int getPlayerID() {
        return m_id;
    }
    
private:
    id<CHHapticAdvancedPatternPlayer> m_player;
    int m_id;
};

#endif /* HapticPatternPlayerWrapper_h */
