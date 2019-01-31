/**
* A program to test the DependencyGaph class.
*
* Author: Kurt Bruns
* Date: 2017 04 17
*/

#include "../include/DependencyGraph.h"  // testing object
#include "gtest/gtest.h"                // testing framework

#include <set>  // testing return value

TEST( DependencyGraph, AddDependencySimple)
{
  cs3505::DependencyGraph graph = cs3505::DependencyGraph();

  graph.AddDependency( "a", "b" );

  // Check graph
  EXPECT_EQ( 1, graph.GetSize());
  EXPECT_EQ( 1, graph["a"]);
  EXPECT_TRUE( graph.GraphContains("a"));
  EXPECT_TRUE( graph.GraphContains("b"));

  // Check dependents
  EXPECT_TRUE(graph.HasDependents("a"));
  std::set<std::string> dependents;
  dependents.insert("b");
  EXPECT_EQ( dependents, graph.GetDependents("a"));

  // Check dependents
  EXPECT_TRUE(graph.HasDependees("b"));
  std::set<std::string> dependees;
  dependees.insert("a");
  EXPECT_EQ( dependees, graph.GetDependees("b"));
}

TEST( DependencyGraph, AddDependencyDuplicate)
{
  cs3505::DependencyGraph graph = cs3505::DependencyGraph();

  graph.AddDependency( "a", "b" );
  graph.AddDependency( "a", "b" );

  // Check graph
  EXPECT_EQ( 1, graph.GetSize());
  EXPECT_EQ( 1, graph["a"]);
  EXPECT_TRUE( graph.GraphContains("a"));
  EXPECT_TRUE( graph.GraphContains("b"));

  // Check dependents
  EXPECT_TRUE(graph.HasDependents("a"));
  std::set<std::string> dependents;
  dependents.insert("b");
  EXPECT_EQ( dependents, graph.GetDependents("a"));

  // Check dependents
  EXPECT_TRUE(graph.HasDependees("b"));
  std::set<std::string> dependees;
  dependees.insert("a");
  EXPECT_EQ( dependees, graph.GetDependees("b"));
}

TEST( DependencyGraph, AddDependencyAdvancedGraph)
{
  cs3505::DependencyGraph graph = cs3505::DependencyGraph();

  graph.AddDependency( "a", "c" );
  graph.AddDependency( "b", "c" );
  graph.AddDependency( "c", "f" );
  graph.AddDependency( "d", "e" );
  graph.AddDependency( "d", "f" );

  // Check graph
  EXPECT_EQ( 5, graph.GetSize());
  EXPECT_EQ( 1, graph["a"]);
  EXPECT_EQ( 1, graph["b"]);
  EXPECT_EQ( 1, graph["a"]);
  EXPECT_EQ( 2, graph["d"]);
  EXPECT_TRUE( graph.GraphContains("a"));
  EXPECT_TRUE( graph.GraphContains("b"));
  EXPECT_TRUE( graph.GraphContains("c"));
  EXPECT_TRUE( graph.GraphContains("d"));
  EXPECT_TRUE( graph.GraphContains("e"));
  EXPECT_TRUE( graph.GraphContains("f"));
}

TEST( DependencyGraph, AddDependencyHasDependents)
{
  cs3505::DependencyGraph graph = cs3505::DependencyGraph();

  graph.AddDependency( "a", "c" );
  graph.AddDependency( "b", "c" );
  graph.AddDependency( "c", "f" );
  graph.AddDependency( "d", "e" );
  graph.AddDependency( "d", "f" );

  // Check hasdependents
  EXPECT_TRUE(graph.HasDependents("a"));
  EXPECT_TRUE(graph.HasDependents("b"));
  EXPECT_TRUE(graph.HasDependents("c"));
  EXPECT_TRUE(graph.HasDependents("d"));
  EXPECT_FALSE(graph.HasDependents("e"));
  EXPECT_FALSE(graph.HasDependents("f"));

}

TEST( DependencyGraph, AddDependencyHasDependees)
{
  cs3505::DependencyGraph graph = cs3505::DependencyGraph();

  graph.AddDependency( "a", "c" );
  graph.AddDependency( "b", "c" );
  graph.AddDependency( "c", "f" );
  graph.AddDependency( "d", "e" );
  graph.AddDependency( "d", "f" );

  // Check hasdependees
  EXPECT_TRUE(graph.HasDependees("c"));
  EXPECT_TRUE(graph.HasDependees("e"));
  EXPECT_TRUE(graph.HasDependees("f"));
  EXPECT_FALSE(graph.HasDependees("a"));
  EXPECT_FALSE(graph.HasDependees("b"));
  EXPECT_FALSE(graph.HasDependees("d"));
}

