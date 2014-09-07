## `Conclave.SqlServer`, Project Notes

This test is run with 300k topics.

Although not at the performance levels of the Mongo implementation it's better
that I was expecting.

#### -c 8
	
	Server Software:        Microsoft-IIS/8.0
	Server Hostname:        localhost
	Server Port:            80

	Document Path:          /conclave.cms/public/topic/view.aspx?id=Top/Business/
											Arts_and_Entertainment/Sports/Facilities
	Document Length:        5261 bytes

	Concurrency Level:      8
	Time taken for tests:   3.319 seconds
	Complete requests:      10000
	Failed requests:        63
	   (Connect: 0, Receive: 0, Length: 63, Exceptions: 0)
	Write errors:           0
	Keep-Alive requests:    10000
	Total transferred:      58380063 bytes
	HTML transferred:       52610063 bytes
	Requests per second:    3012.57 [#/sec] (mean)
	Time per request:       2.656 [ms] (mean)
	Time per request:       0.332 [ms] (mean, across all concurrent requests)
	Transfer rate:          17175.21 [Kbytes/sec] received

	Connection Times (ms)
				  min  mean[+/-sd] median   max
	Connect:        0    0   0.0      0       1
	Processing:     2    3   1.8      2      22
	Waiting:        2    3   1.8      2      22
	Total:          2    3   1.8      2      22

	Percentage of the requests served within a certain time (ms)
	  50%      2
	  66%      3
	  75%      3
	  80%      3
	  90%      3
	  95%      3
	  98%     10
	  99%     11
	 100%     22 (longest request)

#### -c 256

	Concurrency Level:      256
	...
	Requests per second:    2987.37 [#/sec] (mean)
	Time per request:       85.694 [ms] (mean)
	Time per request:       0.335 [ms] (mean, across all concurrent requests)

#### -c 512

	Concurrency Level:      512
	...
	Requests per second:    2851.89 [#/sec] (mean)
	Time per request:       179.530 [ms] (mean)
	Time per request:       0.351 [ms] (mean, across all concurrent requests)

#### -c 1024

	Concurrency Level:      1024
	...
	Requests per second:    4552.33 [#/sec] (mean)
	Time per request:       224.940 [ms] (mean)
	Time per request:       0.220 [ms] (mean, across all concurrent requests)

#### -c 2048

I'd do more tests before I'd fully trust this.

	Concurrency Level:      2048
	...
	Requests per second:    6917.62 [#/sec] (mean)
	Time per request:       296.056 [ms] (mean)
	Time per request:       0.145 [ms] (mean, across all concurrent requests)

