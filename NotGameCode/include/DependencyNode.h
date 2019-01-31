/* DependencyNode.h
 *
 * Team: HexSpeak
 * Author: Paul Carlson
 *
 * DepenencyNodes are a simple data object that stores a set of depdents and dependees for the
 * node as well as methods for manipulating and accessing these sets.  This class is used by the
 * DependencyGraph class.
 *
 * This code has been directly adapted from the code written by Paul Carlson for CS 3500 
 * in Fall 2016.
 */

#ifndef DEPENDENCY_NODE_H
#define DEPENDENCY_NODE_H

#include<string>
#include<set>


namespace cs3505
{
  /* A nested class to represent the Dependency Nodes which consist of
   * a HashSet of dependents and a HashSet of Dependees
   */
  class DependencyNode
  {
  private:
    // Sets to hold the references to other related nodes
    std::set<std::string> _dependents;
    std::set<std::string> _dependees;

  public:
    /* Create a DependencyNode, by default both the
     * dependents and dependees HashSets are empty
     */
    DependencyNode();

    /* Add the specified dependent to the node */
    void AddDependent (std::string s);

    /* Remove the specified dependent from this node */
    void RemoveDependent (std::string s);

    /* Add the specified dependee to this node */
    void AddDependee (std::string s);

    /* Remove the specified dependee from this node */
    void RemoveDependee (std::string s);

    /* Return a copy of the _dependents HashSet */
    std::set<std::string> GetDependents();

    /* Return a copy of the _dependees HashSet */
    std::set<std::string> GetDependees();

    /* Return the number of dependees in the _dependees set */
    int NumDependees();

    /* Return the number of dependents in the _dependents set */
    int NumDependents();

    /* Checks if the provided string is contained in the set of dependents */
    bool DependentsContains(std::string s);

    /* Checks if the provided string is contained in the set of dependees */
    bool DependeesContains(std::string s);
  };
}

#endif
