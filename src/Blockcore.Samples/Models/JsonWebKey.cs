//using System;
//using System.Collections.Generic;
//using System.Runtime.Serialization;
//using System.Text;
//using MessagePack;
//using Newtonsoft.Json;

//namespace Blockcore.Samples.Models
//{
//   public class JsonWebKey
//   {
//      [Key("alg")]
//      [JsonProperty(PropertyName = "alg")]
//      [DataMember(Name = "alg")]
//      public string Algorithm { get; set; }

//      [Key("kty")]
//      [JsonProperty(PropertyName = "kty")]
//      [DataMember(Name = "kty")]
//      public string KeyType { get; set; } = "EC";

//      [Key("kid")]
//      [JsonProperty(PropertyName = "kid")]
//      [DataMember(Name = "kid")]
//      public string KeyId { get; set; }
//   }
//}
