using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Blockcore.Features.Storage.Models
{
   public class Ticket : EntityBase
   {
      [StringLength(512)]
      [JsonProperty(PropertyName = "name")]
      [DataMember(Name = "name")]
      public string Name { get; set; }

      [StringLength(512)]
      [JsonProperty(PropertyName = "event")]
      [DataMember(Name = "event")]
      public string Event { get; set; }
   }
}
