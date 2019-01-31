using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {

        public String UserID = null;
        public TcpClient TCPSocket = null;
        private bool newFile;
        public String DocID = null;
        public ClientHandler clientHandler;

        public ClientForm(ClientHandler CH)
        {
            this.clientHandler = CH;
            InitializeComponent();
        }

        /// <summary>
        /// Overload for ClientForm, if a userName and IP are given (such as if we're trying to open a second form)
        /// Then we simply put those into the appropriate boxes and hit enter.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="IP"></param>
        public ClientForm(TcpClient client, string ID, ClientHandler CH)
        {
            clientHandler = CH;

            TCPSocket = client;
            UserID = ID;
            InitializeComponent();

            WhichSpreadLabel.Visible = true;
            IPAddressTB.Enabled = false;
            UserNameTB.Enabled = false;
            OpenButton.Enabled = true;
            OpenButton.Visible = true;
            InputEnter.Enabled = false;
            InputEnter.Visible = false;
            CancelB.Enabled = true;
            CancelB.Visible = true;            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// This checks the parameters for a client handshake before attempting the connection.
        /// When the connection is properly made the rest of the handshake executes, storing UserID and 
        /// the socket for elsewise use. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputEnter_Click(object sender, EventArgs e)
        {
            //Check all the input fields.
            if (UserNameTB.Text == "")
            {
                UserNameTB.Text = "You need to input a User Name";
                return;
            } else if (UserNameTB.Text == "You need to input a User Name")
            {
                return;
            }

            if(UserNameTB.Text.Count() > 25)
            {
                UserNameTB.Text = "Username must be shorter than 25 characters";
                return;
            }

            if (IPAddressTB.Text == "")
            {
                IPAddressTB.Text = "You need to input an IP Address.";
                return;
            }  else if (UserNameTB.Text == "You need to input an IP Address.")
            {
                return;
            }

            
            //Initial part of the client handshake.
            TCPSocket = this.clientHandler.Connect(IPAddressTB.Text);

            if(TCPSocket == null)
            {
                IPAddressTB.Text = "The IP address given returned no connection";
                return;
            }

            // Receive the TcpServer.response.

            //The stream for reading/writing
            NetworkStream stream = TCPSocket.GetStream();

            // Buffer to store the response bytes.
            Byte[]  data = new Byte[256];

            // String to store the response ASCII representation.
            UserID = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            UserID = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            //Sending the usename to the server to hold.
            this.clientHandler.SendMessage(UserNameTB.Text, TCPSocket);


            //This ends the technical handshake. Now we send the op code "0\n" so we receive a list of files to open.

            string[] availableFiles = this.clientHandler.AskForFiles(TCPSocket);



            //Populate the FileChoice with the files available
            FileChoice.MaxDropDownItems = availableFiles.Count() + 1;
            //New File is the first, and indicates we want to open a new rather than load an existing.
            FileChoice.Items.Add("New File");
            FileChoice.Items.AddRange(availableFiles);


            //Deactivate all the stuff and activate all the stuff.
            WhichSpreadLabel.Visible = true;
            OpenButton.Enabled = true;
            OpenButton.Visible = true;
            IPAddressTB.Enabled = false;
            UserNameTB.Enabled = false;
            FileChoice.Enabled = true;
            FileChoice.Visible = true;
            InputEnter.Enabled = false;
            InputEnter.Visible = false;
            CancelB.Enabled = true;
            CancelB.Visible = true;
        }


        /// <summary>
        /// When the open button is clicked, this uses input from the FileChoice combo box
        /// It can be used for opening or new.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenButton_Click(object sender, EventArgs e)
        {
            //If the new file choice hasn't been selected yet.
            if (!newFile)
            {
                //Change to all the new file formats.
                if (FileChoice.SelectedIndex == 0)
                {
                    FileChoice.Enabled = false;
                    FileChoice.Visible = false;
                    NewFileTB.Enabled = true;
                    NewFileTB.Visible = true;
                    NewFileLabel.Enabled = true;
                    NewFileLabel.Visible = true;
                    newFile = true;
                    CancelB.Enabled = true;
                    CancelB.Visible = true;
                    return;
                } else
                {
                    //Since the choices are selected from the given names of the server.
                    //We assume it's a valid selection.
                    string[] newSpeadsheetCells = this.clientHandler.Open(FileChoice.SelectedText, TCPSocket); 
                    
                    //If we have somehow received a message other than the spreadsheet we were expecting
                    if(newSpeadsheetCells == null)
                    {
                        IPAddressTB.Text = "Error, invalid message received while trying to open file";
                        return;
                    } else
                    {
                        DocID = newSpeadsheetCells[1];

                        //TODO, make new spreadsheet, inputting all the new cells.
                        for(int i = 2; i < newSpeadsheetCells.Count(); i++)
                        {
                            string cellName = newSpeadsheetCells[i];
                            i++;
                            string cellContents = newSpeadsheetCells[i];

                            //TODO, interact with spreadsheet form to give all cells.
                        }

                        //TODO, transfer data like socket and docID over.
                    }
                }

                ////TODO, Check out page 7, Notes point 1, Do I need to change my implementation for this?
                

            //If the new file choice has been selected, now we're taking on a name.
            } else
            {
                if (NewFileTB.Text == "")
                {
                    NewFileTB.Text = "Please enter a valid filename";
                    return;
                }

                DocID = this.clientHandler.NewSpreadsheet(NewFileTB.Text, TCPSocket);

                //If that name has been taken already.
                if(DocID == null)
                {
                    NewFileTB.Text = "That name has been taken";
                    return;
                } 

                //If name is valid and spreadsheet has assigned ID, start a spreadsheet and start accepting cells
                //TODO, Needs spreadsheet/receive cells.
                
            }
        }

        private void CancelB_Click(object sender, EventArgs e)
        {
            //Deactivate all the stuff and activate all the stuff.
            WhichSpreadLabel.Visible = false;
            OpenButton.Enabled = false;
            OpenButton.Visible = false;
            FileChoice.Enabled = false;
            FileChoice.Visible = false;
            InputEnter.Enabled = true;
            InputEnter.Visible = true;
            IPAddressTB.Enabled = false;
            UserNameTB.Enabled = false;
            CancelB.Enabled = false;
            CancelB.Visible = false;

            TCPSocket.Close();
            TCPSocket = null;
        }
    }
}
