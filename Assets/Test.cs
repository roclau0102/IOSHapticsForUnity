using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IOSHaptics;

public class Test : MonoBehaviour
{
    public Button SingleTapButton;
    public Button DrumsByFileButton;
    public Button BoingByDataButton;
    public Button BoingByFileButton;

    public Toggle loopEnabledToogle;
    public InputField loopEndInputField;
    public InputField playbackRateInputField;
    public Button createPlayerButton;
    public Button destroyPlayerButton;
    public Button startPlayerButton;
    public Button pausePlayerButton;
    public Button resumePlayerButton;
    public Button stopPlayerButton;

    private HapticPatternPlayer patternPlayer;

    // Start is called before the first frame update
    void Start()
    {
        // test haptics engine
        if (HapticEngine.IsSupported())
        {
            HapticEngine.SetLogCallback(Debug.Log);
            HapticEngine.CreateEngine();
        }

        SingleTapButton.onClick.AddListener(PlaySingleTap);
        DrumsByFileButton.onClick.AddListener(PlayDrumsFile);

        BoingByDataButton.onClick.AddListener(() =>
        {
            PlayBoingData();
        });

        BoingByFileButton.onClick.AddListener(() =>
        {
            PlayBoingFile();
        });

        // test haptics pattern player
        loopEnabledToogle.onValueChanged.AddListener((isOn) =>
        {
            if (patternPlayer != null)
                patternPlayer.LoopEnabled = isOn;
        });

        createPlayerButton.onClick.AddListener(() =>
        {
            patternPlayer?.Destroy();

            patternPlayer = HapticPatternPlayer.Create(GetHeartbeatsData());
            patternPlayer?.SetCompletionHandler((error) =>
            {
                Debug.Log($"pattern player finished with error: {error}");
            });

            patternPlayer.LoopEnabled = loopEnabledToogle.isOn;
            if (float.TryParse(loopEndInputField.text.Trim(), out var loopEnd))
            {
                patternPlayer.LoopEnd = loopEnd;
            }
            if (float.TryParse(playbackRateInputField.text.Trim(), out var playbackRate))
            {
                patternPlayer.PlaybackRate = playbackRate;
            }

            Debug.Log($"pattern player: loopEnd-{patternPlayer.LoopEnd}, loopEnabled-{patternPlayer.LoopEnabled}, playbackRate-{patternPlayer.PlaybackRate}");
        });

        destroyPlayerButton.onClick.AddListener(() =>
        {
            patternPlayer?.Destroy();
            patternPlayer = null;
        });

        startPlayerButton.onClick.AddListener(() =>
        {
            patternPlayer?.Start();
        });

        pausePlayerButton.onClick.AddListener(() =>
        {
            patternPlayer?.Pause();
        });

        resumePlayerButton.onClick.AddListener(() =>
        {
            patternPlayer?.Resume();
        });

        stopPlayerButton.onClick.AddListener(() =>
        {
            patternPlayer?.Stop();
        });
    }

    void PlaySingleTap()
    {
        HapticEngine.PlaySingleTap();
    }

    void PlayBoingFile()
    {
        var path = Application.streamingAssetsPath + "/AHAP/Boing.ahap";
        HapticEngine.PlayPatternFromFile(path);
    }

    void PlayBoingData()
    {
        patternPlayer?.Destroy();

        patternPlayer = HapticPatternPlayer.Create(GetBoingData());
        patternPlayer?.SetCompletionHandler((error) =>
        {
            Debug.Log($"finished boing with error: {error}");
        });
        patternPlayer?.Start();
    }

    void PlayDrumsFile()
    {
        var path = Application.streamingAssetsPath + "/AHAP/Drums.ahap";
        HapticEngine.PlayPatternFromFile(path);
    }

    void PlayDrumsData()
    {
        if (patternPlayer != null)
            patternPlayer.Destroy();

        patternPlayer = HapticPatternPlayer.Create(GetDrumsData());
        patternPlayer.SetCompletionHandler((error) =>
        {
            var message = string.IsNullOrEmpty(error) ? "Drums finished" : $"Drums finished with error: {error}";
            Debug.Log(message);
        });
        patternPlayer.Start();
    }

    void PlayHeartbeatsFile()
    {
        var path = Application.streamingAssetsPath + "/AHAP/Heartbeats.ahap";
        HapticEngine.PlayPatternFromFile(path);
    }

    void PlayHeartbeatsData()
    {
        if (patternPlayer != null)
            patternPlayer.Destroy();

        patternPlayer = HapticPatternPlayer.Create(GetHeartbeatsData());
        patternPlayer.SetCompletionHandler((error) =>
        {
            var message = string.IsNullOrEmpty(error) ? "Heartbeats finished" : $"Heartbeats finished with error: {error}";
            Debug.Log(message);
        });
        patternPlayer.Start();
    }

