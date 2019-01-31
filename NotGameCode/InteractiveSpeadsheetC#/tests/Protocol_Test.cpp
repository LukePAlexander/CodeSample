/**
* A program that tests our service.
*
* Author: Kurt Bruns
* Date: 2017 04 21
*/

#include "../include/TcpServer.h"  // testing object
#include "../include/TestClient.h" // helper object
#include "gtest/gtest.h"           // testing framework

/**
* Helper method to split the incoming string using a delimiter.
*/
std::vector<std::string> split( std::string message, std::string delimiter );

/**
* Helper method that creates a new client, performs a handshake with the server
* sending a username (client + id) and returns the client. Assumes that the 
* service is already running locally.
*
* TODO: throw error if service isn't started. Also, in the future the service 
* could be started automatically
*/
cs3505::TestClient * Handshake()
{
  // Create a new client
  cs3505::TestClient * client =  new cs3505::TestClient("localhost", 2112);

  // Send a name to server
  client->send("client" + client->receive() + "\n");

  return client;
}

// void timeMethod( void (*method)(), bool * flag, double seconds )
// {
//   bool flag = false;

//   method

//   bool flag = true;
// }

/**
* Test that the service sends an integer first thing.
*/
TEST( TcpServer, Handshake)
{

  // Create a new client
  cs3505::TestClient * client =  new cs3505::TestClient("localhost", 2112);

  // Check that the first message received represents a integer
  // TODO: could check that it is unique.
  std::string message = client->receive();
  for(char& c : message)
  {
    ASSERT_TRUE(isdigit(c));
  }

  // Send a message
  client->send("client1\n");

}

TEST( TcpServer, FileListCode)
{
  // Create a new client
  cs3505::TestClient * client = Handshake();

  // When a client wants to open a new spreadsheet, a packet containing the
  // operation code ‘0’ is sent to the server to request the list of existing
  // files.
  client->send("0");

  // In response, the server sends a packet containing the operation code ‘0’
  // followed by a list of the names of all the existing spreadsheets to the
  // client for reference.
  std::string response = client->receive();

  // Check that the response contains the operations code '0'
  ASSERT_EQ( response.at(0), '0' );

}

TEST( TcpServer, NewSpreadsheet)
{
  // Stores received messages from the server.
  std::string response;

  // Create a new client
  cs3505::TestClient * client = Handshake();

  // When a client wants to open a new spreadsheet, a packet containing the
  // operation code ‘0’ is sent to the server to request the list of existing
  // files.
  client->send("0");

  // In response, the server sends a packet containing the operation code ‘0’
  // followed by a list of the names of all the existing spreadsheets to the
  // client for reference.
  response = client->receive();
  ASSERT_EQ( response.at(0), '0' );

  // Send a file name
  client->send("1\tsomeNewFileName\n");

  // If the name doesn’t exist, the server sends a packet beginning with the
  // operation code ‘1, followed by the document ID that the server assigns to
  // the new spreadsheet. Both the client and the server will open blank
  // spreadsheets and begin communicating edits.
  response = client->receive();
  ASSERT_EQ( response.at(0), '1' );


  std::string s = "scott>=tiger";
  std::string delimiter = "\t";
  std::string token = s.substr(0, s.find(delimiter)); // token is "scott"


}

// TODO: Test that actual file list is returned


// TESTS HELPER METHODS NO NEED TO LOOK FURTHER //

/**
* Helper method to split the incoming string using a delimiter.
*/
std::vector<std::string> split( std::string message, std::string delimiter )
{
  // Stores the result
  std::vector<std::string> result;

  // Position as we move through string
  size_t position = 0;

  // Token instracted
  std::string token;

  // Iterate through the message looking for the delimiter, pulling out tokens
  // and storing them in the vecot.
  while ((position = message.find(delimiter)) != std::string::npos)
  {
      token = message.substr(0, position);

      result.push_back(token);

      message.erase(0, position + delimiter.length());
  }

  // If there are characters left we want those too.
  if( message.length() != 0)
  {
    result.push_back(message);
  }

  return result;

}

TEST( Split, Functionality)
{
  std::string test = "this\tshould\tsplit\tcorrectly";

  std::vector<std::string> expected;
  expected.push_back("this");
  expected.push_back("should");
  expected.push_back("split");
  expected.push_back("correctly");

  std::vector<std::string> result = split( test, "\t");

  ASSERT_EQ( expected, result );
}
















