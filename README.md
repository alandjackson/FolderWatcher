## Folder Watcher Sample Code

Folder watcher is a simple utility class for getting notifications of new files inside a folder.

The most common use case is to set up the watched folder then start it:


    FolderWatcherInstance = new FolderWatcher();
    FolderWatcherInstance.FolderToWatch = FolderTextBox.Text;
    FolderWatcherInstance.IgnoreStartingFiles = true;
    FolderWatcherInstance.RunAsync();


The sample project includes unit tests and a wpf test UI.