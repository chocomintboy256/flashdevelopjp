using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using FlashDevelop;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Helpers;
using ProjectManager.Projects;
using ScintillaNet;
using ProjectManager.Actions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore.FRService;
using System.Collections.Generic;
using FlashDevelop.Utilities;
using PluginCore.Localization;

public class ClassFileRename
{
    private static PluginCore.FRService.FRRunner runner;
    private static MainForm mainForm;
    private static String currentFile;
    private static String newFilePath;

    public static void Execute()
    {
        if (Globals.SciControl == null) return;

        NewNameDialog newNameDialog = new NewNameDialog();
        newNameDialog.Show();
    }

    public static void DoRename(String newName)
    {        
        ScintillaControl sci = Globals.SciControl;

        mainForm = (MainForm)Globals.MainForm;

        // 開いているファイルを保存
        mainForm.CallCommand("SaveAllModified", null);

        ITabbedDocument document = mainForm.CurrentDocument;
        String documentDirectory = Path.GetDirectoryName(document.FileName);
        ASContext context = (ASContext)ASContext.Context;
        List<ClassModel> classes = context.CurrentModel.Classes;

        currentFile = document.FileName;
        //String newFileName = classes[0].Name;
        String newFileName = newName;
        newFilePath = documentDirectory + "\\" + newFileName + ".as";

        FileModel currentModel = context.CurrentModel;
        MemberModel currentMember = classes[0].ToMemberModel();
        String oldName = currentMember.Name;

        if (newFileName == "") return;
        if (oldName == newFileName) return; 

        Project project = (Project) PluginBase.CurrentProject;
        if (project == null) return;

        // 本当に実行してよいか確認
        /*
        if (!Globals.Settings.DisableReplaceFilesConfirm)
        {
            String caption = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
            String message = TextHelper.GetString("FlashDevelop.Info.AreYouSureToReplaceInFiles");
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel) return;
        }
        */

        List<String> fileList = new List<string>();

        foreach (PathModel path in ASContext.Context.Classpath)
        {
            lock (path.Files.Values)
            {
                foreach (FileModel model in path.Files.Values)
                {

                    if (model.Package == currentModel.Package)
                    {
                        fileList.Add(model.FileName);
                        continue;
                    }

                    //TraceManager.Add(model.Classes);
                    foreach(MemberModel import in model.Imports)
                    {
                        if (import.Type == currentMember.Type)
                        {
                            fileList.Add(model.FileName);
                        }
                    }
                }
            }
        }

        document.Close();

        fileList.Add(currentFile);

        Replace(fileList, oldName, newFileName);
    }

    private static void RenameClassFile()
    {
        File.Move(currentFile, newFilePath);

        mainForm.OpenEditableDocument(newFilePath);
    }

    private static void Replace(List<String> path, String search, String replace)
    {
        FRConfiguration config = new FRConfiguration(path, GetFRSearch(search));
        config.Replacement = replace;
        runner = new FRRunner();
        runner.ProgressReport += new FRProgressReportHandler(RunnerProgress);
        runner.Finished += new FRFinishedHandler(ReplaceFinished);
        runner.ReplaceAsync(config);
    }

    /// <summary>
    /// Gets seartch object for find and replace
    /// </summary>
    private static FRSearch GetFRSearch(String pattern)
    {
        FRSearch search = new FRSearch(pattern);
        search.WholeWord = true;
        search.NoCase = !true;
        search.Filter = SearchFilter.None;
        search.Filter |= SearchFilter.OutsideCodeComments;
        search.Filter |= SearchFilter.OutsideStringLiterals;
        return search;
    }

    private static void ReplaceFinished(FRResults results)
    {
        RenameClassFile();
    }
}

public class NewNameDialog : Form
{
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button closeButton;
    private System.Windows.Forms.TextBox newFileNameTextBox;
    private System.Windows.Forms.Label valueLabel;

    public NewNameDialog()
    {
        this.Owner = (MainForm)Globals.MainForm;
        //this.Font = (MainForm)Globals.Settings.DefaultFont;
        this.InitializeComponent();
        //this.ApplyLocalizedTexts();
    }

