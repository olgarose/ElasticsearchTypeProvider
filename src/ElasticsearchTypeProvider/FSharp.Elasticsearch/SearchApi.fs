module FSharp.Elasticsearch.SearchApi

open Nest

let internal search<'T when 'T : not struct> (query:QueryBase) (index:string) (client : IElasticClient) =
  let request = SearchRequest(Indices.Parse index, Query = QueryContainer query)
  client.Search<'T>(request)

let rawQuery<'T when 'T : not struct> query =
  search<'T> (RawQuery query)

let queryStringQuery<'T when 'T : not struct> query =
  search<'T> (QueryStringQuery(Query = query))

