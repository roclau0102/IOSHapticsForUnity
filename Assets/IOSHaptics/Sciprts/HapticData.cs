using System.Collections;
using System.Collections.Generic;

namespace IOSHaptics
{
    /// <summary>
    /// Haptic数据结构类，定义设备如何震动。
    /// </summary>
    public class HapticData
    {
        /// <summary>
        /// 该震动模式的版本信息
        /// </summary>
        public float Version;

        /// <summary>
        /// Haptic震动模式数组
        /// </summary>
        public List<HapticPattern> Pattern = new List<HapticPattern>();
    }

    /// <summary>
    /// haptic震动模式，指示具体的震动方式。
    /// </summary>
    public class HapticPattern
    {
        /// <summary>
        /// 事件驱动震动
        /// </summary>
        public HapticEvent Event;

        /// <summary>
        /// 参数驱动震动（动态）
        /// </summary>
        public HapticParameter Parameter;

        /// <summary>
        /// 参数曲线驱动震动（动态）
        /// </summary>
        public HapticParameterCurve ParameterCurve;
    }

    /// <summary>
    /// haptic事件。指示震动事件如何开始，包括事件类型、时间和可选的事件参数集合(EventParameters)。
    /// </summary>
    public class HapticEvent
    {
        /// <summary>
        /// 事件的相对时间。 单位是秒。
        /// </summary>
        public float Time;

        /// <summary>
        /// 事件类型
        /// </summary>
        public HapticEventType EventType;

        /// <summary>
        /// 事件的持续时间（以秒为单位）。
        /// </summary>
        public float EventDuration;

        /// <summary>
        /// 事件参数数组
        /// </summary>
        public List<HapticEventParameter> EventParameters = new List<HapticEventParameter>();
    }

    /// <summary>
    /// Haptic事件驱动震动的参数
    /// </summary>
    public class HapticEventParameter
    {
        /// <summary>
        /// 参数ID
        /// </summary>
        public HapticEventParameterID ParameterID;

        /// <summary>
        /// 参数值。范围将随参数而变化。
        /// </summary>
        public float ParameterValue;
    }

    /// <summary>
    /// Haptic模式参数。
    /// </summary>
    public class HapticParameter
    {
        public HapticDynamicParameterID ParameterID;
        public float Time;
        public float ParameterValue;
    }

    /// <summary>
    /// Haptic参数曲线
    /// </summary>
    public class HapticParameterCurve
    {
        public HapticDynamicParameterID ParameterID;
        public float Time;
        public string Shape;
        public List<HapticParameterCurveControlPoint> ParameterCurveControlPoints = new List<HapticParameterCurveControlPoint>();
    }

    public class HapticParameterCurveControlPoint
    {
        public float Time;
        public float ParameterValue;
    }

    /// <summary>
    /// Haptic事件类型
    /// </summary>
    public class HapticEventType
    {
        /// <summary>
        /// 生成具有固定长度的触觉的事件，例如点击。瞬态事件将自行完成。如果提供的持续时间大于触觉本身的时间，则包含事件的模式将被“填充”到指定的持续时间。
        /// </summary>
        public static HapticEventType HapticTransient => new HapticEventType("HapticTransient");

        /// <summary>
        /// 产生任意长度触觉的事件。根据 CHHapticEventParameterIDSustained 参数的值，事件可以表现为瞬态或连续。
        /// </summary>
        public static HapticEventType HapticContinuous => new HapticEventType("HapticContinuous");

        public string Value { get; }

