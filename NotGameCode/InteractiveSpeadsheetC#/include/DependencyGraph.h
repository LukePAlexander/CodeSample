/* DependencyGraph.h
 *
 * Team: HexSpeak
 * Author: Paul Carlson
 *
 * DepenencyGraph represents the relationships between nodes in a graph.  This will be
 * applied to a Spreadsheet where the graph nodes are cells and the edges or
 * dependencies exist between cells whose values are dependent on each other.
 *
 * This code has been directly adapted from the code written by Paul Carlson for CS 3500 
 * in Fall 2016.
 */

#ifndef DEPENDENCY_GRAPH_H
#define DEPENDENCY_GRAPH_H

#include<string>
#include<map>
#include<set>


#include "../include/DependencyNode.h"


namespace cs3505
{

  /*
   * (s1,t1) is an ordered pair of strings
   * t1 depends on s1; s1 must be evaluated before t1
   * 
   * A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
   * (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
   * Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
   * set, and the element is already in the set, the set remains unchanged.
   * 
   * Given a DependencyGraph DG:
   * 
   *    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
   *        (The set of things that depend on s)    
   *        
   *    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
   *        (The set of things that s depends on) 
   *
   */
  class DependencyGraph
  {
  private:
    // Store the graph in a Dictionary

    std::map<std::string, cs3505::DependencyNode> DGraph;
    int size;


  public:
    /* Creates an empty DependencyGraph. */
    DependencyGraph();

    /* The number of ordered pairs in the DependencyGraph. */
    int GetSize();

    /*
     * The size of dependees(s).
     * This property is an example of an indexer.  If dg is a DependencyGraph, you would
     * invoke it like this:
     * dg["a"]
     * It should return the size of dependees("a")
     */
    int operator[](std::string s);


    /* Reports whether dependents(s) is non-empty. */
    bool HasDependents(std::string s);


    /* Reports whether dependees(s) is non-empty. */
    bool HasDependees(std::string s);


    /*  Enumerates dependents(s). */
    std::set<std::string> GetDependents(std::string s);

    /*  Enumerates dependees(s). */
    std::set<std::string> GetDependees(std::string s);

    /* Adds the ordered pair (s,t), if it doesn't exist 
     * This should be thought of as:
     * 
     *   t depends on s
     * s must be evaluated first. T depends on S
     * <param name="t"> t cannot be evaluated until s is
     */ 
    void AddDependency(std::string s, std::string t);
  

    /* Removes the ordered pair (s,t), if it exists */
    void RemoveDependency(std::string s, std::string t);


    /* Removes all existing ordered pairs of the form (s,r).  Then, for each
     * t in newDependents, adds the ordered pair (s,t).
     */
    void ReplaceDependents(std::string s, std::set<std::string> newDependents);


    /*  Removes all existing ordered pairs of the form (r,s).  Then, for each 
     * t in newDependees, adds the ordered pair (t,s).
     */
    void ReplaceDependees(std::string s, std::set<std::string> newDependees);

    /* Check if the dependency graph dGraph contains s */
    bool GraphContains(std::string s);
  }; 
}

#endif

