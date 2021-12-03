# Serafor
Multithreaded C# CLI util for searching files using regex.

<hr>

Usage: 
```
Usage: serafor REGEX-QUERY [OPTIONS]...
```

Options:
```
-f, --files - Search for files only.
-d, --directories - Search for directories only.
-h, --hidden - Include hidden files and directories in the search.
```

Usage example:
```
cd ~/Projects
serafor .cs
serafor MyProgram -d
serafor MyProgram -f
serafor config -h -f
```
