# Assignment 1: Tree-based Search

Option B: Robot Navigation via tree-based search algorithms in software, from scratch, + extensions.

---

## Report Structure

​	

---

## Preface

Originally I thought this would be more researched-based, but looking at the marking scheme, programming makes up 25 + 15 + 4*8 + 20 = 92 marks. That means I should get started in developing the framework that uses a data object with MVC. 

There is a mark penalty for code that doesn't good practice, but the takeaway from the rubric is that there should be helpful comments -- I should follow in Markus' footsteps.



### Development

Let's start by creating a world that will eventually accept two views: SFML and Console view. The second module will likely be file parsing, and after that, assembling the controller.

In this bastardisation of the MVC paradigm, the controller will be the program's Main() function which will contain a Model and View module. 

* The *Model* will contain all the logic to run the program, notably, telling the agent what percepts it has, and telling the agent to do stuff. This will all be encapsulated into one Run() command provided to the controller to get things working.
* The *View* provides I/O and Display output, however, we won't worry about I/O for view today since we're not doing GUI stuff. Instead, the view simply takes all the game data/program data from the program and converts that into a view. That's it.

The final design will look very much like the vacuum cleaner world. I'll want to consult closely with Danny and Tony on this.

Oh yes -- and unit testing. Need to refresh up on it as well.





### Student Details

### Table of Contents

### Instructions

### Introduction

* Abstract
* Introduce Robot Navigation Problem

The robot navigation problem is that, for an N by M grid, the goal is to navigate the grid whilst avoiding obstacles and reaching the goal in the shortest path (if any is available). Loaded from a map file, the navigation problem is quite general and has been abstracted heavily for this problem in particular. This forms of tree-based navigation problems have seen applications in games, such as pathfinding AI for enemies around obstacles or certain blockades (such as weapon ranges). Furthermore, they have application in real-life robotics, where given a SLAM map of a location, methods of internal mapping allow a robot to seamlessly navigate complex locations such as office hallways or wreckages after a natural disaster in the shortest period of time to achieve a certain goal.

* Basic Graph + Tree Concepts

GRAPH VS TREE: AIMA p. 77

When broken down, problems can be broken down into a solution that requires a sequence of actions -- notably in this case where we focus on navigation. This is the key behind designing intelligent agents: by abstracting highly complex problems in the real world just as GPS navigation to simple actions and sequences, we can provide smart solutions that are able to take in more information and process all the possible outcomes faster than any human could.

From our initial conditions, or the root node, every action we take is a decision -- we list these possible actions and the state they lead to. After that, do the same thing for each state in the layer down, and so on, and so forth.

This is the basis behind a search tree -- a representation of "what happens if we do X". Search algorithms are designed to navigate search trees smartly and rationally. (i.e. not randomly).

Search trees are a graphical representation of the all the output states and sequences of the problem given a root node and travel restrictions. With each node representing a tree and with in and out arrows. Consider simple cases: etc.

A tree search algorithm is one that travels down the nodes of a tree with a specified strategy. A graph search extends upon this, as the algorithm then remembers the nodes it has explored, known as an *explored set* or *closed list*. This means that graph searches are incapable of loops and does not waste time exploring nodes that have been explored before. The downside is that it has an increased memory requirement, where in cases given presumptions, assumptions, or known facts about the world, storing a list of previously explored places may not be necessary. 

* Other related terminology
* Approach of this report

This report will be broken down into the following:

### Search Algorithms



* Present and discuss each algorithm:

Algorithms

* Uninformed
  * Breadth-first search
  * Depth-first search

Define uninformed algorithm.

given the unknown nature of each node, we'll have to explore all of them given a strategy. 

The BFS  simply traverses all nodes in order, notably, from left to right. Once it's done, it'll move down a level and then continue the search.

The DFS uses a stack and traverses the node down each extreme path and recursively does so.

Pseudocode!

* Informed
  * Greedy-best first
  * A*

* Custom (1 informed + 1 uninformed)
  * Informed: https://www.geeksforgeeks.org/ml-monte-carlo-tree-search-mcts/ ?
    * For a simpler one, hill climb search: https://stackoverflow.com/questions/8946719/hill-climbing-algorithm-simple-example 
  * Uninformed: https://www.geeksforgeeks.org/bidirectional-search/ (good for this robot since we know where the goal is)



Reading:

Informed Algorithms -- good comparison of greedy vs. A*

https://www.javatpoint.com/ai-informed-search-algorithms

Greedy / Best First 

https://www.geeksforgeeks.org/best-first-search-informed-search/

Lots of different Informed searches

https://medium.com/datadriveninvestor/searching-algorithms-for-artificial-intelligence-85d58a8e4a42 





A Star

https://www.geeksforgeeks.org/a-search-algorithm/



### Implementation

In addition: GUI, Testing, Unit Tests, Try to reach all green cells.

### Features/Bugs/Missing



### Research

### Acknowledgements/Resources

### References



