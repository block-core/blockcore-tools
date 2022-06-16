using System;

namespace Blockcore.Samples
{
   class Program
   {
      static void Main(string[] args)
      {
         Console.WriteLine("Blockcore Developer Samples");

         string sample = (args.Length > 0) ? args[0] : "identity";

         Console.WriteLine("Running: " + sample);

         switch (sample)
         {
            case "identity":
               var identitySamples = new IdentitySamples(args);
               identitySamples.Run();
               break;
            case "data":
               var dataSamples = new DataSamples(args);
               dataSamples.Run();
               break;
         }

         Console.WriteLine("Press ENTER to exit.");
         Console.ReadLine();
      }
   }
}
