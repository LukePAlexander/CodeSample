/*
* Client for testing our service.
*
* Author: Kurt Bruns
* Date: April 20, 2017
*/

// Socket related
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <netdb.h> 

// Header file
#include "../include/TestClient.h"

#include "iostream"

namespace cs3505
{


/**
* Called when something goes wrong.
*/
void TestClient::error( const char *msg)
{
    perror(msg);
    exit(0);
}

/*
* Constructs a client with the provided address and port.
*/
TestClient::TestClient(std::string address, int port)
{
	// Store the hostname
	this->address = address;

	// Store the port number
	this->port = port;

	// A sockaddr_in is a structure containing an internet address. Defined in 
	// netinet/in.h 
	struct sockaddr_in serv_addr;

	// Used to represent an entry in the hosts database
	struct hostent *server;

	// Create the socket endpoint
	this->sockfd = socket(AF_INET, SOCK_STREAM, 0);
	if (this->sockfd < 0) 
	{
		error("ERROR opening socket");
	}

	// Get the host
	server = gethostbyname(address.c_str());
	if (server == NULL)
	{
		fprintf(stderr,"ERROR, no such host\n");
		exit(0);
	}
	bzero((char *) &serv_addr, sizeof(serv_addr));
	serv_addr.sin_family = AF_INET;

	// Set the fields in server_addr
	bcopy((char *)server->h_addr, 
	     (char *)&serv_addr.sin_addr.s_addr,
	     server->h_length);

	serv_addr.sin_port = htons(port);

	if (connect(this->sockfd,(struct sockaddr *) &serv_addr,sizeof(serv_addr)) < 0)
	{
		error("ERROR connecting");
	}
	else
	{
		std::cout << "Client connected to " << address << " on port " << port << std::endl;
	}
}

/**
* Closes the socket.
*/
TestClient::~TestClient()
{
	close(this->sockfd);
}

/**
* Sends a message accross the connection
*/
void TestClient::send (std::string message)
{
	// Contains the number of character written.
	int num_characters;

	num_characters = write(this->sockfd, message.c_str(), message.length());
	if (num_characters < 0)
	{
		error("ERROR writing to socket");
	}

	// TODO: send longer messages.
	if (num_characters != message.length())
	{
		error("ERROR full message was not sent");
	}
}

/**
* Receives a message accross the connection.
*/
std::string TestClient::receive()
{
	// Reads characters from the socket connection into this buffer.
	char buffer[1024];

	// Return value for the read() call, contains the number of characters read.
	int return_value;

	// Initialize buffer
	bzero( buffer, 1024);

	// Read from the socket into the buffer. This will block until there is 
	// something for it to read in the socket, i.e. after the client has 
	// executed a write().
	return_value = read( this->sockfd, buffer, 1023);
	
	// It will read either the total number of characters in the socket or 255, 
	// whichever is less, and return the number of characters read. The read() 
	// man page has more information: http://www.linuxhowtos.org/data/6/read.txt
	if (return_value < 0)
	{
		error("ERROR reading from socket");
	}

	return buffer;
}

} /* namespace */