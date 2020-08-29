using System;
using System.Collections.Generic;
using System.Text;
using Blockcore.Features.Storage.Models;

namespace Blockcore.Samples.Models
{
   public class IdentityDocument : Document<Identity>
   {
      /// <summary>
      /// Version of identity that this document holds. This is not revisions of the document instance, but version of type definition used for compatibility.
      /// </summary>
      public short Version { get; set; }
   }
}
