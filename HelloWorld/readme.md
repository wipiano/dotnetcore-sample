# .NET Core 2.0 (1) - Hello, World

[サンプルコード](https://github.com/wipiano/dotnetcore-sample/tree/master/HelloWorld)

## やること

* .NET Core 概要
* .NET Standard 概要
* Hello World
* フレームワーク依存の展開 (FDD: Framework-Dependent Deployments)
* 自己完結型の展開(SCD: Self-Contained Deployments)
* .NET Standard クラスライブラリの作成と利用
* nuget パッケージの作成

## .NET Core

* .NET Framework みたいなもの
* クロスプラットフォーム
* オープンソース
* パフォーマンスが良い (ASP.NET Core は ASP.NET の 10 倍速い)
* ランタイムがインストールされてなくても動かせる (self-contained deployment)
  - ランタイムに依存しない実行ファイルを作れる
  - ランタイムのバージョンを気にせず新しいバージョンを使える
  - ランタイムのインストールが不要なので同一サーバ上に複数バージョンの .NET Core を使用するアプリケーションをデプロイできる
  - デプロイするファイルサイズ自体は大きくなる
* .NET Core 2.0 からは API がかなり増えて .NET Framework の dll を直接参照可能に (.NET Core で使えないものが含まれてるとダメだけど)
* 一部サポートされてないものがある
  - ASP.NET の WebForms
  - WPF アプリ

## .NET Standard

* .NET はこういう API を実装しましょうね、という仕様
* クラスライブラリしか書けない
* これで書かれていれば .NET Framework, .NET Core 両方から使える
* 実際の実装は個々のフレームワーク(.NET Framework, .NET Core) におまかせ
* .NET Framework にしかない機能 (Windows 依存とか)、 .NET Core にしかない新機能両方ある
  - これらは含まれてない
* とはいえよく使う API はひととおりそろってるので移行はラク
* .NET Core 2.0 , .NET Framework 4.6.1 以上は .NET Standard 2.0 の API をフルでサポート 

.NET Standard のバージョンについては https://github.com/dotnet/standard/blob/master/docs/versions.md

## Hello, World

```csharp
using System;

namespace NetCoreSample.HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
```

### プロジェクトの作成

新規作成 -> プロジェクト -> Visual C# -> .NET Core -> コンソールアプリ (.NET Core) target framework は気にしない  
これで .NET Core 2.0 の ConsoleApp ができる  
プロジェクトのプロパティを見ると .NET Core 2.0 になってる  

とりあえず実行してみる (Ctrl + F5)  

### bin の中を見てみる

`bin/Debug/netcoreapp2.0`
.NET Framework のビルドとちがって exe が生成されない。 dll のみ。  

### dll を実行してみる
この状態だと、実行するときは  
`dotnet NetCoreSample.HelloWorld.dll`  
で実行できる  
csproj 直接指定して ビルド→実行 もできる  
`dotnet run NetCoreSample.HelloWorld.csproj`  

### csproj のちがい
csproj の中をみてみる (.NET Framework の旧形式との比較)  
とてもシンプル、Program.cs とかも含まれてない  
旧形式みたいにがんばって全部のファイル細かく書いていても、たいていデフォルトのまま使っちゃうので書かないことにした  
None Include とか特別に指定するものだけ書く  
`<Project Sdk="Microsoft.NET.Sdk">`  
とか書いてあると、 System.dll など基本的に必要な dll はすべて参照にはいる  
細かい指定とかは csproj を直接編集していろいろ指定できる  

## フレームワーク依存の展開 (Framework-dependent deployments)

.NET Framework のデプロイと同じイメージ。  
実行にはそのバージョンの .NET Core のランタイムがインストールされてないといけない。

普通にビルド or publish  
コマンドラインでは `dotnet publish`  

作成した dll は `dotnet [dll名]` で実行可能  


## 単体で実行可能なファイルをつくる (Self-contained deployments)

### csproj の編集

実行可能ファイルを作るには、プロジェクトファイルに RuntimeIndentifiers を追加する  
https://docs.microsoft.com/ja-jp/dotnet/core/rid-catalog

例:

```xml
<PropertyGroup>
  <RuntimeIdentifiers>win8-x64;ubuntu.16.04-x64;osx.10.10-x64</RuntimeIdentifiers>
</PropertyGroup>
```

### 発行

とりあえず適当にプロファイル作って発行すると、その OS 向けの実行可能ファイル (.NET Core ランタイム非依存) ができる  
target 変えたければ Settings... をクリックして target runtime を変更  
(じつは OS だけじゃなくて target framework も複数かけちゃったりする)  

windows 向けの exe を実行してみるとちゃんと実行できることがわかる  
ubuntu むけ、 mac 向けも同様 (実行可能なバイナリができてる)  


#### コマンドでの発行

Visual Studio がなくても発行もビルドもできちゃう  
全部 dotnet コマンドでできる  

* nuget パッケージの復元: `dotnet restore`
* ビルド: `dotnet build`
* 発行: `dotnet publish -r win8-x64`

コマンドで発行する場合は csproj に RuntimeIdentifiers の指定は不要  

## .NET Standard クラスライブラリの作成と利用

特別に理由 (.NET Core/Framework どちらかでしか使えない機能を使うとか) がなければ、.NET Standard でかけばOK  
.NET Standard 1.x 系のときはリフレクションまわりの互換性などの問題があったが、 .NET Standard 2.0 で解消されてる  
(ほんとうはリフレクションを Type クラスから分離して TypeInfo 経由のリフレクションにしたかった)  

プロジェクト -> .NET Standard -> クラスライブラリ  

(.NET Standard は API の定義しか提供しないのでクラスライブラリしかつくれない)  

.NET Standard 2.0 は .NET Core 2.0 または .NET Framework 4.6.1 以上 で使用可能  
使うときは普通にプロジェクト参照で  

### ためしに MessagePack でいろいろするライブラリをつくってみる

nuget で `MessagePack`, `MessagepackAnalyzer` をいれる  
MessagePack は .NET Standard 対応済み  

#### ライブラリ側 

.NET Standard 2.0 でつくる  

```csharp
using System;

using MessagePack;

namespace NetCoreSample.StandardLibrary
{
    // MessagePack では immutable なオブジェクトも使える (public な setter を要求しない)

    [MessagePackObject]
    public class ImmutablePerson
    {
        [Key("firstName")]
        public string FirstName { get; }

        [Key("lastName")]
        public string LastName { get; }

        [Key("birthDay")]
        public DateTime BirthDay { get; }

        public ImmutablePerson(string firstName, string lastName, DateTime birthDay)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.BirthDay = birthDay;
        }
    }


    public static class PersonArraySerializer
    {
        public static byte[] ToMessagePackBinary(this ImmutablePerson[] that) =>
            MessagePackSerializer.Serialize(that);

        public static byte[] ToLZ4MessagePackBinary(this ImmutablePerson[] that) =>
            LZ4MessagePackSerializer.Serialize(that);

        public static ImmutablePerson[] FromMessagePackBinary(byte[] bytes) =>
            MessagePackSerializer.Deserialize<ImmutablePerson[]>(bytes);

        public static ImmutablePerson[] FromLZ4MessagePackBinary(byte[] bytes) =>
            LZ4MessagePackSerializer.Deserialize<ImmutablePerson[]>(bytes);
    }
}
```

#### .NET Core の Console App

.NET Core 2.0 で

```csharp
using System;
using System.Linq;
using System.Text;

using NetCoreSample.StandardLibrary;

namespace NetCoreSample.MessagePackCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var deliveryScore = new ImmutablePerson("taro", "yamada", new DateTime(1993, 1, 5));
            var targetObj = Enumerable.Repeat(deliveryScore, 3).ToArray();

            Console.WriteLine(ToJson(targetObj));

            var serialized = targetObj.ToMessagePackBinary();
            var lz4Serialized = targetObj.ToLZ4MessagePackBinary();

            Console.WriteLine("MessagePack: ");
            Console.WriteLine(Encoding.ASCII.GetChars(serialized));
            Console.WriteLine("LZ4MessagePack: ");
            Console.WriteLine(Encoding.ASCII.GetChars(lz4Serialized));

            var deserialized = PersonArraySerializer.FromMessagePackBinary(serialized);
            var lz4Deserialized = PersonArraySerializer.FromLZ4MessagePackBinary(lz4Serialized);

            Console.WriteLine(ToJson(deserialized));
            Console.WriteLine(ToJson(lz4Deserialized));
        }

        private static string ToJson(object obj) => MessagePack.MessagePackSerializer.ToJson(obj);
    }
}
```

#### .NET Framework の Console App

.NET Framework 4.6.1 で

```csharp
using System;
using System.Linq;
using System.Text;

using NetCoreSample.StandardLibrary;

namespace NetCoreSample.MessagePackNet461
{
    class Program
    {
        static void Main(string[] args)
        {
            var deliveryScore = new ImmutablePerson("taro", "yamada", new DateTime(1993, 1, 5));
            var targetObj = Enumerable.Repeat(deliveryScore, 3).ToArray();

            Console.WriteLine(ToJson(targetObj));

            var serialized = targetObj.ToMessagePackBinary();
            var lz4Serialized = targetObj.ToLZ4MessagePackBinary();

            Console.WriteLine("MessagePack: ");
            Console.WriteLine(Encoding.ASCII.GetChars(serialized));
            Console.WriteLine("LZ4MessagePack: ");
            Console.WriteLine(Encoding.ASCII.GetChars(lz4Serialized));

            var deserialized = PersonArraySerializer.FromMessagePackBinary(serialized);
            var lz4Deserialized = PersonArraySerializer.FromLZ4MessagePackBinary(lz4Serialized);

            Console.WriteLine(ToJson(deserialized));
            Console.WriteLine(ToJson(lz4Deserialized));
        }

        private static string ToJson(object obj) => MessagePack.MessagePackSerializer.ToJson(obj);
    }
}
```

## Nuget パッケージの作成

### 設定

プロジェクトのプロパティの　「パッケージ」 でパッケージ名など指定

.csproj 直接編集でもいい

```xml
<PropertyGroup>
    <TargetFramework>netstandard2.0</TargetFramework>
    <Description>.NET Standard Sample Library</Description>
    <Copyright>(c) hogehoge</Copyright>
    <PackageLicenseUrl />
    <RepositoryUrl>https://github.com/wipiano/dotnetcore-sample</RepositoryUrl>
    <PackageReleaseNotes>first release</PackageReleaseNotes>
</PropertyGroup>
```

### 発行

プロジェクト右クリックしてパック。それだけ。