    #region Get haptic data
    HapticData GetBoingData()
    {
        var data = new HapticData();
        data.Version = 1.0f;

        var pattern = new HapticPattern()
        {
            Event = new HapticEvent()
            {
                Time = 0f,
                EventType = HapticEventType.HapticTransient,
                EventParameters = new List<HapticEventParameter>()
                {
                    new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.8f },
                    new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.4f }
                }
            }
        };

        var pattern2 = new HapticPattern()
        {
            Event = new HapticEvent()
            {
                Time = 0.015f,
                EventType = HapticEventType.HapticContinuous,
                EventDuration = 0.25f,
                EventParameters = new List<HapticEventParameter>()
                {
                    new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.8f },
                    new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.4f }
                }
            }
        };

        var pattern3 = new HapticPattern()
        {
            ParameterCurve = new HapticParameterCurve()
            {
                ParameterID = HapticDynamicParameterID.HapticIntensityControl,
                Time = 0.015f,
                ParameterCurveControlPoints = new List<HapticParameterCurveControlPoint>()
                {
                    new HapticParameterCurveControlPoint() { Time = 0, ParameterValue = 1 },
                    new HapticParameterCurveControlPoint() { Time = 0.1f, ParameterValue = 0.5f },
                    new HapticParameterCurveControlPoint() { Time = 0.25f, ParameterValue = 0f }
                }
            }
        };

        var pattern4 = new HapticPattern()
        {
            ParameterCurve = new HapticParameterCurve()
            {
                ParameterID = HapticDynamicParameterID.SharpnessControl,
                Time = 0.015f,
                ParameterCurveControlPoints = new List<HapticParameterCurveControlPoint>()
                {
                    new HapticParameterCurveControlPoint() { Time = 0, ParameterValue = 0 },
                    new HapticParameterCurveControlPoint() { Time = 0.25f, ParameterValue = -0.3f }
                }
            }
        };

        data.Pattern.Add(pattern);
        data.Pattern.Add(pattern2);
        data.Pattern.Add(pattern3);
        data.Pattern.Add(pattern4);

        return data;
    }

    HapticData GetDrumsData()
    {
        var data = new HapticData()
        {
            Version = 1,
            Pattern = new List<HapticPattern>()
            {
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 0,
                        EventType = HapticEventType.HapticTransient,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0 }
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 0.391f,
                        EventType = HapticEventType.HapticTransient,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0 },
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 1 }
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 0.8f,
                        EventType = HapticEventType.HapticTransient,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.3f },
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 1f }
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 1.2f,
                        EventType = HapticEventType.HapticTransient,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0 },
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 1}
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 1.41f,
                        EventType = HapticEventType.HapticTransient,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0 },
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 1}
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 1.6f,
                        EventType = HapticEventType.HapticTransient,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0 }
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 0,
                        EventType = HapticEventType.HapticContinuous,
                        EventDuration = 0.2f,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.6f }
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 0.2f,
                        EventType = HapticEventType.HapticContinuous,
                        EventDuration = 0.2f,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.3f }
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 0.4f,
                        EventType = HapticEventType.HapticContinuous,
                        EventDuration = 0.15f,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.25f },
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 1.0f }
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 0.8f,
                        EventType = HapticEventType.HapticContinuous,
                        EventDuration = 0.15f,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.25f },
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 1.0f }
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 1.2f,
                        EventType = HapticEventType.HapticContinuous,
                        EventDuration = 0.15f,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.25f },
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 1.0f }
                        }
                    }
                },
                new HapticPattern()
                {
                    Event = new HapticEvent()
                    {
                        Time = 1.6f,
                        EventType = HapticEventType.HapticContinuous,
                        EventDuration = 1.0f,
                        EventParameters = new List<HapticEventParameter>()
                        {
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.45f },
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.9f },
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.DecayTime, ParameterValue = 1f },
                            new HapticEventParameter() { ParameterID = HapticEventParameterID.Sustained, ParameterValue = 0f }
                        }
                    }
                }
            }
        };

        return data;
    }

    HapticData GetHeartbeatsData()
    {
        var data = new HapticData();
        data.Version = 1.0f;
        data.Pattern = new List<HapticPattern>()
        {
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 0.0f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.8f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.2f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 0.013f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 1.0f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.3f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 0.220f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.8f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.1f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 0.255f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.7f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.0f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 1.0f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.8f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.2f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 1.013f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 1.0f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.3f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 1.220f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.8f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.1f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 1.255f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.7f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.0f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 2.0f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.8f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.2f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 2.013f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 1.0f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.3f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 2.220f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.8f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.1f }
                    }
                }
            },
            new HapticPattern()
            {
                Event = new HapticEvent()
                {
                    Time = 2.255f,
                    EventType = HapticEventType.HapticTransient,
                    EventParameters = new List<HapticEventParameter>()
                    {
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticIntensity, ParameterValue = 0.7f },
                        new HapticEventParameter() { ParameterID = HapticEventParameterID.HapticSharpness, ParameterValue = 0.0f }
                    }
                }
            },
        };

        return data;
    }
    #endregion
}
