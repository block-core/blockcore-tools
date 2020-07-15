using System;
using System.Collections.Generic;
using System.Text;
using Blockcore.Samples.Models;
using MessagePack;
using NBitcoin;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Blockcore.Samples
{
   public class IdentitySamples
   {
      private readonly string[] args;

      private readonly Network profileNetwork;

      private readonly Network paymentNetwork;

      public IdentitySamples(string[] args)
      {
         this.args = args;

         profileNetwork = ProfileNetwork.Instance;
         paymentNetwork = City.Networks.Networks.City.Mainnet.Invoke();
      }

      public void GenerateRandomIdentity()
      {
         // Generate a random seed and new identity.
         var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);
         ExtKey masterNode = mnemonic.DeriveExtKey();

         ExtPubKey walletRoot = masterNode.Derive(new KeyPath("m/44'")).Neuter();
         ExtKey identityRoot = masterNode.Derive(new KeyPath("m/302'"));

         ExtKey identity0 = identityRoot.Derive(0, true);
         ExtKey identity1 = identityRoot.Derive(1, true);

         BitcoinPubKeyAddress identity0Id = identity0.PrivateKey.PubKey.GetAddress(profileNetwork);
         BitcoinPubKeyAddress identity1Id = identity1.PrivateKey.PubKey.GetAddress(profileNetwork);
      }

      public void GenerateIdentity()
      {
         // Generate a fixed identity.
         string recoveryPhrase = "mystery problem faith negative member bottom concert bundle asthma female process twelve";
         var mnemonic = new Mnemonic(recoveryPhrase);
         ExtKey masterNode = mnemonic.DeriveExtKey();

         ExtPubKey walletRoot = masterNode.Derive(new KeyPath("m/44'")).Neuter();
         ExtKey identityRoot = masterNode.Derive(new KeyPath("m/302'"));

         ExtKey identity0 = identityRoot.Derive(0, true);
         ExtKey identity1 = identityRoot.Derive(1, true);

         BitcoinPubKeyAddress identity0Address = identity0.PrivateKey.PubKey.GetAddress(profileNetwork);
         BitcoinPubKeyAddress identity1Address = identity1.PrivateKey.PubKey.GetAddress(profileNetwork);

         string identity0Id = identity0Address.ToString(); // PTe6MFNouKATrLF5YbjxR1bsei2zwzdyLU
         string identity1Id = identity1Address.ToString(); // PAcmQwEMW2oxzRBz7u6oFQMtYPSYqoXyiw

         // Create an identity profile that should be signed and pushed.
         IdentityModel identityModel = new IdentityModel
         {
            Id = identity0Id,
            Name = "John Doe",
            ShortName = "JD",
            Alias = "Jane"
         };

         byte[] entityBytes = MessagePackSerializer.Serialize(identityModel);

         // Testing to only sign the name.
         string signature = identity0.PrivateKey.SignMessage(entityBytes);

         IdentityDocument identityDocument = new IdentityDocument
         {
            Owner = identityModel.Id,
            Body = identityModel,
            Signature = signature
         };

         string json = JsonConvert.SerializeObject(identityDocument);

         RestClient client = CreateClient();

         // Get the identity, if it exists.
         var request = new RestRequest($"/identity/{identityModel.Id}");
         IRestResponse<string> response = client.Get<string>(request);

         //if (response.StatusCode != System.Net.HttpStatusCode.OK)
         //{
         //   throw new ApplicationException(response.ErrorMessage);
         //}

         string data = response.Data;

         // Persist the identity.
         var request2 = new RestRequest($"/identity/{identityModel.Id}");
         request2.AddJsonBody(identityDocument);
         IRestResponse<string> response2 = client.Put<string>(request2);

         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            throw new ApplicationException(response.ErrorMessage);
         }
      }

      public void PublishIdentity()
      {
         // Publish identity to your local node running with storage feature enabled.
      }

      public void Run()
      {
         GenerateIdentity();
      }

      private RestClient CreateClient()
      {
         var client = new RestClient($"http://localhost:{paymentNetwork.DefaultAPIPort}/api");
         client.UseNewtonsoftJson();
         client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
         client.AddDefaultHeader("Node-Api-Key", "1234");

         return client;
      }
   }
}
