Used to convert a file or directory into a UNC path. For instance:
    idir R:\ADS\Brad\git\Practice\idir
returns:
    \\edcc.ctc.edu\shares\rdrive\ADS\Brad\git\Practice\idir
    
idir with no parameters uses the current directory.

If there are no mapped drives or you run idir on the C Drive, you get errors.
