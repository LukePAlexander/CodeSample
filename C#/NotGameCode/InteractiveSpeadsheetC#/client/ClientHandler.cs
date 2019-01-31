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

       //Dictionary<string, spreadsheetform>

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        void Main()
        {

            // Temporary call TOOD: move to controller.
            //Connect("45.33.43.145", 13138, "Hi from undergrad lab!\n");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ClientForm(this));
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
            char spaceToken = ' ';

            string[] inputTokens = message.Split(spaceToken);


            switch (inputTokens[0])
            {
            }
        }


        /// <summary>
        /// If the client has attempted to change a cell in error
        /// pop up a warning window and reason. No other changes are made. 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        private void ReceiveError(string cellName, string errorMessage)
        {
            DialogResult result = MessageBox.Show("That cell change at " + cellName + " would cause the error:" + errorMessage, "Warning",
MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                TcpClient client = new TcpClient(server, 13138);

                return client;
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
        /// This opens a spreadsheet GUI window for the client to use.
        /// If the file doesn't exist the protocol states to simply create a new one.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public void LoadSpreadsheet(String fileName, TcpClient TCPSocket)
        {
            if (fileName.Substring(fileName.Length - 5, 5) == ".sprd")
            {
                SendMessage("Load " + fileName, TCPSocket);
            }
            else
            {
                SendMessage("Load " + fileName + ".sprd", TCPSocket);
            }
        }

        

        /// <summary>
        /// Ask for the files available and return them as a list to populate the combobox.
        /// </summary>
        /// <param name="TCPSocket"></param>
        public string[] AskForFiles(TcpClient TCPSocket)
        {
            NetworkStream stream = TCPSocket.GetStream();

            //Encode the Byte array as our message to send to the server.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("0\n");

            //Write to stream (from my understanding) sends the message to the server.
            stream.Write(data, 0, data.Length);


            //Code for continual reading and deocding found at "http://stackoverflow.com/questions/25712809/how-to-read-unknown-data-length-in-a-tcp-client"
            List<byte> bigbuffer = new List<byte>();

            byte[] tempbuffer = new byte[254];
            //can be in another size like 1024 etc.. 
            //depend of the data as you sending from de client
            //i recommend small size for the correct read of the package


            while (stream.Read(tempbuffer, 0, tempbuffer.Length) > 0)
            {

                bigbuffer.AddRange(tempbuffer);
            }

            // now you can convert to a native byte array
            byte[] completedbuffer = new byte[bigbuffer.Count];

            bigbuffer.CopyTo(completedbuffer);

            //Do something with the data
            string fileNames = System.Text.Encoding.ASCII.GetString(completedbuffer);     

            //End of continual reading/decoding code citation.

            //Split the string into multiple filenames using the '\t' char as a splitter.
            char tabtoken = '\t';
            string[] fileNameStrings = fileNames.Split(tabtoken);

            stream.Close();

            return fileNameStrings;
        }


        /// <summary>
        /// Our method for opening a new filename, we presume the filename is valid as 
        /// it was chosen from the options given by the server.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="TCPSocket"></param>
        /// <returns></returns>
        public string[] Open(string fileName, TcpClient TCPSocket)
        {
            NetworkStream stream = TCPSocket.GetStream();

            Byte[] data;


            if (fileName.Substring(fileName.Length - 5) == ".sprd")
            {
                data = System.Text.Encoding.ASCII.GetBytes("1\t" + fileName + "\n");
            }
            else
            {
                data = System.Text.Encoding.ASCII.GetBytes("1\t" + fileName + ".sprd\n");
            }

            //Encode the Byte array as our message to send to the server.

            //Write to stream (from my understanding) sends the message to the server.
            stream.Write(data, 0, data.Length);


            //Code for continual reading and deocding found at "http://stackoverflow.com/questions/25712809/how-to-read-unknown-data-length-in-a-tcp-client"
            List<byte> bigbuffer = new List<byte>();

            byte[] tempbuffer = new byte[254];
            //can be in another size like 1024 etc.. 
            //depend of the data as you sending from de client
            //i recommend small size for the correct read of the package


            while (stream.Read(tempbuffer, 0, tempbuffer.Length) > 0)
            {

                bigbuffer.AddRange(tempbuffer);
            }

            // now you can convert to a native byte array
            byte[] completedbuffer = new byte[bigbuffer.Count];

            bigbuffer.CopyTo(completedbuffer);

            //Do something with the data
            string DocID = System.Text.Encoding.ASCII.GetString(completedbuffer);

            //End of continual reading/decoding code citation.

            //Split the string into multiple filenames using the '\t' char as a splitter.
            char tabtoken = '\t';
            string[] newSpreadsheetCells = DocID.Split(tabtoken);

            string[] output = null;

            //This will assign output if it's a valid newname to the DocID.
            //Otherwise the output will be null, indicating the name is taken.
            if (newSpreadsheetCells[0] != "3")
            {
                output = null;
            } else
            {
                output = newSpreadsheetCells;
            }

            stream.Close();

            return output;
        }



        /// <summary>
        /// When the client asks for a new spreadsheet. 
        /// The method sends the appropriate message and reads back the server message.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="TCPSocket"></param>
        /// <returns></returns>
        public string NewSpreadsheet(string fileName, TcpClient TCPSocket)
        {
            NetworkStream stream = TCPSocket.GetStream();

            Byte[] data;


            if(fileName.Substring(fileName.Length-5) == ".sprd")
            {
                data = System.Text.Encoding.ASCII.GetBytes("1\t" + fileName + "\n");
            } else
            {
                data = System.Text.Encoding.ASCII.GetBytes("1\t" + fileName + ".sprd\n");
            }

            //Encode the Byte array as our message to send to the server.

            //Write to stream (from my understanding) sends the message to the server.
            stream.Write(data, 0, data.Length);


            //Code for continual reading and deocding found at "http://stackoverflow.com/questions/25712809/how-to-read-unknown-data-length-in-a-tcp-client"
            List<byte> bigbuffer = new List<byte>();

            byte[] tempbuffer = new byte[254];
            //can be in another size like 1024 etc.. 
            //depend of the data as you sending from de client
            //i recommend small size for the correct read of the package


            while (stream.Read(tempbuffer, 0, tempbuffer.Length) > 0)
            {

                bigbuffer.AddRange(tempbuffer);
            }

            // now you can convert to a native byte array
            byte[] completedbuffer = new byte[bigbuffer.Count];

            bigbuffer.CopyTo(completedbuffer);

            //Do something with the data
            string DocID = System.Text.Encoding.ASCII.GetString(completedbuffer);

            //End of continual reading/decoding code citation.

            //Split the string into multiple filenames using the '\t' char as a splitter.
            char tabtoken = '\t';
            string[] fileNameStrings = DocID.Split(tabtoken);

            string output = null;

            //This will assign output if it's a valid newname to the DocID.
            //Otherwise the output will be null, indicating the name is taken.
            if(fileNameStrings[0] == "1")
            {
                output = fileNameStrings[1];
            }

            stream.Close();

            return output;
        }


        /// <summary>
        /// The method to send a change request to the server. 
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="contents"></param>
        public void TrySet(string docID, string cellName, string contents, TcpClient TCPSocket)
        {
            SendMessage("3\t" + docID + "\t" + cellName + "\t" + contents + "\n", TCPSocket);
        }

        /// <summary>
        /// A method that undoes the last action of the spreadsheet.
        /// </summary>
        public void undo(string docID, TcpClient TCPSocket)
        {
            SendMessage("3\t" + docID + "\n", TCPSocket);
        }

        /// <summary>
        /// A method that redos the last undo of the spreadsheet.
        /// </summary>
        public void redo(string docID, TcpClient TCPSocket)
        {
            SendMessage("5\t" + docID + "\n", TCPSocket);
        }


        /// <summary>
        /// A method that requests a save of the spreadsheet
        /// </summary>
        public void save(string docID, TcpClient TCPSocket)
        {
            SendMessage("6\t" + docID + "\n", TCPSocket);
        }

        /// <summary>
        /// General method to send something to the server.
        /// Used by TrySet and OpenSpreadsheet.
        /// </summary>
        /// <param name="v"></param>
        public void SendMessage(string message, TcpClient TCPSocket)
        {
            NetworkStream stream = TCPSocket.GetStream();

            //Encode the Byte array as our message to send to the server.
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            //Write to stream (from my understanding) sends the message to the server.
            stream.Write(data, 0, data.Length);

            //Remember to close the streams. 
            stream.Close();
        }
    }
}
