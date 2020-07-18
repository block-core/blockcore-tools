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

         BitcoinPubKeyAddress identity0Address = identity0.PrivateKey.PubKey.GetAddress(profileNetwork);
         BitcoinPubKeyAddress identity1Address = identity1.PrivateKey.PubKey.GetAddress(profileNetwork);

         string identity0Id = identity0Address.ToString();

         // Create an identity profile that should be signed and pushed.
         IdentityModel identityModel = new IdentityModel
         {
            Id = identity0Id,
            Name = "Random Person",
            ShortName = "Random",
            Alias = "Who Knows",
            Email = "mail@mail.com",
            Title = "President"
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

         // Persist the identity.
         var request2 = new RestRequest($"/identity/{identityModel.Id}");
         request2.AddJsonBody(identityDocument);
         IRestResponse<string> response2 = client.Put<string>(request2);

         if (response2.StatusCode != System.Net.HttpStatusCode.OK)
         {
            throw new ApplicationException(response2.ErrorMessage);
         }
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
            Name = "This is a person",
            ShortName = "None of yoru concern",
            Alias = "JD",
            Title = "President of the World",
            Email = "president@president.com"
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

         if (response2.StatusCode != System.Net.HttpStatusCode.OK)
         {
            throw new ApplicationException(response2.ErrorMessage);
         }
      }

      public void PublishIdentity()
      {
         // Publish identity to your local node running with storage feature enabled.
      }

      public void Run()
      {
         //GenerateIdentity();
         GenerateRandomIdentity();
      }

      private RestClient CreateClient()
      {
         // Change the URL to publish to a public node.
         //var client = new RestClient("https://identity.city-chain.org/api");
         var client = new RestClient($"http://localhost:{paymentNetwork.DefaultAPIPort}/api");

         client.UseNewtonsoftJson();
         client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

         return client;
      }
   }
}
