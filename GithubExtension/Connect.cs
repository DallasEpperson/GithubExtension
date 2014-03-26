using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;

namespace GithubExtension
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
        private DTE2 _applicationObject;
        private AddIn _addInInstance;
        private GithubExtensionSettings _settings;

		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;
            _settings = new GithubExtensionSettings();

			if(connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
                addToolsMenu();
                addCodeContextMenu();
			}
		}

        private void addToolsMenu()
        {
            object[] contextGUIDS = new object[] { };
            Commands2 commands = (Commands2)_applicationObject.Commands;
            //Place the command on the tools menu.
            //Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
            Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

            //Find the Tools command bar on the MenuBar command bar:
            CommandBarControl toolsControl = menuBarCommandBar.Controls["Tools"];
            CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

            //This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
            //  just make sure you also update the QueryStatus/Exec method to include the new command names.
            try
            {
                //Add a command to the Commands collection:
                Command command = commands.AddNamedCommand2(_addInInstance, 
                                                            "GithubExtensionConfigure", 
                                                            "Github Extension Configuration", 
                                                            "Configure Github Extension", 
                                                            true, 
                                                            59, 
                                                            ref contextGUIDS, 
                                                            (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, 
                                                            (int)vsCommandStyle.vsCommandStylePictAndText, 
                                                            vsCommandControlType.vsCommandControlTypeButton);

                //Add a control for the command to the tools menu:
                if ((command != null) && (toolsPopup != null))
                {
                    command.AddControl(toolsPopup.CommandBar, 1);
                }
            }
            catch (System.ArgumentException)
            {
                //If we are here, then the exception is probably because a command with that name
                //  already exists. If so there is no need to recreate the command and we can 
                //  safely ignore the exception.
            }
        }

        private void addCodeContextMenu()
        {
            CommandBar oCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["Code Window"];

            CommandBarPopup githubPopup = (CommandBarPopup)oCommandBar.Controls.Add(
                MsoControlType.msoControlPopup,
                System.Reflection.Missing.Value,
                System.Reflection.Missing.Value,
                System.Reflection.Missing.Value,
                true);
            githubPopup.Caption = "Github";

            CommandBarButton gistControl = (CommandBarButton)githubPopup.Controls.Add(
                MsoControlType.msoControlButton,
                System.Reflection.Missing.Value,
                System.Reflection.Missing.Value,
                System.Reflection.Missing.Value,
                true);
            gistControl.Caption = "Create Gist...";
            gistControl.Click += gistControl_Click;

            CommandBarButton githubFileLinkControl = (CommandBarButton)githubPopup.Controls.Add(
                MsoControlType.msoControlButton,
                System.Reflection.Missing.Value,
                System.Reflection.Missing.Value,
                System.Reflection.Missing.Value,
                true);
            githubFileLinkControl.Caption = "Link to this file on Github...";
            githubFileLinkControl.Click += githubFileLinkControl_Click;

            CommandBarButton githubLineLinkControl = (CommandBarButton)githubPopup.Controls.Add(
                MsoControlType.msoControlButton,
                System.Reflection.Missing.Value,
                System.Reflection.Missing.Value,
                System.Reflection.Missing.Value,
                true);
            githubLineLinkControl.Caption = "Link to this line on Github...";
            githubLineLinkControl.Click += githubLineLinkControl_Click;

            CommandBarButton githubConfigureControl = (CommandBarButton)githubPopup.Controls.Add(
                MsoControlType.msoControlButton,
                System.Reflection.Missing.Value,
                System.Reflection.Missing.Value,
                System.Reflection.Missing.Value,
                true);
            githubConfigureControl.BeginGroup = true;
            githubConfigureControl.Caption = "Configure...";
            githubConfigureControl.Click += githubConfigureControl_Click;
        }

        void githubConfigureControl_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            throw new NotImplementedException();
        }

        void githubLineLinkControl_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (_settings.GitExeLocation == string.Empty)
            {
                MessageBox.Show("Please locate your git installation.");
                using (var settingsForm = new frmConfig())
                {
                    settingsForm.ShowDialog();
                    _settings.GitExeLocation = settingsForm.GitExeLocation;
                    _settings.Save();
                }
                if (_settings.GitExeLocation == string.Empty)
                {
                    MessageBox.Show("Git installation not found!");
                    return;
                }
            }

            var fileLocation = _applicationObject.ActiveDocument.FullName;
            var selectionInfo = (TextSelection)_applicationObject.ActiveDocument.Selection;
            var githubLocation = GithubUtils.GetGithubFileLocation(fileLocation, _settings.GitExeLocation);
            if (githubLocation != string.Empty)
            {
                githubLocation += "#L" + selectionInfo.TopLine;
                if (!selectionInfo.IsEmpty)
                    githubLocation += "-L" + selectionInfo.BottomLine;
                System.Diagnostics.Process.Start(githubLocation);
            }
            else
            {
                MessageBox.Show("Could not locate file on github!");
            }
        }

        void githubFileLinkControl_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            if (_settings.GitExeLocation == string.Empty)
            {
                MessageBox.Show("Please locate your git installation.");
                using (var settingsForm = new frmConfig())
                {
                    settingsForm.StartPosition = FormStartPosition.CenterParent;
                    settingsForm.ShowDialog();
                    _settings.GitExeLocation = settingsForm.GitExeLocation;
                    _settings.Save();
                }
                if (_settings.GitExeLocation == string.Empty)
                {
                    MessageBox.Show("Git installation not found!");
                    return;
                }
            }

            var fileLocation = _applicationObject.ActiveDocument.FullName;
            var githubLocation = GithubUtils.GetGithubFileLocation(fileLocation, _settings.GitExeLocation);
            if (githubLocation != string.Empty)
            {
                System.Diagnostics.Process.Start(githubLocation);
            }
            else
            {
                MessageBox.Show("Could not locate file on github!");
            }
        }

        void gistControl_Click(CommandBarButton Ctrl, ref bool CancelDefault)
        {
            throw new NotImplementedException();
        }

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "GithubExtension.Connect.GithubExtensionConfigure")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}

            if (commandName != "GithubExtension.Connect.GithubExtensionConfigure")
            {
                var breakhere = 4;
            }
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
				if(commandName == "GithubExtension.Connect.GithubExtensionConfigure")
				{
					handled = true;
					return;
				}
			}
		}

	}
}