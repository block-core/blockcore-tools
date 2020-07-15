using System;
using System.Collections.Generic;
using System.Text;

namespace Blockcore.Samples.Models
{
   public class IdentityDocument
   {
      public IdentityDocument()
      {

      }

      public string Owner { get; set; }

      public string Signature { get; set; }

      public IdentityModel Body { get; set; }
   }
}
