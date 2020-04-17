using System;
using System.Collections.Generic;
using System.Text;

namespace Blockcore.Generator
{
   public static class BlockcoreLogo
   {
      // TODO: Add the logo to the main lib and make it accessible from there.
      public static string GetAsciiLogo(string title)
      {
         return Properties.Resources.Logo.Replace("{title}", title);
      }
   }
}
