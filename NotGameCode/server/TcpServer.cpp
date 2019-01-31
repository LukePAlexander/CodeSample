/*
 * A simple TCP server.
 *
 * Code referenced http://www.linuxhowtos.org/C_C++/socket.htm
 *
 * Author: Kurt Bruns
 * Date: April 5, 2017
 */

#include <string>
#include <iostream>
#include <stdio.h>			// Input / output.
#include <unistd.h>
#include <stdlib.h>
#include <string.h>
#include <sys/types.h>		// Data types used in system calls.
#include <sys/socket.h>		// Sockets.
#include <netinet/in.h>		// Internet domain addresses.
#include <arpa/inet.h>

// TODO remove
#include <thread>
#include <chrono>
#include <vector>

#include "../include/TcpServer.h"

namespace cs3505
{

/*
* This function is called when a system call fails. It displays a message 
* about the error on stderr and then aborts the program. The perror man page 
* gives more information: http://www.linuxhowtos.org/data/6/perror.txt
*/
void TcpServer::error( const char *msg )
{
	perror(msg);
	exit(1);
}

TcpServer::TcpServer( int port )
{
	this->current = 0;

	// Set port
	this->port = port;

	// Initialize wether the service is started 
	this->running = false;

	// A sockaddr_in is a structure containing an internet address. Defined in 
	// netinet/in.h 
	//
	// struct sockaddr_in
	// {
	//   short   sin_family; /* must be AF_INET */
	//   u_short sin_port;
	//   struct  in_addr sin_addr;
	//   char    sin_zero[8]; /* Not used, must be zero */
	// };
	//
	struct sockaddr_in address;

	// Create an endpoint for communication.
	//
	// AF_INET Internet address.
	// 
	// SOCK_STREAM Stream sockets use TCP (Transmission Control Protocol).
	// A reliable, stream oriented protocol.
	//
	// Specifying a protocol of 0 causes socket() to use an unspecified
	// default protocol appropriate for the requested socket type. This will 
	// deafult to TCP for stream sockets.
	this->server_end_point = socket(AF_INET, SOCK_STREAM, 0);

	// Upon successful completion, socket() returns a nonnegative integer,
	// the socket file descriptor. Otherwise a value of -1 is returned and
	// errno is set to indicate the error.
	if( server_end_point < 0)
	{
		error("ERROR opening socket");
	}

	// The function bzero() sets all values in a buffer to zero. It takes two 
	// arguments, the first is a pointer to the buffer and the second is the 
	// size of the buffer. i.e. initializes address to zeros. ----
	bzero((char *) &address, sizeof(address));

	// Describe server address
	address.sin_family = AF_INET;

	// It is necessary to convert this to network byte order using the function 
	// htons() which converts a port number in host byte order to a port number
	// in network byte order. http://www.linuxhowtos.org/data/6/byteorder.html
	address.sin_port = htons(port);

	// Symbolic constant INADDR_ANY describes the IP address of this machine.
	address.sin_addr.s_addr = INADDR_ANY;

	// Assigns an address to an unnamed socket. This can fail for a number of 
	// reasons, a common case is the socket is already in use by this machine.
	if( bind(server_end_point, (struct sockaddr *) &address, sizeof(address)) < 0)
	{
		error("ERROR on binding.");
	}

}

// TODO: comment
TcpServer::~TcpServer()
{
	// TODO: implement
}


/**
* Starts a service that runs indefinitely, listening for TCP connections on port
* 13138. When a connection it reads from the connection and echos and messages 
* that are recieved.
*/
void TcpServer::run( void (*callback)(int) )
{
	// Start the service
	this->running = true;

	// Socket file descriptor. Stores the value returned by the socket system call 
	// and the accept system call : http://www.linuxhowtos.org/data/6/fd.txt
	int new_socket_fd;

	// When child processes are created they are assigned a process ID. 
	int process_id;

	// Stores the size of the address of the client. Needed for accept system
	// call.
	socklen_t client_address_size;

	// A sockaddr_in is a structure containing an internet address. Defined in 
	// netinet/in.h 
	//
	// struct sockaddr_in
	// {
	//   short   sin_family; /* must be AF_INET */
	//   u_short sin_port;
	//   struct  in_addr sin_addr;
	//   char    sin_zero[8]; /* Not used, must be zero */
	// };
	//
	struct sockaddr_in client_address;

	// Listen on the socket for connections. The second argument is the number 
	// of connections that can be waiting while the process is handling a 
	// particular connection. This should be set to 5, the maximum size 
	// permitted by most systems.
	listen( server_end_point, 5);

	// Caluclate size
	client_address_size = sizeof(client_address);

	// The service is always looks for oncoming connections.
	std::cout << "Listening for TCP connections on port " << port << "..." << std::endl;

	while( running )
	{
		// The accept() system call causes the process to block until a client 
		// connects to the server. Thus, it wakes up the process when a connection 
		// from a client has been successfully established. It returns a new file 
		// descriptor, and all communication on this connection should be done 
		// using the new file descriptor. The second argument is a reference 
		// pointer to the address of the client on the other end of the connection, 
		// and the third argument is the size of this structure. The accept() man 
		 // page has more information : http://www.linuxhowtos.org/data/6/accept.txt
		new_socket_fd = accept( 
								server_end_point, 
								(struct sockaddr *) &client_address, 
								&client_address_size
							);

		// If the process is stopped then there will be an error
		// Otherwise, continue as normal.
		if( !running )
		{
			break;
		}

		if (new_socket_fd < 0)
		{
			error("ERROR on accept");
		}

		// Now that a connection is established create a new process to handle
		// the communication on the socket.
		process_id = fork();
		if( process_id < 0 )
		{
			error("ERROR on fork");
		}

		// When the two processes have completed their conversation, this 
		// process simply exits.
		if( process_id == 0 )
		{
			close(server_end_point);

			int id = getpid();
			this->clients[id] = new_socket_fd;
			this->address[id] = inet_ntoa(client_address.sin_addr);

			// std::cout << "callback(" << id << ")" << std::endl;
			callback(id);
			exit(0);
		}
		else
		{
			close(new_socket_fd);
		}
	} /* end of while */

	close(server_end_point);
	
	return; /* we never get here */
}

/**
* Stops the service
*/
void TcpServer::stop()
{
	// Set the state of the service
	this->running = false;

	// Close the socket
	close(server_end_point);
}

/**
* Returns the connected clients
*/
std::vector<int> TcpServer::getClients()
{
	std::vector<int> keys;
	
	for (auto const& client : this->clients)
	{
		keys.push_back(client.first);
	}
	return keys;
}

/**
* Returns the IP address of a client.
* TODO: if client DNE
*/
std::string TcpServer::getAddress( int client )
{
	return this->address[client];
}

void TcpServer::send (int client_id, std::string message)
{
	// TODO: May need to stop listening in order to send in which case:
	// int shutdown(int socket, int how) - shut down socket send and receive operations

	// Return value for the read() and write() calls, meaning it contains the 
	// number of characters read or written.
	int return_value;

	// Respond with a message. The last argument is the size of the message.
	return_value = write( this->clients[client_id], message.c_str(), message.length());

	if (return_value < 0)
	{
		error("ERROR writing to socket");
	}
}

/**
* Returns a 
*
*/
std::string TcpServer::receiveLine (int client_id)
{
	// The server reads characters from the socket connection into this buffer.
	char buffer[256];

	// Return value for the read() call. contains the number of characters read.
	int return_value;

	// Initialize buffer
	bzero( buffer, 256);

	// Flags
	int flags;

	// Read from the socket into the buffer. This will block until there is 
	// something for it to read in the socket, i.e. after the client has 
	// executed a write(). It will read either the total number of characters
	// in the socket or 255, whichever is less, and return the total number of
	// characters read. The read() man page has more information:
	// http://www.linuxhowtos.org/data/6/read.txt
	return_value = recv( this->clients[client_id], buffer, 255, 0);
	//return_value = read( this->clients[client_id], buffer, 255);
	
	if( return_value == 0 )
	{
		throw std::runtime_error("The client has closed the connection");
	}
	else if (return_value < 0)
	{
		error("ERROR reading from socket");
	}

	return buffer;
}

} /* end of namespace */







