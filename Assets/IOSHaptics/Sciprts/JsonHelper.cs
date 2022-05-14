using System;
using Newtonsoft.Json;

namespace IOSHaptics
{
    public static class JsonHelper
    {
        public static string ToJson(HapticData data)
        {
            if (data == null)
                return null;

            var json = JsonConvert.SerializeObject(data, new HapticDataSerializerSettings());
            return json;
        }

        public static HapticData FromJson(this string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var data = JsonConvert.DeserializeObject<HapticData>(json, new HapticDataSerializerSettings());
            return data;
        }

        private class HapticDataSerializerSettings : JsonSerializerSettings
        {
            public HapticDataSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore;
                Formatting = Formatting.Indented;
                Converters.Add(new HapticEventTypeConverter());
                Converters.Add(new HapticEventParameterIDConverter());
                Converters.Add(new HapticDynamicParamterIDConverter());
            }
        }

        private class HapticEventTypeConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(HapticEventType);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if ((string)reader.Value == HapticEventType.HapticTransient.Value)
                    return HapticEventType.HapticTransient;
                if ((string)reader.Value == HapticEventType.HapticContinuous.Value)
                    return HapticEventType.HapticContinuous;

                UnityEngine.Debug.LogError($"无法将{(string)reader.Value}反序列化为{nameof(HapticEventType)}类型！");
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(((HapticEventType)value).Value);
            }
        }

        private class HapticEventParameterIDConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(HapticEventParameterID);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if ((string)reader.Value == HapticEventParameterID.HapticIntensity.Value)
                    return HapticEventParameterID.HapticIntensity;
                if ((string)reader.Value == HapticEventParameterID.HapticSharpness.Value)
                    return HapticEventParameterID.HapticSharpness;
                if ((string)reader.Value == HapticEventParameterID.AttackTime.Value)
                    return HapticEventParameterID.AttackTime;
                if ((string)reader.Value == HapticEventParameterID.DecayTime.Value)
                    return HapticEventParameterID.DecayTime;
                if ((string)reader.Value == HapticEventParameterID.ReleaseTime.Value)
                    return HapticEventParameterID.ReleaseTime;
                if ((string)reader.Value == HapticEventParameterID.Sustained.Value)
                    return HapticEventParameterID.Sustained;

                UnityEngine.Debug.LogError($"无法将{(string)reader.Value}反序列化为{nameof(HapticEventParameterID)}类型！");
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(((HapticEventParameterID)value).Value);
            }
        }

        private class HapticDynamicParamterIDConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(HapticDynamicParameterID);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if ((string)reader.Value == HapticDynamicParameterID.HapticIntensityControl.Value)
                    return HapticDynamicParameterID.HapticIntensityControl;
                if ((string)reader.Value == HapticDynamicParameterID.SharpnessControl.Value)
                    return HapticDynamicParameterID.SharpnessControl;
                if ((string)reader.Value == HapticDynamicParameterID.AttackTimeControl.Value)
                    return HapticDynamicParameterID.AttackTimeControl;
                if ((string)reader.Value == HapticDynamicParameterID.DecayTimeControl.Value)
                    return HapticDynamicParameterID.DecayTimeControl;
                if ((string)reader.Value == HapticDynamicParameterID.ReleaseTimeControl.Value)
                    return HapticDynamicParameterID.ReleaseTimeControl;
                
                UnityEngine.Debug.LogError($"无法将{(string)reader.Value}反序列化为{nameof(HapticDynamicParameterID)}类型！");
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(((HapticDynamicParameterID)value).Value);
            }
        }
    }
}
