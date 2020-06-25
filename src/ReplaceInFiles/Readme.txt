A VERY light weight and DANGEROUS EXE for string search and replace.
Don't use it on large files. DON'T USE IT ON BINARY FILES!

Finds or Finds and Replaces a string in a given file, a set of files, all files.
or all directories recursively.
Usage:

/f:Files - Search file(s). DOS rules for wild cards are used when specifying filenames.
/s:TextToFind - Use quotes to use spaces
/r:ReplacementText - Use quotes to use spaces
/i case insensitive search.
/x recurses through all directories.
/go actually makes the changes.

=========================================

Future enhancements:
Read byte mark to make sure only text files are changed.
Allow the user to specify a regular expression.
Enhance error handling around the use of directory names.