namespace FileKiller5000
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstallerWindowsFileManager = new System.ServiceProcess.ServiceProcessInstaller();
            this.WindowsFileManager = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerWindowsFileManager
            // 
            this.serviceProcessInstallerWindowsFileManager.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerWindowsFileManager.Password = null;
            this.serviceProcessInstallerWindowsFileManager.Username = null;
            // 
            // WindowsFileManager
            // 
            this.WindowsFileManager.ServiceName = "Windows File Manager";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerWindowsFileManager,
            this.WindowsFileManager});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerWindowsFileManager;
        private System.ServiceProcess.ServiceInstaller WindowsFileManager;
    }
}