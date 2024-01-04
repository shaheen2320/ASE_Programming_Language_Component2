using ProgrammingLibrary;
using System;
using System.Windows.Forms;

namespace ASE_Programming_Language
{
    public partial class Form1 : Form
    {
        private Interpreter interpreter = new Interpreter();
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string commandText = textBox1.Text;


            ICommand command = ParseCommand(commandText);

            if (command != null)
            {
                command.Execute(interpreter);
                textBox2.Text = $"Value of {command.GetVariableName()}: {interpreter.GetVariableValue(command.GetVariableName())}";
            }
            else
            {

                textBox2.Text = "invalid command";
            }
        }


        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private ICommand ParseCommand(string commandText)
        {

            string[] parts = commandText.Split(' ');

            if (parts.Length == 4 && parts[0].ToLower() == "set" && parts[1].ToLower() == "valueof")
            {
                string variableName = parts[2];
                int value;

                if (int.TryParse(parts[3], out value))
                {
                    return new Commandinitialization(variableName, value);
                }
            }


            return null;
        }


    }


}




