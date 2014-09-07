## `Conclave.Mongo`, Project Notes

Tests were run on an **i7-3770K, with a surplus of RAM** for any test run. The
database is on the same box as the Web server. The page delivered is a full
end to end with templated render, occurrence wiki resolution,
and authentication; although the
authentication has remained against MariaDB as it's the topic store I want
to gauge. It's the smallest realistic page delivery from `Conclave.CMS` but
it represents a valid performance ceiling.

The Mongo implementation of `ITopicStore` is the first to survive the
*"test to failure"* test that I've run over the years, not only without
failing (breaking a performance or resource requirement), but while offering
the same levels of performance as on smaller datasets. This is personally
quite an arresting point to have reached. *At 2048 concurrent requests
apache bench measures >7k req/sec*.

With some changes to the service container (large proportion of time spent is here),
and authentication I think >8k req/sec for this test is likely.

Currently the mongo implementation of `ITopicStore` is by far the fastest
implementation. With 780k topics loaded...

#### -c 8

    Server Software:        Microsoft-IIS/8.0
    Server Hostname:        localhost
    Server Port:            80
    
    Document Path:          //conclave.cms/public/topic/view.aspx?id=Top/
							Business/Arts_and_Entertainment/Sports/Facilities
    Document Length:        4998 bytes
    
    Concurrency Level:      8
    Time taken for tests:   2.758 seconds
    Complete requests:      10000
    Failed requests:        48
       (Connect: 0, Receive: 0, Length: 48, Exceptions: 0)
    Write errors:           0
    Keep-Alive requests:    10000
    Total transferred:      55750048 bytes
    HTML transferred:       49980048 bytes
    Requests per second:    3625.35 [#/sec] (mean)
    Time per request:       2.207 [ms] (mean)
    Time per request:       0.276 [ms] (mean, across all concurrent requests)
    Transfer rate:          19737.65 [Kbytes/sec] received
    
    Connection Times (ms)
                  min  mean[+/-sd] median   max
    Connect:        0    0   0.0      0       1
    Processing:     1    2   1.9      2      21
    Waiting:        1    2   1.9      2      21
    Total:          1    2   1.9      2      21
    
    Percentage of the requests served within a certain time (ms)
      50%      2
      66%      2
      75%      2
      80%      2
      90%      2
      95%      3
      98%     10
      99%     16
     100%     21 (longest request)

"Length", failed requests are as a result of the content length of the response
being different on 48 pages as a result of a timing counter at the bottom of
the page giving a different time for 48 pages. This has been confirmed.

With higher concurrency levels we still get good results:-
#### -c 256
    Concurrency Level:      256
    ...
    Requests per second:    3785.96 [#/sec] (mean)
    Time per request:       67.618 [ms] (mean)
    Time per request:       0.264 [ms] (mean, across all concurrent requests)

#### -c 512
    Concurrency Level:      512
    ...
    Requests per second:    3663.88 [#/sec] (mean)
    Time per request:       139.743 [ms] (mean)
    Time per request:       0.273 [ms] (mean, across all concurrent requests)

#### -c 1024
    Concurrency Level:      1024
    ...
    Requests per second:    4942.52 [#/sec] (mean)
    Time per request:       207.182 [ms] (mean)
    Time per request:       0.202 [ms] (mean, across all concurrent requests)

#### -c 2048
    Concurrency Level:      2048
    ...
    Requests per second:    7042.34 [#/sec] (mean)
    Time per request:       290.812 [ms] (mean)
    Time per request:       0.142 [ms] (mean, across all concurrent requests)

At `-c 2048` the site remains usable... It just wont die. I am saturating all cores,
(you'd cook your processors) but I'm not spiking the ram... Not caching yet... The event logs are squeaky clean.

In comparison the
`SqlServerTopicStore` yields 2k req/sec with 300k topics, and 2.7k req/sec
with only 7.5k topics... `MongoTopicStore` has been tested with 7.5k, 300k,
and 780k topics, and yields the same benchmark on this test for each.

In terms of write, Mongo will load 780k topics faster than Sql Server will 
load 7.5k topics. If write throughput is in any way important to you
Mongo starts becoming compelling. You can always use a write-through store
pointing at Mongo on the front and Sql Server on the back for belt and braces.
I know we played about with this early on, but a couple of us latterly
took this approach quite a bit in production and it worked well. Rather than
thinking in terms of caching, we thought in terms of fast and slow stores,
and a write-through interface... `MongoTopicStore` looks a good candidate
for a "fast store". I'm still not convinced about Redis for this model,
so Mongo might wind up **it**.

I'm aware of why you wouldn't want to hook up a life-support machine to Mongo,
but most commercial Web applications aren't anywhere in the same proximity
as a life-support machine. No transactions, no locks and dirty reads wont 
magically save you by virtue of being on a rdbms either. 

Lastly `GetPointingTopics` is a comparatively cheap query on Mongo.

The next step is doing throughput tests using random topic selection so I'm not just
testing a hot-spot on a cache somewhere.

I would still expect the real-world experience of a real application backed by
Mongo to come in around 2k r/s, as this test is still quite synthetic.

### Update

7.6k now... I've also hit it with siege, and it just wont fall over... because there's no caching, ram usage is near constant.... -c 8192 will drop it to 5.5k, and the load times jump up to 2s, but it maintains throughput, and stays stable... You'd cook your CPUs however.

I have a vague and unspecified suspicion that the managed state and immutability has more of an impact than is obvious. You can wire it up in complex patterns, but the processing and state model is actually very stripped back and simple.

### Model

This store interacts with the following model:-

	{
	  "_id" : ObjectId("525d4a0f992df13340096f91"),
	  "_type" : "topic",
	  "id" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	  "metadata" : [{
	      "_type" : "metadata",
	      "for" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	      "scope" : "default",
	      "name" : "last-update",
	      "value" : "2007-01-02 22:52:21"
	    }, {
	      "_type" : "metadata",
	      "for" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	      "scope" : "default",
	      "name" : "label",
	      "value" : "Adams"
	    }, {
	      "_type" : "metadata",
	      "for" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	      "scope" : "default",
	      "name" : "description",
	      "value" : "County Demographics: population was 265,038 in 199"
	    }],
	  "associations" : [{
	      "_type" : "association",
	      "parent" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	      "type" : "navigation",
	      "scope" : "default",
	      "reference" : "Top/Regional/North_America/United_States/Colorado/Regions/Front_Range",
	      "role" : "related",
	      "id" : "26a9fdb3-1f87-403f-ae02-5c2cb559e637",
	      "metadata" : [{
	          "_type" : "metadata",
	          "for" : "26a9fdb3-1f87-403f-ae02-5c2cb559e637",
	          "scope" : "default",
	          "name" : "label",
	          "value" : "Front Range"
	        }]
	    }],
	  "occurrences" : [{
	      "_type" : "occurrence",
	      "for" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	      "scope" : "default",
	      "role" : "wiki",
	      "behaviour" : "markdown",
	      "reference" : "self",
	      "string-data" : "To be or not to be that is the question.",
	      "resolved-model" : undefined
	    }]
	}

And the store further assumes the indexes:-
 
* `{"id": 1}`
* `{"associations.id": 1}`
* `{"associations.reference": 1}`

The last index requirement will be moved to the implementation of 
`IExtendedTopicStore` as it is needed for `GetPointingTopics`.
## `T:Conclave.Map.Store.MongoTopicStore`
An  [Conclave.Map.Store.ITopicStore](T-Conclave.Map.Store.ITopicStore)  for MongoDB.

#### Remarks
The Mongo topic store behaves slightly differently that topic stores implemented against relational databases. This difference is largely around adding metadata, occurrences, and associations by themselves directly when there may be no parent topic in the store. See implementation notes.

You can only add child members to parents that exist in the Mongo store.


