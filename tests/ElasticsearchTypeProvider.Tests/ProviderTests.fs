module ElasticsearchTypeProvider.ProviderTests

open NUnit.Framework
open ElasticsearchTypeProvider
open TestHelpers

type ES = FSharp.Elasticsearch.TypeProvider

[<Test>]
let ``generates index patterns`` () =
  let index = ES.IndexPatterns.``twitter*``
  let results = index.Query<Tweet> "id_str: (935265263058149376 OR 935265212227358720)"

  results.Documents 
  |> Seq.iter(fun d -> printf "\n\nDocument: %A" d)

  Assert.Greater(results.Documents.Count, 0)
  ()
