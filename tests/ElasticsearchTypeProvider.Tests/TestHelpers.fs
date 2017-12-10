module ElasticsearchTypeProvider.TestHelpers

type User = {
  screen_name: string
  location: string
  name : string
}

type QuotedStatus = {
   user : User
}

type Tweet = {
  text: string
  user: User
  id_str: string
  quoted_status: QuotedStatus
}