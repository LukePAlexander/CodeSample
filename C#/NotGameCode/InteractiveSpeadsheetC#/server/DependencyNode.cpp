/* DependencyNode.cpp
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

#ifndef DEPENDENCY_NODE_CPP
#define DEPENDENCY_NODE_CPP


#include <string>
#include <set>

#include "../include/DependencyNode.h"

using namespace std;

namespace cs3505
{
  /* Create a DependencyNode, by default both the
   * dependents and dependees HashSets are empty
   */
  DependencyNode::DependencyNode()
  {
  }

  /* Add the specified dependent to the node */

  void DependencyNode::AddDependent (std::string s)
  {
    this->_dependents.insert(s);
  }

  /* Remove the specified dependent from this node */

  void DependencyNode::RemoveDependent (std::string s)
  {
    this->_dependents.erase(s);
  }

  /* Add the specified dependee to this node */

  void DependencyNode::AddDependee (std::string s)
  {
    this->_dependees.insert(s);
  }

  /* Remove the specified dependee from this node */

  void DependencyNode::RemoveDependee (std::string s)
  {
    this->_dependees.erase(s);
  }

  /* Return a copy of the _dependents HashSet */
  set<string> DependencyNode::GetDependents()
  {
    set<string> temp;
    for (set<string>::iterator it = this->_dependents.begin(); it != this->_dependents.end(); it++)
    {
      temp.insert(*it);
    }
    return temp;
  }

  /* Return a copy of the _dependees HashSet */
  set<string> DependencyNode::GetDependees()
  {
    set<string> temp;
    for (set<string>::iterator it = this->_dependees.begin(); it != this->_dependees.end(); it++)
    {
      temp.insert(*it);
    }
    return temp;
  }

  /* Return the number of dependees in the _dependees set */
  int DependencyNode::NumDependees()
  {
    return this->_dependees.size();
  }

  /* Return the number of dependents in the _dependents set */
  int DependencyNode::NumDependents()
  {
    return this->_dependents.size();
  }

  /* Checks if the provided string is contained in the set of dependents */

  bool DependencyNode::DependentsContains(std::string s)
  {
    set<string>::iterator it;
    it = this->_dependents.find(s);
    return it != this->_dependents.end();
  }

  /* Checks if the provided string is contained in the set of dependees */

  bool DependencyNode::DependeesContains(std::string s)
  {
    return this->_dependees.find(s) != this->_dependees.end();
  }
}

#endif
