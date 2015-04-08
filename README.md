# cin (Flame.C)
## Flame.C
Flame.C is an experimental C front-end for the Flame compiler framework, written from the ground up in D#.  
It is still very much a work in progress. Currently, only the most primitive C features are supported, with basic utilities, like a preprocessor, union types, enums and typedefs still missing.

## cin
cin is a C compiler that leverages Flame.C and the rest of the Flame framework.
### Usage
cin uses the same interface as dsc, Flame's D# compiler. Compiling 'Struct.c' (as found somewhere in the 'Tests' folder) for the CLR platform can be accomplished by invoking cin with:

    cin Struct.c -platform CLR
    
### Back-ends
C code can be compiled to any target platform supported by Flame. Currently, two back-ends are functional enough to be used by cin in a somewhat reliable manner:
 * CLR (.Net framework)
 * C++

Other back-ends are also available, but are unlikely to function correctly.
