using System;

public class Class1
{
	public Class1()
	{
        // Register dialogprompt handler to handle dialog box that will be opened from FindQBCompanyFileCommand
        // Subscribe to DialogPrompt command to ensure that unit tests do not popup dialogs
        CommandHandler dialogPromptHandler = (object sender, EventArgs args) =>
        {
            (((sender as CommonDialogPromptProxy).NativeElement) as WinForms.OpenFileDialog).FileName = dummyCompanyFilePath;
            return WinForms.DialogResult.OK;
        };
        ServiceMediator.CommandMediator.Subscribe(ServiceCommands.DialogPrompt, dialogPromptHandler);
	}
}
