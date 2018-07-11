using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MoleDeploy.Contracts
{
    public class VstsBuildStateChangeNotification
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public VstsBuildState State { get; set; }

        public string Message { get; set; }
    }
}
