module FSharp.Elasticsearch.SearchApiTests

open System.Text
open NUnit.Framework
open Elasticsearch.Net
open FSharp.Data

type User = {
  Id : int
  Name : string
}

type Tweet = {
  User : User
  Text : string
}

let someResponse = """[{
  "_source": {
    "User": {
      "Name": "Bob",
      "Id": 123
    },
    "Text" : "I tweet!"
  }
},
{
  "_source": {
    "User": {
      "Name": "Anna",
      "Id": 124
    },
    "Text" : "I tweet too!"
  }
}]"""

let someQuery = """{"some" : "query"}"""
let someIndex = "some-index"

let clientWithRequestResponse onRequestCompleted = 
  onRequestCompleted |> InMemoryElasticClient.withRequestResponse someResponse
      
let clientWithResponse = 
  clientWithRequestResponse (fun d -> ())

[<Test>]
let ``serializes raw query response`` () =
  let result = clientWithResponse |> SearchApi.rawQuery<Tweet> someQuery someIndex
  let tweets = result.Documents

  Assert.AreEqual(tweets.Count, 2)

  let tweetOne = tweets |> Seq.head
  let tweetTwo = tweets |> Seq.last

  Assert.AreEqual(tweetOne.Text, "I tweet!")
  Assert.AreEqual(tweetOne.User.Name, "Bob")

  Assert.AreEqual(tweetTwo.Text, "I tweet too!")
  Assert.AreEqual(tweetTwo.User.Name, "Anna")

[<Test>]
let ``posts to correct index`` () =
  let handler (details: IApiCallDetails) =
    let expected = sprintf "/%s/_search" someIndex
    let actual = details.Uri.PathAndQuery
    Assert.AreEqual(expected, actual)
  
  let client = handler |> clientWithRequestResponse
  client |> SearchApi.rawQuery<Tweet> someQuery someIndex |> ignore

[<Test>]
let ``sends correct raw query`` () =
  let handler (details: IApiCallDetails) =
    let expected = sprintf """ {"query" : %s} """ someQuery |> JsonValue.Parse
    let actual = details.RequestBodyInBytes |> Encoding.ASCII.GetString |> JsonValue.Parse
    Assert.AreEqual(expected, actual)

  let client = handler |> clientWithRequestResponse
  client |> SearchApi.rawQuery<Tweet> someQuery someIndex |> ignore