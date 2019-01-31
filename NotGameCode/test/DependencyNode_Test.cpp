/**
* A program to test the DependencyNode class.
*
* Author: Kurt Bruns
* Date: 2017 04 17
*/

#include "../include/DependencyNode.h"  // testing object
#include "gtest/gtest.h"                // testing framework

#include <set>  // testing return value

TEST( DependencyNode, Initialization)
{
  cs3505::DependencyNode node = cs3505::DependencyNode();

  EXPECT_EQ( 0, node.NumDependents());
  EXPECT_EQ( 0, node.NumDependees());

  std::set<std::string> dependents = node.GetDependents();
  std::set<std::string> dependees = node.GetDependees();
  EXPECT_EQ( 0, dependents.size());
  EXPECT_EQ( 0, dependees.size());
}

TEST( DependencyNode, AddDependent)
{
  cs3505::DependencyNode node = cs3505::DependencyNode();

  // Test one dependent
  node.AddDependent("a");

  EXPECT_TRUE(node.DependentsContains("a"));
  EXPECT_EQ( 1, node.NumDependents());
  EXPECT_EQ( 1, node.GetDependents().size());

  // Test multiple dependets
  node.AddDependent("b");
  node.AddDependent("c");

  EXPECT_TRUE(node.DependentsContains("a"));
  EXPECT_TRUE(node.DependentsContains("b"));
  EXPECT_TRUE(node.DependentsContains("c"));
  EXPECT_EQ( 3, node.NumDependents());

  std::set<std::string> expected;
  expected.insert("a");
  expected.insert("b");
  expected.insert("c");

  EXPECT_EQ( 3, node.GetDependents().size());
  EXPECT_EQ( expected, node.GetDependents());

  // Test that duplicate dependent is not added
  node.AddDependent("a");
  EXPECT_EQ( 3, node.NumDependents());

}

TEST( DependencyNode, RemoveDependent)
{
  cs3505::DependencyNode node = cs3505::DependencyNode();

  node.AddDependent("a");
  node.RemoveDependent("a");

  EXPECT_EQ( 0, node.NumDependents());
  EXPECT_EQ( 0, node.GetDependents().size());
}

TEST( DependencyNode, AddDependee)
{
  cs3505::DependencyNode node = cs3505::DependencyNode();

  // Test one dependent
  node.AddDependee("a");

  EXPECT_TRUE(node.DependeesContains("a"));
  EXPECT_EQ( 1, node.NumDependees());
  EXPECT_EQ( 1, node.GetDependees().size());

  // Test multiple dependets
  node.AddDependee("b");
  node.AddDependee("c");

  EXPECT_TRUE(node.DependeesContains("a"));
  EXPECT_TRUE(node.DependeesContains("b"));
  EXPECT_TRUE(node.DependeesContains("c"));
  EXPECT_EQ( 3, node.NumDependees());

  std::set<std::string> expected;
  expected.insert("a");
  expected.insert("b");
  expected.insert("c");

  EXPECT_EQ( 3, node.GetDependees().size());
  EXPECT_EQ( expected, node.GetDependees());

  // Test that duplicate dependent is not added
  node.AddDependee("a");
  EXPECT_EQ( 3, node.NumDependees());
}

TEST( DependencyNode, RemoveDependee)
{
  cs3505::DependencyNode node = cs3505::DependencyNode();

  node.AddDependee("a");
  node.RemoveDependee("a");

  EXPECT_EQ( 0, node.NumDependees());
  EXPECT_EQ( 0, node.GetDependees().size());
}