    #region Windows Forms Designer Generated Code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.newFileNameTextBox = new System.Windows.Forms.TextBox();
        this.valueLabel = new System.Windows.Forms.Label();
        this.closeButton = new System.Windows.Forms.Button();
        this.okButton = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // newFileNameTextBox
        // 
        this.newFileNameTextBox.Location = new System.Drawing.Point(52, 10);
        this.newFileNameTextBox.Name = "newFileNameTextBox";
        this.newFileNameTextBox.Size = new System.Drawing.Size(150, 21);
        this.newFileNameTextBox.TabIndex = 1;
        // 
        // valueLabel
        // 
        this.valueLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.valueLabel.Location = new System.Drawing.Point(15, 12);
        this.valueLabel.Name = "valueLabel";
        this.valueLabel.Size = new System.Drawing.Size(37, 15);
        this.valueLabel.TabIndex = 0;
        this.valueLabel.Text = "Value:";
        // 
        // closeButton
        //
        this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.closeButton.Location = new System.Drawing.Point(150, 38);
        this.closeButton.Name = "closeButton";
        this.closeButton.Size = new System.Drawing.Size(53, 23);
        this.closeButton.TabIndex = 4;
        this.closeButton.Text = "&Close";
        this.closeButton.Click += new System.EventHandler(this.CancelButtonClick);
        // 
        // okButton
        // 
        this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.okButton.Location = new System.Drawing.Point(79, 38);
        this.okButton.Name = "okButton";
        this.okButton.Size = new System.Drawing.Size(55, 23);
        this.okButton.TabIndex = 2;
        this.okButton.Text = "&OK";
        this.okButton.Click += new System.EventHandler(this.okButtonClick);
        // 
        // GoToDialog
        // 
        this.AcceptButton = this.okButton;
        this.CancelButton = this.closeButton;
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(216, 71);
        this.Controls.Add(this.okButton);
        this.Controls.Add(this.closeButton);
        this.Controls.Add(this.newFileNameTextBox);
        this.Controls.Add(this.valueLabel);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "NewFileNameDialog";
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = " NewFileName";
        this.VisibleChanged += new System.EventHandler(this.VisibleChange);
        this.Closing += new System.ComponentModel.CancelEventHandler(this.DialogClosing);
        this.ResumeLayout(false);
        this.PerformLayout();

    }
    #endregion

    #region Methods And Event Handlers

    /// <summary>
    /// Applies the localized texts to the form
    /// </summary>
    /*
    private void ApplyLocalizedTexts()
    {
        this.okButton.Text = TextHelper.GetString("Label.OK");
        this.closeButton.Text = TextHelper.GetString("Label.Close");
        this.valueLabel.Text = TextHelper.GetString("Info.Value");
        this.Text = " " + TextHelper.GetString("Title.NewFileNameDialog");
    }
    */

    /// <summary>
    /// Selects the textfield's text
    /// </summary>
    private void SelectnewFileNameTextBox()
    {
        this.newFileNameTextBox.Select();
        this.newFileNameTextBox.SelectAll();
    }

    /// <summary>
    /// Some event handling when showing the form
    /// </summary>
    private void VisibleChange(Object sender, System.EventArgs e)
    {
        if (this.Visible)
        {
            this.SelectnewFileNameTextBox();
            this.CenterToParent();
        }
    }

    /// <summary>
    /// Hides only the dialog when user closes it
    /// </summary>
    private void DialogClosing(Object sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Globals.CurrentDocument.Activate();
        this.Hide();
    }

    /// <summary>
    /// Moves the cursor to the specified line
    /// </summary>
    private void okButtonClick(Object sender, System.EventArgs e)
    {
        if (Globals.SciControl == null) return;
        try
        {
            ClassFileRename.DoRename(this.newFileNameTextBox.Text);
            this.Close();
        }
        catch
        {
            String message = TextHelper.GetString("Info.GiveProperInt32Value");
            ErrorManager.ShowInfo(message);
        }
    }

    /// <summary>
    /// Hides the goto dialog
    /// </summary>
    private void CancelButtonClick(Object sender, System.EventArgs e)
    {
        this.Close();
    }

    #endregion

}