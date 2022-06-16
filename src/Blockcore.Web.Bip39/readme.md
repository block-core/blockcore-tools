## BIP39 tool using C# wasm (blazor)

A tool to create bip39 mnemonics and derive bip32 keys   
To read more visit the bip39 spec  
https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki

This tool is inspired by   
https://iancoleman.io/bip39  
https://github.com/iancoleman/bip39

![bip39](https://user-images.githubusercontent.com/7487930/174171344-a182d657-f9ce-4105-ad01-4d660a90e442.gif)


### How to deploy

The tool is based on NBitcoin but still some changes need to be made to NBitoin to be able to run it.  
This is because some of the crypto apis are not yet supported with blazor  (maybe in dotnet7)
https://github.com/dotnet/designs/blob/main/accepted/2021/blazor-wasm-crypto.md  

#### Change to NBitcoin

Apply the chagnes in this PR  
https://github.com/MetacoSA/NBitcoin/pull/1106  

In `NBitcoin.csproj`

Target only dotnet 6

```
<TargetFrameworks>net6.0</TargetFrameworks>
```

Comment out this code

```
<!--<PropertyGroup Condition="'$(TargetFramework)' == 'netstandard2.1' Or '$(TargetFramework)' == 'net6.0'">
	<DefineConstants>$(DefineConstants);NETCORE;HAS_SPAN;NO_BC</DefineConstants>
	<RemoveBC>true</RemoveBC>
</PropertyGroup>-->
```

Add this code instead

```
<PropertyGroup Condition=" '$(TargetFramework)' == 'net6.0'">
	<DefineConstants>NO_NATIVE_HMACSHA512;NO_NATIVE_RFC2898_HMACSHA512;NONATIVEHASH</DefineConstants>
	<RemoveBC>false</RemoveBC>
</PropertyGroup>
```

#### Using Blockcore

Blockcore has some base dependencies that cant wrok with wasm.  
To use bloccore first we will refactor the base lib to remove such dependencies that should not be there in the first place
