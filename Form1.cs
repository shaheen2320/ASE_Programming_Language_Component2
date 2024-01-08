using ProgrammingLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace ASE_Programming_Language
{
    public partial class Form1 : Form
    {
        private int number;
        private int size;
        public Interpreter interpreter = new Interpreter();
        public List<ICommand> commandsInLoop;
        public object command;

        public Form1()
        {
            InitializeComponent();

            Button drawButton = new Button
            {
                Text = "Random.Shapes",
                Location = new Point(10, 10)
            };
            drawButton.Click += DrawButton_Click;
            Controls.Add(drawButton);
            commandsInLoop = new List<ICommand>();
        }

        public ICommand ParseCommand(string commandText)
        {
            // checking multi-line commands
            var lines = commandText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length > 0 && lines[0].Trim().ToLower() == "begin" && lines[lines.Length - 1].Trim().ToLower() == "end")
            {
                List<ICommand> commands = new List<ICommand>();
                for (int i = 1; i < lines.Length - 1; i++)
                {
                    string line = lines[i].Trim().ToLower();
                    string[] commandParts = line.Split(' ');

                    switch (commandParts[0])
                    {
                        case "draw rectangle":
                            if (commandParts.Length == 5 &&
                                int.TryParse(commandParts[1].Trim(), out int x) &&
                                int.TryParse(commandParts[2].Trim(), out int y) &&
                                int.TryParse(commandParts[3].Trim(), out int width) &&
                                int.TryParse(commandParts[4].Trim(), out int height))
                            {
                                commands.Add(new CommandDrawRectangle(x, y, width, height));
                            }
                            break;
                        case "draw circle":
                            if (commandParts.Length == 4 && int.TryParse(commandParts[1], out int cx) && int.TryParse(commandParts[2], out int cy) &&
                                int.TryParse(commandParts[3], out int size))
                            {
                                commands.Add(new CommandDrawCircle(size.ToString(), cx, cy)); // Assuming these are the correct parameters
                            }
                            break;
                        case "draw triangle":
                            if (commandParts.Length == 7 &&
                                int.TryParse(commandParts[1], out int x1) && int.TryParse(commandParts[2], out int y1) &&
                                int.TryParse(commandParts[3], out int x2) && int.TryParse(commandParts[4], out int y2) &&
                                int.TryParse(commandParts[5], out int x3) && int.TryParse(commandParts[6], out int y3))
                            {
                                commands.Add(new CommandDrawTriangle(x1, y1, x2, y2, x3, y3));
                            }
                            break;
                    }
                }
                return new CommandMultiLine(commands);
            }

            string[] CommandParts = commandText.Split(' ');

            // Handle variable assignment

            if (CommandParts.Length == 3 && CommandParts[1].Trim() == "=")
            {
                string variableName = CommandParts[0].Trim();
                if (int.TryParse(CommandParts[2].Trim(), out int value))
                {
                    return new CommandVariableAssignment(variableName, value);
                }
            }
            
            else if (CommandParts.Length == 2 && CommandParts[0].Trim().ToLower() == "circle")
            {
                
                int defaultX = 0;
                int defaultY = 0;
                return new CommandDrawCircle(CommandParts[1].Trim(), defaultX, defaultY);
            }

            else if (CommandParts.Length == 4 && CommandParts[0].Trim().ToLower() == "set" && CommandParts[2].Trim().ToLower() == "with")
            {
                string variableName = CommandParts[1].Trim();
                if (int.TryParse(CommandParts[3].Trim(), out int value))
                {
                    return new CommandInitialization(variableName, value);
                }
            }
            if (commandText.StartsWith("loop"))
            {
                string[] loopParts = commandText.Substring(4).Trim().Split(' ');
                if (loopParts.Length >= 2 && int.TryParse(loopParts[0], out int loopCount))
                {
                    string[] loopCommands = loopParts[1].Trim(new char[] { '[', ']' }).Split(';');
                    List<ICommand> commands = new List<ICommand>();
                    foreach (string cmd in loopCommands)
                    {
                        ICommand innerCommand = ParseCommand(cmd.Trim());
                        if (innerCommand != null)
                        {
                            commands.Add(innerCommand);
                        }
                    }
                    return new CommandLoop(loopCount, commands);
                }
            }
     
            return null;
        }
        public void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void button1_Click(object sender, EventArgs e)
        {
            
            ClearPictureBox(); // Clear the PictureBox before executing commands

            string commandText = textBox1.Text; 
            string input = textBox1.Text; 
            string[] parts = input.Split(' ');
           
            ExecuteMultiLineCommands(commandText);
            string[] lines = commandText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries); // Split the command text into lines
            List<string> errors = CheckSyntax(lines);

            ICommand command = ParseCommand(commandText);

            int loopCount = 0; // Declare loopCount here for broader scope
            if (parts.Length >= 2)
            {
                if (parts[0] == "set" && parts[1] == "loop" && int.TryParse(parts[2], out loopCount))
                {
                    // Set loop count variable
                    interpreter.SetVariable("loopCount", loopCount);
                }
                else if (parts[0] == "circle" && parts[1] == "loop")
                {
                    int radius = int.Parse(parts[2]);
                    int xIncrement = int.Parse(parts[3]);
                    int yIncrement = int.Parse(parts[4]);

                    loopCount = interpreter.GetVariableValue("loopCount");
                    int x = 0, y = 0; // Starting position for the first circle

                    for (int i = 0; i < loopCount; i++)
                    {
                        // Add circle command for each iteration
                        using (Graphics graphics = pictureBox1.CreateGraphics())
                        {
                            // Draw the circle at the current position
                            graphics.DrawEllipse(Pens.Black, x, y, radius * 2, radius * 2);

                            // Increment position for the next circle
                            x += xIncrement;
                            y += yIncrement;
                        }
                    }
                }
            }


            // Single-line if statement to check if the commandText is empty
            if (string.IsNullOrWhiteSpace(commandText)) MessageBox.Show("No command entered.");
            if (command != null)
            {
                // Check if the command is a graphical command before using graphics
                if (command is CommandDrawCircle)
                {
                    // Use the Graphics object of the PictureBox
                    using (Graphics graphics = pictureBox1.CreateGraphics())
                    {
                        command.Execute(interpreter, graphics);
                    }
                }
                else
                {
                    // For non-graphical commands, use the Execute method without Graphics
                    command.Execute(interpreter);

                }
            }

            else
            {
                // Proceed with command execution if no errors
                ExecuteCommands(lines);
            }
        }
        public void DrawButton_Click(object sender, EventArgs e)
        {
            DrawCompositeShape();
        }

        public void DrawCompositeShape()
        {
            Random random = new Random();

            ClearPictureBox();

            // Getting the Graphics object from the PictureBox
            using (Graphics graphics = pictureBox1.CreateGraphics())
            {
                // Draw a rectangle
                graphics.DrawRectangle(Pens.DarkRed, new Rectangle(random.Next(0, 100), random.Next(0, 200), random.Next(20, 300), random.Next(20, 100)));

                // Draw a circle
                int radius = random.Next(30, 50);
                graphics.DrawEllipse(Pens.BlueViolet, new Rectangle(random.Next(0, 100), random.Next(0, 100), radius, radius));

                // Draw a line
                graphics.DrawLine(Pens.Black, new Point(random.Next(0, 100), random.Next(0, 100)), new Point(random.Next(100, 200), random.Next(100, 200)));

            }
        }

        public void ExecuteMultiLineCommands(string commandText)
        {
            var lines = commandText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            using (Graphics graphics = pictureBox1.CreateGraphics())
            {
                ;
            }

            bool conditionMet = true;
            bool inIfBlock = true;
            List<string> commandBlock = new List<string>();

            foreach (var line in lines)
            {

                if (line.Trim().ToLower().StartsWith("set number"))
                {
                    var parts = line.Split(' ');
                    if (parts.Length == 3 && int.TryParse(parts[2], out number))
                    {
                        // Number is successfully set, display the number
                        MessageBox.Show("Number set to: " + number.ToString());
                        continue; // Continue to process other commands
                    }
                    else
                    {
                        MessageBox.Show("Invalid number format.");
                        return; // Stop processing further commands
                    }
                }
                else
                {

                    string trimmedLine = line.Trim().ToLower();
                    if (trimmedLine.StartsWith("size = count *"))
                    {
                        string[] parts = trimmedLine.Split('*');
                        if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out int baseSize))
                        {
                            DrawConcentricCircles(baseSize);
                        }
                        else
                        {
                            MessageBox.Show("Invalid format for size command.");
                        }
                    }

                }

                if (conditionMet)
                {
                    string trimmedLine = line.Trim().ToLower();
                    if (trimmedLine.StartsWith("size ="))
                    {
                        string[] parts = trimmedLine.Split('=');
                        if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out int parsedSize))
                        {
                            size = parsedSize; // Set the size variable
                        }
                    }
                    else if (trimmedLine.StartsWith("if size >"))
                    {
                        string[] parts = trimmedLine.Split('>');
                        if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out int comparisonValue))
                        {
                            conditionMet = size > comparisonValue;
                            inIfBlock = true;
                        }
                    }
                    else if (trimmedLine == "endif")
                    {
                        inIfBlock = false;
                        if (conditionMet)
                        {
                            foreach (var cmd in commandBlock)
                            {
                                ProcessCommand(cmd);
                            }
                        }
                        commandBlock.Clear();
                        conditionMet = false;
                    }
                    else if (inIfBlock)
                    {
                        commandBlock.Add(trimmedLine);
                    }
                }
            }
        }

        private void ProcessCommand(string command)
        {
            if (command.StartsWith("print"))
            {
                MessageBox.Show(command.Substring(6));
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        public void buttonTestLoop_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            commandsInLoop.Clear();

            // Assuming you want to draw 10 random circles as before
            for (int i = 0; i < 6; i++)
            {
                int x = random.Next(pictureBox1.Width);
                int y = random.Next(pictureBox1.Height);
                int size = random.Next(20, 100);
                commandsInLoop.Add(new CommandDrawCircle(size.ToString(), x, y));
            }


            CommandLoop loopCommand = new CommandLoop(commandsInLoop.Count, commandsInLoop);


            pictureBox1.Refresh();  // Clear the PictureBox before drawing new shapes
            using (Graphics graphics = pictureBox1.CreateGraphics())
            {
                foreach (var command in commandsInLoop)
                {
                    if (command is CommandDrawCircle drawCircleCommand)
                    {
                        drawCircleCommand.Execute(interpreter, graphics);
                    }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {

        }
        public void DrawConcentricCircles(int baseSize)
        {
            using (Graphics graphics = pictureBox1.CreateGraphics())
            {
                pictureBox1.Refresh(); // Clear the PictureBox

                int centerX = pictureBox1.Width / 2;
                int centerY = pictureBox1.Height / 2;

                for (int count = 1; count <= 10; count++)
                {
                    int size = count * baseSize;
                    graphics.DrawEllipse(Pens.Black, centerX - size, centerY - size, size * 2, size * 2);
                }
            }
        }
        public List<string> CheckSyntax(string[] lines)
        {
            List<string> errors = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                string[] parts = line.Split(' ');


                if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0]))
                    continue; // Skip empty lines

                switch (parts[0].ToLower())
                {
                    case "set":
                        if (!(parts.Length == 3 && parts[1] == "loop" && int.TryParse(parts[2], out _)))
                            errors.Add($"Syntax error on line {i + 1}: Invalid 'set loop' command.");
                        break;

                    case "circle":
                        if (parts.Length > 2 && parts[1] == "size")
                            errors.Add($"Syntax error wrong command for circle .");

                        break;
                    case "if":
                        if (!(parts.Length >= 4 && parts[1] == "size" && parts[2] == ">" && int.TryParse(parts[3], out _)))
                            errors.Add($"Syntax error on line {i + 1}: Invalid 'if' condition.");
                        break;
                    case "print":
                        if (parts.Length < 2)
                            errors.Add($"Syntax error on line {i + 1}: 'print' command requires additional text.");
                        break;
                    case "endif":
                        if (parts.Length != 1)
                            errors.Add($"Syntax error on line {i + 1}: 'endif' command should not have additional parameters.");
                        break;

                    default:
                        errors.Add($"Syntax error on line {i + 1}: Unknown command '{parts[0]}'.");
                        break;

                }
            }
            return errors;
        }



        private void ExecuteCommands(string[] lines)
        {
            using (Graphics graphics = pictureBox1.CreateGraphics())
            {
                foreach (var line in lines)
                {
                    // Parse the command
                    ICommand command = ParseCommand(line);
                    if (command != null)
                    {
                        if (command is CommandDrawCircle && line == "circle size")
                        {
                            // Execute the CommandDrawCircle with the Graphics object
                            ((CommandDrawCircle)command).Execute(interpreter, graphics);
                        }
                        else
                        {
                            // Execute other commands normally
                            command.Execute(interpreter);
                        }
                    }
                }
            }
        }

        private void ClearPictureBox()
        {
            pictureBox1.Refresh(); // This will clear the content of the PictureBox
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}



