#ifndef TESTCLIENT_H
#define TESTCLIENT_H

#include <string>

namespace cs3505
{

	class TestClient
	{

	private:

		// Socket file descriptor. Stores the value returned by the socket system call 
		// and the accept system call : http://www.linuxhowtos.org/data/6/fd.txt
		int sockfd;

		// IP address on which the client connects to
		std::string address;

		// The port number on which the server communicates.
		int port;

		// Called when something goes wrong.
		void error( const char *msg );

	public:

		// Constructor / Destructor
		TestClient(std::string address, int port);
		~TestClient();

		// Methods
		void send (std::string message);
		std::string receive();
	};
}

#endif