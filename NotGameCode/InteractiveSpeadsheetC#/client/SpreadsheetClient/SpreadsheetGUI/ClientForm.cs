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
using Client;

namespace Client
{
    public partial class ClientForm : Form
    {

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
        public ClientForm(TcpClient client, ClientHandler CH)
        {
            clientHandler = CH;

            TCPSocket = client;
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
                IPAddressTB.Text = "The IP address is invalid";
                return;
            }

            // Receive the TcpServer.response.

            //The stream for reading/writing
            NetworkStream stream = TCPSocket.GetStream();

            // Buffer to store the response bytes.
            Byte[]  data = new Byte[256];

            // String to store the response ASCII representation.
            string UserID = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            //Int32 bytes = stream.Read(data, 0, data.Length);
            //UserID = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

            //clientHandler.UserID = UserID;
            clientHandler.UserID = "1";

            clientHandler.UserName = UserNameTB.Text;

            //Sending the usename to the server to hold.
            this.clientHandler.SendMessage(UserNameTB.Text);

            //This ends the technical handshake. Now we send the op code "0\n" so we receive a list of files to open.

            this.clientHandler.AskForFiles();
        }

        
        /// <summary>
        /// Method called by CH that affects the connection. This should populate the combobox with
        /// all the files available.
        /// </summary>
        /// <param name="availableFiles"></param>
        public void receiveFiles(string[] availableFiles)
        {

            //Populate the FileChoice with the files available
            FileChoice.MaxDropDownItems = availableFiles.Count() + 1;
            //New File is the first, and indicates we want to open a new rather than load an existing.
            FileChoice.Items.Add("New File");
            FileChoice.Items.AddRange(availableFiles);


            //Deactivate all the stuff and activate all the stuff.
            UserNameTB.Enabled = false;
            IPAddressTB.Enabled = false;
            WhichSpreadLabel.Visible = true;
            FileChoice.Enabled = true;
            FileChoice.Visible = true;

            OpenButton.Enabled = true;
            OpenButton.Visible = true;
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
                    //Catch a custom modified name from an external change. 
                    if(FileChoice.SelectedText == "File removed externally")
                    {
                        return;
                    }

                    this.clientHandler.Open(FileChoice.SelectedText);
                    return;
                }                

            //If the new file choice has been selected, now we're taking on a name.
            } else
            {
                if (NewFileTB.Text == "")
                {
                    NewFileTB.Text = "Please enter a valid filename";
                    return;
                }

                //NewSpreadsheet will hand DocID a DocID if this is a valid new, or hand it a null if the name has been taken
                this.clientHandler.NewSpreadsheet(NewFileTB.Text);
            }
        }

        /// <summary>
        /// Helper method to receive validNew
        /// </summary>
        /// <param name="DocID"></param>
        public void validNew(string DocID)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SpreadsheetGUI.SpreadsheetForm SF = new SpreadsheetGUI.SpreadsheetForm();
            SF.UpdateSheetName(NewFileTB.Text);
            SF.clientName = UserNameTB.Text;
            SF.sheetID = DocID;
            SF.clientID = Int32.Parse(clientHandler.UserID);
            SF.CH = clientHandler;
            clientHandler.allSheets.Add(DocID, SF);
            Application.Run(SF);

            this.Close();
        }


        /// <summary>
        /// Helper method to receive invalidNew
        /// </summary>
        public void invalidNew()
        {
           NewFileTB.Text = "That name has been taken or is otherwise invalid";
        }



        /// <summary>
        /// Helper method to receive a validOpen and cause the appropriate reaction.
        /// </summary>
        /// <param name="openSpreadsheetResults"></param>
        public void validOpen(string[] openSpreadsheetResults)
        {
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                SpreadsheetGUI.SpreadsheetForm SF = new SpreadsheetGUI.SpreadsheetForm();
                SF.UpdateSheetName(FileChoice.Text);
                SF.clientName = UserNameTB.Text;
                SF.clientID = Int32.Parse(clientHandler.UserID);
                SF.CH = clientHandler;
                SF.sheetID = DocID;
                clientHandler.allSheets.Add(openSpreadsheetResults[1], SF);
                Application.Run(SF);
                this.Close();
            }
        }


        /// <summary>
        /// Helper method to receive invalidOpen
        /// </summary>
        public void invalidOpen()
        {
            FileChoice.SelectedText = "File removed externally";
        }
        
        /// <summary>
        /// When hitting the cancel button, this should return us to the state of connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelB_Click(object sender, EventArgs e)
        {
            if (newFile)
            {
                NewFileLabel.Visible = false;
                NewFileTB.Enabled = false;
                NewFileTB.Visible = false;
                return;
            }


            IPAddressTB.Enabled = true;
            UserNameTB.Enabled = true;

            OpenButton.Enabled = false;
            OpenButton.Visible = false;
            InputEnter.Enabled = true;
            InputEnter.Visible = true;
            CancelB.Enabled = false;
            CancelB.Visible = false;

            WhichSpreadLabel.Visible = false;
            FileChoice.Enabled = false;
            FileChoice.Visible = false;

            NewFileLabel.Visible = false;
            NewFileTB.Enabled = false;
            NewFileTB.Visible = false;

            TCPSocket.Close();
            TCPSocket = null;
            clientHandler.TCP = null;
        }
    }
}
