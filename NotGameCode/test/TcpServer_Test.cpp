/**
* A program that tests our service.
*
* Author: Kurt Bruns
* Date: 2017 04 19
*/

#include "../include/TcpServer.h"   // testing object
#include "../include/TestClient.h"  // testing object
#include "gtest/gtest.h"            // testing framework

#include <fstream>
#include <iostream>
#include <thread>
#include <chrono>
#include <set>
#include <vector>

// Shared resource
cs3505::TcpServer * server;

// Time based seed for random number generator
std::atomic<int> seed (time(NULL));

/**
* Generates a random port from recomended port numbers.
*/
int randomPort()
{
	srand(seed++);
	return 49152 + (rand() % (int)(65535 - 49152 + 1));
}

/**
* Starts a simple echo on a random port which it returns.
*/
int startServer()
{
	// Get a random port so tests can be run over and over and over.
	int port = randomPort();

	//Create a new TCP Socket Server. Start the server on a new thread.
	// Simply echo back any message received, forever.
	server = new cs3505::TcpServer(port);
	std::thread service (&cs3505::TcpServer::run, server, [](int client_id) 
	{
		while(1)
		{
			std::string message = server->receiveLine(client_id);
			server->send(client_id, message);
		}
	});

	// Detaches the thread represented by the object from the calling thread, 
	// allowing them to execute independently from each other.
	service.detach();

	return port;
}

/**
* Cleans up server.
*/
void shutDownServer()
{
	server->stop();
	delete server;
}

/**
* Tests that the server can echo a simple message.
*/
TEST( TcpServer, SendReceive)
{
	int port = startServer();

	// Create a new client
	cs3505::TestClient * client =  new cs3505::TestClient("localhost", port);

	// Send a message
	client->send("Hello, world!\n");

	// Expect the message to be echoed back
	ASSERT_EQ( "Hello, world!\n", client->receive());

	shutDownServer();
}

/**
* Test that the server keeps track of which client is sending
* each message.
*/
TEST( TcpServer, MultipleClients)
{

	int port = startServer();

	// Create five clients to hit the server with a bunch of messages
	cs3505::TestClient * clients [5];
	for (int i = 0; i < 5; i++)
	{
		clients[i] = new cs3505::TestClient("localhost", port);
	}

	// Randomly select a client and send a unique message, check that
	// the same message is sent back.
	for (int i = 0; i < 100; i++)
	{
		// Get a random client
		cs3505::TestClient * client = clients[rand() % 5];

		// Send a unique string
		client->send(std::to_string(i));

		// Check that we recieved the unique string back
		ASSERT_EQ(std::to_string(i), client->receive());
	}

	shutDownServer();
}

/**
* Checks that long messages are fully processed, meaning that if only
* a certain number of characters are sent, it waits until it finds a
* new line before echoing the full message.
*/
TEST( TcpServer, LongMessage)
{

	int port = startServer();

	cs3505::TestClient * client = new cs3505::TestClient("localhost", port);

	// Read through every line in file, sending complete lines.
	std::string line;
	std::ifstream myfile ("./src/test/LongMessage.txt");
	if (myfile.is_open())
	{
		while ( getline (myfile,line) )
		{
			std::string message = line + "\n";
			client->send(message);
			ASSERT_EQ(message, client->receive());
		}
		myfile.close();
	}
	else 
	{
		std::cout << "Unable to open file" << std::endl;; 
		FAIL();
	}

	shutDownServer();
}

/**
* Tests that if a packet contains multiple messages they are handled
* correctly.
*/
TEST( TcpServer, Messages)
{

	int port = startServer();

	cs3505::TestClient * client = new cs3505::TestClient("localhost", port);

	std::string message = "first\nsecond\n";

	// Send two messages at once
	client->send(message);
	ASSERT_EQ("first\n", client->receive());
	ASSERT_EQ("second\n", client->receive());

	// Send partial message
	client->send("First message \nSecond message that is...");
	client->send(" now complete\n");

	ASSERT_EQ("First message \n", client->receive());
	ASSERT_EQ("Second message that is... now complete\n", client->receive());

	shutDownServer();
}