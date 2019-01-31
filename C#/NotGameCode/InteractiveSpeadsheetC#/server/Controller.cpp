/**
*
* Program which controls our server.
*
* Author: Kurt Bruns
* Date: April 20, 2017
*/

#include <unistd.h>

#include <iostream>       // std::cout
#include <thread>         // std::thread
#include <mutex>          // std::mutex

#include "../include/Controller.h"

namespace cs3505
{

Controller::Controller( TcpServer * server )
{

	this->server = server;

}

Controller::~Controller()
{
	// TODO:: implement...
}

std::string Controller::getIncoming()
{

}

/**
* Perform the verification specified by the protocol. Sends a unique identifier
* to the client and receives a username in return. Once this has been completed
* enters the regular communication loop.
*/
void Controller::validate( int client_id )
{
	std::cout << "New connection: " << server->getAddress(client_id) << " ";
	std::cout << "with ID " << client_id << std::endl;

	// Send the client it's unique identifier.
	this->server->send( client_id, std::to_string(client_id) );

	// Wait for the client to send us a username.
	std::string message = this->server->receiveLine( client_id );

	// Trim newline character off
	std::string name = message.substr(0, message.length() - 1);

	std::cout << "Recieved username from client: " << name << std::endl;

	// Store the username
	this->addClient( client_id, name);
}

/**
* Stores the client username. If the client_id already exists throws
* a runtime exception error.
*/
void Controller::addClient( int client_id, std::string username )
{
	this->clients_mutex.lock();

	// Check to make sure key is unique
	if( clients.count(client_id) == 0 )
	{
		clients[client_id] = username;
	}
	else
	{
		throw std::runtime_error("Client already exists.");
	}

	this->clients_mutex.unlock();
}

// TODO: comment
std::string Controller::getName( int client_id)
{
	return clients[client_id];
}

void Controller::listen( int client_id )
{
	std::string message;

	while(1)
	{
		try
		{
			message = this->server->receiveLine(client_id);
		}
		catch (const std::exception& e)
		{
			std::cout << "Client " << client_id << " has closed the connection." << std::endl;
			break;
		}

		this->message_mutex.lock();

		this->incoming.push( std::make_pair( client_id, message ));

		std::cout << "push to incoming <" << client_id << ", " << message.substr(0,message.length()-1) << ">" << std::endl;

		this->message_mutex.unlock();
	}
}

/**
* This method handles all communication once a connection has been established. 
* Communication involves listening for and handling messages from clients. 
* These messages include requests to create or open spreadsheets, edit cells, 
* rename files, and so forth.
*/
void Controller::handle ( int client, std::string request )
{
	// Parse message
	if( request.at(0) > 47 && request.at(0) < 58)
	{
		// Switch on operation code
		switch ( std::atoi(&request.at(0)) )
		{
			case 0:
				this->ReceivedFileList( client, request );
				break;
			case 1:
				this->ReceivedNew( client, request );
				break;
			case 2:
				this->ReceivedOpen( client, request );
				break;
			case 3:
				this->ReceivedEdit( client, request );
				break;
			case 4:
				this->ReceivedUndo( client, request );
				break;
			case 5:
				this->ReceivedRedo( client, request );
				break;
			case 6:
				this->ReceivedSave( client, request );
				break;
			case 7:
				this->ReceivedRename( client, request );
				break;
			default:
				this->RecievedUnknown( client, request );
				break;
		}
	}
	else
	{
		RecievedUnknown( client, request );
	}
}

// TODO: comment
void Controller::ReceivedFileList( int client, std::string request )
{
	std::cout << clients[client] << " (" + server->getAddress(client) + ") " << __func__ << "(request) " << std::endl;
	std::cout << "request : " << request;
}

// TODO: comment
void Controller::ReceivedNew( int client, std::string request )
{
	std::cout << clients[client] << " (" + server->getAddress(client) + ") " << __func__ << "(request) " << std::endl;
	std::cout << "request : " << request;
}

// TODO: comment
void Controller::ReceivedOpen( int client, std::string request )
{
	std::cout << clients[client] << " (" + server->getAddress(client) + ") " << __func__ << "(request) " << std::endl;
	std::cout << "request : " << request;
}

// TODO: comment
void Controller::ReceivedEdit( int client, std::string request )
{
	std::cout << clients[client] << " (" + server->getAddress(client) + ") " << __func__ << "(request) " << std::endl;
	std::cout << "request : " << request;
}

// TODO: comment
void Controller::ReceivedUndo( int client, std::string request )
{
	std::cout << clients[client] << " (" + server->getAddress(client) + ") " << __func__ << "(request) " << std::endl;
	std::cout << "request : " << request;
}

// TODO: comment
void Controller::ReceivedRedo( int client, std::string request )
{
	std::cout << clients[client] << " (" + server->getAddress(client) + ") " << __func__ << "(request) " << std::endl;
	std::cout << "request : " << request;
}

// TODO: comment
void Controller::ReceivedSave( int client, std::string request )
{
	std::cout << clients[client] << " (" + server->getAddress(client) + ") " << __func__ << "(request) " << std::endl;
	std::cout << "request : " << request;
}

// TODO: comment
void Controller::ReceivedRename( int client, std::string request )
{
	std::cout << clients[client] << " (" + server->getAddress(client) + ") " << __func__ << "(request) " << std::endl;
	std::cout << "request : " << request;
}

// TODO: comment
void Controller::RecievedUnknown( int client, std::string request )
{
	std::cout << clients[client] << " (" + server->getAddress(client) + ") " << __func__ << "(request) " << std::endl;
	std::cout << "request : " << request;
}

} /* end of namespace */


cs3505::Controller * controller;
cs3505::TcpServer * server;


void callback( int client )
{
	// std::cout << "callback(" << client << ")" << std::endl;

	controller->validate(client);

	// Start the regular communication diagram.
	controller->listen( client);

	// Wait for message from socket
	// std::string request = this->server->receiveLine(client);
}

/**
* Starts a server that manages spreadsheets that multiple clients can connect 
* to simultaneously. Controls all communication in between the server and 
* client.
*/
int main()
{
	server = new cs3505::TcpServer(2112);
	controller = new cs3505::Controller( server );

	std::thread service (&cs3505::TcpServer::run, server, callback);

	service.join();
}





// Start the server. Whenever a new TCP connection is created the server 
// calls the handshake method with an identifier for the client as the 
// parameter
//this->server->run( callback );

// NOTE: run the server on a separate thread -> have a process that queues 
// up messages to send to clients. 

// NOTE: (alt.) Sending information could also be completely handled within
// the communication loop, given that appropriate locks are established.

// TODO: Run -> handshake yields back to main method

// NOTE: after talking w/Peter
// Dynamic Buffer
// Yield
// Volatile




