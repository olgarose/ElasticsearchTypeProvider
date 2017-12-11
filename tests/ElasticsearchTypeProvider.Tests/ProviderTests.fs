module ElasticsearchTypeProvider.ProviderTests

open NUnit.Framework
open FSharp.Elasticsearch

type ES = FSharp.Elasticsearch.TypeProvider

[<Test>]
let ``generates index patterns`` () =
  ES.IndexPatterns.``twitter*`` |> ignore
  ES.IndexPatterns.``facebook*`` |> ignore
  ()
