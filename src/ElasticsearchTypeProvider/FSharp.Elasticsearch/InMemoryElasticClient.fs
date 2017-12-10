module FSharp.Elasticsearch.InMemoryElasticClient

open System
open System.Text
open Elasticsearch.Net
open Nest

let withRequestResponse response onRequestCompleted =
    let pool = new SingleNodeConnectionPool(Uri "http://in-memory-endpoint")
  
    let json = response |> sprintf """ {
      "hits": {
        "hits": %s
      }
    }""" 

    let bytes = json |> Encoding.ASCII.GetBytes
    let connection = new InMemoryConnection(responseBody = bytes)
    let handler = Action<IApiCallDetails>(onRequestCompleted)
    let settings = (new ConnectionSettings(pool, connection)).OnRequestCompleted(handler).EnableDebugMode().DisableDirectStreaming()  
    ElasticClient(settings)

let withResponse response =
    let handler = fun _ -> ()
    handler |> withRequestResponse response