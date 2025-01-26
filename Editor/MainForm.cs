using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Text;

namespace Editor;

public partial class MainForm : Form
{
    private readonly string? memoryName = null;
    private readonly string windowTitle;
    private string? message = null;
    public MainForm(string memoryName, string windowTitle)
    {
        this.memoryName = memoryName;
        this.windowTitle = windowTitle;
        InitializeComponent();
    }


    private void MainForm_Shown(object sender, EventArgs e)
    {
        this.Text = windowTitle;

        if (this.memoryName is null)
        {
            const string errorMsg = "Cannot load app, memoryName is null";
            MessageBox.Show(this, errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw new Exception(errorMsg);
        }


        try
        {
            var memoryMap = MemoryMappedFile.OpenExisting(this.memoryName);

            #if DEBUG
            Console.WriteLine($"connect to {this.memoryName}");
            #endif

            using (var accessor = memoryMap.CreateViewAccessor())
            {
                var length = accessor.ReadInt32(0);
                byte[] data = new byte[length];
                accessor.ReadArray(4, data, 0, length);
                string msg = Encoding.UTF8.GetString(data);
                this.message = msg;


                accessor.Write(0, 0);
            }


            if (this.message is not null)
                this.richTextBox1.Text = this.message;


            memoryMap.Dispose();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    
}