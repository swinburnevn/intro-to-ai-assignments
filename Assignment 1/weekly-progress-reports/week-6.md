# Progress Week 6

## Programming

* Decided not to do IDA* since it would've been very similar to A Star
* Went with Dijkstra's Algorithm since it's related to A-Star, UCS, and GBFS to cover the following table:

| Algorithm  | Cost Function        | Heuristic Function |
| ---------- | -------------------- | ------------------ |
| UCS        | Steps to get to Node |                    |
| GBFS       |                      | Distance to Goal   |
| A*         | Steps to get to Node | Distance to Goal   |
| Dijkstra's | Distance from Start  |                    |

* My concern is that IDDA* may be too similar given that the current A* Algorithm uses the number of steps to get to a given goal. Either or, Dijktra's is implemented.
* Added a new map, Map 5 which contains two paths to get to one goal. 
* Re-run all the tests, approx. 70 tests now
* Entered all the data manually, since there's no dedicated data output mode. I should do that next time
* Overall polish of program roughly complete, no major overhauls as of late. Everything is done.

## Research

* Finished all of the writing re: searching algorithms and the presenting examples
* Finished writing report 
* Finish analysis of data in excel