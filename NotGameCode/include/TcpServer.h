#ifndef TCPSERVER_H
#define TCPSERVER_H

#include <map>
#include <vector>
#include <string>

namespace cs3505
{

	class TcpServer
	{

	private:

		// Socket file descriptor. Stores the value returned by the socket system call 
		// and the accept system call : http://www.linuxhowtos.org/data/6/fd.txt
		int server_end_point;

		// The port number on which the server communicates.
		int port;

		// Variable to assign client ID's
		int current;

		// Wether or not the server is running
		bool running;

		// Stores the current client connections.
		// map < PID, sockfd >
		std::map < int, int > clients;

		std::map < int, std::string > address;

		// Called when something goes wrong.
		void error( const char *msg );

	public:

		// Constructor / Destructor
		TcpServer(int port);
		~TcpServer();

		// Methods
		void run( void (*callback)(int) );
		void stop();
		std::vector<int> getClients();
		std::string getAddress( int client );
		void communicate( int socket );
		void send (int client, std::string message);
		std::string receiveLine (int socket);
	};

}

#endif