## `Conclave.MySql`, Project Notes

This test is run with 7.5k topics, and is actually using MariaDB, not MySql.

The results for MariaDB unlike SqlServer were a lot more spiky, varying from
test to test from 1.9k r/s to 2.4k r/s. This one was pretty representative.

#### -c 8
	
	Server Software:        Microsoft-IIS/8.0
	Server Hostname:        localhost
	Server Port:            80
	
	Document Path:          /conclave.cms/public/topic/view.aspx?id=Kids_and_Teens/
							Entertainment/Museums/Social_Studies/History
	Document Length:        5172 bytes
	
	Concurrency Level:      8
	Time taken for tests:   4.751 seconds
	Complete requests:      10000
	Failed requests:        86
	   (Connect: 0, Receive: 0, Length: 86, Exceptions: 0)
	Write errors:           0
	Total transferred:      57440086 bytes
	HTML transferred:       51720086 bytes
	Requests per second:    2104.99 [#/sec] (mean)
	Time per request:       3.800 [ms] (mean)
	Time per request:       0.475 [ms] (mean, across all concurrent requests)
	Transfer rate:          11807.72 [Kbytes/sec] received
	
	Connection Times (ms)
	              min  mean[+/-sd] median   max
	Connect:        0    0   0.3      0       8
	Processing:     2    4   1.7      3      23
	Waiting:        2    4   1.7      3      23
	Total:          2    4   1.7      3      23
	
	Percentage of the requests served within a certain time (ms)
	  50%      3
	  66%      4
	  75%      4
	  80%      4
	  90%      4
	  95%      5
	  98%     11
	  99%     12
	 100%     23 (longest request)

## `T:Conclave.Map.Store.MySqlTopicStore`
An implementation of  [Conclave.Map.Store.ITopicStore](T-Conclave.Map.Store.ITopicStore) for MySql.

#### Remarks
This is implemented as a  [Conclave.Data.Store.SqlStore](T-Conclave.Data.Store.SqlStore) , and internally             uses the extensions provided by  [Conclave.Map.Store.DataReaderEx](T-Conclave.Map.Store.DataReaderEx) .
