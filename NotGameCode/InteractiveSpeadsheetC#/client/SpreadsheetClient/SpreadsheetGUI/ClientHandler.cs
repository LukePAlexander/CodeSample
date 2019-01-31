//using SpreadsheetGUI;
using Client;
using SpreadsheetGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public class ClientHandler
    {

        public Dictionary<string, SpreadsheetForm> allSheets;
        public TcpClient TCP;
        public ClientForm CF;
        public string UserID;
        public string UserName;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public ClientHandler()
        {

            // Temporary call TOOD: move to controller.
            //Connect("45.33.43.145", 13138, "Hi from undergrad lab!\n");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ClientForm FirstCF = new ClientForm(this);


            Application.Run(FirstCF);
        }



        /////This starts the Server Message handling

        /// <summary> 
        /// General receive to be called when we receive a message, 
        /// this will break down into the 2 possibilties, error or set.
        /// If neither code is sent to us, ignore message.
        /// </summary>
        /// <param name="message"></param>
        public void ReceiveCode(String message)
        {
            char tabToken = '\t';

            string[] inputTokens = message.Split(tabToken);

            if (!validDocID(inputTokens[1]))
            {
                return;
            }

            SpreadsheetForm SF = allSheets[inputTokens[1]];
            string[] concatInput = new string[inputTokens.Count() - 1];
            for (int i = 0; i < inputTokens.Count() - 1; i++)
            {
                concatInput[i] = inputTokens[i + 1];
            }

            switch (inputTokens[0])
            {
                case "0":

                    CF.receiveFiles(concatInput);
                    return;
                case "1":
                    CF.validNew(inputTokens[1]);                    
                    return;
                case "2":
                    CF.validOpen(inputTokens);
                    return;
                case "3":
                    UpdateCell(inputTokens[1], inputTokens[2], inputTokens[3]);
                    return;
                case "4":
                    SF.ValidUpdate();
                    return;
                case "5":
                    SF.InvalidUpdate();
                    return;
                case "6":
                    SF.UpdateSheetName(inputTokens[2]);
                    return;
                case "7":                    
                    return;
                case "8":                    
                    return;
                case "9":
                    return;
                case "A":
                    EditFocus(inputTokens[1], inputTokens[2], inputTokens[3], inputTokens[4]);
                    break;
            }
        }

        /// <summary>
        /// Wrapper that handles the input before invoking SF.ServerCellActive
        /// </summary>
        /// <param name="DocID"></param>
        /// <param name="CellName"></param>
        /// <param name="UserID"></param>
        /// <param name="UserName"></param>
        private void EditFocus(string DocID, string CellName, string UserID, string UserName)
        {
            SpreadsheetForm SF = allSheets[DocID];
            SF.ServerCellActive(CellName, Int32.Parse(UserID), UserName);
        }


        /// <summary>
        /// Update the cell contents based on DocID, CellName, and CellContents.
        /// </summary>
        /// <param name="DocID"></param>
        /// <param name="CellName"></param>
        /// <param name="CellContents"></param>
        private void UpdateCell(string DocID, string CellName, string CellContents)
        {
            //We know from validDocID it's contained, so this is safe.
            SpreadsheetForm SSF = allSheets[DocID];
            SSF.ServerUpdateCell(CellName, CellContents);
            //Spreadsheet SS = SSF.sheet
        }


        /// <summary>
        /// Helper method to determine if the docID we recieved is something this client cares about.
        /// </summary>
        /// <param name="docID"></param>
        /// <returns></returns>
        private bool validDocID(string docID)
        {
            return (allSheets.Keys.Contains(docID));
        }





        //////This starts the Client messages

        /// <summary>
        /// Client side handshake initial connection, connects to server along IP and port number,
        /// returns the connect to the ClientForm so we can send the name and receive an id.
        /// </summary>
        /// <param name="server"></param>
        /// <returns></returns>
        public TcpClient Connect(String server)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
               // TCP = new TcpClient(server, 13138);
                TCP = new TcpClient(server, 2114);


                NetworkStream stream = TCP.GetStream();

                byte[] buffer = new byte[256];
                stream.BeginRead(buffer, 0, 255, ReadCallback, stream);

                return TCP;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            return null;

        }


        /// <summary>
        /// This loop should be started by Connect and should read every return from the server.
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallback(IAsyncResult ar)
        {

            //Cited from the example in: https://msdn.microsoft.com/en-us/library/system.net.sockets.networkstream.endread(v=vs.110).aspx
            NetworkStream myNetworkStream = (NetworkStream)ar.AsyncState;
            byte[] myReadBuffer = new byte[1024];
            String myCompleteMessage = "";
            int numberOfBytesRead;

            numberOfBytesRead = myNetworkStream.EndRead(ar);
            myCompleteMessage =
                String.Concat(myCompleteMessage, System.Text.Encoding.ASCII.GetString(myReadBuffer, 0, numberOfBytesRead));

            // message received may be larger than buffer size so loop through until you have it all.
            while (myNetworkStream.DataAvailable)
            {

                myNetworkStream.BeginRead(myReadBuffer, 0, myReadBuffer.Length,
                                                           ReadCallback,
                                                           myNetworkStream);
            }

            //Once we get here there is no more to read?

            char tabToken = '\n';

            string[] inputTokens = myCompleteMessage.Split(tabToken);

            for(int i = 0; i < inputTokens.Count(); i++)
            {
                ReceiveCode(inputTokens[i]);
            }

            myNetworkStream.BeginRead(myReadBuffer, 0, myReadBuffer.Length, ReadCallback, myNetworkStream);

        }


        /// <summary>
        /// This opens a spreadsheet GUI window for the client to use.
        /// If the file doesn't exist the protocol states to simply create a new one.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public void LoadSpreadsheet(String fileName)
        {
            if (fileName.Substring(fileName.Length - 5, 5) == ".sprd")
            {
                SendMessage("Load " + fileName);
            }
            else
            {
                SendMessage("Load " + fileName + ".sprd");
            }
        }

        
        /// <summary>
        /// Ask for the files available and return them as a list to populate the combobox.
        /// </summary>
        public void AskForFiles()
        {
            SendMessage("0\n");            
        }


        /// <summary>
        /// Our method for opening a new filename, we presume the filename is valid as 
        /// it was chosen from the options given by the server.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public void Open(string fileName)
        {   
            if (fileName.Substring(fileName.Length - 5) != ".sprd")
            {
                fileName += ".sprd";
            }

            SendMessage("2\t" + fileName + "\n");
        }


        /// <summary>
        /// When the client asks for a new spreadsheet. 
        /// The method sends the appropriate message and reads back the server message.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public void NewSpreadsheet(string fileName)
        {
            if(fileName.Substring(fileName.Length-5) != ".sprd")
            {
                fileName += ".sprd";
            }

            SendMessage("1\t" + fileName + "\n");

        }


        /// <summary>
        /// Changes the focus, indicated by spreadsheet GUI
        /// </summary>
        /// <param name="DocID"></param>
        /// <param name="CellName"></param>
        public void ChangeFocus(string DocID, string CellName)
        {
            SendMessage("8\t" + DocID + "\t" + CellName + "\n");
        }


        /// <summary>
        /// Sends attempt to rename spreedsheet.
        /// </summary>
        /// <param name="docID"></param>
        /// <param name="newFileName"></param>
        public void rename(string docID, string newFileName)
        {
            SendMessage("7\t" + docID + "\t" + newFileName + "\n");
        }


        /// <summary>
        /// The method to send a change request to the server. 
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="contents"></param>
        public void TrySet(string docID, string cellName, string contents)
        {
            SendMessage("3\t" + docID + "\t" + cellName + "\t" + contents + "\n");
        }
    

        /// <summary>
        /// A method that undoes the last action of the spreadsheet.
        /// </summary>
        public void undo(string docID)
        {
            SendMessage("3\t" + docID + "\n");
        }


        /// <summary>
        /// A method that redos the last undo of the spreadsheet.
        /// </summary>
        public void redo(string docID)
        {
            SendMessage("5\t" + docID + "\n");
        }


        /// <summary>
        /// A method that requests a save of the spreadsheet
        /// </summary>
        public void save(string docID)
        {
            SendMessage("6\t" + docID + "\n");
        }


        /// <summary>
        /// General method to send something to the server.
        /// Used by TrySet and OpenSpreadsheet.
        /// </summary>
        /// <param name="v"></param>
        public void SendMessage(string message)
        {
            NetworkStream stream = TCP.GetStream();

            //Encode the Byte array as our message to send to the server.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            //Write to stream (from my understanding) sends the message to the server.
            stream.Write(data, 0, data.Length);            

        }
    }
}
