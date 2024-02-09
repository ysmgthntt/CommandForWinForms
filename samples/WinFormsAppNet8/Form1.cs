using CommandForWinForms;
using System.Diagnostics;

namespace WinFormsAppNet8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private static readonly UICommand Sample = new UICommand("Sample", "Sample", [new KeyGesture(Keys.F5)]);
        private static readonly UICommand Switch = new UICommand("Switch", "Switch", [new KeyGesture(Keys.F6)]);

        private void Form1_Load(object sender, EventArgs e)
        {
            new CommandManager(this);

            toolStripButtonCommand1.SetCommand(Sample);
            toolStripButtonCommand2.Command = Sample;

            toolStripButtonClose.SetCommand(ApplicationCommands.Close);
            ApplicationCommands.Close.InputGestures.Add(new KeyGesture(Keys.Escape));
            this.GetCommandBindings().Add(new CommandBinding(ApplicationCommands.Close, (s, e) => Close()));

            //this.GetCommandBindings().Add(new CommandBinding(Sample, Form1_CommandExecuted));
            panel1.GetCommandBindings().Add(new CommandBinding(Sample, Panel1_CommandExecuted));
            panel2.GetCommandBindings().Add(new CommandBinding(Sample, Panel2_CommandExecuted));

            this.GetCommandBindings().Add(new CommandBinding(Switch, Switch_CommandExecuted));
        }

        private void Form1_CommandExecuted(object? sender, ExecutedEventArgs e)
        {
            MessageBox.Show(this, "Form1_CommandExecuted");
        }

        private void Panel1_CommandExecuted(object? sender, ExecutedEventArgs e)
        {
            MessageBox.Show(this, "Panel1_CommandExecuted");
        }

        private void Panel2_CommandExecuted(object? sender, ExecutedEventArgs e)
        {
            MessageBox.Show(this, "Panel2_CommandExecuted");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Form1().Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            Debug.WriteLine(GC.GetTotalMemory(true).ToString("N0"));
        }

        private void Switch_CommandExecuted(object? sender, ExecutedEventArgs e)
        {
            if (toolStripButtonCommand1.GetCommand() is null)
            {
                toolStripButtonCommand1.SetCommand(Sample);
                Debug.WriteLine("toolStripButtonCommand1.SetCommand(Sample);");
            }
            else
            {
                toolStripButtonCommand1.SetCommand(null);
                Debug.WriteLine("toolStripButtonCommand1.SetCommand(null);");
            }

            if (toolStripButtonCommand2.Command is null)
            {
                toolStripButtonCommand2.Command = Sample;
                Debug.WriteLine("toolStripButtonCommand2.Command = Sample;");
            }
            else
            {
                toolStripButtonCommand2.Command = null;
                Debug.WriteLine("toolStripButtonCommand2.Command = null;");
            }
        }
    }
}
