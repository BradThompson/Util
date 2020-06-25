I spent WAY to much time trying to speed up the FixWhite program.
Currently it's a pile of excrement that loads everything into a
string in memory and writes everything to a new string using +=
For files over about 50K, it gets very slow. At 444K it seems
to just freeze. It's just that slow.
I tried to make it using StreamReader and StreamWriter. This made the
tool blazing fast, but I discovered that it makes it more dangerous.
In the old program, it was very cautious. If a single CR was listed
without a LF after it, it would write it to the file regardless.
For trimming the white space at the end of lines, it would make sure
it was only whitespace.

With this dangerous version, it doesn't handle the singleton CR case.
It also appends a CRLF to the end of files when cleaning the white
space at the end of ines where the file didn't have a CRLF at the end.

It doesn't sound like this is a big deal, but I wrote the original
so that I could feel safe running it against any file and not worry
that the file would get changed in a way I didn't want it to change.

Thank juju for unit tests.

Before I feel comfortable with this new version, I have to modify it:
 o Singleton CFs should still be written to the file, especially if
   the CF is at the end of the file.
 o Fixing the whitespace at the ends of lines has to pay attention to
   whether a file ends with a CRLF and not add one if it does not.
 o Trimming also needs to handle the case where the user doesn't
   do the tab conversions.
 o To make these changes worth while, we might as well add a parameter
   that allows the user to specify the number of spaces for tab
   replacement.
 o Add unit tests that will attempt to change various kinds of binary
   files. We want to make it robust enough that we can really test it.
 o A nice to have would be a fix that simply converts a unicode or
   double wide character set file to UTF8. It would verify that
   the new file will not lose anything in the translation.


