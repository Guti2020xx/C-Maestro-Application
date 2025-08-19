using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Database_Control
{
    public partial class InitForm : Form
    {
        //Declaration of private fields Can
        InputField.InputEnd EndAction; //Handles input completion
        Action QuitAction; //Handles quit action
        string OptionsFilePath; //Stores file path

        //Constructor
        public InitForm(InputField.InputEnd EndAction, Action QuitAction, string FilePath)
        {
            InitializeComponent();

            //Reads from a specified file and removes any empty lines
            List<string> Options = new List<string>(File.ReadAllLines(FilePath));
            for (int i = Options.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Options[i]))
                    Options.RemoveAt(i);
            }
            OptionsFilePath = FilePath;
            ServerOptions.Items.AddRange(Options.ToArray()); //Populate a combo box with server options
            this.FilePath.Text = FilePath; //Displays file path in a text box
            this.EndAction = EndAction; //Stores input action
            this.QuitAction = QuitAction; // Stores quit action
        }

        private void InitForm_Load(object sender, EventArgs e)
        {

        }

        //Activates quit button on mouse click
        private void QuitBtn_Click(object sender, EventArgs e)
        {
            QuitAction();
            this.Close();
        }

        //Activates connection button on mouse click
        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            EndAction(ServerOptions.Text);
            this.Close();
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        //Activate reset button on mouse click
        private void RESET_Click(object sender, EventArgs e)
        {
            File.Delete(OptionsFilePath);
            ServerOptions.Text = "";
            this.Close();
        }
    }
}
