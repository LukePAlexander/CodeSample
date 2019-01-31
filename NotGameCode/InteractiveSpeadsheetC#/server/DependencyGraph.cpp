/* DependencyGraph.cpp
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

#include<string>
#include<map>
#include<set>


#include "../include/DependencyGraph.h"
#include "../include/DependencyNode.h"

using namespace std;

namespace cs3505
{
  /* Creates an empty DependencyGraph. */
  DependencyGraph::DependencyGraph()
  {
    this->size = 0;
  }


  /* The number of ordered pairs in the DependencyGraph. */
  int DependencyGraph::GetSize()
  {
    return this->size;
  }


  /*
   * The size of dependees(s).
   * This property is an example of an indexer.  If dg is a DependencyGraph, you would
   * invoke it like this:
   * dg["a"]
   * It should return the size of dependees("a")
   */
  int DependencyGraph::operator[](string s)
  {
    if (this->GraphContains(s))
    {
      return this->DGraph[s].NumDependees();
    }

    else
    {
      return 0;
    }
  }


  /* Reports whether dependents(s) is non-empty. */
  bool DependencyGraph::HasDependents(string s)
  {
    // First check that DGraph contains s, then check for dependents
    if (this->GraphContains(s))
    {
      return this->DGraph[s].NumDependents() > 0;
    }
    return false;
  }


  /* Reports whether dependees(s) is non-empty. */
  bool DependencyGraph::HasDependees(string s)
  {
    // First check that DGraph contains s, then check for dependees
    if (this->GraphContains(s))
    {
      return this->DGraph[s].NumDependees() > 0;
    }
    return false;
  }


  /*  Enumerates dependents(s). */
  set<string> DependencyGraph::GetDependents(string s)
  {
    if (this->GraphContains(s))
    {
      return DGraph[s].GetDependents();
    }
    // If DGraph does not contain s, return an empty List
    set<string> temp;
    return temp;
  }


  /*  Enumerates dependees(s). */
  set<string> DependencyGraph::GetDependees(string s)
  {
    if (this->GraphContains(s))
    {
      return this->DGraph[s].GetDependees();
    }
    // If DGraph does not contain s, return an empty List
    set<string> temp;
    return temp;
  }


  /* Adds the ordered pair (s,t), if it doesn't exist 
   * This should be thought of as:
   * 
   *   t depends on s
   * s must be evaluated first. T depends on S
   * <param name="t"> t cannot be evaluated until s is
   */ 
  void DependencyGraph::AddDependency(string s, string t)
  {
    // This method will also added nodes as needed to create a dependency
    // Check if s is a key first
    if (this->GraphContains(s))
    {
      if (this->DGraph[s].DependentsContains(t))
      {
        return;
      }
      else
      {
	this->DGraph[s].AddDependent(t);          
	size++;

        // Add s as a dependee of t
        if (this->GraphContains(t))
        {
          if (this->DGraph[t].DependeesContains(s))
          {
            return; 
          }
          else
          {
            this->DGraph[t].AddDependee(s);
          }
        }
        else
	{
	  DependencyNode newT;
          newT.AddDependee(s);
          DGraph[t] = newT;
        }
      }
    }
    // Otherwise we need to add it
    else
    {
      DependencyNode newS;
      newS.AddDependent(t);
      DGraph[s] = newS;
      size++;
      // Add s as a dependee of t
      if (this->GraphContains(t))
      {
        if (this->DGraph[t].DependeesContains(s))
	{ 
          return;
        }
        else
        {
          this->DGraph[t].AddDependee(s);
        }
      }
      else
      {
        DependencyNode newT;
        newT.AddDependee(s);
        DGraph[t] = newT;
      }
    }
  }


  /* Removes the ordered pair (s,t), if it exists */
  void DependencyGraph::RemoveDependency(string s, string t)
  {
    // Check if DGraph contains s or (s, t)
    if (this->GraphContains(s))
    {
      if (this->DGraph[s].DependentsContains(t))
      {
	this->DGraph[s].RemoveDependent(t);
	size--;

        // Also remove the back reference in t's dependees
        if (this->GraphContains(t))
        {
          this->DGraph[t].RemoveDependee(s);
        }

        // Some cleanup to remove unused nodes
        if (!HasDependents(s) && !HasDependees(s))
        {
          this->DGraph.erase(s);
        }

        if (this->GraphContains(t))
        {
          if (!HasDependents(t) && !HasDependees(t))
          {
            this->DGraph.erase(t);
          }
        }
      }
    }
    // s is not a key or t is not a dependent of s so there is no 
    // dependency to remove
    return; 
  }


  /* Removes all existing ordered pairs of the form (s,r).  Then, for each
   * t in newDependents, adds the ordered pair (s,t).
   */
  void DependencyGraph::ReplaceDependents(string s, set<string> newDependents)
  {
    // First make sure DGraph contains s
    if (this->GraphContains(s))
    {
      // Then remove the dependencys
      set<string> temp = this->DGraph[s].GetDependents();
      for (set<string>::iterator it = temp.begin(); it != temp.end(); it++)
      {
        this->RemoveDependency(s, *it);
      }
      for (set<string>::iterator it = newDependents.begin(); it != newDependents.end(); it++)
      {
        this->AddDependency(s, *it);
      }
    }
    // s is not a key in DGraph so return
    return;
  }


  /*  Removes all existing ordered pairs of the form (r,s).  Then, for each 
   * t in newDependees, adds the ordered pair (t,s).
   */
  void DependencyGraph::ReplaceDependees(string s, set<string> newDependees)
  {
    // First make sure DGraph contains s
    if (this->GraphContains(s))
    {
      // Then remove the dependees
      set<string> temp = this->DGraph[s].GetDependees();
      for (set<string>::iterator it = temp.begin(); it != temp.end(); it++)
      {
        this->RemoveDependency(*it, s);
      }
      for (set<string>::iterator it = newDependees.begin(); it != newDependees.end(); it++)
      {
        this->AddDependency(*it, s);
      }
    }
    // s is not a key in DGraph so return
    return;
  }

  /* Check if the dependency graph dGraph contains s */
  bool DependencyGraph::GraphContains(string s)
  {
    return this->DGraph.find(s) != this->DGraph.end();
  }
}

