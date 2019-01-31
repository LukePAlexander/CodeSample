/**
* Controller.h
*
* Header file for the controller of our server.
*
* Author: Kurt Bruns
* Date: April 20, 2017
*/

#ifndef CONTROLLER_H
#define CONTROLLER_H

#include <map>
#include <queue>
#include <string>
#include <mutex>
#include <utility>

#include "../include/TcpServer.h"

namespace cs3505
{

	class Controller
	{

	private:

		TcpServer * server;

		std::mutex clients_mutex;
		std::map< int, std::string> clients;

		std::mutex message_mutex;
		std::queue<std::pair <int, std::string>> incoming;
		std::queue<std::pair <int, std::string>> outgoing;

	public:

		Controller( TcpServer * server );
		~Controller();

		std::string getIncoming();


		// TODO:: Comment
		void addClient( int client_id, std::string username );
		std::string getName( int client_id);

		// TODO: comment
		void validate( int client );

		void listen( int client);

		/**
		* This method handles all communication once a connection has been established. 
		* Communication involves listening for and handling messages from clients. 
		* These messages include requests to create or open spreadsheets, edit cells, 
		* rename files, and so forth.
		*/
		void handle ( int client, std::string request );

		void ReceivedFileList( int client, std::string request );
		void ReceivedNew( int client, std::string request );
		void ReceivedOpen( int client, std::string request );
		void ReceivedEdit( int client, std::string request );
		void ReceivedUndo( int client, std::string request );
		void ReceivedRedo( int client, std::string request );
		void ReceivedSave( int client, std::string request );
		void ReceivedRename( int client, std::string request );
		void RecievedUnknown( int client, std::string request );

	};

}

#endif



