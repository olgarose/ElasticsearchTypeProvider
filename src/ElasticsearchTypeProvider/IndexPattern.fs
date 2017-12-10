namespace ElasticsearchTypeProvider

open FSharp.Elasticsearch
open Nest

type IndexPattern(index) =
  let client = ElasticClient()
  member this.Pattern = index
  member this.Query<'T when 'T : not struct> query =
    client |> SearchApi.queryStringQuery<'T> query index
