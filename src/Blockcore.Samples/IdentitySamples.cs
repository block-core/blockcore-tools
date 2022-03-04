using System;
using System.Collections.Generic;
using Blockcore.Features.Storage.Models;
using Blockcore.Networks;
using Blockcore.Samples.Models;
using Jose;
using NBitcoin;
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
         paymentNetwork = Networks.City.Networks.Networks.City.Mainnet.Invoke();
      }

      public static long ToUnixEpochDate(DateTime date) => new DateTimeOffset(date).ToUniversalTime().ToUnixTimeSeconds();

      public string GenerateRandomIdentity()
      {
         // Generate a random seed and new identity.
         var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

         // string recoveryPhrase = "";
         // var mnemonic = new Mnemonic(recoveryPhrase);

         ExtKey masterNode = mnemonic.DeriveExtKey();

         ExtPubKey walletRoot = masterNode.Derive(new KeyPath("m/44'")).Neuter();
         ExtKey identityRoot = masterNode.Derive(new KeyPath("m/302'"));

         ExtKey identity0 = identityRoot.Derive(0, true);
         ExtKey identity1 = identityRoot.Derive(1, true);

         BitcoinPubKeyAddress identity0Address = identity0.PrivateKey.PubKey.GetAddress(profileNetwork);
         BitcoinPubKeyAddress identity1Address = identity1.PrivateKey.PubKey.GetAddress(profileNetwork);

         string identity0Id = identity0Address.ToString();

         // Create an identity profile that should be signed and pushed.
         //Identity identityModel = new Identity
         //{
         //   Identifier = identity0Id,
         //   Name = "Blockcore Hub",
         //   Email = "post@blockcore.net",
         //   Url = "https://city.hub.liberstad.com",
         //   Image = "https://www.blockcore.net/assets/blockcore-256x256.png"
         //};

         //Identity identityModel = new Identity
         //{
         //   Identifier = identity0Id,
         //   Name = "Liberstad Hub",
         //   Email = "post@liberstad.com",
         //   Url = "https://city.hub.liberstad.com",
         //   Image = "https://file.city-chain.org/liberstad-square-logo.png"
         //};

         string key = identity0Address.ToString();
         string identifier = "did:is:" + key;

         var identity = new Identity
         {
            Identifier = identifier,
            Name = "John Doe",
            ShortName = "John",
            Alias = "JD",
            Title = "Gone missing",
            Type = "identity",
            Timpestamp = ToUnixEpochDate(new DateTime(2020, 2, 2))
         };

         //JsonWebKey key = new JsonWebKey();
         //key.KeyId = identity0Id;

         var header = new Dictionary<string, object>();
         header.Add("typ", "JWT");
         header.Add("kid", key);

         string token = JWT.Encode(identity, identity0.PrivateKey, JwsAlgorithm.ES256, header);
         // This should crash with exception: "Blockcore.Jose.JoseException: Payload is missing required signature".

         Message message = new Message()
         {
            Version = 4,
            Content = token
         };

         RestClient client = CreateClient();

         // Persist the identity.
         var request2 = new RestRequest($"/api/identity");
         request2.AddJsonBody(message);
         IRestResponse<string> response2 = client.Put<string>(request2);

         if (response2.StatusCode != System.Net.HttpStatusCode.OK)
         {
            throw new ApplicationException(response2.ErrorMessage);
         }

         return identity0Id;
      }

      //public string GenerateHubIdentity()
      //{
      //   // Generate a random seed and new identity.
      //   var mnemonic = new Mnemonic(Wordlist.English, WordCount.Twelve);

      //   // string recoveryPhrase = "";
      //   // var mnemonic = new Mnemonic(recoveryPhrase);

      //   ExtKey masterNode = mnemonic.DeriveExtKey();

      //   ExtPubKey walletRoot = masterNode.Derive(new KeyPath("m/44'")).Neuter();
      //   ExtKey identityRoot = masterNode.Derive(new KeyPath("m/302'"));

      //   ExtKey identity0 = identityRoot.Derive(0, true);
      //   ExtKey identity1 = identityRoot.Derive(1, true);

      //   BitcoinPubKeyAddress identity0Address = identity0.PrivateKey.PubKey.GetAddress(profileNetwork);
      //   BitcoinPubKeyAddress identity1Address = identity1.PrivateKey.PubKey.GetAddress(profileNetwork);

      //   string identity0Id = identity0Address.ToString();

      //   // Create an identity profile that should be signed and pushed.
      //   //Identity identityModel = new Identity
      //   //{
      //   //   Identifier = identity0Id,
      //   //   Name = "Blockcore Hub",
      //   //   Email = "post@blockcore.net",
      //   //   Url = "https://city.hub.liberstad.com",
      //   //   Image = "https://www.blockcore.net/assets/blockcore-256x256.png"
      //   //};

      //   //Identity identityModel = new Identity
      //   //{
      //   //   Identifier = identity0Id,
      //   //   Name = "Liberstad Hub",
      //   //   Email = "post@liberstad.com",
      //   //   Url = "https://city.hub.liberstad.com",
      //   //   Image = "https://file.city-chain.org/liberstad-square-logo.png"
      //   //};

      //   Identity identityModel = new Identity
      //   {
      //      Type = "identity",
      //      Identifier = identity0Id,
      //      Name = "City Chain Hub",
      //      Email = "post@city-chain.org",
      //      Url = "https://hub.city-chain.org",
      //      Image = "https://city-chain.org/images/icons/city-coin-128x128.png"
      //   };

      //   byte[] entityBytes = MessagePackSerializer.Serialize(identityModel);

      //   // Testing to only sign the name.
      //   string signature = identity0.PrivateKey.SignMessage(entityBytes);

      //   IdentityDocument identityDocument = new IdentityDocument
      //   {
      //      Version = 3,
      //      Id = "identity/" + identityModel.Identifier,
      //      Content = identityModel,
      //      Signature = new Signature() { Identity = identityModel.Identifier, Value = signature }
      //   };

      //   string json = JsonConvert.SerializeObject(identityDocument);

      //   RestClient client = CreateClient();

      //   // Persist the identity.
      //   var request2 = new RestRequest($"/api/identity/{identityModel.Identifier}");
      //   request2.AddJsonBody(identityDocument);
      //   IRestResponse<string> response2 = client.Put<string>(request2);

      //   if (response2.StatusCode != System.Net.HttpStatusCode.OK)
      //   {
      //      throw new ApplicationException(response2.ErrorMessage);
      //   }

      //   return identity0Id;
      //}

      //public string GenerateIdentity()
      //{
      //   // Generate a fixed identity.
      //   string recoveryPhrase = "mystery problem faith negative member bottom concert bundle asthma female process twelve";
      //   var mnemonic = new Mnemonic(recoveryPhrase);
      //   ExtKey masterNode = mnemonic.DeriveExtKey();

      //   ExtPubKey walletRoot = masterNode.Derive(new KeyPath("m/44'")).Neuter();
      //   ExtKey identityRoot = masterNode.Derive(new KeyPath("m/302'"));

      //   ExtKey identity0 = identityRoot.Derive(0, true);
      //   ExtKey identity1 = identityRoot.Derive(1, true);

      //   BitcoinPubKeyAddress identity0Address = identity0.PrivateKey.PubKey.GetAddress(profileNetwork);
      //   BitcoinPubKeyAddress identity1Address = identity1.PrivateKey.PubKey.GetAddress(profileNetwork);

      //   string identity0Id = identity0Address.ToString(); // PTe6MFNouKATrLF5YbjxR1bsei2zwzdyLU
      //   string identity1Id = identity1Address.ToString(); // PAcmQwEMW2oxzRBz7u6oFQMtYPSYqoXyiw

      //   // Create an identity profile that should be signed and pushed.
      //   Identity identityModel = new Identity
      //   {
      //      Type = "identity",
      //      Identifier = identity0Id,
      //      Name = "Sondre Bjellås7",
      //      ShortName = "SondreB",
      //      Alias = "Vanarki",
      //      Title = "I do a little bit of everything.",
      //      Url = "https://www.sondreb.com",
      //      Image = "https://www.sondreb.com/sondre-profile-1024.jpg",
      //      Height = 812689,
      //      Hubs = new string[2] { "PN9Gibo37UzogRC2cBxymvBtbM2p5eNfWi", "PSAiUNhkGpXyW5bHyZHVKPmJNr9R4ZufZe" }
      //   };

      //   byte[] entityBytes = MessagePackSerializer.Serialize(identityModel);

      //   // Testing to only sign the name.
      //   string signature = identity0.PrivateKey.SignMessage(entityBytes);

      //   var identityDocument = new IdentityDocument
      //   {
      //      Id = "identity/" + identityModel.Identifier,
      //      Content = identityModel,
      //      Signature = new Signature() { Identity = identityModel.Identifier, Value = signature }
      //   };

      //   string json = JsonConvert.SerializeObject(identityDocument);

      //   RestClient client = CreateClient();

      //   // Get the identity, if it exists.
      //   var request = new RestRequest($"/api/identity/{identityModel.Identifier}");
      //   IRestResponse<string> response = client.Get<string>(request);

      //   //if (response.StatusCode != System.Net.HttpStatusCode.OK)
      //   //{
      //   //   throw new ApplicationException(response.ErrorMessage);
      //   //}

      //   string data = response.Data;

      //   // Persist the identity.
      //   var request2 = new RestRequest($"/api/identity/{identityModel.Identifier}");
      //   request2.AddJsonBody(identityDocument);
      //   IRestResponse<string> response2 = client.Put<string>(request2);

      //   if (response2.StatusCode != System.Net.HttpStatusCode.OK)
      //   {
      //      throw new ApplicationException(response2.ErrorMessage);
      //   }

      //   return identity0Id;
      //}

      public void PublishIdentity()
      {
         // Publish identity to your local node running with storage feature enabled.
      }

      public void Run()
      {
         //string knownIdentity = GenerateIdentity();
         //Console.WriteLine("Known Identity ID: " + knownIdentity);

         string randomIdentity = GenerateRandomIdentity();
         Console.WriteLine("Generated Identity ID: " + randomIdentity);

         //for (int i = 0; i < 10000; i++)
         //{
         //   string randomIdentity = GenerateRandomIdentity();
         //   Console.WriteLine("Generated Identity ID: " + randomIdentity);
         //}
      }

      private RestClient CreateClient()
      {
         // Change the URL to publish to a public node.
         // var client = new RestClient("https://hub.city-chain.org");
         var client = new RestClient($"http://localhost:{paymentNetwork.DefaultAPIPort}");

         client.UseNewtonsoftJson();
         client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

         return client;
      }
   }
}
