using System;
using System.Windows.Forms;

/// <summary>
/// 
/// </summary>
/// <author>Paul C Carlson</author>
/// <author>Spenser Riches</author>
namespace SpreadsheetGUI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Replaces Application.Run(new SpreadsheetForm()) to allow windows to exist
            // after the original window is closed.
            SpreadsheetForm new_form = new SpreadsheetForm();
            new_form.Show();
            Application.Run();
        }
    }

    class DemoApplicationContext : ApplicationContext
    {
        // Number of open forms
        private int formCount = 0;

        // Singleton ApplicationContext
        private static DemoApplicationContext appContext;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private DemoApplicationContext()
        {
        }

        /// <summary>
        /// Returns the one DemoApplicationContext.
        /// </summary>
        public static DemoApplicationContext getAppContext()
        {
            if (appContext == null)
            {
                appContext = new DemoApplicationContext();
            }
            return appContext;
        }

        /// <summary>
        /// Runs the form
        /// </summary>
        public void RunForm(Form form)
        {
            // One more form is running
            formCount++;

            // When this form closes, we want to find out
            form.FormClosed += (o, e) => { if (--formCount <= 0) ExitThread(); };

            // Run the form
            form.Show();
        }
    }
}
