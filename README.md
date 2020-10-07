# OwO Converter</br>
### Converts text to OwO</br>
[Try it!](https://owo.drinkpoint.me)

A url version of this wonderfull collection of regexes.
[Firefox OwO Plugin](https://addons.mozilla.org/en-US/firefox/addon/owofox/)

This was used as a PoC for deployment of [KNative](https://github.com/knative) Serverless Apps in an on Prem Kubernetes Cluster.

### Example Use

```bash
$ git commit -m "$(curl --silent --fail https://owo.drinkpoint.me/My%20Commit%20Message\!)"
```

### Load Test Behavior

```
          /\      |‾‾|  /‾‾/  /‾/    
     /\  /  \     |  |_/  /  / /     
    /  \/    \    |      |  /  ‾‾\   
   /          \   |  |‾\  \ | (_) |  
  / __________ \  |__|  \__\ \___/ .io 

  execution: local-
     script: .\k6OwoStressTest.js
     output: -

  scenarios: (100.00%) 1 executors, 1900 max VUs, 23m30s max duration (incl. graceful stop):
           * default: Up to 1900 looping VUs for 23m0s over 9 stages (gracefulRampDown: 30s, gracefulStop: 30s)

WARN[0794] Request Failed                                error="unexpected EOF"
WARN[0802] Request Failed                                error="unexpected EOF"
WARN[1095] Request Failed                                error="unexpected EOF"
r
running (23m00.8s), 0000/1900 VUs, 1374732 complete and 0 interrupted iterations.
default ✓ [======================================] 0000/1900 VUs  23m0s.


    data_received..............: 4.9 GB  3.5 MB/s
    data_sent..................: 6.1 GB  4.4 MB/s
    http_req_blocked...........: avg=9.99µs   min=0s     med=0s      max=1.02s   p(90)=0s    p(95)=0s
    http_req_connecting........: avg=9.82µs   min=0s     med=0s      max=1.02s   p(90)=0s    p(95)=0s
    http_req_duration..........: avg=109.1ms  min=2.99ms med=13.99ms max=53.04s  p(90)=210ms p(95)=1.01s
    http_req_receiving.........: avg=312.51µs min=0s     med=0s      max=49.77s  p(90)=0s    p(95)=0s
    http_req_sending...........: avg=3.58µs   min=0s     med=0s      max=60.99ms p(90)=0s    p(95)=0s
    http_req_tls_handshaking...: avg=0s       min=0s     med=0s      max=0s      p(90)=0s    p(95)=0s
    http_req_waiting...........: avg=108.78ms min=2.99ms med=13.99ms max=53.04s  p(90)=210ms p(95)=1.01s
    http_reqs..................: 1374732 995.621744/s
    iteration_duration.........: avg=1.11s    min=1s     med=1.01s   max=54.04s  p(90)=1.22s p(95)=2.01s
    iterations.................: 1374732 995.621744/s
    vus........................: 8       min=4    max=1900
    vus_max....................: 1900    min=1900 max=1900
```

![owo load test results](https://raw.githubusercontent.com/Demonslyr/OwOConverter/master/k6/StressResponse.PNG)
