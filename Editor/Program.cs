namespace Editor;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        if (args.Length != 2)
        {
            const string errorMsg = $"Cannot load app.";
            MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            throw new System.Exception(errorMsg);
        }
        
        var memoryName = args[0];
        var windowTitle = args[1];
        
        Application.Run(new MainForm(memoryName, windowTitle));
    }
}