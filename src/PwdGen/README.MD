PwdGen
    /?       - This message
    /V       - Verbose output. Can be used to view default values.
    /Min     - Minimum number of characters
    /Max     - Maximum number of characters
    /UC      - Include Upper Case letters
    /LC      - Inlude Lower Case letters
    /Num     - Include numbers
    /Simple  - Include subset of special symbols
    /Symbols - Include all special symbols
    /ExAmb   - Exclude Ambiguous characters

Generates a password given the inputs. The inputs are stored in the registry,
so you don't have to re-enter or even remember the switches. You can simply call PwdGen.
The algorthim is really inefficient. I through this together just for fun.
I'd like to create a generator that gives you a Passowrd that is easy to remember but
hard to crack.