TEST( DependencyGraph, AddDependencyGetDependents)
{
  cs3505::DependencyGraph graph = cs3505::DependencyGraph();

  graph.AddDependency( "a", "c" );
  graph.AddDependency( "b", "c" );
  graph.AddDependency( "c", "f" );
  graph.AddDependency( "d", "e" );
  graph.AddDependency( "d", "f" );

  std::set<std::string> dependents_a;
  dependents_a.insert("c");
  EXPECT_EQ( dependents_a, graph.GetDependents("a"));

  std::set<std::string> dependents_b;
  dependents_a.insert("c");
  EXPECT_EQ( dependents_a, graph.GetDependents("b"));

  std::set<std::string> dependents_c;
  dependents_c.insert("f");
  EXPECT_EQ( dependents_c, graph.GetDependents("c"));

  std::set<std::string> dependents_d;
  dependents_d.insert("e");
  dependents_d.insert("f");
  EXPECT_EQ( dependents_d, graph.GetDependents("d"));
}

TEST( DependencyGraph, AddDependencyGetDependees)
{
  cs3505::DependencyGraph graph = cs3505::DependencyGraph();

  graph.AddDependency( "a", "c" );
  graph.AddDependency( "b", "c" );
  graph.AddDependency( "c", "f" );
  graph.AddDependency( "d", "e" );
  graph.AddDependency( "d", "f" );

  // Check dependees
  std::set<std::string> dependees_c;
  dependees_c.insert("a");
  dependees_c.insert("b");
  EXPECT_EQ( dependees_c, graph.GetDependees("c"));

  std::set<std::string> dependees_f;
  dependees_f.insert("c");
  dependees_f.insert("d");
  EXPECT_EQ( dependees_f, graph.GetDependees("f"));

  std::set<std::string> dependees_e;
  dependees_e.insert("d");
  EXPECT_EQ( dependees_e, graph.GetDependees("e"));
}

// TODO: test RemoveDependency
TEST( DependencyGraph, RemoveDependency)
{
  cs3505::DependencyGraph  * graph = new cs3505::DependencyGraph();

  graph->AddDependency( "a", "b" );
  EXPECT_TRUE(graph->HasDependees("b"));
  EXPECT_TRUE(graph->HasDependents("a"));

  graph->RemoveDependency( "a", "b" );
  EXPECT_FALSE(graph->HasDependees("b"));
  EXPECT_FALSE(graph->HasDependents("a"));

  // Check graph
  EXPECT_EQ( 0, graph->GetSize());
  EXPECT_FALSE( graph->GraphContains("a"));
  EXPECT_FALSE( graph->GraphContains("b"));

  // Assume this does nothing
  graph->RemoveDependency( "a", "b" );
  EXPECT_EQ( 0, graph->GetSize());
}

TEST( DependencyGraph, RemoveDependencySharedNode)
{
  cs3505::DependencyGraph  * graph = new cs3505::DependencyGraph();

  graph->AddDependency( "a", "b" );
  graph->AddDependency( "b", "c" );
  graph->AddDependency( "c", "d" );

  graph->RemoveDependency( "b", "c");

  EXPECT_TRUE(graph->HasDependees("b"));
  EXPECT_FALSE(graph->HasDependents("b"));
  EXPECT_EQ( 2, graph->GetSize());
  EXPECT_TRUE( graph->GraphContains("a"));
  EXPECT_TRUE( graph->GraphContains("b"));
  EXPECT_TRUE( graph->GraphContains("c"));
  EXPECT_TRUE( graph->GraphContains("d"));

  graph->RemoveDependency("a","b");

  EXPECT_FALSE(graph->HasDependees("b"));
  EXPECT_FALSE(graph->HasDependents("a"));
  EXPECT_EQ( 1, graph->GetSize());
  EXPECT_FALSE( graph->GraphContains("a"));
  EXPECT_FALSE( graph->GraphContains("b"));

  // Empty graph
  graph->RemoveDependency("c","d");

  EXPECT_EQ( 0, graph->GetSize());
}

TEST( DependencyGraph, ReplaceDependents)
{
  cs3505::DependencyGraph  * graph = new cs3505::DependencyGraph();

  graph->AddDependency( "a", "b" );
  graph->AddDependency( "a", "c" );
  graph->AddDependency( "a", "d" );

  std::set<std::string> myset;
  myset.insert("e");
  myset.insert("f");

  graph->ReplaceDependents( "a", myset);

  EXPECT_EQ( myset, graph->GetDependents("a"));
  EXPECT_FALSE(graph->HasDependees("b"));
  EXPECT_FALSE(graph->HasDependees("c"));
  EXPECT_FALSE(graph->HasDependees("d"));
}

TEST( DependencyGraph, ReplaceDependees)
{
  cs3505::DependencyGraph  * graph = new cs3505::DependencyGraph();

  graph->AddDependency( "a", "d" );
  graph->AddDependency( "b", "d" );
  graph->AddDependency( "c", "d" );

  std::set<std::string> myset;
  myset.insert("e");
  myset.insert("f");

  graph->ReplaceDependees( "d", myset);

  EXPECT_EQ( myset, graph->GetDependees("d"));
  EXPECT_FALSE(graph->HasDependents("a"));
  EXPECT_FALSE(graph->HasDependents("b"));
  EXPECT_FALSE(graph->HasDependents("c"));
}
