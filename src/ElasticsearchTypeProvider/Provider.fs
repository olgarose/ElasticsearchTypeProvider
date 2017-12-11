namespace ElasticsearchTypeProvider

open System.Reflection
open Microsoft.FSharp.Core.CompilerServices
open ProviderImplementation.ProvidedTypes
open Nest
open FSharp.Elasticsearch

[<TypeProvider>]
type Provider(cfg:TypeProviderConfig) as this = 
  inherit TypeProviderForNamespaces(cfg)
  let ns = "FSharp.Elasticsearch"
  let asm = Assembly.LoadFrom(cfg.RuntimeAssembly)

  let buildClient = 
    // build in memory client simulating response from elasticsearch .kibana index
    let response = """ 
    [
      {
        "_index": ".kibana",
        "_type": "index-pattern",
        "_id": "123",
        "_score": 1,
        "_source": {
          "title": "twitter*"
        }
      }, 
      {
        "_index": ".kibana",
        "_type": "index-pattern",
        "_id": "456",
        "_score": 1,
        "_source": {
          "title": "facebook*"
        }
      }
    ]
    """

    response |> InMemoryElasticClient.withResponse

  let buildSearchPatterns (client:IElasticClient) =
    // try to extract index schema from .kibana index
    let patterns = ProvidedTypeDefinition(asm, ns, "IndexPatterns", Some typeof<obj>)
    let query = """{"match_phrase":{"_type":"index-pattern"}}"""
    let result = client |> SearchApi.rawQuery<KibanaIndexPattern> query ".kibana"

    let addSearchPattern pattern =
      let title = pattern.title

      let property =
        ProvidedProperty(title, typeof<IndexPattern>,
          getterCode = 
            (fun _ -> 
              <@@
                IndexPattern(title) @@>))
      patterns.AddMemberDelayed(fun _ -> property)

    result.Documents |> Seq.iter(addSearchPattern)
    patterns

  let createAllTypes (root:ProvidedTypeDefinition) =
    // this client needs to be built from TypeProvider inputs
    let client = buildClient
    let patterns = client |> buildSearchPatterns 
    root.AddMemberDelayed(fun _ -> ProvidedProperty("IndexPatterns", patterns, isStatic = true, getterCode = (fun _ -> <@@ () @@>)))

    [root; patterns]

  let createRootType =
    ProvidedTypeDefinition(asm, ns, "TypeProvider", Some typeof<obj>)

  do
    let root = createRootType
    this.AddNamespace(ns, (createAllTypes root))
            
[<assembly:TypeProviderAssembly()>]
do ()
