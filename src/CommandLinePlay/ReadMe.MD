CommandLinePlay

Creating a cleaner method for processing command lines. The one I have in
the Edmonds repository is clunky. It has problems with case sensitivity,
and I went way overboard with "Magic Strings".

In addition, I've moved away from deriving the BaseCommandLine object. WAY
over engineered.

In the latest effort, there is one class called CommandLineProcessor. You call the
Create method to indicate what the expected parameters are. Then you call
ProcessCommandLine and pass in the user command line. The first is compared
with the second and appropriate errors are sent to the user.

It is up to the caller to put the array of values in CommandLineProcessor into
properties if they so desire.

See the UnitTests for 98% code coverage of the CommandLineProcessor class.
This also gives sample code on how to call the Create and ProcessCommandLine
methods.