        private HapticEventType(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// 用于修改单个Haptic震动事件的参数。
    /// </summary>
    public class HapticEventParameterID
    {
        /// <summary>
        /// 触觉事件的感知强度。
        /// 范围：0.0（最弱） 到 1.0（最强）。
        /// </summary>
        public static HapticEventParameterID HapticIntensity = new HapticEventParameterID("HapticIntensity");

        /// <summary>
        /// 根据事件的信号内容，这可能映射到频率、频率内容（即过滤）或其他一些信号处理。
        /// 范围：0.0（最不尖锐）到 1.0（最尖锐）。
        /// </summary>
        public static HapticEventParameterID HapticSharpness = new HapticEventParameterID("HapticSharpness");

        /// <summary>
        /// 连续事件包络的起音时间调节器。
        /// 范围：0.0 到 1.0，默认值：0.0（最短起音时间）。 较高的值成倍增加时间。
        /// 并非所有事件类型都响应此参数。
        /// </summary>
        public static HapticEventParameterID AttackTime = new HapticEventParameterID("AttackTime");

        /// <summary>
        /// 连续事件包络的衰减时间调节器。
        /// 范围：0.0 到 1.0，默认值：0.0（最短衰减时间）。 较高的值成倍增加时间。
        /// 要使包络衰减生效，必须将 `HapticEventParameterID.Sustained` 参数设置为 0.0。
        /// 并非所有事件类型都响应此参数。
        /// </summary>
        public static HapticEventParameterID DecayTime = new HapticEventParameterID("DecayTime");

        /// <summary>
        /// 连续事件包络的释放时间调整器。
        /// 范围：0.0 到 1.0，默认值：0.0（最短释放时间）。 较高的值成倍增加时间。
        /// 并非所有连续事件类型都响应此参数。
        /// </summary>
        public static HapticEventParameterID ReleaseTime = new HapticEventParameterID("ReleaseTime");

        /// <summary>
        /// 一个布尔值（1.0 或 0.0），指示连续事件是否持续其指定的持续时间。
        /// （使用 Attack/Release 包络）或事件是否在其包络衰减段达到其最小值时结束。
        /// （即使用无延音的起音/衰减包络）。 默认值：1.0（sustained、attack/release）。
        /// </summary>
        public static HapticEventParameterID Sustained = new HapticEventParameterID("Sustained");

        public string Value { get; }

        private HapticEventParameterID(string value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// 用于动态修改模式中所有触觉事件的参数。
    /// </summary>
    public class HapticDynamicParameterID
    {
        /// <summary>
        /// 调整所有正在播放的的和未来播放的震动事件的强度。
        /// 范围：0.0（事件强度减少最大量）到 1.0（对事件强度没有影响）。
        /// 默认值：1.0
        /// </summary>
        public static HapticDynamicParameterID HapticIntensityControl = new HapticDynamicParameterID("HapticIntensityControl");

        /// <summary>
        /// 这将调整所有正在播放的和未来播放的震动事件的频率、频率内容（即过滤）或其他方面。
        /// 范围：-1.0（不那么尖锐）到 1.0（更尖锐）。 默认值：0.0（无效果）。
        /// </summary>
        public static HapticDynamicParameterID SharpnessControl = new HapticDynamicParameterID("HapticSharpnessControl");

        /// <summary>
        /// 调整所有正在播放的和未来播放的震动事件的包络Attack时间。
        /// 范围：-1.0（事件Attack更短）到 1.0（事件Attack更长）。 默认值：0.0（无效果）。
        /// 并非所有事件类型都响应此参数。
        /// </summary>
        public static HapticDynamicParameterID AttackTimeControl = new HapticDynamicParameterID("AttackTimeControl");

        /// <summary>
        /// 调整所有正在播放的和未来播放的瞬态震动事件的包络衰减时间。
        /// 范围：-1.0（事件衰减更短）到 1.0（事件衰减更长）。 默认值：0.0（无效果）。
        /// 并非所有事件类型都响应此参数。
        /// </summary>
        public static HapticDynamicParameterID DecayTimeControl = new HapticDynamicParameterID("DecayTimeControl");

        /// <summary>
        /// 调整所有正在播放的和未来播放的瞬态震动事件的包络释放时间。
        /// 范围：-1.0（事件释放时间较短）到 1.0（事件释放时间较长）。 默认值：0.0（无效果）。
        /// 并非所有事件类型都响应此参数。
        /// </summary>
        public static HapticDynamicParameterID ReleaseTimeControl = new HapticDynamicParameterID("ReleaseTimeControl");

        public string Value { get; }

        private HapticDynamicParameterID(string value)
        {
            Value = value;
        }
    }
}
